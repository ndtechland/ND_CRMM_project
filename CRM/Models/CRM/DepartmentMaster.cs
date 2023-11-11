using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class DepartmentMaster
    {
        public DepartmentMaster()
        {
            EmployeeRegistrations = new HashSet<EmployeeRegistration>();
        }

        public int Id { get; set; }
        public string DepartmentName { get; set; } = null!;

        public virtual ICollection<EmployeeRegistration> EmployeeRegistrations { get; set; }
    }
}
