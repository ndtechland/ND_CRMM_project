using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeerEpf
    {
        public int Id { get; set; }
        public string? EpfNumber { get; set; }
        public string? DeductionCycle { get; set; }
        public string? EmployerContributionRate { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
