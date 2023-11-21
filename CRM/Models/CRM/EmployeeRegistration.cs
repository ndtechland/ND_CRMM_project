using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CRM.Models.Crm
{
    public partial class EmployeeRegistration
    {
        public EmployeeRegistration()
        {
            EmployeeBankDetails = new HashSet<EmployeeBankDetail>();
            EmployeeLogins = new HashSet<EmployeeLogin>();
            EmployeePersonalAddresses = new HashSet<EmployeePersonalAddress>();
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
        public int GenderId { get; set; }
        public int WorkLocationId { get; set; }
        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }

        public virtual DepartmentMaster Department { get; set; } = null!;
        public virtual GenderMaster Gender { get; set; } = null!;
        public virtual WorkLocation WorkLocation { get; set; } = null!;
        public virtual ICollection<EmployeeBankDetail> EmployeeBankDetails { get; set; }
        public virtual ICollection<EmployeeLogin> EmployeeLogins { get; set; }
        public virtual ICollection<EmployeePersonalAddress> EmployeePersonalAddresses { get; set; }
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }
        public virtual ICollection<Payroll> Payrolls { get; set; }
    }
}
