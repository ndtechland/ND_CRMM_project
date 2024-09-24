using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class LeaveType
    {
        public int Id { get; set; }
        public string? Leavetype1 { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? Createddate { get; set; }
    }
}
