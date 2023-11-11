using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class AccountTypeMaster
    {
        public int Id { get; set; }
        public string AccountType { get; set; } = null!;
    }
}
