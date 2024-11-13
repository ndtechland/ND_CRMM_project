﻿namespace CRM.Models.DTO
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
    public class EmployeeBreakDto
    {
        public int? Id { get; set; }
        public string? EmpId { get; set; }
        public string? EmployeeName { get; set; }
        public string? BreakIntime { get; set; }
        public string? BreakOuttime { get; set; }
        public string? CurrentDate { get; set; }
        public string? Breakhour { get; set; }
        public List<EmployeeBreakDto> Breakdetail { get; set; }
    }

}
