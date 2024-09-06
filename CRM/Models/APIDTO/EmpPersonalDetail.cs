using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.APIDTO
{
    public class EmpPersonalDetail
    {
        public string FullName { get; set; } = null!;
        public string WorkEmail { get; set; } = null!;
        public string? MobileNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
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

        [NotMapped]
        public IFormFile Empprofile { get; set; }
        public List<IFormFile> AadharImage { get; set; }
        public IFormFile PanbaseImage { get; set; }
    }
}
