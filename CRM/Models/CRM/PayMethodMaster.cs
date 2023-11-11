using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class PayMethodMaster
    {
        public PayMethodMaster()
        {
            TransactionDetails = new HashSet<TransactionDetail>();
        }

        public int Id { get; set; }
        public string PayMethod { get; set; } = null!;

        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; }
    }
}
