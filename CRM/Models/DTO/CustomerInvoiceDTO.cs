﻿using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class CustomerInvoiceDTO
    {
        public int? ProductId { get; set; }
        public decimal? ProductPrice { get; set; }
        public string? Description { get; set; }
        public int InvoiceId { get; set; }
        public int? CustomerId { get; set; }
        public string? CompanyName { get; set; }
        public string? VendorCompanyName { get; set; }
        public string? CompanyLogo { get; set; }
        public string Email { get; set; }
        public string AlternateNumber { get; set; }
        public string MobileNumber { get; set; }
        public string ProductName { get; set; }
        public decimal? RenewPrice { get; set; }
        public int? NoOfRenewMonth { get; set; }
        public string HsnSacCode { get; set; }
        public string VendorOfficeState { get; set; }
        public string OfficeState { get; set; }
        public string BillingState { get; set; }
        public string BillingCity { get; set; }
        public string VendorOfficeCity { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeAddress { get; set; }
        public string VendorOfficeAddress { get; set; }
        public string BillingAddress { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? RenewDate { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? VendorGstNumber { get; set; }
        public string? CustomerGstNumber { get; set; }
        public decimal? IGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
        public List<ProductDetailList> ProductDetailLists { get; set; }
        public List<CustomerInvoice> customerInvoice { get; set; }
    }

    public class ProductDetail
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string? Description { get; set; }
        public int? CustomerId { get; set; }
        public decimal? ProductPrice { get; set; } 
        public decimal? RenewPrice { get; set; }
        public int? NoOfRenewMonth { get; set; }
        public string? HsnSacCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? RenewDate { get; set; }
        public decimal? IGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
    }

    public class ProductDetailList
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? ProductPrice { get; set; }
        public decimal? IGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
        public string? HsnSacCode { get; set; }
        public decimal? RenewPrice { get; set; }
        public int? NoOfRenewMonth { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? RenewDate { get; set; }
    }


}
