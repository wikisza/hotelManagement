document.addEventListener('DOMContentLoaded', function () {
    console.log('orders.js załadowany');
    
    const board = document.getElementById('order-board');
    const createOrderModal = document.getElementById('createOrderModal');
    const openModalBtn = document.getElementById('openCreateOrderModal');
    const closeModalBtn = document.getElementById('closeCreateOrderModal');
    const modalContent = document.getElementById('modalContent');

    // ========== OBSŁUGA NATYWNEGO DIALOGU ==========
    if (openModalBtn && createOrderModal) {
        console.log('Przycisk i dialog znalezione');
        
        openModalBtn.addEventListener('click', function () {
            console.log('Kliknięto przycisk otwórz modal');
            createOrderModal.showModal();
            loadCreateOrderForm();
        });
    } else {
        console.error('Nie znaleziono elementów:', { openModalBtn, createOrderModal });
    }

    if (closeModalBtn && createOrderModal) {
        closeModalBtn.addEventListener('click', function () {
            createOrderModal.close();
        });
    }

    // Zamykanie na kliknięcie backdrop
    if (createOrderModal) {
        createOrderModal.addEventListener('click', function (e) {
            const dialogDimensions = createOrderModal.getBoundingClientRect();
            if (
                e.clientX < dialogDimensions.left ||
                e.clientX > dialogDimensions.right ||
                e.clientY < dialogDimensions.top ||
                e.clientY > dialogDimensions.bottom
            ) {
                createOrderModal.close();
            }
        });
    }

    // OBSŁUGA PRZYCISKÓW ANULUJ (Event Delegation)
    if (modalContent) {
        modalContent.addEventListener('click', function (e) {
            // Przycisk Anuluj w kroku 1 (weryfikacja)
            if (e.target && e.target.id === 'btnCancelVerification') {
                e.preventDefault();
                createOrderModal.close();
                return;
            }
            
            // Przycisk Wróć w kroku 2
            if (e.target && e.target.id === 'btnBackToStep1') {
                e.preventDefault();
                loadCreateOrderForm();
                return;
            }
        });
    }

    function loadCreateOrderForm() {
        if (!modalContent) return;

        modalContent.innerHTML = `
            <div class="text-center p-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Ładowanie...</span>
                </div>
            </div>`;

        fetch('/Orders/Create')
            .then(response => {
                if (!response.ok) throw new Error('Błąd ładowania widoku');
                return response.text();
            })
            .then(html => {
                modalContent.innerHTML = html;
            })
            .catch(error => {
                console.error('Błąd:', error);
                modalContent.innerHTML = `
                    <div class="text-center p-5">
                        <p style="color: #dc3545;">⚠️ Nie udało się załadować formularza</p>
                    </div>`;
            });
    }

    // ========== OBSŁUGA ZMIANY STATUSU ZAMÓWIEŃ ==========
    if (board) {
        board.addEventListener('click', function (e) {
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

    function updateOrderStatus(button, card, orderId, newStatus, url) {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenInput ? tokenInput.value : '';
        const originalText = button.textContent;
        const originalHTML = button.innerHTML;

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
                button.disabled = false;
                button.innerHTML = originalHTML;
                button.textContent = originalText;
                alert('⚠️ Błąd: Nie udało się zmienić statusu zamówienia.\n\nSprawdzić konsolę przeglądarki (F12) dla szczegółów.');
            });
    }
});