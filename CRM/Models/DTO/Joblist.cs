using System.Web.Mvc;

namespace CRM.Models.DTO
{
    public class Joblist
    {
        public int id { get; set; }
        public string DesignationName { get; set; }
        public int NoOfOpening { get; set; }
        public string RequiredExperience { get; set; }
        [AllowHtml]
        public string JobDescription { get; set; }
        public string Department { get; set; }
        public bool Status { get; set; }
        public DateTime AddedOn { get; set; }

        public string? Package { get; set; }
        [AllowHtml]
        public string? Skills { get; set; }
        public string? QualificationName { get; set; }
        public string EmployeementType { get; set; }
        public string? PostedBy { get; set; }
        public string? WorkMode { get; set; }
        public string? stateName { get; set; }
        public string? cityName { get; set; }
        public string? CompanyDescription { get; set; }
        [AllowHtml]
        public string? QualificationDescription { get; set; }
        [AllowHtml]
        public string? AboutDescription { get; set; }
        [AllowHtml]
        public string? ResponsebilitiesDescription { get; set; }
        public List<Joblist> JobList { get; set; }
    }
    public class CurrentOpeningInputs
    {
        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }
        public int StateId { get; set; }
        public int cityId { get; set; }
    }
}
