namespace CRM.Models.DTO
{
    public class GenerateSalary
    {
        public long CustomerID { get; set; }
        public long LocationID { get; set; }
        public DateTime Month { get; set; }
        public DateTime Year { get; set; }
    }
}
