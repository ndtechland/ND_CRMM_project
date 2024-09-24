using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class LeaveApplyDto
    {
        public string Reason { get; set; }
        public int? leavetypeid { get; set; }
        public int? typeofleaveid { get; set; }
        public List<Applieddate> applieddates { get; set; }

    }
}
