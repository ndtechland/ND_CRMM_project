namespace CRM.Models.DTO
{
    public class OfficeBreakDto
    {
        public int Id { get; set; }
        public DateTime? Createdate { get; set; }
        public string? Starttime { get; set; }
        public string? Endtime { get; set; }
        public string? Breakstatus { get; set; }
        public string? ShiftType { get; set; }
    }
}
