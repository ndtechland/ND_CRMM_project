using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CRM.Models.DTO
{
    public class EmpMultiform
    {
        // Personal Information
        public int id { get; set; }        
        public int? Vendorid { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string? WorkEmail { get; set; }
        public int? GenderID { get; set; }
        public int WorkLocationID { get; set; }
        public int DesignationID { get; set; }
        public int DepartmentID { get; set; }
        public string? Emp_Reg_ID { get; set; }
        public string? EmployeeId { get; set; }
        public int stateId { get; set; }
        public int officeshiftTypeid { get; set; }
        public int offerletterid { get; set; }
        // Salary Details
        [NotMapped]
        public decimal AnnualCTC { get; set; }
        public decimal? Basic { get; set; }
        public decimal? HouseRentAllowance { get; set; }
        public decimal? TravellingAllowance { get; set; }
        public decimal? ESIC { get; set; }
        public decimal? Professionaltax { get; set; }
        public decimal? EPF { get; set; }
        public decimal? MonthlyGrossPay { get; set; }
        public decimal MonthlyCTC { get; set; }
        public decimal? Servicecharge { get; set; }
        public decimal? SpecialAllowance { get; set; }
        public decimal? Gross { get; set; }
        public decimal? Conveyanceallowance { get; set; }
        //public decimal? FixedAllowance { get; set; }
        public decimal? Medical { get; set; }
        public decimal? VariablePay { get; set; }
        public decimal? EmployerContribution { get; set; }
        public decimal? Tdsvalue { get; set; }
        public decimal? Basicpercentage { get; set; }
        public decimal? Hrapercentage { get; set; }
        public decimal? Conveyancepercentage { get; set; }
        public decimal? Medicalpercentage { get; set; }
        public decimal? Variablepercentage { get; set; }
        public decimal? EmployerContributionpercentage { get; set; }
        public decimal? Epfpercentage { get; set; }
        public decimal? Esipercentage { get; set; }

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
        public string? AccountNumber { get; set; }
        public string? ReEnterAccountNumber { get; set; }
        public string? IFSC { get; set; }
        public int AccountTypeID { get; set; }
        public string nominee { get; set; }

        [NotMapped]
        public string? EPF_Number { get; set; }
        [NotMapped]
        public string? Deduction_Cycle { get; set; }
        [NotMapped]
        public string? Employee_Contribution_Rate { get; set; }

        public decimal? Tdspercentage { get; set; }
        public decimal? Amount { get; set; }

        public int? RoleId { get; set; }
        public bool? IsIncrement { get; set; }
        public int? CustomerCompanyid { get; set; }

    }
}
