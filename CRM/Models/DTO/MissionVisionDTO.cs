using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class MissionVisionDTO
    {
        public int Id { get; set; }
        public string? MissionVisionName { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<MissionVision> MissionVisions { get; set; }
    }
}
