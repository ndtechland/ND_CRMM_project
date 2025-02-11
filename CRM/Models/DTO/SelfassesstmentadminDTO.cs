using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class SelfassesstmentadminDTO
    {
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public bool? Ispoint { get; set; }
        public string? Pointname { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Isdelete { get; set; }
        public List<Selfassesstmentadmin> SelfAssessmentList { get; set; }
    }
}
