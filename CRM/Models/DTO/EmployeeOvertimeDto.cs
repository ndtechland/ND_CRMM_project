namespace CRM.Models.DTO
{
    public class EmployeeOvertimeDto
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? TotalOvertimeHours { get; set; }
        public bool? Approved { get; set; }
        public string? ApprovalDate { get; set; }

    }
}
