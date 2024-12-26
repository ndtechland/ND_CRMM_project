namespace CRM.Models.DTO
{
    public class LeavemasterDto
    {
        public int id { get; set; }
        public string LeavetypeId { get; set; }
        public decimal? Value { get; set; }
        public string? EmpId { get; set; }
        public string? EmpName { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? LeaveStartDate { get; set; }
        public bool IsActive { get; set; }
        public List<LeavemasterDto> lmd { get; set; }
    }
}
