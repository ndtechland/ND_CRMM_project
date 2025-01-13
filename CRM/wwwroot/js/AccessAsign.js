
//function toggleSubOptionCheckboxes(type, mainCheckbox) {
//    const moduleRow = mainCheckbox.closest('.module-row');
//    if (moduleRow) {
//        const nextSibling = moduleRow.nextElementSibling;
//        if (nextSibling) {
//            const subCheckboxes = nextSibling.querySelectorAll(`.${type}-checkbox`);
//            subCheckboxes.forEach(checkbox => {
//                checkbox.checked = mainCheckbox.checked;
//                // Add or remove the 'subheading-checkbox' class based on checkbox state
//                if (!checkbox.checked) {
//                    checkbox.classList.remove('subheading-checkbox');
//                } else {
//                    checkbox.classList.add('subheading-checkbox');
//                }
//            });
//        }
//    }
//}

function toggleSubOptionCheckboxes(type, mainCheckbox) {
    const moduleRow = mainCheckbox.closest('.module-row'); // Find the current module row
    if (moduleRow) {
        const nextSibling = moduleRow.nextElementSibling; // Get the associated sub-options container
        if (nextSibling) {
            // Select all checkboxes inside the sub-options container
            const subCheckboxes = nextSibling.querySelectorAll(`input[type="checkbox"]`);
            subCheckboxes.forEach(checkbox => {
                checkbox.checked = mainCheckbox.checked; // Set checked state to match the main checkbox

                // Update classes based on the checked state
                if (!checkbox.checked) {
                    checkbox.classList.remove('subheading-checkbox');
                } else {
                    checkbox.classList.add('subheading-checkbox');
                }
            });
        }
    }
}

