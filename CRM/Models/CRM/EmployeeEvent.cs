using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeEvent
    {
        public int Id { get; set; }
        public string? Task { get; set; }
        public string? Tittle { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public string? Reason { get; set; }
        public int? Status { get; set; }
    }
}
