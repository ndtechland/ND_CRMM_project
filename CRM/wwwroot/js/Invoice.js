$(document).ready(function () {
    // Handle customer search on keyup
    $('#customerName').on('keyup', function () {
        var searchTerm = $(this).val();
        var cloneId = true;
        if (searchTerm.length >= 1) {
            $.ajax({
                url: '/Sale/GetCustomerNames',
                type: 'GET',
                data: { searchTerm: searchTerm },
                success: function (data) {
                    $('#customerSuggestions').empty();
                    if (data.length > 0) {
                        $.each(data, function (index, customer) {
                            $('#customerSuggestions').append(`<a href="#" class="list-group-item list-group-item-action" onclick="selectCustomer(${customer.id}, '${customer.companyName}','${cloneId}')">${customer.companyName}</a>`);
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
        var cloneId = true;
        var InvoiceID = $('#Id').val();
        if (customerId) {
            fetchCustomerDetails(customerId, cloneId, InvoiceID);
        }
    });
   
    var customerId = $("#customerId").val();
    var customerName = $("#customerName").val();
    var cloneId = $("#cloneId").val();
    var InvoiceID = $('#Id').val();
    if (customerId != '' && customerName != '' && cloneId != '' && InvoiceID != '') {
        selectCustomer(customerId, customerName, cloneId, InvoiceID);
    }
           


    // Numeric validation for input fields
    $(".numeric-input").on("keydown", function (event) {
        validateNumericInput(event);
    });
});

// Handle customer selection
function selectCustomer(customerId, customerName, cloneId, InvoiceID) {
    $('#customerName').val(customerName);
    $('#customerId').val(customerId);
    $('#Id').val(InvoiceID);
    $('#cloneId').val(cloneId);
    $('#customerSuggestions').empty();
    //const clone = $('#cloneId').val();
    fetchCustomerDetails(customerId, cloneId, InvoiceID);
}

// Fetch customer details by ID
function fetchCustomerDetails(customerId, cloneId, InvoiceID) {
    $.ajax({
        url: '/Sale/GetCustomerDetailsById',
        type: 'GET',
        data: { id: customerId, clone: cloneId, InvoiceID: InvoiceID },
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
            $('.billingStateId').text(customerData.billingStateId); 
            $('.InvoiceNumber').val(customerData.invoiceNumber); 
                updateTaxFields(); 
            $('#customerDetailsRow').removeClass('d-none'); // Show customer details
        },
        error: function () {
            console.log('Error fetching customer details.');
        }
    });
}

//Update tax fields based on product and billing state
function updateTaxFields() {
    $(document).on('change', '.ProductDetails', function () {
        var $section = $(this).closest('.customer-section'); // Scope to the current section
        var productId = $section.find(".ProductDetails").val();
        var billingStateId = $('.billingStateId').text(); // Common billing state ID
        var InvoiceID = $section.find('#Id').val();

        if (!productId || !billingStateId) {
            clearTaxFields($section); // Clear fields for this section only
            return;
        }

        $.ajax({
            url: '/Sale/product?id=' + productId + '&invoiceId=' + InvoiceID,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response.data != null) {
                    // Update specific fields for this product section
                    $section.find('.HsnSacCode').val(response.data.hsnSacCode).attr('readonly', true);
                    $section.find('.Price').val(response.data.productPrice);

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
                    clearTaxFields($section); // Clear fields if no data returned
                }
            },
            error: function () {
                alert('Error fetching data');
                clearTaxFields($section); // Clear fields on error
            }
        });
    });
}

function clearTaxFields($section) {
    $section.find('.HsnSacCode, .Price, .CGST, .SGST, .IGST').val('').attr('readonly', true);
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
    if ([67, 86, 88].includes(event.keyCode) && (event.ctrlKey || event.metaKey) ||
        [32, 46, 8, 9, 27, 13].includes(event.keyCode) ||
        (event.keyCode >= 35 && event.keyCode <= 39)) return;

    if ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105)) return;
    event.preventDefault();
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


function sendProductDetailsToAPI() {
    const productDetails = gatherProductDetails(); 

    $.ajax({
        url: '/Sale/Invoice',
        type: 'POST',
        dataType: 'JSON',
        data: {
            model: gatherProductDetails(),
            InvoiceDate: $('#Invoicedate').val(),
            InvoiceDueDate: $('#InvoiceDueDate').val(),
            InvoiceNotes: $('#InvoiceNotes').val(),
            InvoiceTerms: $('#InvoiceTerms').val(),
            Invoiceclone: $('#InvoicecloneId').val(),
            ServiceCharges: $('#ServiceCharges').val(),

        },
        success: function (result) {
            window.location.href = result.path;
        },
        error: function (result) {
            console.log(result)
        }
    })
}
    
//function formatDateToMMDDYYYY(dateString) {
//    if (!dateString) return '';

//    const dateParts = dateString.split('-');  
//    return `${dateParts[1]}/${dateParts[2]}/${dateParts[0]}`;  
//}

function formatDateToYYYYMMDD(dateString) {
    if (!dateString) return '';

    // Split the date string at 'T' and return the first part (the date)
    return dateString.split('T')[0];
}

var ProductList = JSON.parse(productdata.replace(/[\u0000-\u0019]+/g, ""));



window.onload = function () {
//$(document).ready(function () {
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
            newSection.querySelector('.PriceQty').value = product.productQty;
            newSection.querySelector('.NoOfRenewMonth').value = product.noOfRenewMonth;
            newSection.querySelector('.RenewPrice').value = product.renewPrice;
            newSection.querySelector('.HsnSacCode').value = product.hsncode;
            newSection.querySelector('.StartDate').value = formatDateToYYYYMMDD(product.startDate);
            newSection.querySelector('.RenewDate').value = formatDateToYYYYMMDD(product.renewDate);
            newSection.querySelector('.IGST').value = product.igst;
            newSection.querySelector('.SGST').value = product.sgst;
            newSection.querySelector('.CGST').value = product.cgst;
            //newSection.querySelector('.Dueamountdate').value = formatDateToYYYYMMDD(product.dueamountdate);
           
            // Show/remove the remove button accordingly
            if (ProductList.length === 1) {
                newSection.querySelector('.remove-section').style.display = 'none';
            } else {
                newSection.querySelector('.remove-section').style.display = 'inline-block';
            }
        });
    }
/*});*/
    
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
        newSection.querySelector('.Price'); 
        newSection.querySelector('.PriceQty').value = '1';
        newSection.querySelector('.NoOfRenewMonth').removeAttribute('readonly'); // Ensure NoOfRenewMonth is editable
        newSection.querySelector('.RenewPrice').removeAttribute('readonly'); // Ensure RenewPrice is editable
        newSection.querySelector('.HsnSacCode').setAttribute('readonly', true); // Ensure HsnSacCode is readonly
        newSection.querySelector('.StartDate').removeAttribute('readonly'); // Ensure StartDate is editable
        newSection.querySelector('.IGST').setAttribute('readonly', true); 
        newSection.querySelector('.SGST').setAttribute('readonly', true); 
        newSection.querySelector('.CGST').setAttribute('readonly', true); 
        
        // Reset any specific elements if needed
        newSection.querySelector('.RenewDate').value = ''; // Clear RenewDate specifically
        newSection.querySelector('.remove-section').style.display = 'inline-block'; // Show remove button

        // Append the new section to the customer sections
        customerSections.appendChild(newSection);
        Swal.fire({
            icon: 'success',
            title: 'Section Added',
            text: 'New section added successfully!',
            timer: 2000,
            timerProgressBar: true,
            showConfirmButton: false 
        });

    }
});

