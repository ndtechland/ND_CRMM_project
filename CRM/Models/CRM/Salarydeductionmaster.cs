using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Salarydeductionmaster
    {
        public int Id { get; set; }
        public int? Vendorid { get; set; }
        public string? Deductiontype { get; set; }
        public decimal? Deductionpercentage { get; set; }
    }
}
