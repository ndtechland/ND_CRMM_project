namespace CRM.Models.DTO
{
    public class EmpTasksassignDto
    {
        public int Id { get; set; }
        public string? Task { get; set; }
        public string? Tittle { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public string? EmployeeId { get; set; }
    }
}
