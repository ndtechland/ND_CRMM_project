using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Selfassesstmentdatum
    {
        public int Id { get; set; }
        public string? EmpId { get; set; }
        public int? Startyear { get; set; }
        public int? Endyear { get; set; }
        public string? AssesstmentAns { get; set; }
        public DateTime? Createddate { get; set; }
        public bool? IsActive { get; set; }
        public string? ManagerName { get; set; }
    }
}
