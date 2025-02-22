using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class MLocation
    {
        public int Id { get; set; }
        public string? LocationName { get; set; }
        public int? AddedBy { get; set; }
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? AddedOn { get; set; }
    }
}
