using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class BlogDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string BlogImage { get; set; } = null!;
        public bool? IsPublished { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<Blog> Blogs { get; set; }

    }
}
