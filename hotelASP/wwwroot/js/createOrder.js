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
        // Pobierz widok _CreateOrderStep1
        fetch('/Orders/Create')
            .then(res => res.text())
            .then(html => {
                modalContent.innerHTML = html;
                initMenuFilters();
            });
    }

    function initMenuFilters() {
        const searchInput = document.getElementById('menuSearchInput');
        const filterBtns = document.querySelectorAll('.filter-btn');
        const menuItems = document.querySelectorAll('.menu-item-card');
        const categories = document.querySelectorAll('.category-section');

        // 1. Obsługa Filtrowania Kategorii
        filterBtns.forEach(btn => {
            btn.addEventListener('click', () => {
                // Zmiana klasy active
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

                // Reset wyszukiwania po zmianie kategorii
                if (searchInput) searchInput.value = '';
                menuItems.forEach(item => item.style.display = 'flex');
            });
        });

        // 2. Obsługa Wyszukiwarki
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                const searchText = e.target.value.toLowerCase();

                // Reset filtrów kategorii na "Wszystkie" przy wyszukiwaniu
                if (searchText.length > 0) {
                    categories.forEach(cat => cat.style.display = 'block');
                    filterBtns.forEach(b => b.classList.remove('active'));
                    document.querySelector('.filter-btn[data-filter="all"]').classList.add('active');
                }

                menuItems.forEach(item => {
                    const itemName = item.getAttribute('data-item-name');
                    const parentCategory = item.closest('.category-section');

                    if (itemName.includes(searchText)) {
                        item.style.display = 'flex'; // Przywróć widoczność (flex dla układu karty)
                    } else {
                        item.style.display = 'none';
                    }
                });

                // Ukryj puste kategorie (te, w których ukryliśmy wszystkie produkty)
                categories.forEach(cat => {
                    const visibleItems = cat.querySelectorAll('.menu-item-card[style="display: flex;"]');
                    // Uwaga: style.display sprawdza inline style, domyślnie jest puste
                    // Bezpieczniej sprawdzić czy są jakieś widoczne dzieci
                    let hasVisible = false;
                    cat.querySelectorAll('.menu-item-card').forEach(item => {
                        if (item.style.display !== 'none') hasVisible = true;
                    });

                    cat.style.display = hasVisible ? 'block' : 'none';
                });
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

        // Obsługa przycisków +/- w menu (tak jak wcześniej)
        const menuItem = e.target.closest('.menu-item');
        if (menuItem) {
            handleMenuClicks(e, menuItem);
        }
    });

    // 3. Obsługa formularza weryfikacji (Krok 1 -> Krok 2)
    document.body.addEventListener('submit', function (e) {
        if (e.target && e.target.id === 'verifyGuestForm') {
            e.preventDefault();
            const form = e.target;
            const errorDiv = document.getElementById('verificationError');
            const submitBtn = form.querySelector('button[type="submit"]');

            // Zablokuj przycisk na czas requestu
            const originalBtnText = submitBtn.innerHTML;
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Weryfikacja...';

            const formData = new FormData(form);

            fetch(form.action, {
                method: 'POST',
                body: formData
            })
                .then(response => {
                    // Sprawdź typ odpowiedzi (JSON czy HTML?)
                    const contentType = response.headers.get("content-type");

                    if (contentType && contentType.indexOf("application/json") !== -1) {
                        // To jest JSON (błąd logiczny z kontrolera)
                        return response.json().then(data => {
                            if (!data.success) {
                                errorDiv.textContent = data.message;
                                errorDiv.style.display = 'block';
                            }
                        });
                    } else {
                        // To jest HTML (widok _CreateOrderStep2)
                        return response.text().then(html => {
                            modalContent.innerHTML = html;
                        });
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    errorDiv.textContent = "Wystąpił błąd serwera.";
                    errorDiv.style.display = 'block';
                })
                .finally(() => {
                    // Jeśli formularz nadal istnieje (błąd), przywróć przycisk
                    if (document.body.contains(submitBtn)) {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = originalBtnText;
                    }
                });
        }
    });

    // Funkcja pomocnicza do obsługi +/- (z poprzedniego rozwiązania)
    function handleMenuClicks(e, menuItem) {
        const qtyDisplay = menuItem.querySelector('.qty-display');
        const quantityInput = menuItem.querySelector('.item-quantity-value');
        const addBtn = menuItem.querySelector('.add-item-btn');
        let current = parseInt(qtyDisplay.textContent);

        if (e.target.classList.contains('decrease-qty')) {
            updateQuantity(current - 1, qtyDisplay, quantityInput, addBtn);
        } else if (e.target.classList.contains('increase-qty')) {
            updateQuantity(current + 1, qtyDisplay, quantityInput, addBtn);
        } else if (e.target.classList.contains('add-item-btn')) {
            if (current === 0) updateQuantity(1, qtyDisplay, quantityInput, addBtn);
        }
    }

    function updateQuantity(newQty, displayEl, inputEl, btnEl) {
        if (newQty < 0) newQty = 0;
        displayEl.textContent = newQty;
        inputEl.value = newQty;

        if (newQty > 0) {
            btnEl.classList.remove('btn-primary');
            btnEl.classList.add('btn-success');
            btnEl.textContent = '✓ ' + newQty;
        } else {
            btnEl.classList.remove('btn-success');
            btnEl.classList.add('btn-primary');
            btnEl.textContent = 'Dodaj';
        }
    }
});