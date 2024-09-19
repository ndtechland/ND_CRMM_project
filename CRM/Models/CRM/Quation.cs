using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Quation
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string SalesPersonName { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string? Amount { get; set; }
        public string? Mobile { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
