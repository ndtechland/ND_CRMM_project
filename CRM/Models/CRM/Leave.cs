﻿using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Leave
    {
        public int Id { get; set; }
        public string? Typeofleave { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? Createddate { get; set; }
    }
}