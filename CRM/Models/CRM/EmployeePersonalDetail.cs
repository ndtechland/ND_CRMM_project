﻿using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeePersonalDetail
    {
        public int Id { get; set; }
        public string? PersonalEmailAddress { get; set; }
        public string? MobileNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string FatherName { get; set; } = null!;
        public string? Pan { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string StateId { get; set; } = null!;
        public string? Pincode { get; set; }
        public bool? IsDeleted { get; set; }
        public int? EmpId { get; set; }
        public string? EmpRegId { get; set; }
        public string? Aadharcard { get; set; }
        public string? AadharOne { get; set; }
        public string? Panimg { get; set; }
        public string? AadharTwo { get; set; }
    }
}
