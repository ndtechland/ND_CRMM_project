using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class TaxDeductor
    {
        public TaxDeductor()
        {
            OrganisationTaxDetails = new HashSet<OrganisationTaxDetail>();
        }

        public int Id { get; set; }
        public int DeductorTypeId { get; set; }
        public int DeductorNameId { get; set; }
        public string? DeductorFatherName { get; set; }

        public virtual DeductorNameMaster DeductorName { get; set; } = null!;
        public virtual DeductorTypeMaster DeductorType { get; set; } = null!;
        public virtual ICollection<OrganisationTaxDetail> OrganisationTaxDetails { get; set; }
    }
}
