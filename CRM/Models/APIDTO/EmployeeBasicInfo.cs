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
        public string? Currentdate { get; set; }
        public string? LoginStatus { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Breakactivity> loginactivities { get; set; }
       
    }
    public class profilepicture
    {
        public string? EmpProfiles { get; set; }
        public IFormFile Empprofile { get; set; }
    }
    public class Breakactivity
    {
        //public string? CheckIN { get; set; }
        //public string? CheckOut { get; set; }

        public string? BreakIN { get; set; }
        public string? BreakOut { get; set; }
        public string? LoginStatus { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Breakactivity> loginactivities { get; set; } = new List<Breakactivity>();
    }
    public class Loginactivity
    {
        public string? CheckIN { get; set; }
        public string? CheckOut { get; set; }
        public string? LoginStatus { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Loginactivity> loginactivities { get; set; } = new List<Loginactivity>();
    }
    public class WebLoginactivity
    {
        public string? CheckIN { get; set; }
        public string? CheckOut { get; set; }
        public string? Currentdate { get; set; }
        public string? LoginStatus { get; set; }
    }
    public class TasksassignDto
    {
        public int? Id { get; set; }
        public string? TaskName { get; set; }
        public string? TaskTittle { get; set; }
        public DateTime? taskstartdate { get; set; }
        public DateTime? taskEnddate { get; set; }
        public string? TaskDescription { get; set; }
        public string? TaskStatus { get; set; }
    }
    public class TasksassignnameDto
    {
        public int? Id { get; set; }
        public string? TaskTittle { get; set; }
        public string? TaskDescription { get; set; }
        public string? Status { get; set; }
        public string? Duration { get; set; }
        public List<TasksassignlistDto>? Empdata { get; set; }
    }

    public class TasksassignlistDto
    {
        public int? Id { get; set; }
        public string? TaskTittle { get; set; }
        public string? TasksubTittle { get; set; }
        public string? TaskStatus { get; set; }
        public string? TaskDescription { get; set; }
        public string? Duration { get; set; }

    }
    public class updateTasksassignDto
    {
        public int? Id { get; set; }
        public string? TasksubTittle { get; set; }
        public string? TaskStatus { get; set; }
    }
    public class officeEventsDto
    {
        public string? Subtittle { get; set; }
        public string? Tittle { get; set; }
        public DateTime? Date { get; set; }

    }
    public class MeetEventsDto
    {
        public string? EventTittle { get; set; }
        public DateTime? Eventdate { get; set; }
        public string? EventType { get; set; }
        public string? EventTime { get; set; }

    }
    public class MeetEventsAndHolidayDto
    {
        public List<officeEventsDto> officeEventsDtos { get; set; }
        public List<MeetEventsDto> meetEventsDtos { get; set; }
    }
    public class Monthlyattendancedatail
    {
        public int? TotalWorkingDays { get; set; }
        public int? TotalPresentDays { get; set; }
        public int? TotalAbsentDays { get; set; }
        public string? Attendance { get; set; }
    }
    public class TotalLeavelist
    {
        public int? id { get; set; }
        public DateTime? Leavedate { get; set; }
        public string? Reason { get; set; }
        public decimal? Nodays { get; set; }
        public string? LeaveType { get; set; }
        public string? TypeofLeave { get; set; }
        public string? Leaveapplydate { get; set; }
        public string? LeaveSearchdate { get; set; }

    }
    public class TotalLeave
    {
        public decimal? TotalLeaves { get; set; }
        public List<TotalLeavelist> Type { get; set; }
    }
    public class getTotalLeave
    {
        public string? Reason { get; set; }
        public decimal? Totaldays { get; set; }
        public decimal? PaidLeave { get; set; }
        public decimal? UnPaidLeave { get; set; }
    }

    public class getattendancegraph
    {
        public string? Year { get; set; }
        public string? Month { get; set; }
        public int? Value { get; set; }
    }
    public class getTasklist
    {
        public List<getReassignedTasklist> Reassigned { get; set; }
        public List<getCompletedTasklist> Completed { get; set; }
        public List<getUnCompletedTasklist> UnCompleted { get; set; }
    }
    public class getReassignedTasklist
    {
        public int? id { get; set; }
        public string? Taskname { get; set; }
        public string? Duration { get; set; }
        public string? status { get; set; }
    }
    public class getCompletedTasklist
    {
        public int? id { get; set; }
        public string? Taskname { get; set; }
        public string? Duration { get; set; }
        public string? status { get; set; }
    }
    public class getUnCompletedTasklist
    {
        public int? id { get; set; }
        public string? Taskname { get; set; }
        public string? Duration { get; set; }
        public string? status { get; set; }
    }
    public class aboutCompanyDto
    {
        public string? Companylink { get; set; }
    }
}
