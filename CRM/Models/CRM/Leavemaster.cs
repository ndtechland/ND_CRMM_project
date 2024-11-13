using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Leavemaster
    {
        public int Id { get; set; }
        public int LeavetypeId { get; set; }
        public decimal? Value { get; set; }
        public string? EmpId { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? LeaveUpdateDate { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public bool IsActive { get; set; }
        public int? Vendorid { get; set; }
    }
}
