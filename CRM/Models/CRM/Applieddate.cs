using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Applieddate
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime? AppliedDate1 { get; set; }
        public bool? Isactive { get; set; }
        public int? Typeofleaveid { get; set; }
    }
}
