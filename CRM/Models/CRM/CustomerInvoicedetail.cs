using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class CustomerInvoicedetail
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public string? Notes { get; set; }
        public string? Terms { get; set; }
        public decimal? Taxamount { get; set; }
        public bool? IsRenewDate { get; set; }
        public int? Taxid { get; set; }
        public decimal? ServiceCharge { get; set; }
    }
}
