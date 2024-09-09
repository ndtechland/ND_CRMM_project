namespace CRM.Models.DTO
{
    public class ApprovedbankdetailList
    {
        public int id { get; set; }
        public string AccountHolderName { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string? AccountNumber { get; set; }
        public string? ReEnterAccountNumber { get; set; }
        public string? Ifsc { get; set; }
        public string? AccountTypeId { get; set; }
        public string? EpfNumber { get; set; }
        public string? Nominee { get; set; }
        public string? Chequeimage { get; set; }
        public IFormFile Chequebase64 { get; set; }
        public string? EmployeeId { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? UpdateDate { get; set; }
        public List<ApprovedbankdetailList> Approvedbankdetails { get; set; }

    }
}
