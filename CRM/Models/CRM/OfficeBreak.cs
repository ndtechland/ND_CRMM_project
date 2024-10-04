using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class OfficeBreak
    {
        public int Id { get; set; }
        public int? Vendorid { get; set; }
        public DateTime? Createdate { get; set; }
        public string? Starttime { get; set; }
        public string? Endtime { get; set; }
        public int? Breakstatusid { get; set; }
        public int? Shiftid { get; set; }
    }
}
