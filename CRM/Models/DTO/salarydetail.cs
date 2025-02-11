﻿namespace CRM.Models.DTO
{
    public class salarydetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;    
         public string? EmployeeId { get; set; }
        public string? WorkLocation { get; set; }
        public long CustomerID { get; set; }
        public long Attendance { get; set; }
        public string? FatherName { get; set; }
        public decimal? Grosspay { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal? Incentive { get; set; }
        public decimal? TravellingAllowance { get; set; }
        public decimal? MonthlyPay { get; set; }
        public decimal? EmployeeEpf { get; set; }
        public decimal? EmployeeEsi { get; set; }
		public decimal? lop { get; set; }

	}
}
