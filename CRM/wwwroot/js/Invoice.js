$(document).ready(function () {
    // Handle customer search on keyup
    $('#customerName').on('keyup', function () {
        var searchTerm = $(this).val();
        if (searchTerm.length >= 1) {
            $.ajax({
                url: '/Sale/GetCustomerNames',
                type: 'GET',
                data: { searchTerm: searchTerm },
                success: function (data) {
                    $('#customerSuggestions').empty();
                    if (data.length > 0) {
                        $.each(data, function (index, customer) {
                            $('#customerSuggestions').append(`<a href="#" class="list-group-item list-group-item-action" onclick="selectCustomer(${customer.id}, '${customer.companyName}')">${customer.companyName}</a>`);
                        });
                    } else {
                        $('#customerSuggestions').append('<div class="list-group-item">No customers found</div>');
                    }
                },
                error: function () {
                    console.log('Error fetching customer names.');
                }
            });
        } else {
            $('#customerSuggestions').empty();  // Clear suggestions if input is empty
        }
    });

    // Call updateTaxFields if a product is already selected on page load
    if ($(".ProductDetails").val()) {
        updateTaxFields();
    }

    // Update tax fields on product or state change
    $(".ProductDetails, .stateid").change(updateTaxFields);

    // Handle customer selection on change
    $('#customerName').on('change', function () {
        var customerId = $(this).val();
        if (customerId) {
            fetchCustomerDetails(customerId);
        }
    });
   
    var customerId = $("#customerId").val();
    var customerName = $("#customerName").val();
    if (customerId != '' && customerName != '') {
        selectCustomer(customerId, customerName);
    }
           


    // Numeric validation for input fields
    $(".numeric-input").on("keydown", function (event) {
        validateNumericInput(event);
    });
});

// Handle customer selection
function selectCustomer(customerId, customerName) {
    $('#customerName').val(customerName);
    $('#customerId').val(customerId);
    $('#customerSuggestions').empty();
    fetchCustomerDetails(customerId);
}

// Fetch customer details by ID
function fetchCustomerDetails(customerId) {
    $.ajax({
        url: '/Sale/GetCustomerDetailsById',
        type: 'GET',
        data: { id: customerId },
        success: function (customerData) {
            $('#billingAddressLabel').text(customerData.billingAddress);
            $('#officeAddressLabel').text(customerData.location);
            $('#officeStateLabel').text(customerData.officeState);
            $('#officeCityLabel').text(customerData.officeCity);
            $('#billingCityLabel').text(customerData.billingCity);
            $('#billingStateLabel').text(customerData.billingState);
            $('#mobileNumberLabel').text(customerData.mobileNumber);
            $('#emailNumberLabel').text(customerData.email);
            $('#gstNumberLabel').text(customerData.gstNumber);
            $('.billingStateId').text(customerData.billingStateId); // Set billing state ID
            updateTaxFields(); // Recalculate GST
            $('#customerDetailsRow').removeClass('d-none'); // Show customer details
        },
        error: function () {
            console.log('Error fetching customer details.');
        }
    });
}

 //Update tax fields based on product and billing state
function updateTaxFields() {
    $('.customer-section').each(function () {
        var $section = $(this);
        var productId = $section.find(".ProductDetails").val();
        var billingStateId = $('.billingStateId').text(); // Common billing state ID

        if (!productId || !billingStateId) {
            clearTaxFields($section);
            return;
        }

        $.ajax({
            url: '/Sale/product?id=' + productId,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response.data != null) {
                    // Update specific fields for this product section
                    $section.find('.HsnSacCode').val(response.data.hsnSacCode).attr('readonly', true);
                    $section.find('.Price').val(response.data.productPrice).attr('readonly', true);

                    // Update GST fields based on state match
                    var checkVendorBillingStateId = $("#checkvendorbillingstateid").val();

                    if (checkVendorBillingStateId === billingStateId) {
                        $section.find('.CGST').val(response.data.cgst).attr('readonly', true);
                        $section.find('.SGST').val(response.data.sgst).attr('readonly', true);
                        $section.find('.IGST').val('').attr('readonly', true); // Clear IGST
                    } else {
                        $section.find('.IGST').val(response.data.igst).attr('readonly', true);
                        $section.find('.CGST, .SGST').val('').attr('readonly', true); // Clear CGST and SGST
                    }

                } else {
                    clearTaxFields($section);
                }
            },
            error: function () {
                alert('Error fetching data');
                clearTaxFields($section);
            }
        });
    });
}


