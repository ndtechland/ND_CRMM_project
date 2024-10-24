using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class OurCoreValue
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Author { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? Image { get; set; }
    }
}
