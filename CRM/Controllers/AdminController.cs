using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using CRM.Repository;
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
                                 Vendorid = (int?)cr.Id
                              }).FirstOrDefault();

                if(result != null)
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

                else
                {
                    ViewBag.Message = "Invalid User Name or Password!";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error: " + Ex.Message);
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
                ViewBag.UserName = AddedBy;
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
                ViewBag.UserName = addedBy;
            }
            return RedirectToAction("Login", "Admin");

        }

        [HttpGet]
        public async Task<IActionResult> Blogs(int id)
        {
            try
            {
                List<Blog> blogs = _context.Blogs.Where(b => b.IsPublished == true).OrderByDescending(b=>b.Id).ToList();
;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.BlogTitle = "";
                ViewBag.Content = "";
                ViewBag.BlogImage = "";
                ViewBag.heading = "Add Blog";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.Blogs.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.BlogTitle = data.Title;
                        ViewBag.Content = data.Content;
                        ViewBag.BlogImage = data.BlogImage;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Blog";

                    }
                }

                return View(blogs);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Blogs(Blog model)
        {
            try
            {
                bool check = await _ICrmrpo.AddAndUpdateBlog(model);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("Blogs");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("Blogs");
                    }
                }
                else
                {
                    return RedirectToAction("Blogs");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteBlogs(int id)
        {
            try
            {
                var dlt = _context.Blogs.Find(id);
                if (dlt != null)
                {
                    _context.Blogs.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("Blogs");

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
