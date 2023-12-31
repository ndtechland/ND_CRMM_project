﻿using CRM.Models;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
                var emp = new Customer();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                ViewBag.ProductDetails = _context.ProductMasters.Select(p => new SelectListItem
              {
                  Value = p.Id.ToString(),
                  Text = p.ProductName,                 
              }).ToList();
                ViewBag.WorkLocations = _context.WorkLocations.Select(p => new SelectListItem
              {
                  Value = p.Id.ToString(),
                  Text = p.AddressLine1
              }).ToList();      
                return View(emp);               
            }

            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Customer(Customer model)
        {
            try
            {              
                var response = await _ICrmrpo.Customer(model);
                if (response != null)
                {
                    return RedirectToAction("CustomerList", "Home");
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
        public IActionResult Invoicelist()
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

                    return RedirectToAction("QuationList", "Home");
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
                return RedirectToAction("QuationList");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteQuationList:" + ex.Message);
            }
        }


        [HttpGet]
        public JsonResult EditQuation(int id)
        {
            var emp = _ICrmrpo.GetempQuationById(id);
            var Productdata = _context.ProductMasters.ToList();
            var result = new
            {
                Emp = emp,
                Productdata = Productdata,

            };

            return new JsonResult(result);
        }


        [HttpPost]
        public async Task<IActionResult> EditQuation(Quation model)
        {
            try
            {
                var response = await _ICrmrpo.Iupdate(model);
                if (response != null)
                {

                    return RedirectToAction("QuationList", "Home");
                    TempData["msg"] = "Quation Successfully.";
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
        public IActionResult Department()
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
        public async Task<IActionResult> Department(DepartmentMaster model)
        {
            if (model != null)
            {
                DepartmentMaster master = new DepartmentMaster
                {
                    DepartmentName = model.DepartmentName,
                };
                _context.DepartmentMasters.Add(master);
                _context.SaveChanges();
                return RedirectToAction("Departmentlist");
            }
            else
            {
                ModelState.Clear();
                return View("Departmentlist");
            }
            return View("Departmentlist");
        }

        [HttpGet]
        public IActionResult Departmentlist()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = _context.DepartmentMasters.ToList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var data = _context.DepartmentMasters.Find(id);
                if (data != null)
                {
                    _context.DepartmentMasters.Remove(data); 
                    _context.SaveChanges();
                }
                return RedirectToAction("Departmentlist");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteQuationList:" + ex.Message);
            }
        }
        [HttpGet]
        public IActionResult WorkLocation()
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
        public async Task<IActionResult> WorkLocation(WorkLocation model)
        {
            if (model != null)
            {
                WorkLocation master = new WorkLocation
                {
                    AddressLine1 = model.AddressLine1,
                    Commissoninpercentage=model.Commissoninpercentage,
                };
                _context.WorkLocations.Add(master);
                _context.SaveChanges();
                return RedirectToAction("WorkLocationlist");
            }
            else
            {
                ModelState.Clear();
                return View("WorkLocationlist");
            }
            return View("WorkLocationlist");
        }

        [HttpGet]
        public IActionResult WorkLocationlist()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = _context.WorkLocations.ToList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public async Task<IActionResult> DeleteWorkLocation(int id)
        {
            try
            {
                var data = _context.WorkLocations.Find(id);
                if (data != null)
                {
                    _context.WorkLocations.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("WorkLocationlist");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        [HttpGet]
        public IActionResult Designation()
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
        public async Task<IActionResult> Designation(DesignationMaster model)
        {
            if (model != null)
            {
                DesignationMaster master = new DesignationMaster
                {
                    DesignationName = model.DesignationName,
                };
                _context.DesignationMasters.Add(master);
                _context.SaveChanges();
                return RedirectToAction("Designationlist");
            }
            else
            {
                ModelState.Clear();
                return View("Designationlist");
            }
            return View("Designationlist");
        }

        [HttpGet]
        public IActionResult Designationlist()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = _context.DesignationMasters.ToList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public async Task<IActionResult> DeleteDesignation(int id)
        {
            try
            {
                var data = _context.DesignationMasters.Find(id);
                if (data != null)
                {
                    _context.DesignationMasters.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("Designationlist");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        [HttpGet]
        public JsonResult product(int? id)
        {
            var data = (from pm in _context.ProductMasters
                          join gm in _context.GstMasters on pm.Gst equals gm.Id.ToString()
                          where pm.Id == id
                          select new Customer
                          {
                              Scgst = gm.Scgst,
                              Cgst = gm.Cgst,
                              Igst = gm.Igst,
                              Price=pm.Price,
                              HsnSacCode=pm.HsnSacCode,
                          }).FirstOrDefault();
            var result = new
            {
                Data = data,
            };
            return new JsonResult(result);
        }
    }

}
