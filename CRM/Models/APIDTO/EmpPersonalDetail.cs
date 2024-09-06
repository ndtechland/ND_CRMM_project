using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.APIDTO
{
    public class EmpPersonalDetail
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
        public string? AadharNo { get; set; }
        public string? AadharOne { get; set; }
        public string? Panimg { get; set; }
        public string? AadharTwo { get; set; }

        [NotMapped]
        public IFormFile Empprofile { get; set; }
        public List<IFormFile> Aadharbase64 { get; set; }
        public IFormFile Panbase64 { get; set; }
    }
}