// Handle the remove section button
//document.getElementById('customerSections').addEventListener('click', function (e) {
//    if (e.target.classList.contains('remove-section')) {
//        const sections = document.querySelectorAll('.customer-section');
//        const section = e.target.closest('.customer-section');
//        const sectionIdInput = section.querySelector('.Id'); // Get the hidden input within the section
//        const sectionIdValue = sectionIdInput ? sectionIdInput.value : 0;
//        if (sections.length > 1) { // Prevent removal if only one section is left
//            const confirmed = confirm('Are you sure you want to delete this Product?');
//            if (confirmed) {
//                e.target.closest('.customer-section').remove();
//                if (sectionIdValue > 0) {
//                    $.ajax({
//                        url: '/Sale/DeleteProdbyUpdate',
//                        type: 'POST',
//                        dataType: 'JSON',
//                        data: {
//                            id: sectionIdValue
//                        },
//                        success: function (result) {
//                            //window.location.href = result.path;
//                            location.reload
//                        },
//                        error: function (result) {
//                            console.log(result)
//                        }
//                    })
//                }
//            }
//        } else {
//            alert('At least one section must remain.');
//        }
//    }
//});
document.getElementById('customerSections').addEventListener('click', function (e) {
    if (e.target.classList.contains('remove-section')) {
        const sections = document.querySelectorAll('.customer-section');
        const section = e.target.closest('.customer-section');
        const sectionIdInput = section.querySelector('.Id'); // Get the hidden input within the section
        const sectionIdValue = sectionIdInput ? sectionIdInput.value : 0;
        var cloneId = $("#cloneId").val();
       
        if (sections.length > 1) { // Prevent removal if only one section is left
            Swal.fire({
                title: 'Are you sure?',
                text: 'Do you want to delete this product?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, delete it!',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    section.remove(); // Remove the section immediately
                    if (sectionIdValue > 0) {
                        $.ajax({
                            url: '/Sale/DeleteProdbyUpdate',
                            type: 'POST',
                            dataType: 'JSON',
                            data: { id: sectionIdValue, clone: cloneId },
                            success: function (response) {
                                if (response.success) {
                                    Swal.fire({
                                        title: 'Deleted!',
                                        text: response.message || 'Product deleted successfully.',
                                        icon: 'success'
                                    }).then(() => {
                                        if (response.redirectUrl) {
                                            window.location.href = response.redirectUrl;
                                         }
                                        else {
                                            window.location.reload();
                                        }
                                    });
                                } else {
                                    Swal.fire({
                                        title: 'Error',
                                        text: response.message || 'Failed to delete the product.',
                                        icon: 'error'
                                    }).then(() => {
                                        if (response.redirectUrl) {
                                            window.location.href = response.redirectUrl;
                                        }
                                        else {
                                            window.location.reload();
                                        }
                                    });
                                }
                            },
                            error: function (xhr, status, error) {
                                console.error(`Error: ${error}`);
                                Swal.fire({
                                    title: 'Error',
                                    text: 'An error occurred while deleting the product. Please try again.',
                                    icon: 'error'
                                });
                            }
                        });
                    }
                }
            });
        } else {
            Swal.fire({
                title: 'Cannot Delete',
                text: 'At least one section must remain.',
                icon: 'info',
                confirmButtonText: 'OK'
            });
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
            Qty: section.querySelector('.PriceQty').value,
            NoOfRenewMonth: section.querySelector('.NoOfRenewMonth').value,
            RenewPrice: section.querySelector('.RenewPrice').value,
            HsnSacCode: section.querySelector('.HsnSacCode').value,
            StartDate: section.querySelector('.StartDate').value,
            RenewDate: section.querySelector('.RenewDate').value,
            //Dueamountdate: section.querySelector('.Dueamountdate').value,
            IGST: section.querySelector('.IGST').value,
            SGST: section.querySelector('.SGST').value,
            CGST: section.querySelector('.CGST').value
            
        };

        productDetails.push(productDetail);
    });

    return productDetails;
}


