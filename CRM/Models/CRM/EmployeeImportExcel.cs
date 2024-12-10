using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeImportExcel
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string? WorkEmail { get; set; }
        public int GenderId { get; set; }
        public string? Gender { get; set; }
        public int WorkLocationId { get; set; }
        public string? WorkLocation { get; set; }
        public int DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmpRegId { get; set; }
        public decimal AnnualCtc { get; set; }
        public decimal Basic { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal TravellingAllowance { get; set; }
        public decimal Esic { get; set; }
        public decimal Epf { get; set; }
        public decimal MonthlyGrossPay { get; set; }
        public decimal MonthlyCtc { get; set; }
        public string? PersonalEmailAddress { get; set; }
        public int MobileNumber { get; set; }
        public string? Mobile { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string? FatherName { get; set; }
        public string? Pan { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? StateId { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public string? AccountHolderName { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? ReEnterAccountNumber { get; set; }
        public string? Ifsc { get; set; }
        public int AccountTypeId { get; set; }
        public string? AccountType { get; set; }
        public string? EpfNumber { get; set; }
        public string? DeductionCycle { get; set; }
        public string? EmployeeContributionRate { get; set; }
        public string Nominee { get; set; } = null!;
        public decimal? SpecialAllowance { get; set; }
        public decimal? Servicecharge { get; set; }
        public decimal? Gross { get; set; }
        public string? EmpProfile { get; set; }
        public string? Aadharcard { get; set; }
        public string? AadharOne { get; set; }
        public string? Panimg { get; set; }
        public string? AadharTwo { get; set; }
        public string? Chequeimage { get; set; }
        public string? Offerletterid { get; set; }
        public string? OfficeshiftTypeid { get; set; }
        public string? Shifttype { get; set; }
        public long? ShiftTypeid { get; set; }
        public decimal? Conveyanceallowance { get; set; }
        public decimal? FixedAllowance { get; set; }
    }
}
