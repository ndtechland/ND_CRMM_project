
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

    for (let index = 0; index < stepNumber; index++) {
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
    {id: "FirstName", errorId: "FirstNameError" },
    //{id: "MiddleName", errorId: "MiddleNameError" },
    //{id: "LastName", errorId: "LastNameError" },
    //{id: "WorkEmail", errorId: "WorkEmailError" },
    {id: "DateOfJoining", errorId: "DateOfJoiningError" },
    {id: "ddlGender", errorId: "GenderError", isSelect: true },
    {id: "ddlStates", errorId: "StateError", isSelect: true },
    {id: "ddlCity", errorId: "WorkLocationError", isSelect: true },
    {id: "ddDepartmentID", errorId: "DepartmentError", isSelect: true },
    {id: "ddDesignationID", errorId: "DesignationError", isSelect: true },
    {id: "ddOfferletterid", errorId: "OfferLetterError", isSelect: true },
    {id: "ddshifttypeidid", errorId: "ShiftTypeError", isSelect: true }
    ];

    requiredFields.forEach(({id, errorId, isSelect}) => {
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

    // Event listener for navigation buttons
    document.querySelectorAll(".btn-navigate-form-step").forEach((formNavigationBtn) => {
        formNavigationBtn.addEventListener("click", () => {
            const stepNumber = parseInt(formNavigationBtn.getAttribute("step_number"));

            // Only validate step 1 before moving to the next step
            if (stepNumber === 2 && !validateStep1()) {
                console.log("Form is invalid, staying on the current step.");
                return; // Stop navigation if validation fails
            }

            navigateToFormStep(stepNumber);
        });
    });



document.querySelector(".btn-navigate-form-step").addEventListener("click", function (e) {
    // Prevent default action of the button
    e.preventDefault();

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
        isValid = false;
    } else {
        document.getElementById("FirstNameError").style.display = "none";
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

    if (!dateOfJoining.value) {
        document.getElementById("DateOfJoiningError").style.display = "block";
        isValid = false;
    } else {
        document.getElementById("DateOfJoiningError").style.display = "none";
    }

    if (gender.value === "0") {
        document.getElementById("GenderError").style.display = "block";
        isValid = false;
    } else {
        document.getElementById("GenderError").style.display = "none";
    }

    if (state.value === "0") {
        document.getElementById("StateError").style.display = "block";
        isValid = false;
    } else {
        document.getElementById("StateError").style.display = "none";
    }

    if (workLocation.value === "0") {
        document.getElementById("WorkLocationError").style.display = "block";
        isValid = false;
    } else {
        document.getElementById("WorkLocationError").style.display = "none";
    }

    if (department.value === "0") {
        document.getElementById("DepartmentError").style.display = "block";
        isValid = false;
    } else {
        document.getElementById("DepartmentError").style.display = "none";
    }

    if (designation.value === "0") {
        document.getElementById("DesignationError").style.display = "block";
        isValid = false;
    } else {
        document.getElementById("DesignationError").style.display = "none";
    }

    if (offerLetter.value === "0") {
        document.getElementById("OfferLetterError").style.display = "block";
        isValid = false;
    } else {
        document.getElementById("OfferLetterError").style.display = "none";
    }

    if (officeShiftType.value === "0") {
        document.getElementById("ShiftTypeError").style.display = "block";
        isValid = false;
    } else {
        document.getElementById("ShiftTypeError").style.display = "none";
    }

    // Prevent navigation if the form is not valid
    if (!isValid) {
        console.log("Form is invalid, staying on the current step.");
    } else {
        // Logic to move to the next step
        console.log("Form is valid, moving to the next step.");
        // You can include your step navigation logic here if needed.
    }
});
