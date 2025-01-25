namespace CRM.Models.DTO
{
    public class Priewempdata
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Emp_Reg_ID { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string? WorkEmail { get; set; }
        public string? Gender { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
        public decimal AnnualCtc { get; set; }
        public string MonthlySalary => (AnnualCtc / 12).ToString("N2");

        public bool? Isactive { get; set; }

    }
}
