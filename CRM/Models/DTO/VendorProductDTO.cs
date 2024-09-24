namespace CRM.Models.DTO
{
    public class VendorProductDTO
    {
        public int Id { get; set; }
        public int? VendorId { get; set; }
        public string? ProductName { get; set; }
        public string? Category { get; set; }
        public int? CategoryId { get; set; }
        public string? Gst { get; set; }
        public decimal? ProductPrice { get; set; }
        public string? Hsncode { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
