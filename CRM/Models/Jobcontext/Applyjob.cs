using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Applyjob
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? JobId { get; set; }
        public DateTime Createddate { get; set; }
        public bool IsActive { get; set; }
        public int Status { get; set; }
    }
}
