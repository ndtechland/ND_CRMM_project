﻿using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeRegistration
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string WorkEmail { get; set; } = null!;
        public string GenderId { get; set; } = null!;
        public string WorkLocationId { get; set; } = null!;
        public string DesignationId { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public bool? IsDeleted { get; set; }
        public int? Vendorid { get; set; }
        public string? EmployeeId { get; set; }
        public int? RoleId { get; set; }
        public long? StateId { get; set; }
        public string? EmpProfile { get; set; }
        public string? Appoinmentletter { get; set; }
        public int? Offerletterid { get; set; }
        public long? OfficeshiftTypeid { get; set; }
        public bool? Isactive { get; set; }
        public int? CustomerCompanyid { get; set; }
        public long? CustomerWorkLocationid { get; set; }
    }
}
