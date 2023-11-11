using CRM.Models.Crm;
using System;
using System.Collections.Generic;

namespace CRM.Models.CRM
{
    public partial class GenerateQuation
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public decimal Mobile { get; set; }
        public string Email { get; set; } = null!;
        public string SalesPersonName { get; set; } = null!;
        public int ProductDetails { get; set; }
        public string Subject { get; set; } = null!;
        public double Amount { get; set; }

        public virtual ProductMaster ProductDetailsNavigation { get; set; } = null!;
    }
}
