using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class CustomerRegistration
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public string? MobileNumber { get; set; }
        public string? AlternateNumber { get; set; }
        public string? Email { get; set; }
        public string? GstNumber { get; set; }
        public string? BillingAddress { get; set; }
        public int? StateId { get; set; }
        public int? Vendorid { get; set; }
        public int? BillingCityId { get; set; }
        public int? BillingStateId { get; set; }
        public string? Location { get; set; }
        public bool? IsSameAddress { get; set; }
        public int? CityId { get; set; }
    }
}
