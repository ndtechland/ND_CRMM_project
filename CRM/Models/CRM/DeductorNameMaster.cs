using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class DeductorNameMaster
    {
        public DeductorNameMaster()
        {
            TaxDeductors = new HashSet<TaxDeductor>();
        }

        public int Id { get; set; }
        public string DeductorName { get; set; } = null!;

        public virtual ICollection<TaxDeductor> TaxDeductors { get; set; }
    }
}
