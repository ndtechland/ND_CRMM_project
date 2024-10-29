using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class PricingPlanFeature
    {
        public int Id { get; set; }
        public int? PricingPlanId { get; set; }
        public string? Feature { get; set; }
    }
}
