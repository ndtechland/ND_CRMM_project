using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Chargesmaster
    {
        public int Id { get; set; }
        public string? Chargesname { get; set; }
        public decimal? Chargespercentage { get; set; }
    }
}
