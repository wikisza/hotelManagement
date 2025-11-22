document.addEventListener('DOMContentLoaded', function () {
    const menuItems = document.querySelectorAll('.menu-item');

    menuItems.forEach(item => {
        const decreaseBtn = item.querySelector('.decrease-qty');
        const increaseBtn = item.querySelector('.increase-qty');
        const qtyDisplay = item.querySelector('.qty-display');
        const quantityInput = item.querySelector('.item-quantity-value');
        const addBtn = item.querySelector('.add-item-btn');

        function updateQuantity(newQty) {
            if (newQty < 0) newQty = 0;
            qtyDisplay.textContent = newQty;
            quantityInput.value = newQty;

            if (newQty > 0) {
                addBtn.style.background = 'linear-gradient(135deg, #28a745 0%, #20c997 100%)';
                addBtn.textContent = '✓ Dodane (' + newQty + ')';
            } else {
                addBtn.style.background = 'linear-gradient(135deg, #28a745 0%, #20c997 100%)';
                addBtn.textContent = 'Dodaj';
            }
        }

        decreaseBtn.addEventListener('click', (e) => {
            e.preventDefault();
            const current = parseInt(qtyDisplay.textContent);
            updateQuantity(current - 1);
        });

        increaseBtn.addEventListener('click', (e) => {
            e.preventDefault();
            const current = parseInt(qtyDisplay.textContent);
            updateQuantity(current + 1);
        });

        addBtn.addEventListener('click', (e) => {
            e.preventDefault();
            const current = parseInt(qtyDisplay.textContent);
            if (current === 0) {
                updateQuantity(1);
            }
        });
    });
});