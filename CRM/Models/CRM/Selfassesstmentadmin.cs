using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Selfassesstmentadmin
    {
        public int Id { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public bool? Ispoint { get; set; }
        public string? Pointname { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Isdelete { get; set; }
    }
}
