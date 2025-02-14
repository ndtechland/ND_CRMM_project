using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Hrsignature
    {
        public int Id { get; set; }
        public string? HrSignature1 { get; set; }
        public string? HrJobTitle { get; set; }
        public string? HrName { get; set; }
        public int? Vendorid { get; set; }
    }
}
