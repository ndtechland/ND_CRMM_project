using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class DemoRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? CompanyName { get; set; }
        public string? Description { get; set; }
        public DateTime? RequestDate { get; set; }
    }
}
