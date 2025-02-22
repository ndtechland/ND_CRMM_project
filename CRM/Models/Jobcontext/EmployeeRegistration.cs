using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class EmployeeRegistration
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? WorkEmail { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string? Gender { get; set; }
        public int? CompanyId { get; set; }
        public int? OfcStateId { get; set; }
        public int? OfccityId { get; set; }
        public int DepartmentId { get; set; }
        public int? JobTitle { get; set; }
        public string? PersonalEmailAddress { get; set; }
        public long MobileNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string? FatherName { get; set; }
        public string? Pan { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string? Pincode { get; set; }
        public string? AccountHolderName { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? ReEnterAccountNumber { get; set; }
        public string? Ifsc { get; set; }
        public string? EpfNumber { get; set; }
        public string? NomineeName { get; set; }
        public int AccountTypeId { get; set; }
        public decimal AnnualCtc { get; set; }
        public decimal MonthlyCtc { get; set; }
        public decimal? SpecialAllowance { get; set; }
        public decimal? Basic { get; set; }
        public decimal? HouseRentAllowance { get; set; }
        public decimal? Gross { get; set; }
        public decimal? MonthlyGrossPay { get; set; }
        public decimal? EmployeeEpf { get; set; }
        public decimal? Professionaltax { get; set; }
        public decimal? EmployeeEsic { get; set; }
        public decimal? Tdspercentage { get; set; }
        public decimal? EmployerEpf { get; set; }
        public decimal? EmployerEsic { get; set; }
        public string? EmployeePassword { get; set; }
        public string? AadharNo { get; set; }
        public string? AadharOne { get; set; }
        public string? AadharTwo { get; set; }
        public string? Panimg { get; set; }
        public string? Chequeimage { get; set; }
        public string? Profileimage { get; set; }
        public DateTime? Entry { get; set; }
        public string? Offerletter { get; set; }
        public string? Appointmentletter { get; set; }
        public bool? IsGenerate { get; set; }
    }
}
