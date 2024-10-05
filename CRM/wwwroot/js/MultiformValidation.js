// Function to navigate to the specified step
const navigateToFormStep = (stepNumber) => {
    document.querySelectorAll(".form-step").forEach((formStepElement) => {
        formStepElement.classList.add("d-none");
    });
    document.querySelectorAll(".form-stepper-list").forEach((formStepHeader) => {
        formStepHeader.classList.add("form-stepper-unfinished");
        formStepHeader.classList.remove("form-stepper-active", "form-stepper-completed");
    });
    document.querySelector("#step-" + stepNumber).classList.remove("d-none");

    const formStepCircle = document.querySelector('li[step="' + stepNumber + '"]');
    formStepCircle.classList.remove("form-stepper-unfinished", "form-stepper-completed");
    formStepCircle.classList.add("form-stepper-active");

    for (let index = 1; index < stepNumber; index++) {
        const formStepCircle = document.querySelector('li[step="' + index + '"]');
        if (formStepCircle) {
            formStepCircle.classList.remove("form-stepper-unfinished", "form-stepper-active");
            formStepCircle.classList.add("form-stepper-completed");
        }
    }
};

// Function to validate step 1 fields
const validateStep1 = () => {
    let isValid = true;

    const requiredFields = [
        { id: "FirstName", errorId: "FirstNameError" },
        { id: "DateOfJoining", errorId: "DateOfJoiningError" },
        { id: "ddlGender", errorId: "GenderError", isSelect: true },
        { id: "ddlStates", errorId: "StateError", isSelect: true },
        { id: "ddlCity", errorId: "WorkLocationError", isSelect: true },
        { id: "ddDepartmentID", errorId: "DepartmentError", isSelect: true },
        { id: "ddDesignationID", errorId: "DesignationError", isSelect: true },
        { id: "ddOfferletterid", errorId: "OfferLetterError", isSelect: true },
        { id: "ddshifttypeidid", errorId: "ShiftTypeError", isSelect: true }
    ];

    requiredFields.forEach(({ id, errorId, isSelect }) => {
        const field = document.getElementById(id);
        const errorField = document.getElementById(errorId);
        if ((isSelect && field.value === "0") || !field.value) {
            errorField.style.display = "block";
            isValid = false;
        } else {
            errorField.style.display = "none";
        }
    });

    return isValid;
};

