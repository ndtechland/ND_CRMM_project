using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CRM.Models.Jobcontext
{
    public partial class CJobOpen
    {
        public int Id { get; set; }
        public string? JobTitle { get; set; }
        public string? Opening { get; set; }
        public int? Location { get; set; }
        public string? RequiredExperience { get; set; }
        public string? JobDescription { get; set; }
        public int? Department { get; set; }
        public int? AddedBy { get; set; }
        public DateTime? AddedOn { get; set; }
        public bool? Isdelete { get; set; }
        public bool? Status { get; set; }
        public string? Package { get; set; }
        public string? Skills { get; set; }
        public int? Qualificationid { get; set; }
        public string? EmployeementType { get; set; }
        public int? PostedById { get; set; }
        public int? WorkModeId { get; set; }
        public int? Stateid { get; set; }
        public int? Cityid { get; set; }
        public int Companyid { get; set; }
        public string? QualificationDescription { get; set; }
        public string? AboutDescription { get; set; }
        public string? ResponsebilitiesDescription { get; set; }
        public int Vendorid { get; set; }
        public bool? IsVendor { get; set; }
    }
}
