using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class TblAdminLogin
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Useremail { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public DateTime? Addedon { get; set; }
        public bool? Isdelete { get; set; }
        public bool? IsActive { get; set; }
    }
}
