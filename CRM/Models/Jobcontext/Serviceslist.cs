using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Serviceslist
    {
        public int Id { get; set; }
        public string? ServiceName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
