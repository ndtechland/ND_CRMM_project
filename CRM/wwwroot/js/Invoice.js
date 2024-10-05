﻿$(document).ready(function () {
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

// Update tax fields based on product and billing state
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
            url: '/Home/product?id=' + productId,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response.data != null) {
                    // Update specific fields for this product section
                    $section.find('.HsnSacCode').val(response.data.hsnSacCode).attr('readonly', true);
                    $section.find('.Price').val(response.data.price).attr('readonly', true);

                    // Update GST fields based on state match
                    if ('@ViewBag.checkvendorbillingstateid' === billingStateId) {
                        $section.find('.CGST').val(response.data.cgst).attr('readonly', true);
                        $section.find('.SGST').val(response.data.scgst).attr('readonly', true);
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


document.getElementById('addSection').addEventListener('click', function () {
    const customerSections = document.getElementById('customerSections');
    const newSection = document.querySelector('.customer-section').cloneNode(true);

    // Reset values in the cloned section
    newSection.querySelectorAll('input').forEach(input => {
        if (input.type === 'date') {
            input.value = ''; // Clear the date input as well
        } else {
            input.value = ''; // Clear the input
        }
        input.removeAttribute('readonly'); // Ensure the fields are editable
    });

    // Ensure the remove button is displayed for all cloned sections
    newSection.querySelector('.remove-section').style.display = 'inline-block';

    // Append the cloned section to the customer sections
    customerSections.appendChild(newSection);

    // Show the remove button for the original section if more than one section exists
    if (customerSections.children.length > 1) {
        customerSections.querySelector('.customer-section .remove-section').style.display = 'inline-block';
    }
});

document.getElementById('customerSections').addEventListener('click', function (e) {
    if (e.target.classList.contains('remove-section')) {
        const sections = document.querySelectorAll('.customer-section');
        if (sections.length > 1) { // Prevent removal if only one section is left
            e.target.closest('.customer-section').remove();

            // Hide remove button for the remaining section if it's the only one left
            if (sections.length === 2) {
                document.querySelector('.customer-section .remove-section').style.display = 'none';
            }
        } else {
            alert('At least one section must remain.');
        }
    }
});


// On page load, hide the remove button if only one section exists
window.onload = function () {
    const sections = document.querySelectorAll('.customer-section');
    if (sections.length === 1) {
        document.querySelector('.customer-section .remove-section').style.display = 'none';
    }
};



// For renew date
function calculateRenewDate(event) {
    const $section = event.target.closest('.customer-section'); // Get the closest section
    const renewMonth = parseInt($section.querySelector('.NoOfRenewMonth').value); // Get renew month from that section
    const startDate = $section.querySelector('.StartDate').value; // Get start date from that section

    if (renewMonth && startDate) {
        const start = new Date(startDate);
        start.setMonth(start.getMonth() + renewMonth); // Add the inputted number of months

        // Format date as YYYY-MM-DD
        const formattedDate = start.toISOString().split('T')[0];

        const renewDateInput = $section.querySelector('.RenewDate');
        renewDateInput.value = formattedDate; // Set the RenewDate input
        renewDateInput.setAttribute('readonly', true); // Ensure RenewDate is read-only
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



function gatherProductDetails() {
    const productSections = document.querySelectorAll('.customer-section');
    const productDetails = [];

    productSections.forEach((section, index) => {
        const productDetail = {
            CustomerId: $('#customerId').val(),
            ProductId: $('.ProductDetails').eq(index).val(),
            ProductPrice: $('.Price').eq(index).val(),
            NoOfRenewMonth: $('.NoOfRenewMonth').eq(index).val(),
            RenewPrice: $('.RenewPrice').eq(index).val(),
            HsnSacCode: $('.HsnSacCode').eq(index).val(),
            StartDate: $('.StartDate').eq(index).val(),
            RenewDate: $('.RenewDate').eq(index).val(),
            IGST: $('.IGST').eq(index).val(),
            SGST: $('.SGST').eq(index).val(),
            CGST: $('.CGST').eq(index).val()
        };

        productDetails.push(productDetail);
    });

    return productDetails;
}

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
    


// Call the function when you want to gather the details and send them to the API
// sendProductDetailsToAPI(); // Uncomment to call the function when needed