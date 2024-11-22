using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class InvoiceChargesmaster
    {
        public int Id { get; set; }
        public int? Vendorid { get; set; }
        public bool? Isactive { get; set; }
    }
}
