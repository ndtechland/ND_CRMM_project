using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Qualification
    {
        public int Id { get; set; }
        public string? Qualificationame { get; set; }
        public int? EducationTypeId { get; set; }
        public DateTime AddedOn { get; set; }
        public int? Status { get; set; }
        public bool IsDelete { get; set; }
    }
}
