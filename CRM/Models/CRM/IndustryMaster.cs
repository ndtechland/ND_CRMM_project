using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class IndustryMaster
    {
        public IndustryMaster()
        {
            OrganisationProfiles = new HashSet<OrganisationProfile>();
        }

        public int Id { get; set; }
        public string IndustryName { get; set; } = null!;

        public virtual ICollection<OrganisationProfile> OrganisationProfiles { get; set; }
    }
}
