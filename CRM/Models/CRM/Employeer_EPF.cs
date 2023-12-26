using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Employeer_EPF
    {
        public int Id { get; set; }
        public string? EPF_Number { get; set; }
        public string? Deduction_Cycle { get; set; }
        public string? Employer_Contribution_Rate { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
