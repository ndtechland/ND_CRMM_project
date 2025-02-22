namespace CRM.Models.DTO
{
    public class GetApplyjob
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? EmailID { get; set; }
        public long MobileNumber { get; set; }
        public string? Experience { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }
        public string? GenderName { get; set; }
        public string? Dateofbirth { get; set; }
        public string? ResumeFilePath { get; set; }
        public int? Pincode { get; set; }
        public string? Address { get; set; }
        public string? ProfileImage { get; set; }
        public string? Designation { get; set; }
        public string? CarrierStatus { get; set; }
        public int? carrierlistid { get; set; }
        public decimal CurrentCTC { get; set; }
        public decimal ExpectedCTC { get; set; }
        public string? PositionApplyFor { get; set; }
        public string? QualificationName { get; set; }
    }
}
