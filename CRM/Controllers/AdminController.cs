using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Net;
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
        [HttpGet]
        public async Task<IActionResult> Product(int id=0)
        {

                List<CRM.Models.Crm.ProductMaster> product = new List<CRM.Models.Crm.ProductMaster>();
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    string AddedBy = HttpContext.Session.GetString("UserName");
                    ViewBag.UserName = AddedBy;
                    ViewBag.Gst = _context.GstMasters
                  .Select(w => new SelectListItem
                  {
                      Value = w.Id.ToString(),
                      Text = w.GstPercentagen
                  });
                    product = await _ICrmrpo.GetproductById(id);
                    if (product[0].Gst != null)
                    {
                        ViewBag.Gst1 = product[0].Gst;
                    }
                    else
                    {
                        return RedirectToAction("Login", "Admin");
                    }
                }
                else
                {
                return RedirectToAction("Login", "Admin");
                }
               return View(product);

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
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = await _ICrmrpo.ProductList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var data = _context.ProductMasters.Find(id);
                _context.ProductMasters.Remove(data);
                _context.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server error");
            }
        }

    }
}
