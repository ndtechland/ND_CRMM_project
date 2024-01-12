//function openErrorPopUp(errorMessage) {
//    var arryMessage;
//    var message = '';
//    if (errorMessage == undefined) {
//        document.location.reload(true);
//    }
//    arryMessage = errorMessage.split(",");
//    var length = arryMessage.length;

//    if (arryMessage[arryMessage.length - 1] == "") {
//        length = length - 1;
//    }
//    errorMessage = "";
//    for (var i = 0; i < length; i++) {
//        if (arryMessage[i] != '') {
//            errorMessage = errorMessage + (parseInt(i) + 1) + "." + arryMessage[i] + "<br/>";
//        }
//    }

//    message = errorMessage;
//    $('#errorMessage').html(message);
//    $('#errorMessage1').html(message);
//    errorPopUp('error');
//}

//function errorPopUp(Type) {
//    if (Type == 'success') {
//        $('#closemodal').removeClass('btn-danger');
//        $('#closemodal').addClass('btn-success');
//        $('#myModal').modal('show');
//        setTimeout(function () {
//            $("#myModal").modal('hide');
//        }, 10000);
//    }
//    else if (Type == 'error') {
//        $('#closemodal').removeClass('btn-success');
//        $('#closemodal').addClass('btn-danger');
//        $('#myModal').modal('show');
//    }
//}


//document.getElementById('btnFirstTab').addEventListener('click', function (event) {
//    debugger

//    var errormessage = '';
//    if ($("#FirstName").val().trim() == '') {
//        errormessage += 'Please Enter First Name,'
//    }
//    //if ($("#MiddleName").val().trim() == '') {
//    //    errormessage += 'Please Enter Middle Name,'
//    //}
//    if ($("#LastName").val().trim() == '') {

//        errormessage += 'Please Enter Last Name,'
//    }
//    if ($("#WorkEmail").val().trim() == '') {

//        errormessage += 'Please Enter Work Email,'
//    }
//    var email = $("#WorkEmail").val();
//    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
//    if (!emailRegex.test(email)) {
//        errormessage += 'Invalid Work Email Id,';

//    }


//    var dateOfJoining = $("#DateOfJoining").val();
//    var dateRegex = /^\d{4}-\d{2}-\d{2}$/;
//    if (!dateRegex.test(dateOfJoining)) {
//        errormessage += 'Please enter a valid date in the format mm/dd/yyyy,';

//    }
//    var parts = dateOfJoining.split("/");
//    var selectedDate = new Date(parts[2], parts[0] - 1, parts[1]);
//    var currentDate = new Date();
//    if (selectedDate > currentDate) {
//        errormessage += 'Date of Joining cannot be in the future,';
//    }
//    if ($("#ddlGender").val().trim() == '0') {

//        errormessage += 'Please Select Gender,'
//    }
//    if ($("#ddCustomerID").val().trim() == '0') {

//        errormessage += 'Please Select Company,'
//    }
//    if ($("#ddlWorkLocationID").val().trim() == '0') {

//        errormessage += 'Please Select Work Location,'
//    }
//    if ($("#ddDesignationID").val().trim() == '0') {

//        errormessage += 'Please Select Designation,'
//    }
//    if ($("#ddDepartmentID").val().trim() == '0') {

//        errormessage += 'Please Select Department,'
//    }
//    //
//    if (errormessage != '') {

//        $(".next-step").off("click");
//        openErrorPopUp(errormessage);
//        return false;

//    }
//    else {
//        return true;

//    }
//});


//document.getElementById('btnSecondTab').addEventListener('click', function (event) {
//    debugger
   
//    var errormessage = '';
//    if ($("#PersonalEmailAddress").val().trim() == '') {
//        errormessage += 'Please Enter Personal Email Address,'
//    }
//    var email = $("#PersonalEmailAddress").val();
//    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
//    if (!emailRegex.test(email)) {
//        errormessage += 'Invalid Personal Email Address,';

//    }

//    var mobileNumber = $("#MobileNumber").val();


//    var mobileRegex = /^[0-9]{10}$/;

//    if (!mobileRegex.test(mobileNumber)) {
//        errormessage += 'Please enter a valid 10-digit mobile number,'

//    }
//    var dateRegex = /^\d{4}-\d{2}-\d{2}$/;

//    var inputValue = $("#dobInput").val();

//    if (!dateRegex.test(inputValue)) {
//        errormessage += 'Please enter a valid date of birth in the format dd/mm/yyyy,'

//    }
//    if ($("#ageInput").val().trim() == '') {

//        errormessage += 'Please Enter Age,'
//    }
//    if ($("#FatherName").val().trim() == '') {

//        errormessage += 'Please Enter Father Name,'
//    }

//    if ($("#PAN").val().trim() == '') {

//        errormessage += 'Please Enter Pan,'
//    }


//    var panNumber = $('#PAN').val().trim();


//    var panRegex = /^([A-Z]){5}([0-9]){4}([A-Z]){1}?$/;

