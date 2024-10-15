using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class VendorBankDetail
    {
        public int Id { get; set; }
        public int? VendorId { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? Ifsc { get; set; }
        public string? AccountHolderName { get; set; }
        public string? BranchAddress { get; set; }
    }
}
