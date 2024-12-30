using CRM.Models.Crm;
using CRM.Models.DTO;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace CRM.Components
{

    [ViewComponent]
    public class SidebarViewComponent : ViewComponent
    {
        private readonly admin_NDCrMContext _context;

        public SidebarViewComponent(admin_NDCrMContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string Username = HttpContext.Session.GetString("UserName");
            if (Username?.ToLower() == "admin")
            {
                var HeadingQuery = @"select * from SoftwareLink where  IsHeading=1 and Isvendor = 0";
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await con.OpenAsync();
                    var HeadingList = await con.QueryAsync<SoftwareLinkDTO>(HeadingQuery, commandType: CommandType.Text);
                    foreach (var item in HeadingList)
                    {
                        var SubHeadingQuery = @"select * from SoftwareLink where IsSubHeading=1 and Isvendor = 0 and ParentID = " + item.Id + "";
                        var SubHeadingList = await con.QueryAsync<SubSoftwarelink>(SubHeadingQuery, commandType: CommandType.Text);
                        item.SubHeading = SubHeadingList;

                        foreach (var item2 in item.SubHeading)
                        {
                            var SubHeadingTwoQuery = @"select * from SoftwareLink where IsSubHeadingTwo=1 and Isvendor = 0 and ParentID = " + item2.Id + "";
                            var SubHeadingTwoList = await con.QueryAsync<SubSoftwarelinkTwo>(SubHeadingTwoQuery, commandType: CommandType.Text);
                            item2.SubHeadingTwo = SubHeadingTwoList;

                            foreach (var item3 in item2.SubHeadingTwo)
                            {
                                var SubHeadingTwoChildQuery = @"select * from SoftwareLink where Isvendor = 0 and ParentID = " + item2.Id + "";
                                var SubHeadingTwoChildList = await con.QueryAsync<Softwarelink>(SubHeadingTwoChildQuery, commandType: CommandType.Text);
                                item2.ChildMenus = SubHeadingTwoChildList;
                            }

                        }
                        foreach (var item4 in item.SubHeading)
                        {
                            var SubHeadingChildQuery = @"select * from SoftwareLink where Isvendor = 0 and ParentID = " + item4.Id + "";
                            var SubHeadingChildList = await con.QueryAsync<Softwarelink>(SubHeadingChildQuery, commandType: CommandType.Text);
                            item4.ChildMenus = SubHeadingChildList;
                        }
                        var HeadingChildQuery = @"select * from SoftwareLink where Isvendor = 0 and ParentID = " + item.Id + "";
                        var HeadingChildList = await con.QueryAsync<Softwarelink>(HeadingChildQuery, commandType: CommandType.Text);
                        item.ChildMenus = HeadingChildList;
                    }
                    // HttpContext.Session.Set<IEnumerable<CRM.Models.DTO.SoftwareLinkDTO>>("HeadingList", HeadingList);
                    //HttpContext.Session.SetString("HeadingList", JsonConvert.SerializeObject(HeadingList));

                    return View("_SidebarMenus", HeadingList);

                }
            }
            return View("_SidebarMenus");
        }
    }
}
