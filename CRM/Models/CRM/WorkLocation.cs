﻿using System;
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

        public virtual ICollection<EmployeeRegistration> EmployeeRegistrations { get; set; }
    }
}
