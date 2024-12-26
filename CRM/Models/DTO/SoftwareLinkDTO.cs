using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class SoftwareLinkDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsHeading { get; set; }
        public bool IsMenu { get; set; }
        public int? Parent_Id { get; set; }
        public bool? IsSubHeading { get; set; }
        public bool? IsSubHeadingTwo { get; set; }
        public bool? Isvendor { get; set; }
        public IEnumerable<Softwarelink> SubHeading { get; set; }
        public IEnumerable<Softwarelink> SubHeadingTwo { get; set; }
        public IEnumerable<Softwarelink> ChildMenus { get; set; }
    }
}
