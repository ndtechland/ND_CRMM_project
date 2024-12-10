using System.ComponentModel.DataAnnotations;
namespace CRM.Models.DTO
{
    public class GenerateSalary
    {
        [Key]
        public int Id { get; set; }
        public long CustomerID { get; set; }
        public long LocationID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public decimal? MonthlyGrossPay { get; set; }
        public decimal? MonthlyCtc { get; set; }
        public string? SalarySlip { get; set; }
        public string? SalarySlipName { get; set; }
       
        public List<GenerateSalary> GeneratedSalaries { get; set; }
    }
}
