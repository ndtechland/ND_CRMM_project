namespace CRM.Models.DTO
{
    public class SalarySlipDetails
    {
        public int Id { get; set; }
        public string? Email_Id { get; set; }
        public string? Employee_ID { get; set; }
        public decimal Epf { get; set; }
        public string? EPF_Number { get; set; }
        public string? ESINo { get; set; }
        public string? Designation_Name { get; set; }
        public string? First_Name { get; set; }
        public string? Address_Line_1 { get; set; }
        public decimal? Basic { get; set; }
        public string? DA { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public string? WA { get; set; }       
        public string? MA { get; set; }        
        public string? TotalEarning { get; set; }
        public string? NetPay { get; set; }
        public string? InWords { get; set; }
        public string? ESI { get; set; }
        public string? TDS { get; set; }
        public string? LOP { get; set; }
        public string? PT { get; set; }
        public string? SPLDeduction { get; set; }
        public string? TotalDeductions { get; set; }
        public int Account_Number { get; set; }
        public string? Bank_Name { get; set; }
        public string? Month { get; set; }
        public int? Year { get; set; }

    }
}

