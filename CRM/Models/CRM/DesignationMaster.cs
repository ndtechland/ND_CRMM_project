using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class DesignationMaster
    {
        public int Id { get; set; }
        public string DesignationName { get; set; } = null!;
        public int? AdminLoginId { get; set; }
    }
}
