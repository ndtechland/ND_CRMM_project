namespace CRM.Models.DTO
{

        public partial class InvoiceMaster
        {
            public int Id { get; set; }
            public string InvoiceNumber { get; set; }
            public string CompanyName { get; set; }
            public string InvoiceDate { get; set; }
            public string Mobile { get; set; }
            public string PhoneNumber { get; set; }
            public Nullable<bool> IsApproved { get; set; }
            public Nullable<bool> isPaid { get; set; }
            public string InvoiceDescription { get; set; }
            public Nullable<decimal> AmountPaid { get; set; }
            public string BillPdf { get; set; }
            public string Email { get; set; }
            public string CompanyAddress { get; set; }
            public string CustomrGstn { get; set; }
            public string ChequeNo { get; set; }
            public string ChequeImage { get; set; }
            public string MOP { get; set; }
            public Nullable<int> ModeOfPayment { get; set; }
            public Nullable<decimal> grandTotal { get; set; }
            public Nullable<decimal> remain { get; set; }
            public string BankName { get; set; }
        }
    public partial class Invoice
    {
        public int ID { get; set; }
        public string? CompanyName { get; set; }
        public string? BillingAddress { get; set; }
        public string? GstNumber { get; set; }
        public decimal? Scgst { get; set; }
        public decimal? Igst { get; set; }
        public decimal? Cgst { get; set; }
        public decimal? Productprice { get; set; }
        public string? ProductName { get; set; } 
        public string? HsnSacCode { get; set; } 
        public string? StateName { get; set; }
        public string? CityName { get; set; }
    
    }
}
