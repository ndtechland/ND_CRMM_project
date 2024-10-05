using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class CustomerInvoice
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? VendorId { get; set; }
        public int? ProductId { get; set; }
        public decimal? ProductPrice { get; set; }
        public decimal? RenewPrice { get; set; }
        public int? NoOfRenewMonth { get; set; }
        public string? Hsncode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? RenewDate { get; set; }
        public decimal? Igst { get; set; }
        public decimal? Sgst { get; set; }
        public decimal? Cgst { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
