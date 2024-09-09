using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class ApprovedPresnolInfo
    {
        public int Id { get; set; }
        public string? PersonalEmailAddress { get; set; }
        public long? MobileNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Pan { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string StateId { get; set; } = null!;
        public string? Pincode { get; set; }
        public string? EmployeeId { get; set; }
        public string? AadharNo { get; set; }
        public string? AadharOne { get; set; }
        public string? Panimg { get; set; }
        public string? AadharTwo { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? Vendorid { get; set; }
    }
}
