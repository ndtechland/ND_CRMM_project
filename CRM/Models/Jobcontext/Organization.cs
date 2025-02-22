using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Organization
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; } = null!;
        public int? AddedBy { get; set; }
        public bool? Status { get; set; }
        public string? Description { get; set; }
        public DateTime? AddedOn { get; set; }
        public bool? IsDelete { get; set; }
        public int? CompanytypeId { get; set; }
        public string? CompanyImage { get; set; }
        public int? CityId { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string? OfficeUrl { get; set; }
        public string? InstaLink { get; set; }
        public string? YoutubeLink { get; set; }
        public string? LinkdnLink { get; set; }
        public string? FacebookLink { get; set; }
        public string? TiwtterLink { get; set; }
        public int? StateId { get; set; }
        public int? Companycategoryid { get; set; }
        public string? BgCompanyImage { get; set; }
        public string? CompanyAddress { get; set; }
    }
}
