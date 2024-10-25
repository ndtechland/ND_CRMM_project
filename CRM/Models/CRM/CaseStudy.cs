using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class CaseStudy
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
