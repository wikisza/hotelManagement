document.addEventListener('DOMContentLoaded', function() {
    
    const dropdownToggles = document.querySelectorAll('.nav-dropdown-toggle');
        
    dropdownToggles.forEach(toggle => {
        toggle.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            
            const dropdown = this.closest('.nav-dropdown');
            const wasActive = dropdown.classList.contains('active');
            
            document.querySelectorAll('.nav-dropdown').forEach(d => {
                d.classList.remove('active');
            });
            
            if (!wasActive) {
                dropdown.classList.add('active');
            } else {
            }
        });
    });
    
    document.addEventListener('click', function(e) {
        if (!e.target.closest('.nav-dropdown')) {
            document.querySelectorAll('.nav-dropdown').forEach(d => {
                d.classList.remove('active');
            });
        }
    });
    
    document.querySelectorAll('.nav-dropdown-menu a').forEach(link => {
        link.addEventListener('click', function(e) {
            e.stopPropagation();
        });
    });
});