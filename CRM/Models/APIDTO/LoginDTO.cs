namespace CRM.Models.APIDTO
{
    public class LoginDTO
    {
        public string? Employee_ID { get; set; }
        public string? Password { get; set; }
    }
    public class refreshTokenModel
    {
        public string? refreshToken { get; set; }
    }
    public class LoginProfile
    {
        public int? userid { get; set; }
        public string? Employee_Name { get; set; }
        public string? Employee_ID { get; set; }
    }
    public class DevicetokenDTO
    {
        public int? userid { get; set; }
        public string? DeviceId { get; set; }
    }

}
