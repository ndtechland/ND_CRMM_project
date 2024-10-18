using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Crm
{
    public partial class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string BlogImage { get; set; } = null!;
        public bool? IsPublished { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
