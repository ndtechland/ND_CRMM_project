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
        public decimal? Attendance { get; set; }
        public DateTime? Entry { get; set; }
        public decimal? Lop { get; set; }
        public decimal? Incentive { get; set; }
        public decimal? TravellingAllowance { get; set; }
        public decimal? Basicsalary { get; set; }
        public decimal? Hra { get; set; }
        public decimal? SpecialAllowance { get; set; }
        public decimal? Conveyanceallowance { get; set; }
        public decimal? MedicalAllowance { get; set; }
        public decimal? VariablePay { get; set; }
        public decimal? Tds { get; set; }
        public decimal? Professionaltax { get; set; }
        public List<GenerateSalaryReportDTO> GenerateSalaryReports { get; set; }

    }
}
