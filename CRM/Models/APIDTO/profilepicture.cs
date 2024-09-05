using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.APIDTO
{
    public class profilepicture
    {
        public string? EmpProfiles { get; set; }
        [NotMapped]
        public IFormFile Empprofile { get; set; }
    }
}
