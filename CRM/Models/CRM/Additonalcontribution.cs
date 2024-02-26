using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Additonalcontribution
    {
        public int Id { get; set; }
        public string? ContributionName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Isactive { get; set; }
        public long? CustomerId { get; set; }
        public long? WorkLocationId { get; set; }
    }
}
