namespace CRM.Models.DTO
{
    public class EmpMultiform
    {
        // Personal Information
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string? WorkEmail { get; set; }
        public int GenderID { get; set; }
        public int WorkLocationID { get; set; }
        public int DesignationID { get; set; }
        public int DepartmentID { get; set; }

        // Salary Details
        public decimal AnnualCTC { get; set; }
        public decimal Basic { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal ConveyanceAllowance { get; set; }
        public decimal FixedAllowance { get; set; }
        public decimal EPF { get; set; }
        public decimal MonthlyGrossPay { get; set; }
        public decimal MonthlyCTC { get; set; }

        // Personal Info
        public string? PersonalEmailAddress { get; set; }
        public long MobileNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? FatherName { get; set; }
        public string? PAN { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? StateID { get; set; }
        public string? Pincode { get; set; }

        // Bank Details
        public string? AccountHolderName { get; set; }
        public string? BankName { get; set; }
        public int AccountNumber { get; set; }
        public int ReEnterAccountNumber { get; set; }
        public string? IFSC { get; set; }
        public int AccountTypeID { get; set; }
    }
}
