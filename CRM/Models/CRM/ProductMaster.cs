using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class ProductMaster
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Gst { get; set; } = null!;
        public string HsnSacCode { get; set; } = null!;
        public double Price { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
