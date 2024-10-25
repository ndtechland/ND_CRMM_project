﻿using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class PricingPlanDTO
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool? IsActive { get; set; }
        public IFormFile ImageFile { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<PricingPlan> PricingPlans { get; set; }
    }
}