namespace CRM.Models.APIDTO
{
    public class EmployeeBasicInfo
    {

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string WorkEmail { get; set; } = null!;
        public string GenderName { get; set; } = null!;
        public string WorkLocationName { get; set; } = null!;
        public string DesignationName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public bool? IsDeleted { get; set; }
        public string? CustomerName { get; set; }
        public string? EmployeeId { get; set; }
       // public int? RoleId { get; set; }
    }
}
