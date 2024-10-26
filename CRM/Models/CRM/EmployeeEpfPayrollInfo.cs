using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeEpfPayrollInfo
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? Epfnumber { get; set; }
        public decimal? Epfpercentage { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Vendorid { get; set; }
    }
}
