using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class HelpCenter
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public DateTime? SubmissionDate { get; set; }
    }
}