// Clear tax fields for a specific product section
function clearTaxFields($section) {
    $section.find('.HsnSacCode').val('');
    $section.find('.Price').val('');
    $section.find('.CGST').val('');
    $section.find('.SGST').val('');
    $section.find('.IGST').val('');
}

// Validate numeric input
function validateNumericInput(event) {
    var validKeys = [46, 8, 9, 27, 13];
    if (validKeys.indexOf(event.keyCode) !== -1 || (event.keyCode == 65 && event.ctrlKey === true) || (event.keyCode >= 35 && event.keyCode <= 39)) {
        return;
    }
    if ((event.shiftKey || (event.keyCode < 48 || event.keyCode > 57)) && (event.keyCode < 96 || event.keyCode > 105)) {
        event.preventDefault();
    }
}

// GST validation
function ValidateGST() {
    var gstField = document.querySelector(".GstNumber");
    if (gstField.value != "") {
        var gstPat = /^([0-9]{2}[a-zA-Z]{4}([a-zA-Z]{1}|[0-9]{1})[0-9]{4}[a-zA-Z]{1}([a-zA-Z]|[0-9]){3}){0,15}$/;
        if (gstField.value.search(gstPat) == -1) {
            swal("Invalid GST Number. It should be in this format: 11AAAAA1111Z1");
            gstField.value = '';
            return false;
        }
    }
}

//document.getElementById('addSection').addEventListener('click', function () {
//    const customerSections = document.getElementById('customerSections');
//    const newSection = document.querySelector('.customer-section').cloneNode(true);

//    // Reset values in the cloned section
//    newSection.querySelectorAll('input').forEach(input => {
//        if (input.type === 'date') {
//            input.value = ''; // Clear the date input as well
//        } else {
//            input.value = ''; // Clear the input
//        }
//        input.removeAttribute('readonly'); // Ensure the fields are editable
//    });

//    // Ensure the remove button is displayed for all cloned sections
//    newSection.querySelector('.remove-section').style.display = 'inline-block';

//    // Append the cloned section to the customer sections
//    customerSections.appendChild(newSection);

//    // Show the remove button for the original section if more than one section exists
//    if (customerSections.children.length > 1) {
//        customerSections.querySelector('.customer-section .remove-section').style.display = 'inline-block';
//    }
//});

//document.getElementById('customerSections').addEventListener('click', function (e) {
//    if (e.target.classList.contains('remove-section')) {
//        const sections = document.querySelectorAll('.customer-section');
//        if (sections.length > 1) { // Prevent removal if only one section is left
//            e.target.closest('.customer-section').remove();

//            // Hide remove button for the remaining section if it's the only one left
//            if (sections.length === 2) {
//                document.querySelector('.customer-section .remove-section').style.display = 'none';
//            }
//        } else {
//            alert('At least one section must remain.');
//        }
//    }
//});


// On page load, hide the remove button if only one section exists
//window.onload = function () {
//    const sections = document.querySelectorAll('.customer-section');
//    if (sections.length === 1) {
//        document.querySelector('.customer-section .remove-section').style.display = 'none';
//    }
//};



// For renew date
//function calculateRenewDate(event) {
//    const $section = event.target.closest('.customer-section'); // Get the closest section
//    const renewMonth = parseInt($section.querySelector('.NoOfRenewMonth').value); // Get renew month from that section
//    const startDate = $section.querySelector('.StartDate').value; // Get start date from that section

//    if (renewMonth && startDate) {
//        const start = new Date(startDate);
//        start.setMonth(start.getMonth() + renewMonth); // Add the inputted number of months

//        // Format date as YYYY-MM-DD
//        const formattedDate = start.toISOString().split('T')[0];

//        const renewDateInput = $section.querySelector('.RenewDate');
//        renewDateInput.value = formattedDate; // Set the RenewDate input
//        renewDateInput.setAttribute('readonly', true); // Ensure RenewDate is read-only
//    } else {
//        $section.querySelector('.RenewDate').value = ''; // Clear RenewDate if inputs are invalid
//    }
//}

//// Event listeners for both Start Date and Number Of Renew Month fields
//document.getElementById('customerSections').addEventListener('input', function (event) {
//    if (event.target.classList.contains('NoOfRenewMonth') || event.target.classList.contains('StartDate')) {
//        calculateRenewDate(event);
//    }
//});



//function gatherProductDetails() {
//    const productSections = document.querySelectorAll('.customer-section');
//    const productDetails = [];

