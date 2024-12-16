using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class ScheduledTask
    {
        public int Id { get; set; }
        public string? Schedulemethod { get; set; }
        public TimeSpan? Scheduletime { get; set; }
        public string? Scheduleday { get; set; }
        public bool? IsExcute { get; set; }
        public DateTime? Excutetime { get; set; }
    }
}
