using DocumentFormat.OpenXml.Office2010.Excel;
using Org.BouncyCastle.Asn1.Ocsp;

namespace CRM.Models.CRM
{
    public class TErrorLog
    {
            public int Id { get; set; }
            public string? UserId { get; set; }
            public string? Role { get; set; }
            public string? Method { get; set; }
            public string? Request { get; set; }
            public string? Reponse { get; set; }
            public string? Despcrtiption { get; set; }
            public DateTime EntryDate { get; set; }
    }
}