//    productSections.forEach((section, index) => {
//        const productDetail = {
//            CustomerId: $('#customerId').val(),
//            ProductId: $('.ProductDetails').eq(index).val(),
//            Description: $('.Description').eq(index).val(),
//            ProductPrice: $('.Price').eq(index).val(),
//            NoOfRenewMonth: $('.NoOfRenewMonth').eq(index).val(),
//            RenewPrice: $('.RenewPrice').eq(index).val(),
//            HsnSacCode: $('.HsnSacCode').eq(index).val(),
//            StartDate: $('.StartDate').eq(index).val(),
//            RenewDate: $('.RenewDate').eq(index).val(),
//            IGST: $('.IGST').eq(index).val(),
//            SGST: $('.SGST').eq(index).val(),
//            CGST: $('.CGST').eq(index).val()
//        };

//        productDetails.push(productDetail);
//    });

//    return productDetails;
//}

function sendProductDetailsToAPI() {
    const productDetails = gatherProductDetails(); // Gather product details here

    //fetch('/Sale/Invoice', {
    //    method: 'POST',
    //    headers: {
    //        'Content-Type': 'application/json'
    //    },
    //    body: JSON.stringify(productDetails) // Send the gathered details
    //})
    //    .then(response => {
    //        if (!response.ok) {
    //            throw new Error('Network response was not ok');
    //        }
    //        return response.json();
    //    })
    //    .then(data => {
    //        console.log('Success:', data);
    //    })
    //    .catch(error => {
    //        console.error('Error:', error);
    //

    $.ajax({
        url: '/Sale/Invoice',
        type: 'POST',
        dataType: 'JSON',
        data: {
            model: gatherProductDetails()
        },
        success: function (result) {
            window.location.href = result.path;
            /*for (let k = 0; k < result.length; k++) {
                $('tbody tr:eq(' + (result[k].employeeId - 1) + ')').css('border', '3px solid #dc3545');
                let props = Object.entries(employees[result[k].employeeId])
                console.log(result[k])
                let cellIndex = props.findIndex(p => p[0] === result[k].field)
                $('tbody tr:eq(' + (result[k].employeeId - 1) + ') td:eq(' + cellIndex + ')').css({ 'background-color': '#dc3545', 'color': '#ffffff' });
                $('tbody tr:eq(' + (result[k].employeeId - 1) + ') td:eq(' + cellIndex + ')').attr("title", result[k].error)
            }
            $(".main-container").append(`
                   <div class="alert alert-success alert-dismissible fade show" role="alert">
                       Data Successfully imported into the database
                   </div>
                `)*/
        },
        error: function (result) {
            console.log(result)
        }
    })
}
    
function formatDateToMMDDYYYY(dateString) {
    if (!dateString) return '';

    const dateParts = dateString.split('-'); // Split the string into parts
    return `${dateParts[1]}/${dateParts[2]}/${dateParts[0]}`; // Return formatted date
}


var ProductList = JSON.parse(productdata);


window.onload = function () {
    if (ProductList && ProductList.length > 0) {
        const customerSections = document.getElementById('customerSections');
        const firstSection = document.querySelector('.customer-section');

        ProductList.forEach((product, index) => {
            let newSection;

            if (index === 0) {
                newSection = firstSection; // Use the existing section for the first product
            } else {
                newSection = firstSection.cloneNode(true); // Clone a new section for additional products
                customerSections.appendChild(newSection);
            }

            // Populate fields with product data
            newSection.querySelector('.Id').value = product.id;
            newSection.querySelector('.InvoiceNumber').value = product.invoiceNumber;
            newSection.querySelector('.ProductDetails').value = product.productId;
            newSection.querySelector('.Description').value = product.description;
            newSection.querySelector('.Price').value = product.productPrice;
            newSection.querySelector('.NoOfRenewMonth').value = product.noOfRenewMonth;
            newSection.querySelector('.RenewPrice').value = product.renewPrice;
            newSection.querySelector('.HsnSacCode').value = product.hsnSacCode;
            newSection.querySelector('.StartDate').value = formatDateToMMDDYYYY(product.startDate);
            newSection.querySelector('.RenewDate').value = formatDateToMMDDYYYY(product.renewDate);
            newSection.querySelector('.IGST').value = product.iGST;
            newSection.querySelector('.SGST').value = product.sGST;
            newSection.querySelector('.CGST').value = product.cGST;

            // Show/remove the remove button accordingly
            if (ProductList.length === 1) {
                newSection.querySelector('.remove-section').style.display = 'none';
            } else {
                newSection.querySelector('.remove-section').style.display = 'inline-block';
            }
        });
    }
};

