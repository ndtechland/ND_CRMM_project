using System.ComponentModel.DataAnnotations;

namespace CRM.Models.CRM
{
    public class forgotpassword
    {
        [Required,EmailAddress,Display(Name ="Registered email address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
    }
}
