document.addEventListener('DOMContentLoaded', function () {
    const board = document.getElementById('order-board');

    // OBSŁUGA ZMIANY STATUSU ZAMÓWIEŃ
    if (board) {
        board.addEventListener('click', function (e) {
            // Sprawdzenie czy kliknięty element to przycisk zmiany statusu
            if (e.target && e.target.classList.contains('update-status-btn')) {
                e.preventDefault();

                const button = e.target;
                const card = button.closest('.kanban-card');

                if (!card) {
                    console.error('Nie znaleziono karty zamówienia');
                    return;
                }

                const orderId = card.getAttribute('data-order-id');
                const newStatus = button.getAttribute('data-new-status');
                const url = board.getAttribute('data-update-url') || '/Orders/UpdateStatus';

                if (!orderId || !newStatus) {
                    console.error('Brakuje orderId lub newStatus', { orderId, newStatus });
                    return;
                }

                updateOrderStatus(button, card, orderId, newStatus, url);
            }
        });
    }

    // FUNKCJA AKTUALIZACJI STATUSU
    function updateOrderStatus(button, card, orderId, newStatus, url) {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenInput ? tokenInput.value : '';
        const originalText = button.textContent;
        const originalHTML = button.innerHTML;

        // Zablokuj przycisk
        button.disabled = true;
        button.innerHTML = '<span class="spinner-border spinner-border-sm"></span> ...';

        const formData = new FormData();
        formData.append('orderId', orderId);
        formData.append('newStatus', newStatus);
        formData.append('__RequestVerificationToken', token);

        fetch(url, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => {
                if (response.ok) {
                    // Animacja zanikania
                    card.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
                    card.style.opacity = '0';
                    card.style.transform = 'scale(0.95)';

                    setTimeout(() => {
                        location.reload();
                    }, 300);
                } else {
                    throw new Error(`HTTP Error: ${response.status}`);
                }
            })
            .catch(error => {
                console.error('Błąd podczas aktualizacji statusu:', error);

                // Przywróć przycisk do poprzedniego stanu
                button.disabled = false;
                button.innerHTML = originalHTML;
                button.textContent = originalText;

                // Pokaż alert z błędem
                alert('⚠️ Błąd: Nie udało się zmienić statusu zamówienia.\n\nSprawdzić konsolę przeglądarki (F12) dla szczegółów.');
            });
    }

    // OBSŁUGA MODALA - TWORZENIE ZAMÓWIENIA
    const createOrderModal = document.getElementById('createOrderModal');
    const modalContent = document.getElementById('modalContent');

    if (createOrderModal && modalContent) {
        createOrderModal.addEventListener('show.bs.modal', function () {
            modalContent.innerHTML = `
                <div class="text-center p-5">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                </div>`;

            fetch('/Orders/Create')
                .then(response => {
                    if (!response.ok) throw new Error('Błąd ładowania widoku');
                    return response.text();
                })
                .then(html => {
                    modalContent.innerHTML = html;
                    // Inicjalizuj filtry po załadowaniu formularza
                    initMenuFilters();
                })
                .catch(error => {
                    console.error('Błąd ładowania formularza:', error);
                    modalContent.innerHTML = '<div class="alert alert-danger">❌ Nie udało się załadować formularza.</div>';
                });
        });
    }

    // OBSŁUGA WERYFIKACJI GOŚCIA
    document.addEventListener('submit', function (e) {
        if (e.target && e.target.id === 'verifyGuestForm') {
            e.preventDefault();

            const form = e.target;
            const formData = new FormData(form);
            const submitBtn = form.querySelector('button[type="submit"]');
            const errorDiv = document.getElementById('verificationError');

            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Weryfikacja...';
            }

            fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
                .then(response => {
                    const contentType = response.headers.get("content-type");
                    if (contentType && contentType.indexOf("application/json") !== -1) {
                        return response.json().then(data => {
                            if (!data.success) {
                                if (errorDiv) {
                                    errorDiv.textContent = data.message;
                                    errorDiv.style.display = 'block';
                                }
                                if (submitBtn) {
                                    submitBtn.disabled = false;
                                    submitBtn.innerHTML = 'Weryfikuj i kontynuuj →';
                                }
                            }
                        });
                    } else {
                        return response.text().then(html => {
                            modalContent.innerHTML = html;
                            initMenuFilters();
                        });
                    }
                })
                .catch(error => {
                    console.error('Błąd weryfikacji:', error);
                    if (errorDiv) {
                        errorDiv.textContent = 'Błąd połączenia.';
                        errorDiv.style.display = 'block';
                    }
                    if (submitBtn) {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = 'Weryfikuj i kontynuuj →';
                    }
                });
        }
    });

    // INICJALIZACJA FILTRÓW MENU
    function initMenuFilters() {
        const searchInput = document.getElementById('menuSearchInput');
        const filterBtns = document.querySelectorAll('.filter-btn');
        const menuItems = document.querySelectorAll('.menu-item-card');
        const categories = document.querySelectorAll('.category-section');

        // 1. OBSŁUGA FILTROWANIA KATEGORII
        filterBtns.forEach(btn => {
            btn.addEventListener('click', () => {
                filterBtns.forEach(b => b.classList.remove('active'));
                btn.classList.add('active');

                const filterValue = btn.getAttribute('data-filter');

                if (searchInput) searchInput.value = '';

                categories.forEach(cat => {
                    const catName = cat.getAttribute('data-category-name');

                    cat.querySelectorAll('.menu-item-card').forEach(item => {
                        item.style.display = 'flex';
                    });

                    if (filterValue === 'all' || catName === filterValue) {
                        cat.style.display = 'block';
                    } else {
                        cat.style.display = 'none';
                    }
                });
            });
        });

        // 2. OBSŁUGA WYSZUKIWARKI W CZASIE RZECZYWISTYM
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                const searchText = e.target.value.toLowerCase().trim();

                if (searchText.length > 0) {
                    categories.forEach(cat => cat.style.display = 'block');
                    filterBtns.forEach(b => b.classList.remove('active'));
                    document.querySelector('.filter-btn[data-filter="all"]').classList.add('active');
                }

                let totalVisibleItems = 0;

                categories.forEach(cat => {
                    let categoryHasVisibleItems = false;
                    const itemsInCategory = cat.querySelectorAll('.menu-item-card');

                    itemsInCategory.forEach(item => {
                        const itemName = item.getAttribute('data-item-name') || '';
                        const itemDesc = item.querySelector('.menu-item-desc')?.textContent.toLowerCase() || '';
                        const itemTitle = item.querySelector('.menu-item-title')?.textContent.toLowerCase() || '';

                        const matches = itemName.includes(searchText) ||
                            itemDesc.includes(searchText) ||
                            itemTitle.includes(searchText);

                        if (searchText.length === 0 || matches) {
                            item.style.display = 'flex';
                            categoryHasVisibleItems = true;
                            totalVisibleItems++;
                        } else {
                            item.style.display = 'none';
                        }
                    });

                    if (!categoryHasVisibleItems && searchText.length > 0) {
                        cat.style.display = 'none';
                    } else if (searchText.length === 0) {
                        cat.style.display = 'block';
                    }
                });

                if (searchText.length > 0 && totalVisibleItems === 0) {
                    const oldMessage = document.getElementById('noResultsMessage');
                    if (oldMessage) oldMessage.remove();

                    const message = document.createElement('div');
                    message.id = 'noResultsMessage';
                    message.className = 'alert alert-info text-center my-4';
                    message.innerHTML = '<p class="mb-0">😢 Nie znaleziono produktów pasujących do wyszukiwania</p>';

                    const menuBody = document.querySelector('.menu-body');
                    if (menuBody) {
                        menuBody.appendChild(message);
                    }
                } else {
                    const oldMessage = document.getElementById('noResultsMessage');
                    if (oldMessage) oldMessage.remove();
                }
            });
        }

        // 3. OBSŁUGA ZDARZEŃ +/- I DODAJ
        const menuItemsContainer = document.querySelector('.menu-body') || modalContent;
        if (menuItemsContainer) {
            menuItemsContainer.addEventListener('click', function (e) {
                const decreaseBtn = e.target.closest('.decrease-qty');
                const increaseBtn = e.target.closest('.increase-qty');
                const addBtn = e.target.closest('.add-item-btn');

                // Jeśli nie kliknięto na przycisk ilości lub dodaj - wyjdź
                if (!decreaseBtn && !increaseBtn && !addBtn) return;

                // Zatrzymaj propagację zdarzenia
                e.stopPropagation();
                e.preventDefault();

                const menuItem = e.target.closest('.menu-item-card');
                if (!menuItem) return;

                const qtyDisplay = menuItem.querySelector('.qty-val');
                const quantityInput = menuItem.querySelector('.item-quantity-value');
                const addButtonEl = menuItem.querySelector('.add-item-btn');

                if (!qtyDisplay || !quantityInput || !addButtonEl) return;

                let current = parseInt(qtyDisplay.textContent) || 0;

                if (decreaseBtn) {
                    updateQuantity(Math.max(0, current - 1), qtyDisplay, quantityInput, addButtonEl);
                } else if (increaseBtn) {
                    updateQuantity(current + 1, qtyDisplay, quantityInput, addButtonEl);
                } else if (addBtn && current === 0) {
                    updateQuantity(1, qtyDisplay, quantityInput, addButtonEl);
                }
            });
        }
    }

    // AKTUALIZACJA ILOŚCI PRODUKTU
    function updateQuantity(newQty, displayEl, inputEl, btnEl) {
        if (newQty < 0) newQty = 0;

        displayEl.textContent = newQty;
        inputEl.value = newQty;

        if (newQty > 0) {
            btnEl.classList.remove('btn-primary');
            btnEl.classList.add('added');
            btnEl.textContent = '✓ ' + newQty;
        } else {
            btnEl.classList.remove('added');
            btnEl.classList.add('btn-primary');
            btnEl.textContent = 'Dodaj';
        }
    }
});