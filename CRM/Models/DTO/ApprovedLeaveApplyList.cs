namespace CRM.Models.DTO
{
    public class ApprovedLeaveApplyList
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmpMobileNumber { get; set; }
        public string? StartLeaveId { get; set; }
        public string EndeaveId { get; set; }
        public string TypeOfLeaveId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal UnPaidCountLeave { get; set; }
        public string? Month { get; set; }
        public string? Reason { get; set; }
        public bool? Isapprove { get; set; }
        public decimal? PaidCountLeave { get; set; }
    }
}
