using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class BannerMaster
    {
        public int Id { get; set; }
        public string? BannerImage { get; set; }
        public string? Bannerdescription { get; set; }
        public string? Imagepath { get; set; }
        public string? AddedBy { get; set; }
        public DateTime? AddedOn { get; set; }
    }
}
