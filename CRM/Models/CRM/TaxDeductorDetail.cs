using CRM.Models.Crm;
using System;
using System.Collections.Generic;

namespace CRM.Models.CRM
{
    public partial class TaxDeductorDetail
    {
        public TaxDeductorDetail()
        {
            OrganisationTaxDetails = new HashSet<OrganisationTaxDetail>();
        }

        public int Id { get; set; }
        public int DeductorTypeId { get; set; }
        public int DeductorNameId { get; set; }
        public string DeductorFathername { get; set; } = null!;
        public int GrossPay { get; set; }
        public string? EsiNumber { get; set; }
        public string DeductionCycle { get; set; } = null!;
        public decimal? EmployeeContribution { get; set; }
        public decimal? EmployerContribution { get; set; }
        public string EpfNumber { get; set; } = null!;
        public string EmployeeContributionRate { get; set; } = null!;
        public int PfWage { get; set; }
        public int? EmployeeContributionEpf { get; set; }
        public decimal? EmployerContributionEps { get; set; }
        public decimal? EmployerContributionEpf { get; set; }

        public virtual DeductorNameMaster DeductorName { get; set; } = null!;
        public virtual DeductorTypeMaster DeductorType { get; set; } = null!;
        public virtual ICollection<OrganisationTaxDetail> OrganisationTaxDetails { get; set; }
    }
}
