using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class VendorProductMaster
    {
        public int Id { get; set; }
        public int? VendorId { get; set; }
        public string? ProductName { get; set; }
        public int? CategoryId { get; set; }
        public decimal? ProductPrice { get; set; }
        public string? Hsncode { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Gst { get; set; }
    }
}
