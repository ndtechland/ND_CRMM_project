using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using CRM.Repository;
using Dapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Common;
using NETCore.MailKit.Core;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CRM.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly admin_NDCrMContext _context;
        public AdministratorController(admin_NDCrMContext context)
        {
            _context = context;
        }
        private static int[] StringToIntArray(string myNumbers)
        {
            List<int> myIntegers = new List<int>();
            Array.ForEach(myNumbers.Split(",".ToCharArray()), s =>
            {
                int curr_contextInt;
                if (Int32.TryParse(s, out curr_contextInt))
                    myIntegers.Add(curr_contextInt);
            });
            return myIntegers.ToArray();
        }
        public async Task<ActionResult> CreateRole(int menuId = 0, int id = 0)
        {
            string Username = HttpContext.Session.GetString("UserName");
            var model = new UserRoleDTO();
            //model.Companies = new SelectList(_context.Customers.Where(c => c.IsActive == true).OrderByDescending(c => c.Id).ToList(), "Id", "OrgName");
            //model.Companies = _context.Customers.OrderByDescending(c => c.Id).Select(d => new SelectListItem
            //{
            //    Value = d.Id.ToString(),
            //    Text = d.OrgName
            //}).ToList();

           // model.UserRoleLists = await _administrator.GetRoles();

            //int userId = int.Parse(User.Id_contextity.Name);
            if (Username?.ToLower() == "admin")
            {
                var HeadingQuery = @"select * from SoftwareLink where  IsHeading=1 and Isvendor = 1";
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await con.OpenAsync();
                    var HeadingList = await con.QueryAsync<SoftwareLinkDTO>(HeadingQuery, commandType: CommandType.Text);
                    foreach (var item in HeadingList)
                    {
                        var SubHeadingQuery = @"select * from SoftwareLink where IsSubHeading=1 and Isvendor = 1 and ParentID = " + item.Id + "";
                        var SubHeadingList = await con.QueryAsync<SubSoftwarelink>(SubHeadingQuery, commandType: CommandType.Text);
                        item.SubHeading = SubHeadingList;

                        foreach (var item2 in item.SubHeading)
                        {
                            var SubHeadingTwoQuery = @"select * from SoftwareLink where IsSubHeadingTwo=1 and Isvendor = 1 and ParentID = " + item2.Id + "";
                            var SubHeadingTwoList = await con.QueryAsync<SubSoftwarelinkTwo>(SubHeadingTwoQuery, commandType: CommandType.Text);
                            item2.SubHeadingTwo = SubHeadingTwoList;

                            foreach (var item3 in item2.SubHeadingTwo)
                            {
                                var SubHeadingTwoChildQuery = @"select * from SoftwareLink where Isvendor = 1 and ParentID = " + item2.Id + "";
                                var SubHeadingTwoChildList = await con.QueryAsync<Softwarelink>(SubHeadingTwoChildQuery, commandType: CommandType.Text);
                                item2.ChildMenus = SubHeadingTwoChildList;
                            }

                        }
                        foreach (var item4 in item.SubHeading)
                        {
                            var SubHeadingChildQuery = @"select * from SoftwareLink where Isvendor = 1 and ParentID = " + item4.Id + "";
                            var SubHeadingChildList = await con.QueryAsync<Softwarelink>(SubHeadingChildQuery, commandType: CommandType.Text);
                            item4.ChildMenus = SubHeadingChildList;
                        }
                        var HeadingChildQuery = @"select * from SoftwareLink where Isvendor = 1 and ParentID = " + item.Id + "";
                        var HeadingChildList = await con.QueryAsync<Softwarelink>(HeadingChildQuery, commandType: CommandType.Text);
                        item.ChildMenus = HeadingChildList;
                    }
                    model.SoftwareLinkDTO = HeadingList;
                    ViewBag.BtnTXT = "Create Role";
                    return View(model);

                }


               
            }
            
            return View();
            //if (id > 0)
            //{
            //    var data = _context.UserRoles.Where(x => x.Id == id).FirstOrDefault();
            //    model.Id = data.Id;
            //    model.CompanyId = data.CompanyId;
            //    model.RoleName = data.RoleName;
            //    model.IsAllRead = (bool)data.IsAllRead;
            //    model.IsAllWrite = (bool)data.IsAllWrite;
            //    ViewBag.IsAllRead = (bool)data.IsAllWrite;
            //    ViewBag.IsAllWrite = (bool)data.IsAllWrite;
            //    model.IsReadChecked = !string.IsNullOrEmpty(data.IsReadChecked) ? StringToIntArray(data.IsReadChecked) : new int[0]; //.Split(',').Select(item => int.TryParse(item, out int number) ? number : 0).ToArray();
            //    model.IsWriteChecked = !string.IsNullOrEmpty(data.IsWriteChecked) ? StringToIntArray(data.IsWriteChecked) : new int[0]; //.Split(',').Select(item => int.TryParse(item, out int number) ? number : 0).ToArray();
            //    model.IsSubReadChecked = !string.IsNullOrEmpty(data.IsSubReadChecked) ? StringToIntArray(data.IsSubReadChecked) : new int[0]; //.Split(',').Select(item => int.TryParse(item, out int number) ? number : 0).ToArray();
            //    model.IsSubWriteChecked = !string.IsNullOrEmpty(data.IsSubWriteChecked) ? StringToIntArray(data.IsSubWriteChecked) : new int[0]; //.Split(',').Select(item => int.TryParse(item, out int number) ? number : 0).ToArray();
            //    ViewBag.Heading = "Update Role";
            //    ViewBag.BtnTXT = "Update";
            //    return View(model);
            //}
            //else
            //{
            //    model.Id = 0;
            //    model.CompanyId = 0;
            //    model.RoleName = "";
            //    model.IsReadChecked = null;
            //    model.IsWriteChecked = null;
            //    model.IsSubReadChecked = null;
            //    model.IsSubWriteChecked = null;
            //    model.IsAllWrite = false;
            //    model.IsAllRead = false;
            //    ViewBag.IsAllRead = false;
            //    ViewBag.IsAllWrite = false;
            //    ViewBag.BtnTXT = "Save";
            //    ViewBag.Heading = "Create Role";
            //    return View(model);
            //}
        }
        public async Task<ActionResult> AccessAssign()
        {
            string Username = HttpContext.Session.GetString("UserName");
            var model = new UserRoleDTO();
            //model.Companies = new SelectList(_context.Customers.Where(c => c.IsActive == true).OrderByDescending(c => c.Id).ToList(), "Id", "OrgName");
            //model.Companies = _context.Customers.OrderByDescending(c => c.Id).Select(d => new SelectListItem
            //{
            //    Value = d.Id.ToString(),
            //    Text = d.OrgName
            //}).ToList();

            // model.UserRoleLists = await _administrator.GetRoles();

            //int userId = int.Parse(User.Id_contextity.Name);
            if (Username?.ToLower() == "admin")
            {
                var HeadingQuery = @"select * from SoftwareLink where IsHeading=1 and Isvendor=1";
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await con.OpenAsync();
                    var HeadingList = await con.QueryAsync<SoftwareLinkDTO>(HeadingQuery, commandType: CommandType.Text);

                    foreach (var item in HeadingList)
                    {
                        var SubHeadingQuery = @"select * from SoftwareLink where IsSubHeading=1 and Isvendor=1 and ParentID=" + item.Id;
                        var SubHeadingList = await con.QueryAsync<SubSoftwarelink>(SubHeadingQuery, commandType: CommandType.Text);
                        item.SubHeading = SubHeadingList;

                        foreach (var item2 in item.SubHeading)
                        {
                            var SubHeadingTwoQuery = @"select * from SoftwareLink where IsSubHeadingTwo=1 and Isvendor=1 and ParentID=" + item2.Id;
                            var SubHeadingTwoList = await con.QueryAsync<SubSoftwarelinkTwo>(SubHeadingTwoQuery, commandType: CommandType.Text);
                            item2.SubHeadingTwo = SubHeadingTwoList;

                            foreach (var item3 in item2.SubHeadingTwo)
                            {
                                var SubHeadingTwoChildQuery = @"select * from SoftwareLink where Isvendor=1 and ParentID=" + item3.Id;
                                var SubHeadingTwoChildList = await con.QueryAsync<Softwarelink>(SubHeadingTwoChildQuery, commandType: CommandType.Text);
                                item3.ChildMenus = SubHeadingTwoChildList; // Assign to item3 (SubHeadingTwo) 
                            }
                        }

                        foreach (var item4 in item.SubHeading)
                        {
                            var SubHeadingChildQuery = @"select * from SoftwareLink where Isvendor=1 and ParentID=" + item4.Id;
                            var SubHeadingChildList = await con.QueryAsync<Softwarelink>(SubHeadingChildQuery, commandType: CommandType.Text);
                            item4.ChildMenus = SubHeadingChildList;
                        }

                        var HeadingChildQuery = @"select * from SoftwareLink where Isvendor=1 and ParentID=" + item.Id;
                        var HeadingChildList = await con.QueryAsync<Softwarelink>(HeadingChildQuery, commandType: CommandType.Text);
                        item.ChildMenus = HeadingChildList;
                    }

                    model.SoftwareLinkDTO = HeadingList;
                    ViewBag.BtnTXT = "Save";
                    return View(model);
                }
            }

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AccessAssign(UserRoleDTO model)
        {
            string Username = HttpContext.Session.GetString("UserName");
            //model.Companies = new SelectList(_context.Customers.Where(c => c.IsActive == true).OrderByDescending(c => c.Id).ToList(), "Id", "OrgName");
            //model.Companies = _context.Customers.OrderByDescending(c => c.Id).Select(d => new SelectListItem
            //{
            //    Value = d.Id.ToString(),
            //    Text = d.OrgName
            //}).ToList();

            // model.UserRoleLists = await _administrator.GetRoles();

            //int userId = int.Parse(User.Id_contextity.Name);

            if (model.Id == 0)
            {
                var check = _context.UserRoles.Where(u => u.RoleName.ToLower() == model.RoleName.ToLower()).FirstOrDefault();
                if (check != null)
                {
                    return View();
                }
                var EmpReq = new UserRole()
                {
                    CompanyId = model.CompanyId,
                    RoleName = model.RoleName,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    IsHeadChecked = string.Join(",", model.IsHeadChecked),
                    IsChildHeadChecked = string.Join(",", model.IsChildHeadChecked),
                    IsSubHeadChecked = string.Join(",", model.IsSubHeadChecked),
                    IsChildSubHeadChecked = string.Join(",", model.IsChildSubHeadChecked),
                    IsSubHeadTwoChecked = string.Join(",", model.IsSubHeadTwoChecked),
                    IsChildSubHeadTwoChecked = string.Join(",", model.IsChildSubHeadTwoChecked),
                    IsAll = model.IsAll,
                };
                _context.UserRoles.Add(EmpReq);
                _context.SaveChanges();
            }
            else
            {
                var check = _context.UserRoles.Where(u => u.RoleName.ToLower() == model.RoleName.ToLower() && u.Id != model.Id).FirstOrDefault();
                if (check != null)
                {
                    return View();
                }
                var data = _context.UserRoles.Find(model.Id);
                data.CompanyId = model.CompanyId;
                data.RoleName = model.RoleName;
                data.IsHeadChecked = string.Join(",", model.IsHeadChecked);
                data.IsChildHeadChecked = string.Join(",", model.IsChildHeadChecked);
                data.IsSubHeadChecked = string.Join(",", model.IsSubHeadChecked);
                data.IsChildSubHeadChecked = string.Join(",", model.IsChildSubHeadChecked);
                data.IsSubHeadTwoChecked = string.Join(",", model.IsSubHeadTwoChecked);
                data.IsChildSubHeadTwoChecked = string.Join(",", model.IsChildSubHeadTwoChecked);
                data.IsAll = model.IsAll;
                _context.SaveChanges();
            }

            if (Username?.ToLower() == "admin")
            {
                var HeadingQuery = @"select * from SoftwareLink where IsHeading=1 and Isvendor=1";
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await con.OpenAsync();
                    var HeadingList = await con.QueryAsync<SoftwareLinkDTO>(HeadingQuery, commandType: CommandType.Text);
                    foreach (var item in HeadingList)
                    {
                        var SubHeadingQuery = @"select * from SoftwareLink where IsSubHeading=1 and Isvendor=1 and ParentID=" + item.Id;
                        var SubHeadingList = await con.QueryAsync<SubSoftwarelink>(SubHeadingQuery, commandType: CommandType.Text);
                        item.SubHeading = SubHeadingList;

                        foreach (var item2 in item.SubHeading)
                        {
                            var SubHeadingTwoQuery = @"select * from SoftwareLink where IsSubHeadingTwo=1 and Isvendor=1 and ParentID=" + item2.Id;
                            var SubHeadingTwoList = await con.QueryAsync<SubSoftwarelinkTwo>(SubHeadingTwoQuery, commandType: CommandType.Text);
                            item2.SubHeadingTwo = SubHeadingTwoList;

                            foreach (var item3 in item2.SubHeadingTwo)
                            {
                                var SubHeadingTwoChildQuery = @"select * from SoftwareLink where Isvendor=1 and ParentID=" + item3.Id;
                                var SubHeadingTwoChildList = await con.QueryAsync<Softwarelink>(SubHeadingTwoChildQuery, commandType: CommandType.Text);
                                item3.ChildMenus = SubHeadingTwoChildList; // Assign to item3 (SubHeadingTwo) 
                            }
                        }

                        foreach (var item4 in item.SubHeading)
                        {
                            var SubHeadingChildQuery = @"select * from SoftwareLink where Isvendor=1 and ParentID=" + item4.Id;
                            var SubHeadingChildList = await con.QueryAsync<Softwarelink>(SubHeadingChildQuery, commandType: CommandType.Text);
                            item4.ChildMenus = SubHeadingChildList;
                        }

                        var HeadingChildQuery = @"select * from SoftwareLink where Isvendor=1 and ParentID=" + item.Id;
                        var HeadingChildList = await con.QueryAsync<Softwarelink>(HeadingChildQuery, commandType: CommandType.Text);
                        item.ChildMenus = HeadingChildList;
                    }

                    model.SoftwareLinkDTO = HeadingList;
                    ViewBag.BtnTXT = "Create Role";
                    return View(model);
                }
            }

            return View();
        }
        public async Task<ActionResult> AccessAssignlist()
        {
            string Username = HttpContext.Session.GetString("UserName");
            var model = new UserRoleDTO();
            //model.Companies = new SelectList(_context.Customers.Where(c => c.IsActive == true).OrderByDescending(c => c.Id).ToList(), "Id", "OrgName");
            //model.Companies = _context.Customers.OrderByDescending(c => c.Id).Select(d => new SelectListItem
            //{
            //    Value = d.Id.ToString(),
            //    Text = d.OrgName
            //}).ToList();

            // model.UserRoleLists = await _administrator.GetRoles();

            //int userId = int.Parse(User.Id_contextity.Name);
            if (Username?.ToLower() == "admin")
            {
                var HeadingQuery = @"select * from SoftwareLink where IsHeading=1 and Isvendor=1";
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await con.OpenAsync();
                    var HeadingList = await con.QueryAsync<SoftwareLinkDTO>(HeadingQuery, commandType: CommandType.Text);

                    foreach (var item in HeadingList)
                    {
                        var SubHeadingQuery = @"select * from SoftwareLink where IsSubHeading=1 and Isvendor=1 and ParentID=" + item.Id;
                        var SubHeadingList = await con.QueryAsync<SubSoftwarelink>(SubHeadingQuery, commandType: CommandType.Text);
                        item.SubHeading = SubHeadingList;

                        foreach (var item2 in item.SubHeading)
                        {
                            var SubHeadingTwoQuery = @"select * from SoftwareLink where IsSubHeadingTwo=1 and Isvendor=1 and ParentID=" + item2.Id;
                            var SubHeadingTwoList = await con.QueryAsync<SubSoftwarelinkTwo>(SubHeadingTwoQuery, commandType: CommandType.Text);
                            item2.SubHeadingTwo = SubHeadingTwoList;

                            foreach (var item3 in item2.SubHeadingTwo)
                            {
                                var SubHeadingTwoChildQuery = @"select * from SoftwareLink where Isvendor=1 and ParentID=" + item3.Id;
                                var SubHeadingTwoChildList = await con.QueryAsync<Softwarelink>(SubHeadingTwoChildQuery, commandType: CommandType.Text);
                                item3.ChildMenus = SubHeadingTwoChildList; // Assign to item3 (SubHeadingTwo) 
                            }
                        }

                        foreach (var item4 in item.SubHeading)
                        {
                            var SubHeadingChildQuery = @"select * from SoftwareLink where Isvendor=1 and ParentID=" + item4.Id;
                            var SubHeadingChildList = await con.QueryAsync<Softwarelink>(SubHeadingChildQuery, commandType: CommandType.Text);
                            item4.ChildMenus = SubHeadingChildList;
                        }

                        var HeadingChildQuery = @"select * from SoftwareLink where Isvendor=1 and ParentID=" + item.Id;
                        var HeadingChildList = await con.QueryAsync<Softwarelink>(HeadingChildQuery, commandType: CommandType.Text);
                        item.ChildMenus = HeadingChildList;
                    }

                    model.SoftwareLinkDTO = HeadingList;
                    ViewBag.BtnTXT = "Save";
                    return View(model);
                }
            }

            return View();
        }
    }
}