//    if (!panRegex.test(panNumber)) {
//        errormessage += 'PAN Card is not valid,'
//    }

//    if ($("#AddressLine1").val().trim() == '') {

//        errormessage += 'Please Enter First AddressLine,'
//    }
//    if ($("#AddressLine2").val().trim() == '') {

//        errormessage += 'Please Enter second AddressLine,'
//    }
//    if ($("#City").val().trim() == '') {

//        errormessage += 'Please Enter City,'
//    }
//    if ($("#ddlState").val().trim() == '0') {

//        errormessage += 'Please Select State,'
//    }
//    if ($("#Pincode").val().trim() == '') {

//        errormessage += 'Please Enter Pincode'
//    }
//    //
//    if (errormessage != '') {

//        $(".next-step").off("click");
//        openErrorPopUp(errormessage);
//        return false;
//    }
//    else {
//        return true;

//    }
//});

//document.getElementById('btnThirdTab').addEventListener('click', function (event) {
//    debugger

//    var errormessage = '';
//    if ($("#AccountHolderName").val().trim() == '') {
//        errormessage += 'Please Enter Account Holder Name,'
//    }

//    if ($("#BankName").val().trim() == '') {

//        errormessage += 'Please Enter BankName,'
//    }

//    var accountNumber = $("#AccountNumber").val();

//    var accountNumberRegex = /^\d+$/;

//    if (!accountNumberRegex.test(accountNumber)) {
//        errormessage += 'Account Number is not valid,'
//    }

//    var reEnterAccountNumber = $("#ReEnterAccountNumber").val();

//    var ReEnterAccountNumberRegex = /^\d+$/;

//    if (!ReEnterAccountNumberRegex.test(reEnterAccountNumber)) {
//        errormessage += 'reEnterAccountNumber Number is not valid,'
//    }

//    var ifsc = $("#IFSC").val();
//    var ifscRegex = /^[A-Z]{4}[0][A-Z0-9]{6}$/;

//    if (!ifscRegex.test(ifsc)) {
//        errormessage += 'IFSC Code is  not valid,'
//    }


//    var EPF = $("#EPF_Number").val();
//    var epfRegex = /^[A-Z]{3}\/[A-Z]{3}\/\d{6}\/\d{2}$/;

//    if (!epfRegex.test(EPF)) {
//        errormessage += 'EPF Number is not valid,'
//    }

//    if ($("#Employee_Contribution_Rate").val().trim() == '') {

//        errormessage += 'Please Enter Employee Contribution Rate,'
//    }



//    if ($("#ddlDeductionTime").val().trim() == '0') {

//        errormessage += 'Please Select Deduction Cycle,'
//    }


//    var selectedAccountType = $("input[name='AccountTypeID']:checked").val();

//    if (!selectedAccountType) {
//        errormessage += 'Please choose Account'

//    }


//    if (errormessage != '') {

//        $(".next-step").off("click");
//        openErrorPopUp(errormessage);
//        return false;

//    }
//    else {
//        return true;

//    }
//});
//function ValidateFirstTab() {
//    var errormessage = '';
//    if ($("#FirstName").val().trim() == '') {
//        errormessage += 'Please Enter First Name,'
//    }
//    if ($("#MiddleName").val().trim() == '') {
//        errormessage += 'Please Enter Middle Name,'
//    }
//    if ($("#LastName").val().trim() == '') {

//        errormessage += 'Please Enter Last Name,'
//    }
//    if ($("#WorkEmail").val().trim() == '') {

//        errormessage += 'Please Enter Work Email,'
//    }
//    var email = $("#WorkEmail").val();
//    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
//    if (!emailRegex.test(email)) {
//        errormessage += 'Invalid Work Email Id,';

//    }

//    var dateOfJoining = $("#DateOfJoining").val();
//    var dateRegex = /^\d{4}-\d{2}-\d{2}$/;
//    if (!dateRegex.test(dateOfJoining)) {
//        errormessage += 'Please enter a valid date in the format mm/dd/yyyy,';

//    }


//    if ($("#ddlGender").val().trim() == '0') {

//        errormessage += 'Please Select Gender,'
//    }
//    if ($("#ddCustomerID").val().trim() == '0') {

//        errormessage += 'Please Select Company,'
//    }
//    if ($("#ddlWorkLocationID").val().trim() == '0') {

//        errormessage += 'Please Select Work Location,'
//    }
//    if ($("#ddDesignationID").val().trim() == '0') {

//        errormessage += 'Please Select Designation,'
//    }
//    if ($("#ddDepartmentID").val().trim() == '0') {

//        errormessage += 'Please Select Department,'
//    }
//    return errormessage;
//}

//function ValidateSecondTab() {
//    var errormessage = '';
//    if ($("#PersonalEmailAddress").val().trim() == '') {
//        errormessage += 'Please Enter Personal Email Address,'
//    }

//    var email = $("#PersonalEmailAddress").val();
//    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
//    if (!emailRegex.test(email)) {
//        errormessage += 'Invalid Personal Email Address,';

//    }


