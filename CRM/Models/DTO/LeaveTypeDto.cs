using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class EmpattendanceDto
    {
        public int Id { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string? SalarySlipPath { get; set; }
        public string? SalarySlipName { get; set; }
    }
    public class ForgotPassword
    {
        public string? Email { get; set; }
    }
    public class leavedto
    {
        public List<Leave> GetLeaveList { get; set; }
        public List<LeaveTypeValue> GetLeaveTypeList { get; set; }
    }
    public class LeaveTypeValue
    {
        public int Id { get; set; }
        public string? leavetype { get; set; }
        public decimal leaveValue { get; set; }

        public bool? Isactive { get; set; }
    }

    public class ApplyLeave
    {
        public int StartLeaveId { get; set; }
        public int EndeaveId { get; set; }
        public int TypeOfLeaveId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }
    public class EmpchangepasswordDto
    {
        public int userId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
