using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class WorkLocation
    {
        public WorkLocation()
        {
            EmployeeRegistrations = new HashSet<EmployeeRegistration>();
        }

        public int Id { get; set; }
        public string AddressLine1 { get; set; } = null!;
        public string AddressLine2 { get; set; } = null!;
        public string? City { get; set; }
        public int StateId { get; set; }
        public int Pincode { get; set; }

        public virtual StateMaster State { get; set; } = null!;
        public virtual ICollection<EmployeeRegistration> EmployeeRegistrations { get; set; }
    }
}
