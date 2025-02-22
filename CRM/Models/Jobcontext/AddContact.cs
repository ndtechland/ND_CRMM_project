using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class AddContact
    {
        public int Id { get; set; }
        public string? Location { get; set; }
        public string? OpenDays { get; set; }
        public string? OpenHrz { get; set; }
        public string? ClosingHrz { get; set; }
        public string? Email { get; set; }
        public string? CallNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
