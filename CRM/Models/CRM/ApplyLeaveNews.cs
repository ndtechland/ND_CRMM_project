using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class ApplyLeaveNews
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int StartLeaveId { get; set; }
        public int EndeaveId { get; set; }
        public int TypeOfLeaveId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal CountLeave { get; set; }
        public string? Month { get; set; }
        public string? Reason { get; set; }
        public int? Isapprove { get; set; }
        public decimal? PaidCountLeave { get; set; }
    }
}
