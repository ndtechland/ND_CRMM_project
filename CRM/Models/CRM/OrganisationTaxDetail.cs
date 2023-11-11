using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class OrganisationTaxDetail
    {
        public int Id { get; set; }
        public int OrganisationProfileId { get; set; }
        public string Pan { get; set; } = null!;
        public string Tan { get; set; } = null!;
        public string TdsCircleCode { get; set; } = null!;
        public string TaxPaymentFrequency { get; set; } = null!;
        public int TaxDeductorId { get; set; }

        public virtual OrganisationProfile OrganisationProfile { get; set; } = null!;
        public virtual TaxDeductor TaxDeductor { get; set; } = null!;
    }
}
