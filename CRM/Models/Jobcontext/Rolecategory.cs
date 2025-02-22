using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Rolecategory
    {
        public int Id { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? AddedOn { get; set; }
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }
        public int? AddedBy { get; set; }
    }
}
