namespace CRM.Models.DTO
{
    public class salarydetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;    
       public string? EmployeeId { get; set; }
      
        public decimal? MonthlyCtc { get; set; }
        public bool? IsDeleted { get; set; }
       

    }
}
