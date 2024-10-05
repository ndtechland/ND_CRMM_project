// Function to navigate to the specified step
const navigateToFormStep = (stepNumber) => {
    document.querySelectorAll(".form-step").forEach((formStepElement) => {
        formStepElement.classList.add("d-none"); // Hide all steps
    });
    document.querySelectorAll(".form-stepper-list").forEach((formStepHeader) => {
        formStepHeader.classList.add("form-stepper-unfinished");
        formStepHeader.classList.remove("form-stepper-active", "form-stepper-completed");
    });

    document.querySelector("#step-" + stepNumber).classList.remove("d-none"); // Show the desired step

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

    // Validate personal email address
    const personalEmail = document.getElementById("PersonalEmailAddress");
    const personalEmailError = document.getElementById("PersonalEmailAddressError");
    if (!personalEmail.value || !validateEmail(personalEmail.value)) {
        personalEmailError.style.display = "block";
        isValid = false;
    } else {
        personalEmailError.style.display = "none";
    }

    // Validate mobile number
    const mobileNumber = document.getElementById("MobileNumber");
    const mobileNumberError = document.getElementById("MobileNumberError");
    if (!mobileNumber.value || mobileNumber.value.length !== 10 || isNaN(mobileNumber.value)) {
        mobileNumberError.style.display = "block";
        isValid = false;
    } else {
        mobileNumberError.style.display = "none";
    }

    // Validate date of birth
    const dateOfBirth = document.getElementById("dobInput");
    const dateOfBirthError = document.getElementById("DateOfBirthError");
    if (!dateOfBirth.value) {
        dateOfBirthError.style.display = "block";
        isValid = false;
    } else {
        dateOfBirthError.style.display = "none";
    }

    // Validate father's name
    const fatherName = document.getElementById("FatherName");
    const fatherNameError = document.getElementById("FatherNameError");
    if (!fatherName.value) {
        fatherNameError.style.display = "block";
        isValid = false;
    } else {
        fatherNameError.style.display = "none";
    }

    // Validate PAN number
    const pan = document.getElementById("PAN");
    const panError = document.getElementById("PANError");
    const panRegex = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/;
    if (!pan.value || !panRegex.test(pan.value)) {
        panError.style.display = "block";
        isValid = false;
    } else {
        panError.style.display = "none";
    }

    // Validate address line 1
    const addressLine1 = document.getElementById("AddressLine1");
    const addressLine1Error = document.getElementById("AddressLine1Error");
    if (!addressLine1.value) {
        addressLine1Error.style.display = "block";
        isValid = false;
    } else {
        addressLine1Error.style.display = "none";
    }

    // Validate state
    const stateID = document.getElementById("ddlState");
    const stateIDError = document.getElementById("StateIDError");
    if (stateID.value === "0") {
        stateIDError.style.display = "block";
        isValid = false;
    } else {
        stateIDError.style.display = "none";
    }

    // Validate city
    const city = document.getElementById("City");
    const cityError = document.getElementById("CityError");
    if (city.value === "0") {
        cityError.style.display = "block";
        isValid = false;
    } else {
        cityError.style.display = "none";
    }

    // Validate pincode
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

// Function to validate step 3 fields
const validateStep3 = () => {
    let isValid = true;

    // Validate account holder name
    const accountHolderName = document.getElementById("AccountHolderName");
    const accountHolderNameError = document.getElementById("AccountHolderNameError");
    if (!accountHolderName.value.trim()) {
        accountHolderNameError.style.display = "block";
        isValid = false;
    } else {
        accountHolderNameError.style.display = "none";
    }

    // Validate bank name
    const bankName = document.getElementById("BankName");
    const bankNameError = document.getElementById("BankNameError");
    if (!bankName.value.trim()) {
        bankNameError.style.display = "block";
        isValid = false;
    } else {
        bankNameError.style.display = "none";
    }

    // Validate account number
    const accountNumber = document.getElementById("AccountNumber");
    const accountNumberError = document.getElementById("AccountNumberError");
    if (!accountNumber.value.trim()) {
        accountNumberError.style.display = "block";
        isValid = false;
    } else {
        accountNumberError.style.display = "none";
    }

    // Validate re-entered account number
    const reEnterAccountNumber = document.getElementById("ReEnterAccountNumber");
    const reEnterAccountNumberError = document.getElementById("ReEnterAccountNumberError");
    if (reEnterAccountNumber.value.trim() !== accountNumber.value.trim()) {
        reEnterAccountNumberError.style.display = "block";
        isValid = false;
    } else {
        reEnterAccountNumberError.style.display = "none";
    }

    // Validate IFSC code
    const ifsc = document.getElementById("IFSC");
    const ifscError = document.getElementById("IFSCError");
    if (!ifsc.value.trim()) {
        ifscError.style.display = "block";
        isValid = false;
    } else {
        ifscError.style.display = "none";
    }

    // Validate deduction cycle
    //const deductionCycle = document.getElementById("ddlDeductionTime");
    //const deductionCycleError = document.getElementById("DeductionCycleError");
    //if (deductionCycle.value === "0") {
    //    deductionCycleError.style.display = "block";
    //    isValid = false;
    //} else {
    //    deductionCycleError.style.display = "none";
    //}

    return isValid;
};
// Email validation helper function
const validateEmail = (email) => {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(String(email).toLowerCase());
};

// Event listener for navigation buttons
document.querySelectorAll(".btn-navigate-form-step").forEach((button) => {
    button.addEventListener("click", () => {
        const stepNumber = parseInt(button.getAttribute("step_number"));
        const currentStep = stepNumber - 1;

        // Perform validation for each step before navigating
        if (currentStep === 1 && !validateStep1()) {
            return; // Prevent navigation if step 1 is invalid
        }

        if (currentStep === 2 && !validateStep2()) {
            return; // Prevent navigation if step 2 is invalid
        }

        if (currentStep === 3 && !validateStep3()) {
            return; // Prevent navigation if step 3 is invalid
        }

        navigateToFormStep(stepNumber);
    });
});


function fnValidate() {
    const monthlyCTCField = document.getElementById("txtMonthlyCTC");
    const monthlyCTCError = document.getElementById("MonthlyCTCError");

    // Regular expression to check for a decimal point
    const decimalRegex = /^\d+(\.\d{1,2})?$/;

    // Check if Monthly CTC contains a decimal point and is not empty
    if (!monthlyCTCField.value || !decimalRegex.test(monthlyCTCField.value)) {
        monthlyCTCError.style.display = "block"; // Show error message
        return false; // Prevent form submission
    } else {
        monthlyCTCError.style.display = "none"; // Hide error message if valid
    }

    // If all validations pass, allow the form to submit
    return true;
}