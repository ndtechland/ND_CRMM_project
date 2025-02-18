using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class SelfassesstmentadminDTO
    {
        public int Id { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public bool? Ispoint { get; set; }
        public string? Pointname { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Isdelete { get; set; }
        public List<Selfassesstmentadmin> SelfAssessmentList { get; set; }
    }
    public class SelfassesstmentapiDTO
    {
        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }
        public string? financialstartYear  { get; set; }
        public string? financialEndYear { get; set; }
        public List<Selfassesstmentdetails> selfassesstmentdetails { get; set; }
        
    }
    public class Selfassesstmentdetails
    {
        public int Id { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Pointname { get; set; }

    }
}
