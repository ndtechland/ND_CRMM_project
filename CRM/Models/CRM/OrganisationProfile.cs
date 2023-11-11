using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class OrganisationProfile
    {
        public OrganisationProfile()
        {
            OrganisationTaxDetails = new HashSet<OrganisationTaxDetail>();
        }

        public int Id { get; set; }
        public string OrganizationName { get; set; } = null!;
        public string BusinessLocation { get; set; } = null!;
        public int IndustryId { get; set; }
        public int DateFormatId { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public int StateId { get; set; }
        public string City { get; set; } = null!;
        public int Pincode { get; set; }
        public int HeadOfficeAddressId { get; set; }

        public virtual DateFormatMaster DateFormat { get; set; } = null!;
        public virtual HeadOfficeAddress HeadOfficeAddress { get; set; } = null!;
        public virtual IndustryMaster Industry { get; set; } = null!;
        public virtual StateMaster State { get; set; } = null!;
        public virtual ICollection<OrganisationTaxDetail> OrganisationTaxDetails { get; set; }
    }
}
