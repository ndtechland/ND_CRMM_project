using CRM.Models.Crm;
using System.Text.Json.Serialization;

namespace CRM.Models.DTO
{
    public class PricingPlanDTO
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = null!;
        public decimal Price { get; set; }
 
        public decimal? AnnulPrice { get; set; }
        public decimal SavePrice { get; set; }
        public int? AnnulPriceInPercentage { get; set; }
        public string? Title { get; set; }
        public string? Support { get; set; }
        public string? Image { get; set; }
        public bool? IsActive { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IFormFile ImageFile { get; set; }
        public DateTime? CreatedDate { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PricingPlan> PricingPlansList { get; set; }
        public List<PlanFeature> PlanFeatures { get; set; }
    }
    public class PlanFeature
    {
        public int Id { get; set; }
        public int PricingPlanId { get; set; }
        public string Feature { get; set; }
    }
}
