using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class TutorialDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string VedioUrl { get; set; } = null!;
        public string? Author { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public IFormFile VideoFile { get; set; }
        public List<OurTutorial> OurTutorials { get; set; }
    }
}
