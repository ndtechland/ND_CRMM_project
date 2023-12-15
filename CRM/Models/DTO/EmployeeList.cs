namespace CRM.Models.DTO
{
    public class EmployeeList
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string WorkEmail { get; set; } = null!;
        public string GenderId { get; set; } = null!;
        public string WorkLocationId { get; set; } = null!;
        public string DesignationId { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public string MonthlyCTC { get; set; } = null!;

        public bool? IsDeleted { get; set; }
    }
}
