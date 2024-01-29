using System.ComponentModel.DataAnnotations;

namespace CRM.Models.DTO
{
    public class GenerateSalaryReportDTO
    {
         
            [Key]
            public int Id { get; set; }
            public long CustomerID { get; set; }
            public long LocationID { get; set; }
            public DateTime Month { get; set; }
            public DateTime Year { get; set; }
            public string? EmployeeId { get; set; }
            public string? EmployeeName { get; set; }
            public decimal? MonthlyGrossPay { get; set; }
            public decimal? MonthlyCtc { get; set; }
            public decimal? GenerateSalary { get; set; }
            public decimal? EPF { get; set; }
            public decimal? ESIC { get; set; }
            public List<GenerateSalaryReportDTO> GenerateSalaryReports { get; set; }
        
    }
}
