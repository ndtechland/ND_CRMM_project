using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeBankDetail
    {
        public int Id { get; set; }
        public int EmployeeRegistrationId { get; set; }
        public string AccountHolderName { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public int AccountNumber { get; set; }
        public int ReEnterAccountNumber { get; set; }
        public string? Ifsc { get; set; }
        public int AccountTypeId { get; set; }
        public bool? IsDeleted { get; set; }
        public string? EmpId { get; set; }
        public string? EpfNumber { get; set; }
        public string? DeductionCycle { get; set; }
        public string? EmployeeContributionRate { get; set; }
    }
}
