using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class TblUploadBanner
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? FilePath { get; set; }
        public string? BannerHeading { get; set; }
        public string? AddedBy { get; set; }
        public DateTime? AddedOn { get; set; }
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }
        public string? Base64Value { get; set; }
    }
}
