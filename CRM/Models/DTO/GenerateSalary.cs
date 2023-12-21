namespace CRM.Models.DTO
{
    public class GenerateSalary
    {
        public int Id { get; set; }
        public long CustomerID { get; set; }
        public long LocationID { get; set; }
        public DateTime Month { get; set; }
        public DateTime Year { get; set; }
        public string? EmployeeId { get; set; } 
        public string? EmployeeName { get; set; }
        public decimal? MonthlyGrossPay { get; set; }
        public decimal? MonthlyCtc { get; set; }
        public List<GenerateSalary> GeneratedSalaries { get; set; }
    }
}
