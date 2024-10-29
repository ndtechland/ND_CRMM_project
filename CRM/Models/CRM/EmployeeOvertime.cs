using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeOvertime
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? TotalOvertimeHours { get; set; }
        public bool? Approved { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public bool? IsOvertime { get; set; }
    }
}
