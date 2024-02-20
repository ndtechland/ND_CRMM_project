using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeRole
    {
        public int Id { get; set; }
        public string? EmployeeRegistrationId { get; set; }
        public string EmployeeRole1 { get; set; } = null!;
        public string? Description { get; set; }
    }
}
