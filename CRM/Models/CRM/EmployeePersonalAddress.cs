using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeePersonalAddress
    {
        public int Id { get; set; }
        public int EmployeeRegistrationId { get; set; }
        public string? PersonalEmailAddress { get; set; }
        public decimal? MobileNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FatherName { get; set; } = null!;
        public string? Pan { get; set; }
        public string AddressLine1 { get; set; } = null!;
        public string AddressLine2 { get; set; } = null!;
        public string? City { get; set; }
        public int StateId { get; set; }
        public int Pincode { get; set; }

        public virtual EmployeeRegistration EmployeeRegistration { get; set; } = null!;
        public virtual StateMaster State { get; set; } = null!;
    }
}
