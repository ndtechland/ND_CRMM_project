using CRM.Models.Crm;
using System;
using System.Collections.Generic;

namespace CRM.Models.CRM
{
    public partial class PersonalDetail
    {
        public int Id { get; set; }
        public int EmployeeRegistrationId { get; set; }
        public int ResidentialAddressId { get; set; }
        public string FatherName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string Email { get; set; } = null!;
        public decimal Mobile { get; set; }
        public string Pan { get; set; } = null!;

        public virtual EmployeeRegistration EmployeeRegistration { get; set; } = null!;
    }
}
