using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class WorkLocation1
    {
        public int Id { get; set; }
        public DateTime? Createdate { get; set; }
        public bool? Isactive { get; set; }
        public int? Customerid { get; set; }
        public int? Vendorid { get; set; }
    }
}
