namespace CRM.Models.DTO
{
    public class ChangePassworddto
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }
    }
}
//////