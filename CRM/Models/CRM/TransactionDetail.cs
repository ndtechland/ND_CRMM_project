using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class TransactionDetail
    {
        public int Id { get; set; }
        public DateTime DateOfIssue { get; set; }
        public double PaidAmount { get; set; }
        public double PayAmount { get; set; }
        public int PayMethod { get; set; }
        public int BillingId { get; set; }

        public virtual BillingDetail Billing { get; set; } = null!;
        public virtual PayMethodMaster PayMethodNavigation { get; set; } = null!;
    }
}
