using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class CaseStudiesDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<CaseStudy> CaseStudyList { get; set; }
    }
    public class InvoiceChargesmasterDto
    {
        public int Id { get; set; }
        public string? Vendorid { get; set; }
        public bool? Isactive { get; set; }
        public List<InvoiceChargesmasterDto> taxlist { get; set; }

    }

}
