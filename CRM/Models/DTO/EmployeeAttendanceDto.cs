namespace CRM.Models.DTO
{
    public class EmployeeAttendanceDto
    {
        public int? Id { get; set; }
        public string? EmpId { get; set; }
        public string? EmployeeName { get; set; }
        public string? CheckIntime { get; set; }
        public string? CheckOuttime { get; set; }
        public string? CurrentDate { get; set; }
        public string? Workinghour { get; set; }
        public List<EmployeeAttendanceDto> detail { get; set; }
    }
}
