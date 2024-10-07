using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class OfficeEvent
    {
        public int Id { get; set; }
        public string? Subtittle { get; set; }
        public string? Tittle { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public int? Vendorid { get; set; }
    }
}
