using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class CreateProfile
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? EmailId { get; set; }
        public string? Password { get; set; }
        public long MobileNumber { get; set; }
        public string? Experience { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string? GenderName { get; set; }
        public string? ResumeFilePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime Dateofbirth { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? Pincode { get; set; }
        public string? Address { get; set; }
        public string? ProfileImage { get; set; }
        public decimal CurrentCtc { get; set; }
        public decimal ExpectedCtc { get; set; }
    }
}
