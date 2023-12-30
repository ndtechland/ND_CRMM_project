﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CRM.Models.Crm
{
    public partial class EmployeeImportExcel
    {
        // Personal Information
        public int id { get; set; }

        public string? FirstName { get; set; }
        public int? CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string? WorkEmail { get; set; }
        public int GenderID { get; set; }
        public string? Gender { get; set; }
        public int WorkLocationID { get; set; }
        public string? WorkLocation { get; set; }
        public int DesignationID { get; set; }
        public string? DesignationName { get; set; }
        [Key]
        public int EmployeeId { get; set; }
        public int DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public string? Emp_Reg_ID { get; set; }

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
        public int MobileNumber { get; set; }
        public string? Mobile { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string? FatherName { get; set; }
        public string? PAN { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? StateID { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }

        // Bank Details
        public string? AccountHolderName { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? ReEnterAccountNumber { get; set; }
        public string? IFSC { get; set; }
        public int AccountTypeID { get; set; }
        public string? AccountType { get; set; }


        public string? EPF_Number { get; set; }

        public string? Deduction_Cycle { get; set; }

        public string? Employee_Contribution_Rate { get; set; }

    }
}
