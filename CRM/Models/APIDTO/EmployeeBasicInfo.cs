using System.Text.Json.Serialization;

namespace CRM.Models.APIDTO
{
    public class EmployeeBasicInfo
    {
        public int? Userid { get; set; }
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
        public string? FatherName { get; set; }
        public string? ShiftTime { get; set; }
        public string? ShiftType { get; set; }
    }
    public class CompanyLoctionDto
    {
        public string? CompanyOfficeLocation { get; set; }
        public string? Radious { get; set; }
    }
    public class Empattendancedatail
    {
        public string? OfficeHour { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? StartOverTime { get; set; }
        public string? FinishOverTime { get; set; }
        public string? TotalWorkingHours { get; set; }
        public string? MonthlyWorkingHours { get; set; }
        public string? Presencepercentage { get; set; }
        public string? absencepercentage { get; set; }
        public string? OvertimeWorkingHours { get; set; }

    }
    public class profilepicture
    {
        public string? EmpProfiles { get; set; }
        public IFormFile Empprofile { get; set; }
    }
    public class Loginactivity
    {
        // public string? OfficeHour { get; set; }
        public string? CheckIN { get; set; }
        public string? CheckOut { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Loginactivity> loginactivities { get; set; } = new List<Loginactivity>();
    }
    public class TasksassignDto
    {
        public string? TaskName { get; set; }
        public string? TaskTittle { get; set; }
        public string TaskDate { get; set; }
        public string? TaskDescription { get; set; }
        public string? TaskStatus { get; set; }
    }
}
