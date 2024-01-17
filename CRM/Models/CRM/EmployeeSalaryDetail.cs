using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeSalaryDetail
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public decimal Basic { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal TravellingAllowance { get; set; }
        public decimal Esic { get; set; }
        public decimal Epf { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal? MonthlyGrossPay { get; set; }
        public decimal? MonthlyCtc { get; set; }
        public int? EmpId { get; set; }
        public decimal? AnnualCtc { get; set; }
        public decimal? Professionaltax { get; set; }
        public decimal? Incentive { get; set; }
    }
}
