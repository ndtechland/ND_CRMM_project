using CRM.Models;
using CRM.Models.Crm;
using CRM.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CRM.Controllers
{
    public class HomeController : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;

        public HomeController(ICrmrpo _ICrmrpo, admin_NDCrMContext _context)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
        }
        public IActionResult Dashboard()
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
        public IActionResult Customer()
        {
            
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var emp = new CustomerRegistration();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                ViewBag.ProductDetails = _context.ProductMasters
              .Select(p => new SelectListItem
              {
                  Value = p.Id.ToString(),
                  Text = p.ProductName
              })
               .ToList();

                return View(emp);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Customer(CustomerRegistration model)
        {
            try
            {
                
                var response = await _ICrmrpo.Customer(model);
                if (response != null)
                {

                    return RedirectToAction("CustomerList", "Home");
                    ViewBag.Message = "Customer Registration Successfully.!";

                }
                else
                {
                    ModelState.Clear();
                    return RedirectToAction("Customer", "Home");
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> CustomerList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = await _ICrmrpo.CustomerList();      
                    string AddedBy = HttpContext.Session.GetString("UserName");
                    ViewBag.UserName = AddedBy;
                    return View(response);         

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
          
        }
        [HttpGet]
        public IActionResult Banner()
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
        public async Task<IActionResult> Upload(IFormFile file, BannerMaster banerm)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select a file.";
                return View("Banner");
            }

            // Access the file properties
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadsImage", fileName);

            // Save the file to a folder
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            //save in db
            string AddedBy = HttpContext.Session.GetString("UserName");
            banerm.AddedBy = AddedBy;
            banerm.Imagepath = filePath;
            banerm.BannerImage = fileName;
            var response = await _ICrmrpo.Banner(banerm);
            //end
            ViewBag.Message = "File uploaded successfully.";
            return View("Banner");
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


        //======Invoice Section========//
        [HttpGet]
        public IActionResult Invoice()
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

       
        public IActionResult CustomerDetails()
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

        //-----Quation
        [HttpGet]
        public IActionResult Quation()
        {
            var emp = new Quation();
            if (HttpContext.Session.GetString("UserName") != null)
            {
                
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                ViewBag.ProductDetails = _context.ProductMasters
             .Select(p => new SelectListItem
             {
                 Value = p.Id.ToString(),
                 Text = p.ProductName
             })
              .ToList();
                return View(emp);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Quation(Quation model) 
        {
            try
            {
               
                var response = await _ICrmrpo.Quation(model);
                if (response != null)
                {

                    return RedirectToAction("Quation", "Home");
                    TempData["msg"] = "Regiter Successfully.";
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
        // get quation List 
        [HttpGet]
        public async Task<IActionResult> QuationList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = await _ICrmrpo.QuationList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public async Task<IActionResult> DeleteQuation(int id)
        {
            try
            {
                var data = _context.Quations.Find(id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    _context.SaveChanges();

                }
                else
                {
                    return RedirectToAction("QuationList");

                }
                return RedirectToAction("QuationList");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }

}
