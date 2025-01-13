using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using CRM.Repository;
using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Wordprocessing;
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
        }
        [HttpGet, Route("Administrator/AccessAssign")]

        public async Task<ActionResult> AccessAssign(int id =0)
        {
            string Username = HttpContext.Session.GetString("UserName");
            var model = new UserRoleDTO();
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
                                item3.ChildMenus = SubHeadingTwoChildList;  
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
                    model.UserRoleList = _context.UserRoles.ToList();
                    model.SoftwareLinkDTO = HeadingList;
                    if (id > 0)
                    {
                        var data = _context.UserRoles.Where(x => x.Id == id).FirstOrDefault();
                        model.Id = data.Id;
                        ViewBag.IsAll = data.IsAll;
                        model.RoleName = data.RoleName;
                        model.IsAllRead = (bool)data.IsAll;
                        model.IsHeadChecked = !string.IsNullOrEmpty(data.IsHeadChecked) ? StringToIntArray(data.IsHeadChecked) : null;
                        model.IsChildHeadChecked = !string.IsNullOrEmpty(data.IsChildHeadChecked) ? StringToIntArray(data.IsChildHeadChecked) : null;
                        model.IsSubHeadChecked = !string.IsNullOrEmpty(data.IsSubHeadChecked) ? StringToIntArray(data.IsSubHeadChecked) : null;
                        model.IsChildSubHeadChecked = !string.IsNullOrEmpty(data.IsChildSubHeadChecked) ? StringToIntArray(data.IsChildSubHeadChecked) : null; 
                        model.IsSubHeadTwoChecked = !string.IsNullOrEmpty(data.IsSubHeadTwoChecked) ? StringToIntArray(data.IsSubHeadTwoChecked) : null; 
                        model.IsChildSubHeadTwoChecked = !string.IsNullOrEmpty(data.IsChildSubHeadTwoChecked) ? StringToIntArray(data.IsChildSubHeadTwoChecked) : null; 
                        ViewBag.Heading = "Update Assign Access";
                        ViewBag.BtnTXT = "Update";
                        return View(model);
                    }
                    else
                    {
                        model.Id = 0;
                        ViewBag.IsAll  = null;
                        model.RoleName = null;
                        model.IsAllRead = false;
                        model.IsHeadChecked = new int[] { 0 };
                        model.IsChildHeadChecked = new int[] { 0 };
                        model.IsSubHeadChecked = new int[] { 0 };
                        model.IsChildSubHeadChecked = new int[] { 0 };
                        model.IsSubHeadTwoChecked = new int[] { 0 };
                        model.IsChildSubHeadTwoChecked = new int[] { 0 };
                        ViewBag.BtnTXT = "Save";
                        ViewBag.Heading = "Create Assign Access";
                        return View(model);
                    }
                  
                    //ViewBag.BtnTXT = "Save";
                    //return View(model);
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
                    TempData["Message"] = "already exist";
                    return View();
                }
                var EmpReq = new UserRole()
                {
                   // CompanyId = model.CompanyId,
                    RoleName = model.RoleName,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    IsHeadChecked = string.Join(",", model.IsHeadChecked),
                    IsChildHeadChecked = string.Join(",", model.IsChildHeadChecked),
                    IsSubHeadChecked = string.Join(",", model.IsSubHeadChecked),
                    IsChildSubHeadChecked = string.Join(",", model.IsChildSubHeadChecked),
                    IsSubHeadTwoChecked = string.Join(",", model.IsSubHeadTwoChecked),
                    IsChildSubHeadTwoChecked = string.Join(",", model.IsChildSubHeadTwoChecked),
                    IsAll = model.IsAll == true ? (bool?)true : (bool?)false,

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
                //data.CompanyId = model.CompanyId;
                data.RoleName = model.RoleName;
                data.IsHeadChecked = string.Join(",", model.IsHeadChecked);
                data.IsChildHeadChecked = string.Join(",", model.IsChildHeadChecked);
                data.IsSubHeadChecked = string.Join(",", model.IsSubHeadChecked);
                data.IsChildSubHeadChecked = string.Join(",", model.IsChildSubHeadChecked);
                data.IsSubHeadTwoChecked = string.Join(",", model.IsSubHeadTwoChecked);
                data.IsChildSubHeadTwoChecked = string.Join(",", model.IsChildSubHeadTwoChecked);
                data.IsAll = model.IsAll == true ? (bool?)true : (bool?)false;
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
                    return RedirectToAction("AccessAssign");

                }
            }

            return RedirectToAction("AccessAssign");
        }
        public async Task<IActionResult> DeleteAccessAssign(int id)
        {
            try
            {
                var data = _context.UserRoles.Find(id);
                if (data != null)
                {
                    _context.UserRoles.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("AccessAssign");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

    }
}
