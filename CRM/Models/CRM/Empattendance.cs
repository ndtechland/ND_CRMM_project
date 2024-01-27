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
        public int? Attendance { get; set; }
        public DateTime? Entry { get; set; }
        public decimal? Lop { get; set; }
        public decimal? GenerateSalary { get; set; }
        public decimal? Incentive { get; set; }
        public decimal? TravellingAllowance { get; set; }
    }
}
