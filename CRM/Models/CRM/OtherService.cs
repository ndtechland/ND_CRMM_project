using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class OtherService
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
