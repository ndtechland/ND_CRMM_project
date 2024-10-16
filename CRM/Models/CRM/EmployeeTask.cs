﻿using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmployeeTask
    {
        public int Id { get; set; }
        public string? Task { get; set; }
        public string? Tittle { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public string? EmployeeId { get; set; }
        public int? Vendorid { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
    }
}
