using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class EmpTasksList
    {
        public int Id { get; set; }
        public string? Taskreason { get; set; }
        public DateTime? Replydate { get; set; }
        public int? Taskstatus { get; set; }
        public bool? IsApprove { get; set; }
        public int? Subtaskid { get; set; }
        public int? Taskid { get; set; }
        public string? EmployeeId { get; set; }
    }
}
