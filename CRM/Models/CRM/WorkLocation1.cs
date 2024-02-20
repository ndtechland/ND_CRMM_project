using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class WorkLocation1
    {
        public int Id { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public DateTime? Createdate { get; set; }
        public bool? Isactive { get; set; }
        public decimal? Commissoninpercentage { get; set; }
    }
}
