using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class LeaveApply
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string Reason { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public int? Leavetypeid { get; set; }
    }
}
