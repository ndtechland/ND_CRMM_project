using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class HeadOfficeAddress
    {
        public HeadOfficeAddress()
        {
            OrganisationProfiles = new HashSet<OrganisationProfile>();
        }

        public int Id { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = null!;
        public int StateId { get; set; }
        public int Pincode { get; set; }

        public virtual ICollection<OrganisationProfile> OrganisationProfiles { get; set; }
    }
}
