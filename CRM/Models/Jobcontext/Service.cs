using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Service
    {
        public int Id { get; set; }
        public int Servicesid { get; set; }
        public string? ServiceDescription { get; set; }
        public string? ServiceImage { get; set; }
        public string? ServiceBannerImage { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
