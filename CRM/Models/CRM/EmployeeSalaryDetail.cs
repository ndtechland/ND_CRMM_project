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
        public decimal? Servicecharge { get; set; }
        public decimal? SpecialAllowance { get; set; }
        public decimal? Gross { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Tdspercentage { get; set; }
        public decimal? Conveyanceallowance { get; set; }
        public decimal? FixedAllowance { get; set; }
        public decimal? Medical { get; set; }
        public decimal? Composite { get; set; }
        public decimal? VariablePay { get; set; }
        public decimal? EmployerContribution { get; set; }
        public decimal? Tdsvalue { get; set; }
        public decimal? Basicpercentage { get; set; }
        public decimal? Hrapercentage { get; set; }
        public decimal? Conveyancepercentage { get; set; }
        public decimal? Medicalpercentage { get; set; }
        public decimal? Variablepercentage { get; set; }
        public decimal? EmployerContributionpercentage { get; set; }
        public decimal? Epfpercentage { get; set; }
        public decimal? Esipercentage { get; set; }
    }
}
