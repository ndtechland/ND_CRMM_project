using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class City
    {
        public int Id { get; set; }
        public string City1 { get; set; } = null!;
        public int StateId { get; set; }
    }
}
