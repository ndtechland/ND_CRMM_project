namespace CRM.Models.APIDTO
{
    public class EmpCheckIn
    {
        public string? CurrentLat { get; set; }
        public string? Currentlong { get; set; }
        public int? Userid { get; set; }
        public bool Breakin { get; set; }
        public bool Breakout { get; set; }

    }
}
