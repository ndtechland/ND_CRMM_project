namespace CRM.Models.APIDTO
{
    public class LoginDTO
    {
        public string? Employee_ID { get; set; }
        public string? Password { get; set; }
    }
    public class LoginProfile
    {
        public string? Employee_Name { get; set; }
        public string? Employee_ID { get; set; }
    }
}
