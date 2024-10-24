using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class OurExpertise
    {
        public int Id { get; set; }
        public string ExpertiseName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ExperiseImage { get; set; }
    }
}
