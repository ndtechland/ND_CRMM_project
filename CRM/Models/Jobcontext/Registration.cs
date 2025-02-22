using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Registration
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CPassword { get; set; }
        public bool? IsConditionChecked { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? Addedon { get; set; }
    }
}
