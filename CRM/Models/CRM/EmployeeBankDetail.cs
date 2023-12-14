using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeBankDetail
    {
        public EmployeeBankDetail()
        {
            Payrolls = new HashSet<Payroll>();
        }

        public int Id { get; set; }
        public int EmployeeRegistrationId { get; set; }
        public string AccountHolderName { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public int AccountNumber { get; set; }
        public int ReEnterAccountNumber { get; set; }
        public string Ifsc { get; set; } = null!;
        public int AccountTypeId { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual EmployeeRegistration EmployeeRegistration { get; set; } = null!;
        public virtual ICollection<Payroll> Payrolls { get; set; }
    }
}
