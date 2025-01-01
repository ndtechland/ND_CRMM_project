using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.Models.DTO
{
    public class UserRoleDTO
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string RoleName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        //public SelectList Companies { get; set; }
        public IEnumerable<SelectListItem> Companies { get; set; }
        public bool? IsAll { get; set; }
        public int[] IsHeadChecked { get; set; }
        public int[] IsChildHeadChecked { get; set; } = new int[0];
        public int[] IsSubHeadChecked { get; set; } = new int[0];
        public int[] IsChildSubHeadChecked { get; set; } = new int[0];
        public int[] IsSubHeadTwoChecked { get; set; } = new int[0];
        public int[] IsChildSubHeadTwoChecked { get; set; } = new int[0];
         
        public string ReadPermissions { get; set; }
        public string WritePermissions { get; set; }
        public bool IsAllRead { get; set; }
        public bool IsAllWrite { get; set; }
        public IEnumerable<SoftwareLinkDTO> SoftwareLinkDTO { get; set; }
        public IEnumerable<UserRoleList> UserRoleLists { get; set; }
    }
    public class UserRoleList
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string CompanyName { get; set; }
        public string OrgName { get; set; }

    }
}
