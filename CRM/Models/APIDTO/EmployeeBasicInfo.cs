namespace CRM.Models.APIDTO
{
    public class EmployeeBasicInfo
    {
        public string FullName { get; set; } = null!;
        public string WorkEmail { get; set; } = null!;
        public string? MobileNumber { get; set; }
        public string? DateOfBirth { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }
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
        public string? EmployeeId { get; set; }
        public string? AadharNo { get; set; }
        public string? PanNo { get; set; }
        public string? EmpProfile { get; set; }
        public string? AadharOne { get; set; }
        public string? Panimg { get; set; }
        public string? AadharTwo { get; set; }
    }
}
