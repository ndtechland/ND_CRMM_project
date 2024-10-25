namespace CRM.Models.DTO
{
    public class ProfessionaltaxDto
    {
        public int? Id { get; set; }
        public decimal? Minamount { get; set; }
        public decimal? Maxamount { get; set; }
        public decimal? Amountpercentage { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? Iactive { get; set; }
        public string? Finyear { get; set; }
    }
}
