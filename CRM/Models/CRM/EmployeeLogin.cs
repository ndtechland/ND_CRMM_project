using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeLogin
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Password { get; set; } = null!;

        public virtual EmployeeRegistration Employee { get; set; } = null!;
    }
}
