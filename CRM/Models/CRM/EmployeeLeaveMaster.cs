using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeLeaveMaster
    {
        public EmployeeLeaveMaster()
        {
            Payrolls = new HashSet<Payroll>();
        }

        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Reason { get; set; } = null!;
        public int Days { get; set; }

        public virtual ICollection<Payroll> Payrolls { get; set; }
    }
}
