using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmpRelievingletter
    {
        public int Id { get; set; }
        public DateTime? ResignationDate { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime? LastDateofEmployment { get; set; }
        public int? Vendorid { get; set; }
        public string? RelievingletterFile { get; set; }
    }
}
