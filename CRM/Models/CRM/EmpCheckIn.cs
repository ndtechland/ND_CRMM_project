using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmpCheckIn
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? CurrentLat { get; set; }
        public string? Currentlong { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime? Currentdate { get; set; }
        public bool? CheckIn { get; set; }
    }
}
