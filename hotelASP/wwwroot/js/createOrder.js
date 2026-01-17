document.addEventListener('DOMContentLoaded', function () {
    const modalContent = document.getElementById('modalContent');
    const createOrderModal = document.getElementById('createOrderModal');

    // 1. Ładowanie pierwszego kroku po otwarciu modala
    if (createOrderModal) {
        createOrderModal.addEventListener('show.bs.modal', function () {
            loadStep1();
        });
    }

    function loadStep1() {
        modalContent.innerHTML = '<div class="text-center p-4"><div class="spinner-border"></div></div>';
        fetch('/Orders/Create')
            .then(res => res.text())
            .then(html => {
                modalContent.innerHTML = html;
            });
    }

    // 2. Obsługa zdarzeń wewnątrz modala (Event Delegation)
    modalContent.addEventListener('click', function (e) {
        // Obsługa przycisku "Wróć" w kroku 2
        if (e.target && e.target.id === 'btnBackToStep1') {
            e.preventDefault();
            loadStep1();
            return;
        }
    });

    // 3. Obsługa formularza weryfikacji (Krok 1 -> Krok 2)
    document.body.addEventListener('submit', function (e) {
        if (e.target && e.target.id === 'verifyGuestForm') {
            e.preventDefault();
            const form = e.target;
            const errorDiv = document.getElementById('verificationError');
            const submitBtn = form.querySelector('button[type="submit"]');

            const originalBtnText = submitBtn.innerHTML;
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Weryfikacja...';

            const formData = new FormData(form);

            fetch(form.action, {
                method: 'POST',
                body: formData
            })
                .then(response => {
                    const contentType = response.headers.get("content-type");

                    if (contentType && contentType.indexOf("application/json") !== -1) {
                        return response.json().then(data => {
                            if (!data.success) {
                                errorDiv.textContent = data.message;
                                errorDiv.style.display = 'block';
                            }
                        });
                    } else {
                        return response.text().then(html => {
                            modalContent.innerHTML = html;
                            // WAŻNE: Po załadowaniu kroku 2, inicjalizuj filtry i przyciski
                            setTimeout(() => {
                                initMenuFilters();
                                initQuantityButtons();
                            }, 100);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    errorDiv.textContent = "Wystąpił błąd serwera.";
                    errorDiv.style.display = 'block';
                })
                .finally(() => {
                    if (document.body.contains(submitBtn)) {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = originalBtnText;
                    }
                });
        }
    });

    // 4. Inicjalizacja filtrów menu
    function initMenuFilters() {
        const searchInput = document.getElementById('menuSearchInput');
        const filterBtns = document.querySelectorAll('.filter-btn');
        const categories = document.querySelectorAll('.category-section');

        if (!searchInput || !filterBtns.length) {
            console.warn('Nie znaleziono elementów filtrów menu');
            return;
        }

        // Obsługa filtrowania kategorii
        filterBtns.forEach(btn => {
            btn.addEventListener('click', () => {
                filterBtns.forEach(b => b.classList.remove('active'));
                btn.classList.add('active');

                const filterValue = btn.getAttribute('data-filter');

                categories.forEach(cat => {
                    const catName = cat.getAttribute('data-category-name');
                    if (filterValue === 'all' || catName === filterValue) {
                        cat.style.display = 'block';
                    } else {
                        cat.style.display = 'none';
                    }
                });

                if (searchInput) searchInput.value = '';
                
                // Pokaż wszystkie produkty w widocznych kategoriach
                cat.querySelectorAll('.menu-item-card').forEach(item => {
                    item.style.display = 'flex';
                });
            });
        });

        // Obsługa wyszukiwarki
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                const searchText = e.target.value.toLowerCase().trim();

                if (searchText.length > 0) {
                    categories.forEach(cat => cat.style.display = 'block');
                    filterBtns.forEach(b => b.classList.remove('active'));
                    const allBtn = document.querySelector('.filter-btn[data-filter="all"]');
                    if (allBtn) allBtn.classList.add('active');
                }

                let totalVisible = 0;

                categories.forEach(cat => {
                    let categoryHasVisible = false;
                    const items = cat.querySelectorAll('.menu-item-card');

                    items.forEach(item => {
                        const itemName = item.getAttribute('data-item-name') || '';
                        const itemTitle = item.querySelector('.menu-item-title')?.textContent.toLowerCase() || '';
                        const itemDesc = item.querySelector('.menu-item-desc')?.textContent.toLowerCase() || '';

                        const matches = itemName.includes(searchText) || 
                                      itemTitle.includes(searchText) || 
                                      itemDesc.includes(searchText);

                        if (searchText.length === 0 || matches) {
                            item.style.display = 'flex';
                            categoryHasVisible = true;
                            totalVisible++;
                        } else {
                            item.style.display = 'none';
                        }
                    });

                    cat.style.display = categoryHasVisible ? 'block' : 'none';
                });

                // Pokaż komunikat jeśli brak wyników
                const noResults = document.getElementById('noResultsMessage');
                if (noResults) noResults.remove();

                if (searchText.length > 0 && totalVisible === 0) {
                    const message = document.createElement('div');
                    message.id = 'noResultsMessage';
                    message.className = 'alert alert-info text-center my-4';
                    message.innerHTML = '<p class="mb-0">😢 Nie znaleziono produktów</p>';
                    const menuBody = document.querySelector('.menu-body');
                    if (menuBody) menuBody.appendChild(message);
                }
            });
        }
    }

    // 5. Inicjalizacja przycisków ilości (+/-)
    function initQuantityButtons() {
        const menuBody = document.querySelector('.menu-body');
        if (!menuBody) {
            console.warn('Nie znaleziono .menu-body');
            return;
        }

        menuBody.addEventListener('click', function (e) {
            const decreaseBtn = e.target.closest('.decrease-qty');
            const increaseBtn = e.target.closest('.increase-qty');
            const addBtn = e.target.closest('.add-item-btn');

            if (!decreaseBtn && !increaseBtn && !addBtn) return;

            e.stopPropagation();
            e.preventDefault();

            const menuItem = e.target.closest('.menu-item-card');
            if (!menuItem) return;

            const qtyDisplay = menuItem.querySelector('.qty-val');
            const quantityInput = menuItem.querySelector('.item-quantity-value');
            const addButton = menuItem.querySelector('.add-item-btn');

            if (!qtyDisplay || !quantityInput || !addButton) {
                console.error('Brak wymaganych elementów w karcie menu');
                return;
            }

            let current = parseInt(qtyDisplay.textContent) || 0;

            if (decreaseBtn) {
                updateQuantity(Math.max(0, current - 1), qtyDisplay, quantityInput, addButton);
            } else if (increaseBtn) {
                updateQuantity(current + 1, qtyDisplay, quantityInput, addButton);
            } else if (addBtn && current === 0) {
                updateQuantity(1, qtyDisplay, quantityInput, addButton);
            }
        });
    }

    // 6. Aktualizacja ilości produktu
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

        console.log(`Zaktualizowano ilość: ${newQty}`);
    }
});