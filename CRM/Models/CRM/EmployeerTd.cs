using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeerTd
    {
        public int Id { get; set; }
        public decimal? Amount { get; set; }
        public int? CustomerId { get; set; }
        public string WorkLocationId { get; set; } = null!;
        public bool Isactive { get; set; }
        public DateTime? CreateDate { get; set; }
        public decimal? Tdspercentage { get; set; }
    }
}
