using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class empOfferletter
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime Validdate { get; set; }
        public decimal? MonthlyCtc { get; set; }
        public decimal? AnnualCtc { get; set; }
        public string DesignationName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public DateTime DateOfJoining { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }
        public string? CandidateAddress { get; set; }
        public string? CandidatePincode { get; set; }
        public string? HrSignature { get; set; }
        public string? HrJobTitle { get; set; }
        public string? HrName { get; set; }
        public string? CandidateEmail { get; set; }
        public string? OfferletterFile { get; set; }

    }
    public class getempOfferletter
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Validdate { get; set; }
        public decimal? MonthlyCtc { get; set; }
        public decimal? AnnualCtc { get; set; }
        public string? Currentdate { get; set; }
        public string DesignationName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string? DateOfJoining { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }
        public string? CandidateAddress { get; set; }
        public string? CandidatePincode { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyImage { get; set; }
        public string? HrSignature { get; set; }
        public string? HrJobTitle { get; set; }
        public string? HrName { get; set; }
        public string? CandidateEmail { get; set; }
        public string? OfficeLocation { get; set; }
        public string? OfficeState { get; set; }
        public string? OfficeCity { get; set; }
    }
    public partial class Offerletters
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Currentdate { get; set; }
        public DateTime? Validdate { get; set; }
        public decimal? MonthlyCtc { get; set; }
        public decimal? AnnualCtc { get; set; }
        public string DesignationId { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public DateTime? DateOfJoining { get; set; }
        public int? Vendorid { get; set; }
        public bool? IsDeleted { get; set; }
        public long? StateId { get; set; }
        public long? CityId { get; set; }
        public string? CandidateAddress { get; set; }
        public string? CandidatePincode { get; set; }
        public string? HrSignature { get; set; }
        public string? HrJobTitle { get; set; }
        public string? HrName { get; set; }
        public IFormFile ImageFile { get; set; }
        public string? CandidateEmail { get; set; }
    }
    public partial class Experienceletters
    {
        public int Id { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Designation { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? CompanyName { get; set; }
        public string? HrName { get; set; }
        public string? WorkEmail { get; set; }
        public string? ExperienceletterFile { get; set; }
        public string? HrDesignation { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyPhoneNumber { get; set; }
        public string? CompanyImage { get; set; }
    }
    public partial class Relievingletters
    {
        public int Id { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Designation { get; set; }
        public string? ResignationDate { get; set; }
        public string? LastDateofEmployment { get; set; }
        public string? CompanyName { get; set; }
        public string? WorkEmail { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyPhoneNumber { get; set; }
        public string? CompanyImage { get; set; }
        public string? RelievingletterFile { get; set; }
        public bool? Ismail { get; set; }

    }
    public partial class HrsignatureDto
    {
        public int Id { get; set; }
        public string? HrSignature { get; set; }
        public string? HrJobTitle { get; set; }
        public string? HrName { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<Hrsignature> Hrsignaturlist { get; set; }
    }
}
