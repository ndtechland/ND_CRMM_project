using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class SoftwareLinkDTO
    {
        public int Id { get; set; }
        public string? Tittle { get; set; }
        public string? Url { get; set; }
        public bool? Isheading { get; set; }
        public bool? Ismenu { get; set; }
        public int? ParentId { get; set; }
        public bool? Isactive { get; set; }
        public bool? IsSubHeading { get; set; }
        public bool? IsSubHeadingTwo { get; set; }
        public bool? Isvendor { get; set; }
        public IEnumerable<SubSoftwarelink> SubHeading { get; set; }
        public IEnumerable<Softwarelink> ChildMenus { get; set; }
    }
    public class SubSoftwarelink
    {
        public int Id { get; set; }
        public string? Tittle { get; set; }
        public string? Url { get; set; }
        public bool? Isheading { get; set; }
        public bool? Ismenu { get; set; }
        public int? ParentId { get; set; }
        public bool? Isactive { get; set; }
        public bool? IsSubHeading { get; set; }
        public bool? IsSubHeadingTwo { get; set; }
        public bool? Isvendor { get; set; }
        public IEnumerable<SubSoftwarelinkTwo> SubHeadingTwo { get; set; }
        public IEnumerable<Softwarelink> ChildMenus { get; set; }
    }
    public class SubSoftwarelinkTwo
    {
        public int Id { get; set; }
        public string? Tittle { get; set; }
        public string? Url { get; set; }
        public bool? Isheading { get; set; }
        public bool? Ismenu { get; set; }
        public int? ParentId { get; set; }
        public bool? Isactive { get; set; }
        public bool? IsSubHeading { get; set; }
        public bool? IsSubHeadingTwo { get; set; }
        public bool? Isvendor { get; set; }
        public IEnumerable<Softwarelink> ChildMenus { get; set; }
    }
}
