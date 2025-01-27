using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class AddWorkLocation
    {
        public int Id { get; set; }
        public int? WorkLocationid { get; set; }
        public string? WorkLocationName { get; set; }
    }
}
