using System;
using System.Collections.Generic;

namespace CRM.Models.CRM
{
    public partial class BannerMaster
    {
        public int Id { get; set; }
        public string BannerImage { get; set; } = null!;
        public string BannerPath { get; set; } = null!;
    }
}
