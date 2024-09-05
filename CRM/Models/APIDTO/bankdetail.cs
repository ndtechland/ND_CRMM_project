namespace CRM.Models.APIDTO
{
    public class bankdetail
    {
        public string AccountHolderName { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string? AccountNumber { get; set; }
        public string? ReEnterAccountNumber { get; set; }
        public string? Ifsc { get; set; }
        public int? AccountTypeId { get; set; }
        public string? EpfNumber { get; set; }
        public string? EmployeeContributionRate { get; set; }
        public string? Nominee { get; set; }
        public string? Chequeimage { get; set; }
        public IFormFile Chequebase64 { get; set; }

    }
}
