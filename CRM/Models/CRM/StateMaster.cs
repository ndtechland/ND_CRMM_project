using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class StateMaster
    {
        public StateMaster()
        {
            BillingDetails = new HashSet<BillingDetail>();
            OrganisationProfiles = new HashSet<OrganisationProfile>();
        }

        public int Id { get; set; }
        public string StateName { get; set; } = null!;

        public virtual ICollection<BillingDetail> BillingDetails { get; set; }
        public virtual ICollection<OrganisationProfile> OrganisationProfiles { get; set; }
    }
}
