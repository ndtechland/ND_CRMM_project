using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Professionaltax
    {
        public int Id { get; set; }
        public decimal? Minamount { get; set; }
        public decimal? Maxamount { get; set; }
        public decimal? Amountpercentage { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? Iactive { get; set; }
        public int? Finyear { get; set; }
    }
}
