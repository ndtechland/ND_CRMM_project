using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class GenderMaster
    {
        public GenderMaster()
        {
            EmployeeRegistrations = new HashSet<EmployeeRegistration>();
        }

        public int Id { get; set; }
        public string GenderName { get; set; } = null!;

        public virtual ICollection<EmployeeRegistration> EmployeeRegistrations { get; set; }
    }
}
