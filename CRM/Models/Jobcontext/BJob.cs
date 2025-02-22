using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class BJob
    {
        public int Id { get; set; }
        public string? JobTitle { get; set; }
        public string? IndustriesName { get; set; }
        public string? Location { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public bool Status { get; set; }
        public bool IsDelete { get; set; }
    }
}
