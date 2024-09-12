using System;
using System.Collections.Generic;

namespace CRM.Models.Crm
{
    public partial class Offerletter
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Currentdate { get; set; }
        public DateTime? Validdate { get; set; }
        public decimal? MonthlyCtc { get; set; }
        public decimal? AnnualCtc { get; set; }
        public string DesignationId { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public DateTime? DateOfJoining { get; set; }
        public int? Vendorid { get; set; }
        public bool? IsDeleted { get; set; }
        public long? StateId { get; set; }
        public long? CityId { get; set; }
        public string? CandidateAddress { get; set; }
        public string? CandidatePincode { get; set; }
    }
}
