using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Security.Principal;

namespace CRM.Controllers
{
    public class AdminController : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        public AdminController(ICrmrpo _ICrmrpo, admin_NDCrMContext _context)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
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
                DataTable dtresponse = _ICrmrpo.Login(model);
                if (dtresponse != null && dtresponse.Rows.Count > 0)
                {
                    HttpContext.Session.SetString("UserName", dtresponse.Rows[0]["UserName"].ToString());
                    return RedirectToAction("Dashboard", "Home");

                }

                else
                {
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }  
        }
        public IActionResult Product()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Product(ProductMaster model)
        {
            try
            {
                var response = await _ICrmrpo.Product(model);
                if (response != null)
                {
                    return RedirectToAction("ProductList", "Admin");
                }
                else
                {
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        public async Task<IActionResult> ProductList()
        {
            var response = await _ICrmrpo.ProductList();
            return View(response);
        }
    }
}
