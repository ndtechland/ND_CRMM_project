using CRM.Models.Crm;
using System;
using System.Collections.Generic;

namespace CRM.Models.CRM
{
    public partial class FieldSeparatorMaster
    {
        public FieldSeparatorMaster()
        {
            OrganisationProfiles = new HashSet<OrganisationProfile>();
        }

        public int Id { get; set; }
        public string FieldSeparator { get; set; } = null!;

        public virtual ICollection<OrganisationProfile> OrganisationProfiles { get; set; }
    }
}
