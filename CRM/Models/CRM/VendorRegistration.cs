using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class VendorRegistration
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
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
        public string? Maplat { get; set; }
        public string? Maplong { get; set; }
        public string? Radious { get; set; }
        public int? BillingStateId { get; set; }
        public int? BillingCityId { get; set; }
        public bool? IsSameAddress { get; set; }
        public bool? Isactive { get; set; }
        public decimal? Scgst { get; set; }
        public decimal? Igst { get; set; }
        public decimal? Cgst { get; set; }
        public int? CityId { get; set; }
        public string? Invoicefile { get; set; }
        public bool? Isprofessionaltax { get; set; }
        public int? PricingPlanid { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? Duedate { get; set; }
        public string? VendorSingature { get; set; }
        public string? Notes { get; set; }
        public string? Terms { get; set; }
        public int? UserRoleId { get; set; }
    }
}
