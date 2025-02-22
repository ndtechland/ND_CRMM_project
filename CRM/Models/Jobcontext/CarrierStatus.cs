using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class CarrierStatus
    {
        public int Id { get; set; }
        public string? Statusname { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
