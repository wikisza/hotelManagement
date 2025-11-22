document.addEventListener('DOMContentLoaded', function () {
    const buttons = document.querySelectorAll('.update-status-btn');

    buttons.forEach(button => {
        button.addEventListener('click', function () {
            const card = this.closest('.kanban-card');
            const orderId = card.getAttribute('data-order-id');
            const newStatus = this.getAttribute('data-new-status');

            // Wywołanie AJAX do aktualizacji statusu
            fetch('/Order/UpdateStatus', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: `orderId=${orderId}&newStatus=${newStatus}`
            })
                .then(response => {
                    if (response.ok) {
                        // Prosta animacja i przeładowanie strony po sukcesie
                        card.style.transition = 'opacity 0.5s';
                        card.style.opacity = '0';
                        setTimeout(() => location.reload(), 500);
                    } else {
                        alert('Wystąpił błąd podczas aktualizacji statusu.');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert('Wystąpił błąd sieciowy.');
                });
        });
    });
});


// Plik: wwwroot/js/orders.js

document.addEventListener('DOMContentLoaded', function () {
    const createOrderModal = document.getElementById('createOrderModal');
    const modalContent = document.getElementById('modalContent');

    // Kiedy modal jest pokazywany, załaduj pierwszy krok
    createOrderModal.addEventListener('show.bs.modal', function () {
        // Pokaż spinner na czas ładowania
        modalContent.innerHTML = '<div class="text-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>';

        fetch('/Orders/Create')
            .then(response => response.text())
            .then(html => {
                modalContent.innerHTML = html;
            })
            .catch(error => {
                modalContent.innerHTML = '<div class="alert alert-danger">Nie udało się załadować formularza.</div>';
            });
    });

    // Użyj delegacji zdarzeń, aby obsłużyć formularz załadowany dynamicznie
    document.addEventListener('submit', function (e) {
        if (e.target && e.target.id === 'verifyGuestForm') {
            e.preventDefault(); // Zatrzymaj domyślne przesyłanie formularza

            const form = e.target;
            const formData = new FormData(form);

            fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: {
                    // Token jest potrzebny do ochrony przed atakami CSRF
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            })
                .then(response => response.text())
                .then(html => {
                    // Sprawdzamy, czy odpowiedź to JSON (błąd) czy HTML (sukces)
                    try {
                        const errorData = JSON.parse(html);
                        if (!errorData.success) {
                            const errorDiv = document.getElementById('verificationError');
                            errorDiv.textContent = errorData.message;
                            errorDiv.style.display = 'block';
                        }
                    } catch (error) {
                        // Jeśli parsowanie JSON się nie udało, to znaczy, że dostaliśmy HTML
                        modalContent.innerHTML = html;
                    }
                })
                .catch(error => {
                    const errorDiv = document.getElementById('verificationError');
                    errorDiv.textContent = 'Wystąpił błąd sieci. Spróbuj ponownie.';
                    errorDiv.style.display = 'block';
                });
        }
    });

    // ... reszta Twojego istniejącego kodu w orders.js
});