namespace CRM.Models.DTO
{
    public class EPFReportDTO
    {
        public int? Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; } 
        public string? PAN { get; set; } 
        public decimal? MonthlyCtc { get; set; }
        public List<EPFReportDTO> EPFReports { get; set; }
    }
}
