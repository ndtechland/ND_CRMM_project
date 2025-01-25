using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class UserRole
    {
        public int Id { get; set; }
        public bool? IsAll { get; set; }
        public string? IsHeadChecked { get; set; }
        public string? IsChildHeadChecked { get; set; }
        public string? IsSubHeadChecked { get; set; }
        public string? IsChildSubHeadChecked { get; set; }
        public string? IsSubHeadTwoChecked { get; set; }
        public string? IsChildSubHeadTwoChecked { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? PlanId { get; set; }
    }
}
