using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.DTO
{
    public class EmployeePresnolInfoList
    {
        public int id { get; set; }
        public string FullName { get; set; } = null!;
        public string? PersonalEmailAddress { get; set; }
        public long MobileNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string? FatherName { get; set; }
        public string? PAN { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public int? Stateid { get; set; }
        public int? cityid { get; set; }
        public string? Pincode { get; set; }
        public string? AadharNo { get; set; }
        public string? AadharOne { get; set; }
        public string? Panimg { get; set; }
        public string? AadharTwo { get; set; }
        public string? EmployeeId { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? UpdateDate { get; set; }
        public List<EmployeeApprovedPresnolInfo> ApprovedPresnolInfos { get; set; }
        public List<EmployeeApprovedPresnolInfo> PreviousData { get; set; }


        [NotMapped]
        public List<IFormFile> Aadharbase64 { get; set; }
        public IFormFile Panbase64 { get; set; }
    }
    public class EmployeeApprovedPresnolInfo
    {
        public int id { get; set; }
        public string FullName { get; set; } = null!;
        public string? PersonalEmailAddress { get; set; }
        public long MobileNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string? FatherName { get; set; }
        public string? PAN { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Stateid { get; set; }
        public string? cityid { get; set; }
        public string? Pincode { get; set; }
        public string? AadharNo { get; set; }
        public string? AadharOne { get; set; }
        public string? Panimg { get; set; }
        public string? AadharTwo { get; set; }
        public string? EmployeeId { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
