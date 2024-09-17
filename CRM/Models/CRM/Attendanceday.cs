using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Attendanceday
    {
        public int Id { get; set; }
        public string? Nodays { get; set; }
        public long? Vendorid { get; set; }
        public DateTime? Createdate { get; set; }
    }
}
