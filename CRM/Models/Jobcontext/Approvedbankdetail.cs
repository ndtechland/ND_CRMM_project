using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Approvedbankdetail
    {
        public int Id { get; set; }
        public string AccountHolderName { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string? AccountNumber { get; set; }
        public string? ReEnterAccountNumber { get; set; }
        public string? Ifsc { get; set; }
        public string? AccountTypeId { get; set; }
        public string? EpfNumber { get; set; }
        public string? Nominee { get; set; }
        public string? Chequeimage { get; set; }
        public string? EmployeeId { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
