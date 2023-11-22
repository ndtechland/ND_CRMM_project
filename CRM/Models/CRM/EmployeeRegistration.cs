﻿using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeRegistration
    {
        public EmployeeRegistration()
        {
            EmployeeBankDetails = new HashSet<EmployeeBankDetail>();
            EmployeeLogins = new HashSet<EmployeeLogin>();
            EmployeeRoles = new HashSet<EmployeeRole>();
            Payrolls = new HashSet<Payroll>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string WorkEmail { get; set; } = null!;
        public string GenderId { get; set; } = null!;
        public string WorkLocationId { get; set; } = null!;
        public string DesignationId { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;

        public virtual ICollection<EmployeeBankDetail> EmployeeBankDetails { get; set; }
        public virtual ICollection<EmployeeLogin> EmployeeLogins { get; set; }
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }
        public virtual ICollection<Payroll> Payrolls { get; set; }
    }
}
