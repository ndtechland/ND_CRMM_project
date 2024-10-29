using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class MissionVision
    {
        public int Id { get; set; }
        public string? MissionVisionName { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
