using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmpApplywfh
    {
        public int Id { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? Currentdate { get; set; }
        public int? Iswfh { get; set; }
        public string? Reason { get; set; }
        public string? UserId { get; set; }
    }
}
