using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class WorkLocationDTO
    {
        public int Id { get; set; }
        public DateTime? Createdate { get; set; }
        public bool? Isactive { get; set; }
        public int? Customerid { get; set; }
        public List<WorkLocationListDTO> WorkLocation1List { get; set; }

        public List<AddWorkLocationNameDTO> WorkLocationList { get; set; }

    }
    public class AddWorkLocationNameDTO
    {
        public int Id { get; set; }
        public int WorkLocationId { get; set; }
        public string WorkLocationName { get; set; }
    }

    public class WorkLocationListDTO
    {
        public int Id { get; set; }
        public string WorkLocationName { get; set; }
        public string CustomerName { get; set; }
        public int? Vendorid { get; set; }

    }
}
