namespace CRM.Models.DTO
{
    public class empOfferletter
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime Validdate { get; set; }
        public decimal? MonthlyCtc { get; set; }
        public decimal? AnnualCtc { get; set; }
        public string DesignationName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public DateTime DateOfJoining { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }
        public string? CandidateAddress { get; set; }
        public string? CandidatePincode { get; set; }

    }
}
