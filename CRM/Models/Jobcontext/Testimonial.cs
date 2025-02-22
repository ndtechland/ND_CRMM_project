using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Testimonial
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Designation { get; set; }
        public string? Paragraph { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? AddedBy { get; set; }
        public DateTime? AddedOn { get; set; }
        public bool? Status { get; set; }
    }
}
