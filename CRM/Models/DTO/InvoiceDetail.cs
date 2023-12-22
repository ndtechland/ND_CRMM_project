namespace CRM.Models.DTO
{
    public partial class InvoiceDetail
    {
        public int Id { get; set; }
        public Nullable<int> InvoiceMaster_Id { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> ProductQuantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> FinalPrice { get; set; }
        public Nullable<decimal> DiscountInPercent { get; set; }
        public Nullable<decimal> TaxInPercent { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<decimal> SGST_TaxInPercent { get; set; }
        public Nullable<decimal> IGST_TaxInPercent { get; set; }
        public Nullable<decimal> TaxableAmount { get; set; }
        public Nullable<decimal> DiscountedAmount { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public string InvoiceDetail_HSN { get; set; }
    }
}
