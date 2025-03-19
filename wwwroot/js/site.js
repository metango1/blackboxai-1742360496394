// Role Hierarchy Management
document.addEventListener('DOMContentLoaded', function() {
    // Role tree toggle functionality
    const roleToggles = document.querySelectorAll('.role-toggle');
    roleToggles.forEach(toggle => {
        toggle.addEventListener('click', function(e) {
            e.preventDefault();
            const icon = this.querySelector('i');
            const roleItem = this.closest('.role-item');
            const children = roleItem.querySelector('.role-children');

            if (children) {
                // Toggle icon
                icon.classList.toggle('fa-chevron-right');
                icon.classList.toggle('fa-chevron-down');

                // Toggle children visibility with animation
                if (children.style.display === 'none') {
                    children.style.display = 'block';
                    children.style.opacity = '0';
                    setTimeout(() => {
                        children.style.opacity = '1';
                    }, 10);
                } else {
                    children.style.opacity = '0';
                    setTimeout(() => {
                        children.style.display = 'none';
                    }, 200);
                }
            }
        });
    });

    // Form validation enhancement
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            if (!this.checkValidity()) {
                e.preventDefault();
                e.stopPropagation();
                
                // Show validation messages
                const invalidInputs = this.querySelectorAll(':invalid');
                invalidInputs.forEach(input => {
                    const formGroup = input.closest('.mb-3');
                    if (formGroup) {
                        formGroup.classList.add('was-validated');
                    }
                });

                // Scroll to first error
                invalidInputs[0]?.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        });
    });

    // Delete confirmation enhancement
    const deleteButtons = document.querySelectorAll('.delete-role');
    deleteButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            if (!confirm('Are you sure you want to delete this role? This action cannot be undone.')) {
                e.preventDefault();
            }
        });
    });

    // Auto-dismiss alerts
    const alerts = document.querySelectorAll('.alert:not(.alert-validation)');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });

    // Parent role selection validation
    const parentRoleSelects = document.querySelectorAll('select[name="ParentRoleId"]');
    parentRoleSelects.forEach(select => {
        select.addEventListener('change', function() {
            const selectedOption = this.options[this.selectedIndex];
            const currentRoleId = document.querySelector('input[name="Id"]')?.value;

            if (selectedOption.value === currentRoleId) {
                alert('A role cannot be its own parent.');
                this.value = '';
            }
        });
    });

    // Role description character counter
    const descriptionTextareas = document.querySelectorAll('textarea[name="Description"]');
    descriptionTextareas.forEach(textarea => {
        const maxLength = 500;
        const counter = document.createElement('small');
        counter.classList.add('text-muted', 'float-end');
        textarea.parentNode.appendChild(counter);

        function updateCounter() {
            const remaining = maxLength - textarea.value.length;
            counter.textContent = `${remaining} characters remaining`;
        }

        textarea.addEventListener('input', updateCounter);
        updateCounter(); // Initial count
    });

    // Tooltip initialization
    const tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltips.forEach(tooltip => {
        new bootstrap.Tooltip(tooltip);
    });

    // Role hierarchy visualization enhancement
    const roleHierarchy = document.querySelector('.role-hierarchy');
    if (roleHierarchy) {
        const roleItems = roleHierarchy.querySelectorAll('.role-item');
        roleItems.forEach((item, index) => {
            // Add fade-in animation with delay based on hierarchy level
            const level = item.querySelector('.role-content').dataset.level || 0;
            item.style.animationDelay = `${level * 0.1}s`;
            item.classList.add('fade-in');
        });
    }
});

// Helper Functions
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}