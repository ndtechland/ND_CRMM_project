using CRM.Models.Crm;
using System;
using System.Collections.Generic;

namespace CRM.Models.CRM
{
    public partial class DesignationMaster
    {
        public DesignationMaster()
        {
            EmployeeRegistrations = new HashSet<EmployeeRegistration>();
        }

        public int Id { get; set; }
        public string Designation { get; set; } = null!;

        public virtual ICollection<EmployeeRegistration> EmployeeRegistrations { get; set; }
    }
}
