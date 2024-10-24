using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string BlogImage { get; set; } = null!;
        public bool? IsPublished { get; set; }
        public string? AddedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
