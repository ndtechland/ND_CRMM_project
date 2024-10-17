using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeEsicPayrollInfo
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? Esicnumber { get; set; }
        public decimal? Esicpercentage { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Vendorid { get; set; }
    }
}
