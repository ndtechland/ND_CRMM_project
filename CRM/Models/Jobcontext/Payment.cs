using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Payment
    {
        public int Id { get; set; }
        public string Amount { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string ProductInfo { get; set; } = null!;
        public string Surl { get; set; } = null!;
        public string Furl { get; set; } = null!;
        public string TxnId { get; set; } = null!;
        public bool Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modify { get; set; }
        public int Count { get; set; }
        public string? EmployeeId { get; set; }
    }
}
