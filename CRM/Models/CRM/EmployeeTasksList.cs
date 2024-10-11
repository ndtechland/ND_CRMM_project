using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeTasksList
    {
        public int Id { get; set; }
        public int? Emptaskid { get; set; }
        public string? EmployeeId { get; set; }
        public int? TaskStatus { get; set; }
        public string? Taskname { get; set; }
    }
}
