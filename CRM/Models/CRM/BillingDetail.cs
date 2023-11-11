using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class BillingDetail
    {
        public BillingDetail()
        {
            BillingHistories = new HashSet<BillingHistory>();
            TransactionDetails = new HashSet<TransactionDetail>();
        }

        public int Id { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = null!;
        public int StateId { get; set; }
        public int Pincode { get; set; }
        public DateTime Date { get; set; }
        public double Cost { get; set; }
        public bool? Status { get; set; }

        public virtual StateMaster State { get; set; } = null!;
        public virtual ICollection<BillingHistory> BillingHistories { get; set; }
        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; }
    }
}
