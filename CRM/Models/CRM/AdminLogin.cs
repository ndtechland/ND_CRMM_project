using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class AdminLogin
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? Emailid { get; set; }
    }
}
