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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                    ViewBag.Message = "Invalid User Name or Password!";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }  
        }
        public async Task<IActionResult> Product()
        {
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
                ViewBag.Category = _context.Categories
              .Select(w => new SelectListItem
              {
                  Value = w.Id.ToString(),
                  Text = w.CategoryName
              });
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
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
                var  response = await _ICrmrpo.ProductList();
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
                return RedirectToAction("ProductList", "Admin");
            }
            catch (Exception ex)
            {
                throw new Exception( ex.Message);
            }
        }
       
        public JsonResult EditProduct(int id)
        {
            var product = new ProductMaster();
            var data = _ICrmrpo.GetproductById(id);
            var gstdata = _context.GstMasters.ToList();
            product.Id = data.Id;
            product.ProductName=data.ProductName;
            product.Category=data.Category;
            product.HsnSacCode=data.HsnSacCode;
            product.Price=data.Price;
            product.Gst = data.Gst;
            var result = new
            {
                Product = product,
                GstData = gstdata,

            };           
            return new JsonResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductMaster model)
        {
            try
            {
                var product = await _ICrmrpo.updateproduct(model);
                if (product != null)
                {
                    return RedirectToAction("ProductList", "Admin");
                    TempData["msg"] = "product update Successfully.";
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
      
    }
   }
