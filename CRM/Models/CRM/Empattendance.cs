using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Empattendance
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public decimal? Attendance { get; set; }
        public DateTime? Entry { get; set; }
        public decimal? Lop { get; set; }
        public decimal? GenerateSalary { get; set; }
        public decimal? Incentive { get; set; }
        public decimal? TravellingAllowance { get; set; }
        public string? SalarySlip { get; set; }
        public decimal? EmpEpfvalue { get; set; }
        public decimal? EmpEsivalue { get; set; }
        public decimal? Basicsalary { get; set; }
        public decimal? Hra { get; set; }
        public decimal? SpecialAllowance { get; set; }
        public decimal? Conveyanceallowance { get; set; }
        public decimal? MedicalAllowance { get; set; }
        public decimal? VariablePay { get; set; }
        public decimal? Tds { get; set; }
        public decimal? Professionaltax { get; set; }
    }
}
