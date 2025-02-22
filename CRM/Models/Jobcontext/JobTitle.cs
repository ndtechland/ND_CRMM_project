using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class JobTitle
    {
        public int Id { get; set; }
        public int? IndustriesId { get; set; }
        public string? JobTitle1 { get; set; }
        public decimal? Jobamount { get; set; }
        public int? AddedBy { get; set; }
        public DateTime? AddedOn { get; set; }
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }
    }
}
