namespace CRM.Models.DTO
{
    public class EmployeeBasicinfo
    {
        public int Id { get; set; }
        public string? PersonalEmailAddress { get; set; }
        public decimal? MobileNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string FatherName { get; set; } = null!;
        public string? Pan { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? StateId { get; set; }
        public Decimal Pincode { get; set; }
    }
}
