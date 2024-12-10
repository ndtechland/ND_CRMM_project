using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.DTO
{
    public class VendorDto
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        [NotMapped]
        //public string[] WorkLocation { get; set; }
        public int CityId { get; set; }
        public string? MobileNumber { get; set; }
        public string? AlternateNumber { get; set; }
        public string? Email { get; set; }
        public string? GstNumber { get; set; }
        public string? BillingAddress { get; set; }
        public string? ProductDetails { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime RenewDate { get; set; }
        public string? Scgst { get; set; }
        public string? Cgst { get; set; }
        public string? Igst { get; set; }
        public string? Gst { get; set; }
        public string? HsnSacCode { get; set; }
        public double Price { get; set; }
        public string productprice { get; set; }
        public int? NoOfRenewMonth { get; set; }
        public int? BillingCityId { get; set; }
        public string? Renewprice { get; set; }
        public string? State { get; set; }
        public int? BillingStateId { get; set; }
        public int? StateId { get; set; }
        public string? StateName { get; set; } = null!;
        public string? BillingState { get; set; } = null!;
        public string? OfficeCity { get; set; }
        public string? BillingCity { get; set; }
        public string? OfficeState { get; set; }
        public string? Location { get; set; }
        public string? CompanyImage { get; set; }
        public bool? IsSameAddress { get; set; }
        public bool? Isactive { get; set; }
        public string? PricingPlanid { get; set; }
        public double? PricePlan { get; set; }
        public DateTime Duedate { get; set; }
        public string? InvoiceNumber { get; set; }

    }
    public partial class VendorRegistrationDto
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public int? CityId { get; set; }
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
        public int? BillingStateId { get; set; }
        public int? BillingCityId { get; set; }
        public int? OfficeStateId { get; set; }
        public int? OfficeCityId { get; set; }
        public bool? Isprofessionaltax { get; set; }

        public IFormFile ImageFile { get; set; }
        public string UserName { get; set; } = null!;
        public string radious { get; set; } = null!;
        public string maplat { get; set; } = null!;
        public string maplong { get; set; } = null!;
    }
    
}
