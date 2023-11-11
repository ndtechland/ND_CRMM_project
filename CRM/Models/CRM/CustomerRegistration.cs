using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class CustomerRegistration
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string ContactPersonName { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string AlternateNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string GstNumber { get; set; } = null!;
        public string BillingAddress { get; set; } = null!;
        public string ProductDetails { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime RenewDate { get; set; }
    }
}
