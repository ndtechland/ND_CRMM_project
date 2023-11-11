using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class GstMaster
    {
        public int Id { get; set; }
        public string? GstPercentagen { get; set; }
        public string? Scgst { get; set; }
        public string? Cgst { get; set; }
        public string? Igst { get; set; }
    }
}
