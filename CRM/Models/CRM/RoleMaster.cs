using System;
using System.Collections.Generic;

namespace CRM.Models.CRM
{
    public partial class RoleMaster
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
