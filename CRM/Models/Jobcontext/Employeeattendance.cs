using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Employeeattendance
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Attendance { get; set; }
        public DateTime? Entry { get; set; }
        public decimal? Lop { get; set; }
        public decimal? GenerateSalary { get; set; }
        public string? SalarySlipPath { get; set; }
    }
}
