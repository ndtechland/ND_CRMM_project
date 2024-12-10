using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class EmployeerModelEPF
    {
        public int Id { get; set; }

        public string? EPF_Number { get; set; }
        public string? Deduction_Cycle { get; set; }
        public string? Employer_Contribution_Rate { get; set; }
        public string? EsicEPF_Number { get; set; }
        public string? EsicEmployer_Contribution_Rate { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsActive { get; set; }
        public List<EmployeerEpf> EmployeerEpflist { get; set; }
}
}
