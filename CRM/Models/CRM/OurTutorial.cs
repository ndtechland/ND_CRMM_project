using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class OurTutorial
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string VedioUrl { get; set; } = null!;
        public string? Author { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? Description { get; set; }
    }
}
