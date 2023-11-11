using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class DateFormatMaster
    {
        public DateFormatMaster()
        {
            OrganisationProfiles = new HashSet<OrganisationProfile>();
        }

        public int Id { get; set; }
        public string DateFormat { get; set; } = null!;

        public virtual ICollection<OrganisationProfile> OrganisationProfiles { get; set; }
    }
}
