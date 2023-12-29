using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Crm
{
    public partial class EmployeeRegistration
    {
        public EmployeeRegistration()
        {
            EmployeeLogins = new HashSet<EmployeeLogin>();
            EmployeeRoles = new HashSet<EmployeeRole>();
            Payrolls = new HashSet<Payroll>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string WorkEmail { get; set; } = null!;
        public string GenderId { get; set; } = null!;
        public string WorkLocationId { get; set; } = null!;
        public string DesignationId { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public bool? IsDeleted { get; set; }
        public int? CustomerId { get; set; }
        [NotMapped]
        public int? LocationId { get; set; }



        // Salary Details
        [NotMapped]
        public decimal AnnualCTC { get; set; }
        [NotMapped]
        public decimal Basic { get; set; }
        [NotMapped]
        public decimal HouseRentAllowance { get; set; }
        [NotMapped]
        public decimal ConveyanceAllowance { get; set; }
        [NotMapped]
        public decimal FixedAllowance { get; set; }
        [NotMapped]
        public decimal EPF { get; set; }
        [NotMapped]
        public decimal MonthlyGrossPay { get; set; }
        [NotMapped]
        public decimal MonthlyCTC { get; set; }

        // Personal Info
        [NotMapped]
        public string? PersonalEmailAddress { get; set; }
        [NotMapped]
        public int MobileNumber { get; set; }
        [NotMapped]
        public string? Mobile { get; set; }
        [NotMapped]
        public DateTime DateOfBirth { get; set; }
        [NotMapped]
        public int Age { get; set; }
        [NotMapped]
        public string? FatherName { get; set; }
        [NotMapped]
        public string? PAN { get; set; }
        [NotMapped]
        public string? AddressLine1 { get; set; }
        [NotMapped]
        public string? AddressLine2 { get; set; }
        [NotMapped]
        public string? City { get; set; }
        [NotMapped]
        public string? StateID { get; set; }
        [NotMapped]
        public string? State { get; set; }
        [NotMapped]
        public string? Pincode { get; set; }

        // Bank Details
        [NotMapped]
        public string? AccountHolderName { get; set; }
        [NotMapped]
        public string? BankName { get; set; }
        [NotMapped]
        public string? AccountNumber { get; set; }
        [NotMapped]
        public string? ReEnterAccountNumber { get; set; }
        [NotMapped]
        public string? IFSC { get; set; }
        [NotMapped]
        public int AccountTypeID { get; set; }
        [NotMapped]
        public string? AccountType { get; set; }
        [NotMapped]

        public string? EPF_Number { get; set; }
        [NotMapped]
        public string? Deduction_Cycle { get; set; }
        [NotMapped]
        public string? Employee_Contribution_Rate { get; set; }







        public virtual ICollection<EmployeeLogin> EmployeeLogins { get; set; }
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }
        public virtual ICollection<Payroll> Payrolls { get; set; }
    }
}