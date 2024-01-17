namespace CRM.Models.DTO
{
    public class ECS
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? EmployeeId { get; set; }
        public string? WorkLocation { get; set; }
        public long CustomerID { get; set; }
        public int AccountNumber { get; set; }
        public string? Ifsc { get; set; }
        public decimal? netpayment { get; set; }
    }
}
