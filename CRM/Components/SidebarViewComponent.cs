using CRM.Models.Crm;
using CRM.Models.DTO;
using Dapper;
using DocumentFormat.OpenXml.InkML;
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
            try
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
                else
                {
                    int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                   // var adminlogin = await _context.AdminLogins.Where(x => x.Id == UserId).FirstOrDefaultAsync();
                    List<SoftwareLinkDTO> HeadingList = new List<SoftwareLinkDTO>();

                    using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                    {
                        await con.OpenAsync();
                        var adminloginQuery = "SELECT * FROM AdminLogin where id =" + UserId + "";
                        var adminloginList = await con.QueryFirstOrDefaultAsync<AdminLogin>(adminloginQuery, commandType: CommandType.Text);


                        var PlanHeadingQuery = @"SELECT ur.* FROM Vendor_Registration vr
                                  LEFT JOIN UserRole ur ON vr.PricingPlanid = ur.PlanId
                                  where vr.id =" + adminloginList.Vendorid + "";
                        var PlanHeadingList = await con.QueryAsync<UserRole>(PlanHeadingQuery, commandType: CommandType.Text);
                        foreach (var item in PlanHeadingList)
                        {
                            var HeadingQuery = @"select * from SoftwareLink where  IsHeading=1 and Isvendor = 1 and id in(" + item.IsHeadChecked + ")";
                            var HeadingListQ = await con.QueryAsync<SoftwareLinkDTO>(HeadingQuery, commandType: CommandType.Text);
                            HeadingList = (List<SoftwareLinkDTO>)HeadingListQ;
                            foreach (var item1 in HeadingList)
                            {
                                var SubHeadingQuery = @"select * from SoftwareLink where  IsSubHeading=1 and Isvendor = 1 and ParentID = " + item1.Id + " and id in(" + item.IsSubHeadChecked + ")";
                                var SubHeadingList = await con.QueryAsync<SubSoftwarelink>(SubHeadingQuery, commandType: CommandType.Text);
                                item1.SubHeading = SubHeadingList;
                                foreach (var item2 in item1.SubHeading)
                                {
                                    var SubHeadingTwoQuery = @"select * from SoftwareLink where  IsSubHeadingTwo=1 and Isvendor = 1 and ParentID = " + item2.Id + " and id in(" + item.IsSubHeadTwoChecked + ")";
                                    var SubHeadingTwoList = await con.QueryAsync<SubSoftwarelinkTwo>(SubHeadingTwoQuery, commandType: CommandType.Text);
                                    item2.SubHeadingTwo = SubHeadingTwoList;
                                    /*and id in(" + item2.Id + ")*/
                                    foreach (var item3 in item2.SubHeadingTwo)
                                    {
                                        var SubHeadingTwoChildQuery = @"select * from SoftwareLink where Isvendor = 1 and ParentID = " + item3.Id + " and id in(" + item.IsChildSubHeadTwoChecked + ")";
                                        var SubHeadingTwoChildList = await con.QueryAsync<Softwarelink>(SubHeadingTwoChildQuery, commandType: CommandType.Text);
                                        item3.ChildMenus = SubHeadingTwoChildList;

                                    }
                                }
                                foreach (var item4 in item1.SubHeading)
                                {
                                    var SubHeadingChildQuery = @"select * from SoftwareLink where  Isvendor = 1 and ParentID = " + item4.Id + " and id in(" + item.IsChildSubHeadChecked + ")";
                                    var SubHeadingChildList = await con.QueryAsync<Softwarelink>(SubHeadingChildQuery, commandType: CommandType.Text);
                                    item4.ChildMenus = SubHeadingChildList;
                                }
                                var HeadingChildQuery = @"select * from SoftwareLink where Isvendor = 1 and ParentID = " + item1.Id + " and id in(" + item.IsChildHeadChecked + ") ";
                                var HeadingChildList = await con.QueryAsync<Softwarelink>(HeadingChildQuery, commandType: CommandType.Text);

                                var check = @"select * from Vendor_Registration where SelectCompany = 1 and id = " + adminloginList.Vendorid + "";
                                var checkList = await con.QueryAsync<VendorRegistration>(check, commandType: CommandType.Text);

                                var selectCompany = checkList.FirstOrDefault()?.SelectCompany ?? false;
                                var filteredHeadingChildList = selectCompany ? HeadingChildList : HeadingChildList.Where(x => x.Id != 106).ToList();

                                if (selectCompany)
                                {
                                    item1.ChildMenus = filteredHeadingChildList;
                                }
                                else
                                {
                                    item1.ChildMenus = filteredHeadingChildList;
                                }


                            }

                        }
                        return View("_SidebarMenus", HeadingList);
                    }
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            //return View("_SidebarMenus");
        }
    }
}
