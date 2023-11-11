using CRM.Models.Crm;
using System;
using System.Collections.Generic;

namespace CRM.Models.CRM
{
    public partial class BankDetail
    {
        public BankDetail()
        {
            SalaryDetails = new HashSet<SalaryDetail>();
        }

        public int Id { get; set; }
        public int AccountTypeId { get; set; }
        public string AccountHolderName { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public int AccountNumber { get; set; }
        public int ConfirmAccountNumber { get; set; }
        public string Ifsc { get; set; } = null!;

        public virtual AccountTypeMaster AccountType { get; set; } = null!;
        public virtual ICollection<SalaryDetail> SalaryDetails { get; set; }
    }
}
