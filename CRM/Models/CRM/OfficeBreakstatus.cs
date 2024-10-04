using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class OfficeBreakstatus
    {
        public int Id { get; set; }
        public int? Vendorid { get; set; }
        public DateTime? Createdate { get; set; }
        public string? Breakstatus { get; set; }
    }
}
