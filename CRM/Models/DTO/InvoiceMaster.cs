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
        public int EmployeeCount { get; set; }
        public string? Company_Name { get; set; }
        public string? InvoiceDate { get; set; }
        public string? Mobile { get; set; }
        public string? PhoneNumber { get; set; }
        public string? InvoiceDescription { get; set; }
        public Nullable<decimal> AmountPaid { get; set; }
        public string? Email { get; set; }
        public string? Billing_Address { get; set; }
        public string? GST_Number { get; set; }
        public string? HSN_SAC_Code { get; set; }
        public string? Scgst { get; set; }
        public string? Cgst { get; set; }
        public string? Igst { get; set; }
        public Nullable<decimal> grandTotal { get; set; }
        public Nullable<decimal> remain { get; set; }
        public string? BankName { get; set; }
        public string? State { get; set; }

    }
}
