using CRM.Models.Crm;

namespace CRM.Models.DTO
{
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
}