// Add new section dynamically when 'Add Product' button is clicked
document.getElementById('addSection').addEventListener('click', function () {
    const customerSections = document.getElementById('customerSections');
    const firstSection = document.querySelector('.customer-section'); // Get the first section

    if (firstSection) {
        // Clone the first section
        const newSection = firstSection.cloneNode(true);

        // Reset values in the cloned section
        newSection.querySelectorAll('input').forEach(input => {
            input.value = ''; // Clear the input values
            input.removeAttribute('readonly'); // Ensure the fields are editable
        });

        // Specify which fields should be read-only or editable in the new section
        newSection.querySelector('.RenewDate').setAttribute('readonly', true); // Set RenewDate as readonly
        newSection.querySelector('.ProductDetails').removeAttribute('readonly'); // Ensure ProductDetails is editable
        newSection.querySelector('.Price').setAttribute('readonly', true); // Ensure Price is readonly
        newSection.querySelector('.NoOfRenewMonth').removeAttribute('readonly'); // Ensure NoOfRenewMonth is editable
        newSection.querySelector('.RenewPrice').removeAttribute('readonly'); // Ensure RenewPrice is editable
        newSection.querySelector('.HsnSacCode').setAttribute('readonly', true); // Ensure HsnSacCode is readonly
        newSection.querySelector('.StartDate').removeAttribute('readonly'); // Ensure StartDate is editable
        newSection.querySelector('.IGST').setAttribute('readonly', true); // Ensure IGST is readonly
        newSection.querySelector('.SGST').setAttribute('readonly', true); // Ensure SGST is readonly
        newSection.querySelector('.CGST').setAttribute('readonly', true); // Ensure CGST is readonly

        // Reset any specific elements if needed
        newSection.querySelector('.RenewDate').value = ''; // Clear RenewDate specifically
        newSection.querySelector('.remove-section').style.display = 'inline-block'; // Show remove button

        // Append the new section to the customer sections
        customerSections.appendChild(newSection);
    }
});

// Handle the remove section button
document.getElementById('customerSections').addEventListener('click', function (e) {
    if (e.target.classList.contains('remove-section')) {
        const sections = document.querySelectorAll('.customer-section');
        if (sections.length > 1) { // Prevent removal if only one section is left
            e.target.closest('.customer-section').remove();
        } else {
            alert('At least one section must remain.');
        }
    }
});

// Function to calculate and display renew date based on inputs
function calculateRenewDate(event) {
    const $section = event.target.closest('.customer-section');
    const renewMonth = parseInt($section.querySelector('.NoOfRenewMonth').value);
    const startDate = $section.querySelector('.StartDate').value;

    if (renewMonth && startDate) {
        const start = new Date(startDate);
        start.setMonth(start.getMonth() + renewMonth);

        const formattedDate = start.toISOString().split('T')[0];

        const renewDateInput = $section.querySelector('.RenewDate');
        renewDateInput.value = formattedDate;
        renewDateInput.setAttribute('readonly', true);
    } else {
        $section.querySelector('.RenewDate').value = ''; // Clear RenewDate if inputs are invalid
    }
}

// Event listeners for both Start Date and Number Of Renew Month fields
document.getElementById('customerSections').addEventListener('input', function (event) {
    if (event.target.classList.contains('NoOfRenewMonth') || event.target.classList.contains('StartDate')) {
        calculateRenewDate(event);
    }
});

// Function to gather product details from all sections
function gatherProductDetails() {
    const productSections = document.querySelectorAll('.customer-section');
    const productDetails = [];

    productSections.forEach((section) => {
        const productDetail = {
            CustomerId: $('#customerId').val(),
            Id: section.querySelector('.Id').value,
            InvoiceNumber: section.querySelector('.InvoiceNumber').value,
            ProductId: section.querySelector('.ProductDetails').value,
            Description: section.querySelector('.Description').value,
            ProductPrice: section.querySelector('.Price').value,
            NoOfRenewMonth: section.querySelector('.NoOfRenewMonth').value,
            RenewPrice: section.querySelector('.RenewPrice').value,
            HsnSacCode: section.querySelector('.HsnSacCode').value,
            StartDate: section.querySelector('.StartDate').value,
            RenewDate: section.querySelector('.RenewDate').value,
            IGST: section.querySelector('.IGST').value,
            SGST: section.querySelector('.SGST').value,
            CGST: section.querySelector('.CGST').value
        };

        productDetails.push(productDetail);
    });

    return productDetails;
}


