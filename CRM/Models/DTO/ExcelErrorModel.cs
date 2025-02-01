namespace CRM.Models.DTO
{
    public class ExcelErrorModel
    {
        public string ErrorType { get; set; }

        public string Description { get; set; }
        public int AffectedRow { get; set; }
    }
}
