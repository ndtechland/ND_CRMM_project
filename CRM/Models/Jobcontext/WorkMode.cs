﻿using System;
using System.Collections.Generic;

namespace CRM.Models.Jobcontext
{
    public partial class WorkMode
    {
        public int Id { get; set; }
        public string? WorkModeName { get; set; }
        public DateTime? AddedOn { get; set; }
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }
        public int? AddedBy { get; set; }
    }
}
