using CRM.Models.Crm;

namespace CRM.Models.DTO
{
    public class WorkLocationDTO
    {
        public int Id { get; set; }
        public DateTime? Createdate { get; set; }
        public bool? Isactive { get; set; }
        public int? Customerid { get; set; }
        public List<WorkLocation1> WorkLocation1List { get; set; }

        public List<AddWorkLocationNameDTO> WorkLocationList { get; set; }

    }
    public class AddWorkLocationNameDTO
    {
        public int Id { get; set; }
        public int WorkLocationId { get; set; }
        public string WorkLocationName { get; set; }
    }
}
