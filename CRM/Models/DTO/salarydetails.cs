namespace CRM.Models.DTO
{
    public class salarydetails
    {
        public decimal Basic { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal TravellingAllowance { get; set; }
        public decimal Esic { get; set; }
        public decimal Epf { get; set; }
        public decimal? MonthlyGrossPay { get; set; }
        public decimal? MonthlyCtc { get; set; }
        public decimal? Professionaltax { get; set; }
        public decimal? SpecialAllowance { get; set; }
        public decimal? Gross { get; set; }
    }
}
