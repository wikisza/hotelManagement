/* FILE: ~/wwwroot/js/menuManagement.js */

document.addEventListener('DOMContentLoaded', function () {
    const categoryForm = document.querySelector('form[asp-action="AddMenuCategory"]');
    const menuItemForm = document.querySelector('form[asp-action="AddMenuItem"]');
    const categorySelect = document.querySelector('select[asp-for="NewMenuItem.MenuCategoryId"]');
    const isAvailableCheckbox = document.querySelector('input[asp-for="NewMenuItem.IsAvailable"]');
    const containsAlcoholCheckbox = document.querySelector('input[asp-for="NewMenuItem.ContainsAlcohol"]');

    // ========== CATEGORY FORM VALIDATION ==========
    if (categoryForm) {
        categoryForm.addEventListener('submit', function (e) {
            const categoryNameInput = this.querySelector('input[asp-for="NewCategory.Name"]');

            if (!categoryNameInput.value.trim()) {
                e.preventDefault();
                showError(categoryNameInput, 'Nazwa kategorii jest wymagana');
                return false;
            }

            clearError(categoryNameInput);
        });
    }

    // ========== MENU ITEM FORM VALIDATION ==========
    if (menuItemForm) {
        // Set default checked state for IsAvailable
        if (isAvailableCheckbox) {
            isAvailableCheckbox.checked = true;
        }

        menuItemForm.addEventListener('submit', function (e) {
            const nameInput = this.querySelector('input[asp-for="NewMenuItem.Name"]');
            const descriptionTextarea = this.querySelector('textarea[asp-for="NewMenuItem.Description"]');
            const priceInput = this.querySelector('input[asp-for="NewMenuItem.Price"]');
            const categorySelectInput = this.querySelector('select[asp-for="NewMenuItem.MenuCategoryId"]');

            let isValid = true;

            // Validate Name
            if (!nameInput.value.trim()) {
                showError(nameInput, 'Nazwa pozycji jest wymagana');
                isValid = false;
            } else if (nameInput.value.trim().length < 3) {
                showError(nameInput, 'Nazwa musi mieć co najmniej 3 znaki');
                isValid = false;
            } else {
                clearError(nameInput);
            }

            // Validate Description
            if (!descriptionTextarea.value.trim()) {
                showError(descriptionTextarea, 'Opis jest wymagany');
                isValid = false;
            } else if (descriptionTextarea.value.trim().length < 5) {
                showError(descriptionTextarea, 'Opis musi mieć co najmniej 5 znaków');
                isValid = false;
            } else {
                clearError(descriptionTextarea);
            }

            // Validate Price
            if (!priceInput.value) {
                showError(priceInput, 'Cena jest wymagana');
                isValid = false;
            } else if (isNaN(parseFloat(priceInput.value)) || parseFloat(priceInput.value) <= 0) {
                showError(priceInput, 'Cena musi być większa od 0');
                isValid = false;
            } else {
                clearError(priceInput);
            }

            // Validate Category
            if (!categorySelectInput.value) {
                showError(categorySelectInput, 'Wybierz kategorię');
                isValid = false;
            } else {
                clearError(categorySelectInput);
            }

            if (!isValid) {
                e.preventDefault();
            }
        });

        // Real-time validation
        const inputs = menuItemForm.querySelectorAll('input, textarea, select');
        inputs.forEach(input => {
            input.addEventListener('blur', function () {
                validateField(this);
            });

            input.addEventListener('input', function () {
                if (this.parentElement.classList.contains('has-error')) {
                    validateField(this);
                }
            });
        });
    }

    // ========== HELPER FUNCTIONS ==========
    function showError(element, message) {
        clearError(element);

        const errorDiv = document.createElement('span');
        errorDiv.className = 'text-danger error-message';
        errorDiv.textContent = '⚠️ ' + message;

        element.parentElement.classList.add('has-error');
        element.classList.add('input-error');
        element.parentElement.appendChild(errorDiv);
    }

    function clearError(element) {
        element.parentElement.classList.remove('has-error');
        element.classList.remove('input-error');

        const existingError = element.parentElement.querySelector('.error-message');
        if (existingError) {
            existingError.remove();
        }
    }

    function validateField(element) {
        const value = element.value.trim();
        const fieldName = element.getAttribute('asp-for') || element.name;

        if (fieldName.includes('Name') && element.tagName === 'INPUT') {
            if (!value) {
                showError(element, 'To pole jest wymagane');
                return false;
            } else if (value.length < 3) {
                showError(element, 'Minimum 3 znaki');
                return false;
            }
        } else if (fieldName.includes('Description')) {
            if (!value) {
                showError(element, 'Opis jest wymagany');
                return false;
            } else if (value.length < 5) {
                showError(element, 'Minimum 5 znaków');
                return false;
            }
        } else if (fieldName.includes('Price')) {
            if (!value) {
                showError(element, 'Cena jest wymagana');
                return false;
            } else if (isNaN(parseFloat(value)) || parseFloat(value) <= 0) {
                showError(element, 'Cena musi być > 0');
                return false;
            }
        } else if (fieldName.includes('MenuCategoryId')) {
            if (!value) {
                showError(element, 'Wybierz kategorię');
                return false;
            }
        }

        clearError(element);
        return true;
    }

    // ========== SMOOTH SCROLLING FOR MENU LIST ==========
    const menuListContainer = document.querySelector('.menu-list-container');
    if (menuListContainer) {
        menuListContainer.addEventListener('wheel', function (e) {
            // Smooth scrolling is already enabled by CSS
        });
    }

    // ========== FORMAT PRICE INPUT ==========
    const priceInput = menuItemForm?.querySelector('input[asp-for="NewMenuItem.Price"]');
    if (priceInput) {
        priceInput.addEventListener('blur', function () {
            if (this.value) {
                this.value = parseFloat(this.value).toFixed(2);
            }
        });
    }

    // ========== AUTO-EXPAND TEXTAREA ==========
    const descriptionTextarea = menuItemForm?.querySelector('textarea[asp-for="NewMenuItem.Description"]');
    if (descriptionTextarea) {
        descriptionTextarea.addEventListener('input', function () {
            this.style.height = 'auto';
            this.style.height = Math.min(this.scrollHeight, 150) + 'px';
        });
    }

    // ========== HIGHLIGHT NEW ITEMS ==========
    function highlightNewItems() {
        const menuItems = document.querySelectorAll('.menu-item-card');
        if (menuItems.length > 0) {
            const lastItem = menuItems[menuItems.length - 1];
            lastItem.style.animation = 'slideIn 0.3s ease-out';
        }
    }

    // Add animation on page load if there are items
    if (document.querySelector('.menu-item-card')) {
        highlightNewItems();
    }

    console.log('Menu Management initialized successfully');
});

// ========== CSS ANIMATIONS ==========
if (!document.querySelector('style[data-menu-animations]')) {
    const style = document.createElement('style');
    style.setAttribute('data-menu-animations', 'true');
    style.textContent = `
        @keyframes slideIn {
            from {
                opacity: 0;
                transform: translateX(-20px);
            }
            to {
                opacity: 1;
                transform: translateX(0);
            }
        }

        .input-error {
            border-color: #dc3545 !important;
            background-color: #fff5f5 !important;
        }

        .form-group-menu.has-error .input-menu,
        .form-group-menu.has-error .select-menu,
        .form-group-menu.has-error .textarea-menu {
            border-color: #dc3545;
            background-color: #fff5f5;
        }

        .error-message {
            animation: shake 0.3s ease-in-out;
        }

        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-5px); }
            75% { transform: translateX(5px); }
        }
    `;
    document.head.appendChild(style);
}

