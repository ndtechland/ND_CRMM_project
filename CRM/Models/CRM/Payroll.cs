using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Payroll
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int SalaryId { get; set; }
        public int BankDetailsId { get; set; }
        public int LeaveId { get; set; }
        public DateTime Date { get; set; }
        public string Report { get; set; } = null!;
        public int TotalAmount { get; set; }

        public virtual EmployeeBankDetail BankDetails { get; set; } = null!;
        public virtual EmployeeRegistration Employee { get; set; } = null!;
        public virtual EmployeeLeaveMaster Leave { get; set; } = null!;
    }
}
