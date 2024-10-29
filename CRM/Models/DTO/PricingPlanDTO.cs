using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class PricingPlanDTO
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = null!;
        public decimal Price { get; set; }
 
        public decimal? AnnulPrice { get; set; }
        public decimal SavePrice { get; set; }
        public decimal? AnnulPriceInPercentage { get; set; }
        public string? Title { get; set; }
        public string? Support { get; set; }
 
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool? IsActive { get; set; }
        public IFormFile ImageFile { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<PlanFeature> PlanFeatures { get; set; }
    }
    public class PlanFeature
    {
        public int Id { get; set; }
        public int PricingPlanId { get; set; }
        public string Feature { get; set; }
    }
}
