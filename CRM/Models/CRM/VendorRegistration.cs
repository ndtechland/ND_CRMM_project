using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class VendorRegistration
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
        public string? State { get; set; }
        public int? StateId { get; set; }
        public string? Location { get; set; }
        public string? CompanyImage { get; set; }
        public string? Productprice { get; set; }
        public string? Renewprice { get; set; }
        public int? NoOfRenewMonth { get; set; }
        public string? UserName { get; set; }
    }
}
