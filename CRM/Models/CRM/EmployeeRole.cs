using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeRole
    {
        public int Id { get; set; }
        public int EmployeeRegistrationId { get; set; }
        public string EmployeeRole1 { get; set; } = null!;
        public string? Description { get; set; }

        public virtual EmployeeRegistration EmployeeRegistration { get; set; } = null!;
    }
}
