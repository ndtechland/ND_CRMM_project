using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmpExperienceletter
    {
        public int Id { get; set; }
        public string CurrDesignationId { get; set; } = null!;
        public string DesignationId { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Vendorid { get; set; }
        public string? HrDesignation { get; set; }
        public string? HrName { get; set; }
        public string? ExperienceletterFile { get; set; }
        public string? EmployeeId { get; set; }
    }
}
