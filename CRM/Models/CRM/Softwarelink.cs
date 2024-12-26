using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Softwarelink
    {
        public int Id { get; set; }
        public string? Tittle { get; set; }
        public string? Url { get; set; }
        public bool? Isheading { get; set; }
        public bool? Ismenu { get; set; }
        public int? ParentId { get; set; }
        public bool? Isactive { get; set; }
        public bool? IsSubHeading { get; set; }
        public bool? IsSubHeadingTwo { get; set; }
        public bool? Isvendor { get; set; }
    }
}
