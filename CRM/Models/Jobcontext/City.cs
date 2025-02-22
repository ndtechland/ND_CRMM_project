using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class City
    {
        public int Id { get; set; }
        public string CityName { get; set; } = null!;
        public int StateId { get; set; }
    }
}
