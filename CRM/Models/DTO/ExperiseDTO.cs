using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class ExperiseDTO
    {
        public int Id { get; set; }
        public string ExpertiseName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ExperiseImage { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<OurExpertise> OurExperties { get; set; }
    }
}
