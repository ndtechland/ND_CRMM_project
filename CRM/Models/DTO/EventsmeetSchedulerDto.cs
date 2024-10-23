using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class EventsmeetSchedulerDto
    {
        public int Id { get; set; }
        public string[]? EmployeeId { get; set; } = null;
        public string? Tittle { get; set; }
        public string? Description { get; set; }
        public DateTime? Createddate { get; set; }
        public int? Vendorid { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsEventsmeet { get; set; }
        public List<EventsmeetScheduler> Scheduler { get; set; }
    }
}
