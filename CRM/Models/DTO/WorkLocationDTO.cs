namespace CRM.Models.DTO
{
    public class WorkLocationDTO
    {
        public int Id { get; set; }
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public decimal? Commissoninpercentage { get; set; }
    }
}
