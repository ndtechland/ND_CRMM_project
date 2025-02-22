using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Carrier
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? CurrentLocation { get; set; }
        public decimal CurrentCtc { get; set; }
        public decimal ExpectedCtc { get; set; }
        public string? HighestQualification { get; set; }
        public string? PositionApplyFor { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public int CarrierStatus { get; set; }
        public int Stateid { get; set; }
        public int? Cityid { get; set; }
        public string? Password { get; set; }
        public int? Qualificationid { get; set; }
    }
}
