using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class PricingPlan
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal? AnnulPrice { get; set; }
        public decimal? AnnulPriceInPercentage { get; set; }
        public string? Title { get; set; }
    }
}
