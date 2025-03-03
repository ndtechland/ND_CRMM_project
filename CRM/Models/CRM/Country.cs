using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = null!;
    }
}
