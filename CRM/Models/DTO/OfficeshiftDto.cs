namespace CRM.Models.DTO
{
    public class OfficeshiftDto
    {
        public int Id { get; set; }
        public int? Vendorid { get; set; }
        public DateTime? Createdate { get; set; }
        public string? ShiftTypeid { get; set; }
        public TimeSpan? Starttime { get; set; }
        public TimeSpan? Endtime { get; set; }
    }
}