// Function to validate step 2 fields
const validateStep2 = () => {
    let isValid = true;
    // Get form field values
    const firstName = document.getElementById("FirstName");
    //const middleName = document.getElementById("MiddleName");
    //const lastName = document.getElementById("LastName");
    //const workEmail = document.getElementById("WorkEmail");
    const dateOfJoining = document.getElementById("DateOfJoining");
    const gender = document.getElementById("ddlGender");
    const state = document.getElementById("ddlStates");
    const workLocation = document.getElementById("ddlCity");
    const department = document.getElementById("ddDepartmentID");
    const designation = document.getElementById("ddDesignationID");
    const offerLetter = document.getElementById("ddOfferletterid");
    const officeShiftType = document.getElementById("ddshifttypeidid");

    // Validate fields
    if (!firstName.value) {
        document.getElementById("FirstNameError").style.display = "block";
    // Personal Email Address validation
    const personalEmail = document.getElementById("PersonalEmailAddress");
    const personalEmailError = document.getElementById("PersonalEmailAddressError");
    if (!personalEmail.value || !validateEmail(personalEmail.value)) {
        personalEmailError.style.display = "block";
        isValid = false;
    } else {
        personalEmailError.style.display = "none";
    }
    //if (!middleName.value) {
    //    document.getElementById("MiddleNameError").style.display = "block";
    //    isValid = false;
    //} else {
    //    document.getElementById("MiddleNameError").style.display = "none";
    //}

    //if (!lastName.value) {
    //    document.getElementById("LastNameError").style.display = "block";
    //    isValid = false;
    //} else {
    //    document.getElementById("LastNameError").style.display = "none";
    //}

    //if (!workEmail.value) {
    //    document.getElementById("WorkEmailError").style.display = "block";
    //    isValid = false;
    //} else {
    //    document.getElementById("WorkEmailError").style.display = "none";
    //}
    // Mobile Number validation
    const mobileNumber = document.getElementById("MobileNumber");
    const mobileNumberError = document.getElementById("MobileNumberError");
    if (!mobileNumber.value || mobileNumber.value.length !== 10 || isNaN(mobileNumber.value)) {
        mobileNumberError.style.display = "block";
        isValid = false;
    } else {
        mobileNumberError.style.display = "none";
    }

    // Date of Birth validation
    const dateOfBirth = document.getElementById("dobInput");
    const dateOfBirthError = document.getElementById("DateOfBirthError");
    if (!dateOfBirth.value) {
        dateOfBirthError.style.display = "block";
        isValid = false;
    } else {
        dateOfBirthError.style.display = "none";
    }

    // Father Name validation
    const fatherName = document.getElementById("FatherName");
    const fatherNameError = document.getElementById("FatherNameError");
    if (!fatherName.value) {
        fatherNameError.style.display = "block";
        isValid = false;
    } else {
        fatherNameError.style.display = "none";
    }

    // PAN validation
    const pan = document.getElementById("PAN");
    const panError = document.getElementById("PANError");
    const panRegex = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/;
    if (!pan.value || !panRegex.test(pan.value)) {
        panError.style.display = "block";
        isValid = false;
    } else {
        panError.style.display = "none";
    }

    // Address Line 1 validation
    const addressLine1 = document.getElementById("AddressLine1");
    const addressLine1Error = document.getElementById("AddressLine1Error");
    if (!addressLine1.value) {
        addressLine1Error.style.display = "block";
        isValid = false;
    } else {
        addressLine1Error.style.display = "none";
    }

    // State validation
    const stateID = document.getElementById("ddlState");
    const stateIDError = document.getElementById("StateIDError");
    if (stateID.value === "0") {
        stateIDError.style.display = "block";
        isValid = false;
    } else {
        stateIDError.style.display = "none";
    }

    // City validation
    const city = document.getElementById("City");
    const cityError = document.getElementById("CityError");
    if (city.value === "0") {
        cityError.style.display = "block";
        isValid = false;
    } else {
        cityError.style.display = "none";
    }

    // Pincode validation
    const pincode = document.getElementById("Pincode");
    const pincodeError = document.getElementById("PincodeError");
    if (!pincode.value || pincode.value.length !== 6 || isNaN(pincode.value)) {
        pincodeError.style.display = "block";
        isValid = false;
    } else {
        pincodeError.style.display = "none";
    }

    return isValid;
};

// Email validation helper function
const validateEmail = (email) => {
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailPattern.test(email);
};

// Function to restrict numeric input in pincode field
const validateNumericInput = (event) => {
    const keyCode = event.keyCode || event.which;
    if (keyCode < 48 || keyCode > 57) {
        event.preventDefault();
    }
};
const validateStep3 = () => {
    let isValid = true;

    // Account Holder Name validation
    const accountHolderName = document.getElementById("AccountHolderName");
    const accountHolderNameError = document.getElementById("AccountHolderNameError");
    if (!accountHolderName.value.trim()) {
        accountHolderNameError.style.display = "block";
        isValid = false;
    } else {
        accountHolderNameError.style.display = "none";
    }

    // Bank Name validation
    const bankName = document.getElementById("BankName");
    const bankNameError = document.getElementById("BankNameError");
    if (!bankName.value.trim()) {
        bankNameError.style.display = "block";
        isValid = false;
    } else {
        bankNameError.style.display = "none";
    }

    // Account Number validation
    const accountNumber = document.getElementById("AccountNumber");
    const accountNumberError = document.getElementById("AccountNumberError");
    if (!accountNumber.value.trim()) {
        accountNumberError.style.display = "block";
        isValid = false;
    } else {
        accountNumberError.style.display = "none";
    }

    // Re-enter Account Number validation
    const reEnterAccountNumber = document.getElementById("ReEnterAccountNumber");
    const reEnterAccountNumberError = document.getElementById("ReEnterAccountNumberError");
    if (reEnterAccountNumber.value.trim() !== accountNumber.value.trim()) {
        reEnterAccountNumberError.style.display = "block";
        isValid = false;
    } else {
        reEnterAccountNumberError.style.display = "none";
    }

    // IFSC validation
    const ifsc = document.getElementById("IFSC");
    const ifscError = document.getElementById("IFSCError");
    if (!ifsc.value.trim()) {
        ifscError.style.display = "block";
        isValid = false;
    } else {
        ifscError.style.display = "none";
    }

    // Deduction Cycle validation
    const deductionCycle = document.getElementById("ddlDeductionTime");
    const deductionCycleError = document.getElementById("DeductionCycleError");
    if (deductionCycle.value === "0") {
        deductionCycleError.style.display = "block";
        isValid = false;
        deductionCycleError.style.display = "none";
    }

    return isValid;
};
document.querySelectorAll(".btn-navigate-form-step").forEach((formNavigationBtn) => {
    formNavigationBtn.addEventListener("click", () => {
        const stepNumber = parseInt(formNavigationBtn.getAttribute("step_number"));
        if (stepNumber === 2 && !validateStep1()) {
            console.log("Form is invalid, staying on the current step.");
            return; // Stop navigation if validation fails
        }
        // Validate the current step before navigating to the next
        if (stepNumber === 3 && !validateStep2()) {
            console.log("Step 2 form is invalid, staying on the current step.");
            return; // Stop navigation if validation fails
        }

        // Validate the third step before navigating to the next
        if (stepNumber === 4 && !validateStep3()) {
            console.log("Step 3 form is invalid, staying on the current step.");
            return; // Stop navigation if validation fails
        }

        // Move to the next step if validation succeeds
        navigateToFormStep(stepNumber);
    });
});
