namespace CRM.Models.DTO
{

        public class InvoiceDTO
        {
            public InvoiceMaster InvoiceMaster { get; set; }
            public IEnumerable<InvoiceDetail> InvoiceDetails { get; set; }
            //public IEnumerable<UserReportModel> UserReportModels { get; set; }
        }
}
