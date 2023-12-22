using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeLeaveMaster
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Reason { get; set; } = null!;
        public int Days { get; set; }
    }
}
