using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeCheckInRecord
    {
        public long Id { get; set; }
        public string? EmpId { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? CheckIntime { get; set; }
        public DateTime? CheckOuttime { get; set; }
        public DateTime? CurrentDate { get; set; }
        public TimeSpan? Workinghour { get; set; }
    }
}
