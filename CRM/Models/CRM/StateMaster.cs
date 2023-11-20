using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class StateMaster
    {
        public StateMaster()
        {
            BillingDetails = new HashSet<BillingDetail>();
            EmployeePersonalAddresses = new HashSet<EmployeePersonalAddress>();
            HeadOfficeAddresses = new HashSet<HeadOfficeAddress>();
            OrganisationProfiles = new HashSet<OrganisationProfile>();
        }

        public int Id { get; set; }
        public string StateName { get; set; } = null!;

        public virtual ICollection<BillingDetail> BillingDetails { get; set; }
        public virtual ICollection<EmployeePersonalAddress> EmployeePersonalAddresses { get; set; }
        public virtual ICollection<HeadOfficeAddress> HeadOfficeAddresses { get; set; }
        public virtual ICollection<OrganisationProfile> OrganisationProfiles { get; set; }
    }
}
