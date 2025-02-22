using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class Bookmark
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int JobId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Isactive { get; set; }
    }
}
