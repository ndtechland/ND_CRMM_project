using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class DeductorTypeMaster
    {
        public DeductorTypeMaster()
        {
            TaxDeductors = new HashSet<TaxDeductor>();
        }

        public int Id { get; set; }
        public string DeductorTypeName { get; set; } = null!;

        public virtual ICollection<TaxDeductor> TaxDeductors { get; set; }
    }
}
