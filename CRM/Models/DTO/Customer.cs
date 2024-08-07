﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.DTO
{
    public class Customer
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        [NotMapped]
        public string[] WorkLocation { get; set; }
        public string? MobileNumber { get; set; }
        public string? AlternateNumber { get; set; }
        public string? Email { get; set; }
        public string? GstNumber { get; set; }
        public string? BillingAddress { get; set; }
        public string? ProductDetails { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime RenewDate { get; set; }
        public string? Scgst { get; set; }
        public string? Cgst { get; set; }
        public string? Igst { get; set; }
        public string? Gst { get; set; }
        public string? HsnSacCode { get; set; } 
        public double Price { get; set; }
        public string? State { get; set; }
        public int? StateId { get; set; }
        public string StateName { get; set; } = null!;
    }
}
