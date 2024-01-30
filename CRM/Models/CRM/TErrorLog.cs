using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class TErrorLog
    {
        public long Id { get; set; }
        public string UserId { get; set; } = null!;
        public string? Role { get; set; }
        public string? Method { get; set; }
        public string? Request { get; set; }
        public string? Reponse { get; set; }
        public string? Despcrtiption { get; set; }
        public DateTime? EntryDate { get; set; }
    }
}
