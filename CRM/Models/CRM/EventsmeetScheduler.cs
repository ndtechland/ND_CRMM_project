using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EventsmeetScheduler
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? Tittle { get; set; }
        public string? Description { get; set; }
        public DateTime? Createddate { get; set; }
        public int? Vendorid { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsEventsmeet { get; set; }
    }
}
