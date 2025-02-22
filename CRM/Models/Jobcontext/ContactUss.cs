using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class ContactUss
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? AboutusId { get; set; }
        public string? TextMessage { get; set; }
        public bool? IsDelete { get; set; }
    }
}
