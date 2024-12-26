using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using CRM.Repository;
using Dapper;
using DocumentFormat.OpenXml.InkML;
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
    public class AdminController : Controller
    {
        private readonly IApiAccount _apiAccount;
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        private readonly Repository.IEmailService _emailService;
        public AdminController(ICrmrpo _ICrmrpo, admin_NDCrMContext _context, Repository.IEmailService _emailService, IApiAccount apiAccount)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
            this._emailService = _emailService;
            this._apiAccount = apiAccount;
        }




        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminLogin model)
        {
            try
            {
                var result = (from al in _context.AdminLogins
                              join cr in _context.VendorRegistrations
                              on al.Vendorid equals cr.Id into crGroup
                              from cr in crGroup.DefaultIfEmpty()
                              where al.UserName == model.UserName && al.Password == model.Password
                              select new
                              {
                                  al.Id,
                                  al.UserName,
                                  al.Role,
                                  VendorId = (int?)cr.Id,
                                  cr.Isactive
                              }).FirstOrDefault();
                if (result == null)
                {
                    ViewBag.Message = "Invalid User Name or Password!";
                    ModelState.Clear();
                    return View();
                }

                if (result.Role == "admin")
                {
                    if (result.Id != null)
                    {
                        HttpContext.Session.SetString("UserName", result.UserName);
                        HttpContext.Session.SetString("UserId", result.Id.ToString());
                        ViewBag.UserName = result.UserName;
                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        ViewBag.Message = "Invalid User Name or Password!";
                        ModelState.Clear();
                        return View();
                    }
                }
                else if (result.Role == "Vendor")
                {
                    if (result.Isactive == true)
                    {
                        HttpContext.Session.SetString("UserName", result.UserName);
                        HttpContext.Session.SetString("UserId", result.Id.ToString());
                        ViewBag.UserName = result.UserName;
                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        ViewBag.Message = "Your id not activate!";
                        ModelState.Clear();
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid User Name or Password!";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        [AllowAnonymous, HttpGet]
        public IActionResult forgotPassoword()
        {
            ViewBag.Message = "";
            return View();
        }

        [AllowAnonymous, HttpPost("forgotPassoword")]
        public IActionResult forgotPassoword(AdminLogin model)
        {
            try
            {
                DataTable dtresponse = _ICrmrpo.ForgetPassword(model);
                if (dtresponse != null && dtresponse.Rows.Count > 0)
                {
                    string Username = dtresponse.Rows[0]["UserName"].ToString();
                    string Role = dtresponse.Rows[0]["Role"].ToString();
                    string Password = dtresponse.Rows[0]["Password"].ToString();
                    string body = "Hello ! " + Username + " (" + Role + ") Your Password is: " + Password + "";
                    ViewBag.Message = body;
                    //_emailService.SendEmailAsync(model.UserName, "Forget Password", body);
                    return View();

                }
                else
                {
                    ViewBag.Message = "Invalid User Name or Password!";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [Route("Admin/Changepassword")]
        [HttpGet]
        public async Task<IActionResult> Changepassword()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                string id = Convert.ToString(HttpContext.Session.GetString("UserId"));
                int adid = Convert.ToInt32(HttpContext.Session.GetString("AdminId"));
    
                ViewBag.id = id;
                ViewBag.adid = adid;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Changepassword(ChangePassworddto model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                string userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(AddedBy) || string.IsNullOrEmpty(userId))
                {
                    TempData["Message"] = "Session expired. Please login again.";
                    return RedirectToAction("Login", "Admin");
                }

                var user = await _context.AdminLogins.Where(x => x.UserName == AddedBy).FirstOrDefaultAsync();
                if (user == null)
                {
                    TempData["Message"] = "User not found.";
                    return RedirectToAction("Changepassword");
                }

                if (user.Password != model.CurrentPassword)
                {
                    TempData["Message"] = "Current password does not match.";
                    return RedirectToAction("Changepassword");
                }

                if (model.NewPassword == model.CurrentPassword)
                {
                    TempData["Message"] = "New password cannot be the same as the current password.";
                    return RedirectToAction("Changepassword");
                }

                int id = Convert.ToInt32(userId);
                var data = await _ICrmrpo.UpdateChangepassword(model, AddedBy, id);
                TempData["Message"] = "Password updated successfully.";

                return RedirectToAction("Changepassword");
            }
            catch (Exception Ex)
            {
                TempData["Message"] = "An error occurred while changing the password. Please try again later.";
                return RedirectToAction("Changepassword");
            }
        }

        public IActionResult Logout()
        {

            string addedBy = HttpContext.Session.GetString("UserName");
            HttpContext.Session.Remove("UserName");

            if (!string.IsNullOrEmpty(addedBy))
            {
    
            }
            return RedirectToAction("Login", "Admin");

        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<JsonResult> getsessionkey()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }
        public async Task<ActionResult> ShowSidebarMenus()
        {
            string Username = HttpContext.Session.GetString("UserName");
            //var SubHeadingTwoQuery = string.Empty;
            //IEnumerable<SoftwareLinkDTO> SubHeadingTwoList;
            //var SubHeadingQuery = string.Empty;
            //List<SoftwareLinkDTO> SubHeadingList = new List<SoftwareLinkDTO>(); 
            //var HeadingQuery = string.Empty;
            //IEnumerable<SoftwareLinkDTO> HeadingList;
            if (Username?.ToLower() == "admin")
            {
                 var HeadingQuery = @"select * from SoftwareLink where  IsHeading=1 and Isvendor = 0";
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await con.OpenAsync();
                    var HeadingList = await con.QueryAsync<SoftwareLinkDTO>(HeadingQuery, commandType: CommandType.Text);
                    foreach (var item in HeadingList)
                    {
                        var SubHeadingQuery = @"select * from SoftwareLink where  IsSubHeading=1 and Isvendor = 0 and ParentID = "+item.Id+"";
                        item.SubHeading = (IEnumerable<Softwarelink>)await con.QueryAsync<SoftwareLinkDTO>(SubHeadingQuery, commandType: CommandType.Text);
                       
                        foreach (var item1 in item.SubHeading)
                        {
                            var SubHeadingTwoQuery = @"select * from SoftwareLink where  IsSubHeadingTwo=1 and Isvendor = 0 and ParentID = " + item1.Id + "";
                            item1.IsSubHeadingTwo = await con.QueryAsync<SoftwareLinkDTO>(SubHeadingQuery, commandType: CommandType.Text);
                        }
                    }
                    
                }
                //var softwareLinks = _context<SoftwareLinkDTO>(query).ToList();
                //var softwareLinks1=_context.Softwarelinks.Where(x=>x.Isheading=true);
                //foreach (var item in softwareLinks)
                //{
                //    var q = @"select * from SoftwareLink where  Par_context_Id=" + item.Id + "";
                //    var l = _context.Database.SqlQuery<Softwarelink>(q).ToList();
                //    item.ChildMenus = l;
                //}
                //return PartialView(softwareLinks);
            }

            return View();
        }
    }
}
