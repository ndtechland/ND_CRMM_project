using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.APIDTO
{
    public class EmpPersonalDetail
    {
        public string FullName { get; set; } = null!;
        public string WorkEmail { get; set; } = null!;
        public long? MobileNumber { get; set; }
        public string? DateOfBirth { get; set; }
        public long? Stateid { get; set; }
        public int? Cityid { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Pincode { get; set; }
        public string? PersonalEmailAddress { get; set; }
        public string? DateOfJoining { get; set; }
        public string DepartmentName { get; set; } = null!;
        public string DesignationName { get; set; } = null!;
        public string? CompanyName { get; set; }
        public string CompanyLocationName { get; set; } = null!;
        public string? AadharNo { get; set; }
        public string? PanNo { get; set; }
        public string? FatherName { get; set; }

        [NotMapped]
        public IFormFile Empprofile { get; set; }
        public IFormFile Aadhar1 { get; set; }
        public IFormFile Aadhar2 { get; set; }
        public IFormFile PanbaseImage { get; set; }
    }

    public partial class ApprovedPresnolRes
    {
        public int Id { get; set; }
        public string? PersonalEmailAddress { get; set; }
        public long? MobileNumber { get; set; }
        public string? DateOfBirth { get; set; }
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
        public string? FullName { get; set; }
        public string? FatherName { get; set; }
        public string? EmpProfile { get; set; }
    }
}