//    var mobileNumber = $("#MobileNumber").val();


//    var mobileRegex = /^[0-9]{10}$/;

//    if (!mobileRegex.test(mobileNumber)) {
//        errormessage += 'Please enter a valid 10-digit mobile number,'

//    }
//    var dateRegex = /^\d{4}-\d{2}-\d{2}$/;

//    var inputValue = $("#dobInput").val();

//    if (!dateRegex.test(inputValue)) {
//        errormessage += 'Please enter a valid date of birth in the format dd/mm/yyyy,'

//    }
//    if ($("#ageInput").val().trim() == '') {

//        errormessage += 'Please Enter Age,'
//    }
//    if ($("#FatherName").val().trim() == '') {

//        errormessage += 'Please Enter Father Name,'
//    }

//    if ($("#PAN").val().trim() == '') {

//        errormessage += 'Please Enter Pan,'
//    }


//    var panNumber = $('#PAN').val().trim();


//    var panRegex = /^([A-Z]){5}([0-9]){4}([A-Z]){1}?$/;

//    if (!panRegex.test(panNumber)) {
//        errormessage += 'PAN Card is not valid,'
//    }

//    if ($("#AddressLine1").val().trim() == '') {

//        errormessage += 'Please Enter First AddressLine,'
//    }
//    if ($("#AddressLine2").val().trim() == '') {

//        errormessage += 'Please Enter second AddressLine,'
//    }
//    if ($("#City").val().trim() == '') {

//        errormessage += 'Please Enter City,'
//    }
//    if ($("#ddlState").val().trim() == '0') {

//        errormessage += 'Please Select State,'
//    }
//    if ($("#Pincode").val().trim() == '') {

//        errormessage += 'Please Enter Pincode'
//    }
//    return errormessage;
//}

//function ValidateThirdTab() {
//    var errormessage = '';
//    if ($("#AccountHolderName").val().trim() == '') {
//        errormessage += 'Please Enter Account Holder Name,'
//    }

//    if ($("#BankName").val().trim() == '') {

//        errormessage += 'Please Enter BankName,'
//    }

//    var accountNumber = $("#AccountNumber").val();

//    var accountNumberRegex = /^\d+$/;

//    if (!accountNumberRegex.test(accountNumber)) {
//        errormessage += 'Account Number is not valid,'
//    }

//    var reEnterAccountNumber = $("#ReEnterAccountNumber").val();

//    var ReEnterAccountNumberRegex = /^\d+$/;

//    if (!ReEnterAccountNumberRegex.test(reEnterAccountNumber)) {
//        errormessage += 'reEnterAccountNumber Number is not valid,'
//    }

//    var ifsc = $("#IFSC").val();
//    var ifscRegex = /^[A-Z]{4}[0][A-Z0-9]{6}$/;

//    if (!ifscRegex.test(ifsc)) {
//        errormessage += 'IFSC Code is  not valid,'
//    }


//    var EPF = $("#EPF_Number").val();
//    var epfRegex = /^[A-Z]{3}\/[A-Z]{3}\/\d{6}\/\d{2}$/;

//    if (!epfRegex.test(EPF)) {
//        errormessage += 'EPF Number is not valid,'
//    }

//    if ($("#Employee_Contribution_Rate").val().trim() == '') {

//        errormessage += 'Please Enter Employee Contribution Rate,'
//    }



//    if ($("#ddlDeductionTime").val().trim() == '0') {

//        errormessage += 'Please Select Deduction Cycle,'
//    }


//    var selectedAccountType = $("input[name='AccountTypeID']:checked").val();

//    if (!selectedAccountType) {
//        errormessage += 'Please choose Account'

//    }

//    return errormessage;
//}

//function ValidateMainTab() {
//    var errormessage = '';

//    var AnnualCTC = $("#txtAnnualCTC").val();
//    var ctcRegex = /^\d+(\.\d{1,2})?$/;

//    if (!ctcRegex.test(AnnualCTC)) {
//        errormessage += 'Please Enter valid Annual CTC,'

//    }


//    if ($("#TravellingAllowance").val().trim() == '') {

//        errormessage += 'Please Enter Conveyance Allowance,'
//    }

//    if ($("#ESIC").val().trim() == '') {

//        errormessage += 'Please Enter Fixed Allowance'
//    }

//    return errormessage;
//}
//function fnValidate() {

//    var errormsg1 = ValidateFirstTab();
//    var errormsg2 = ValidateSecondTab();
//    var errormsg3 = ValidateThirdTab();
//    var errormsg4 = ValidateMainTab();
//    if (errormsg1 != '') {


//        openErrorPopUp(errormsg1);
//        return false;

//    }
//    if (errormsg2 != '') {


//        openErrorPopUp(errormsg2);
//        return false;

//    }
//    if (errormsg3 != '') {


//        openErrorPopUp(errormsg3);
//        return false;

//    }
//    if (errormsg4 != '') {


//        openErrorPopUp(errormsg4);
//        return false;

//    }
//    return true;
//}
