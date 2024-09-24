using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Job
    {
        public Job()
        {
            JobParameters = new HashSet<JobParameter>();
            State1s = new HashSet<State1>();
        }

        public long Id { get; set; }
        public long? StateId { get; set; }
        public string? StateName { get; set; }
        public string InvocationData { get; set; } = null!;
        public string Arguments { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpireAt { get; set; }

        public virtual ICollection<JobParameter> JobParameters { get; set; }
        public virtual ICollection<State1> State1s { get; set; }
    }
}
