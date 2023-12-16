using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Crm
{
    public partial class CustomerRegistration
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public string? WorkLocation { get; set; }
        public string? MobileNumber { get; set; }
        public string? AlternateNumber { get; set; }
        public string? Email { get; set; }
        public string? GstNumber { get; set; }
        public string? BillingAddress { get; set; }
        public string? ProductDetails { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime RenewDate { get; set; }
    }
}
