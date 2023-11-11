using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class BillingHistory
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductDetailsId { get; set; }
        public int BillingDetailsId { get; set; }

        public virtual BillingDetail BillingDetails { get; set; } = null!;
        public virtual ProductMaster ProductDetails { get; set; } = null!;
    }
}
