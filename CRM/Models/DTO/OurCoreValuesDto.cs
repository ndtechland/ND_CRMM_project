using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class OurCoreValuesDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Author { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? Image { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<OurCoreValue> OurCoreValueList { get; set; }
    }
}
