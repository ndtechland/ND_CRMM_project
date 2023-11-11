using CRM.Models.Crm;
using System;
using System.Collections.Generic;

namespace CRM.Models.CRM
{
    public partial class AddressMaster
    {
        public int Id { get; set; }
        public string AddressLine1 { get; set; } = null!;
        public string AddressLine2 { get; set; } = null!;
        public string City { get; set; } = null!;
        public int StateId { get; set; }
        public int Pincode { get; set; }

        public virtual StateMaster State { get; set; } = null!;
    }
}
