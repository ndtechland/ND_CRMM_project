using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Officeshift
    {
        public int Id { get; set; }
        public int? Vendorid { get; set; }
        public DateTime? Createdate { get; set; }
        public TimeSpan? Starttime { get; set; }
        public TimeSpan? Endtime { get; set; }
        public string? ShiftTypeid { get; set; }
    }
}
