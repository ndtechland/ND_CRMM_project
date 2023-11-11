using CRM.Models.Crm;
using System;
using System.Collections.Generic;

namespace CRM.Models.CRM
{
    public partial class SalaryDetail
    {
        public int Id { get; set; }
        public int EmployeeRegistrationId { get; set; }
        public int BankDetailsId { get; set; }
        public int AnnualCtc { get; set; }
        public int? BasicMonthlyAmount { get; set; }
        public int? BasicAnnualAmount { get; set; }
        public int? HouseRentAllowanceMonthlyAmount { get; set; }
        public int? HouseRentAllowanceAnnualAmount { get; set; }
        public double ConveyanceAllowanceMonthlyAmount { get; set; }
        public double ConveyanceAllowanceAnnualAmount { get; set; }
        public double FixedAllowanceMonthlyAmount { get; set; }
        public double FixedAllowanceAnnualAmount { get; set; }
        public int IncomeTax { get; set; }
        public int TotalDeduction { get; set; }
        public int? NetPay { get; set; }

        public virtual BankDetail BankDetails { get; set; } = null!;
        public virtual EmployeeRegistration EmployeeRegistration { get; set; } = null!;
    }
}
