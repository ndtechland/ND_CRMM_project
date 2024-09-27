using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class VendorCategoryMaster
    {
        public int Id { get; set; }
        public int? VendorId { get; set; }
        public string? CategoryName { get; set; }
    }
}
