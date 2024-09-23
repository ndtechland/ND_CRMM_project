using CRM.Models;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Users;
using System.Diagnostics;
using System.Net;
//using System.Web.Http;
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
        [HttpGet, Route("Home/Quation")]
        public async Task<IActionResult> Quation(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.UserName = AddedBy;

                ViewBag.ProductDetails = await _context.ProductMasters.Where(x => x.IsDeleted == false).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ProductName
                }).ToListAsync();
                ViewBag.id = 0;
                ViewBag.CompanyName = "";
                ViewBag.CustomerName = "";
                ViewBag.Email = "";
                ViewBag.SalesPersonName = "";
                ViewBag.ProductId = "";
                ViewBag.Subject = "";
                ViewBag.Amount = "";
                ViewBag.Mobile = "";
                ViewBag.Heading = "Add Quation";
                ViewBag.btnText = "SAVE";

                if (id != 0)
                {
                    var data = await _context.Quations.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.CompanyName = data.CompanyName;
                        ViewBag.CustomerName = data.CustomerName;
                        ViewBag.Email = data.Email;
                        ViewBag.SalesPersonName = data.SalesPersonName;
                        ViewBag.ProductId = data.ProductId;
                        ViewBag.Subject = data.Subject;
                        ViewBag.Amount = data.Amount;
                        ViewBag.Mobile = data.Mobile;
                        ViewBag.Heading = "Update Quation";
                        ViewBag.btnText = "Update";
                    }
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Quation(QuationDto model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.UserName = AddedBy;

                if (model == null)
                {
                    ModelState.Clear();
                    return View();
                }
                if (model.Id != 0)
                {
                    var response = await _ICrmrpo.Iupdate(model);
                    if (response != null)
                    {
                        TempData["Message"] = "Data Update Successfully.";
                        return RedirectToAction("Quation", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record not found for update.");
                        return View(model);
                    }
                }
                else
                {
                    var response = await _ICrmrpo.Quation(model);
                    TempData["Message"] = "Data Added Successfully.";
                    return RedirectToAction("Quation", "Home");
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuationList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var response = await _ICrmrpo.QuationList();
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

        [HttpGet, Route("Home/Department")]
        public IActionResult Department(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                ViewBag.UserName = AddedBy;
                ViewBag.id = "";
                ViewBag.DepartmentName = "";
                ViewBag.Heading = "Add Department";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data =  _context.DepartmentMasters.Where(x => x.Id == id).FirstOrDefault();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.DepartmentName = data.DepartmentName;
                        ViewBag.Heading = "Update Department";
                        ViewBag.btnText = "Update";
                    }
                }
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
            string AddedBy = HttpContext.Session.GetString("UserName");
            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
            ViewBag.UserName = AddedBy;

            if (model == null)
            {
                ModelState.Clear();
                return View();
            }
            if (model.Id != 0)
            {
                var Department = await _ICrmrpo.updateDepartment(model);
                if (Department != null)
                {
                    TempData["Message"] = "Data Update Successfully.";
                    return RedirectToAction("Department", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Record not found for update.");
                    return View(model);
                }
            }
            else
            {
                DepartmentMaster master = new DepartmentMaster
                {
                    DepartmentName = model.DepartmentName,
                };
                _context.DepartmentMasters.Add(master);
                _context.SaveChanges(); TempData["Message"] = "Data Added Successfully.";
                return RedirectToAction("Department", "Home");
            }
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
        public async Task<IActionResult> WorkLocation(WorkLocation1 model)
        {
            if (model != null)
            {
                WorkLocation1 master = new WorkLocation1
                {
                    StateId = model.StateId,
                    CityId = model.CityId,
                    Createdate = DateTime.Now.Date,
                    Isactive = true,
                    Commissoninpercentage = model.Commissoninpercentage,
                };
                _context.WorkLocations1.Add(master);
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
                var result = from wl in _context.WorkLocations1
                             join ct in _context.Cities on wl.CityId equals ct.Id into cityGroup
                             from city in cityGroup.DefaultIfEmpty()
                             join st in _context.States on wl.StateId equals st.Id into stateGroup
                             from state in stateGroup.DefaultIfEmpty()
                             select new WorkLocationDTO
                             {
                                 Id = wl.Id,
                                 State = state.SName,
                                 City = city.City1,
                                 Commissoninpercentage = Convert.ToInt32(wl.Commissoninpercentage)
                             };

                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(result);

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

        [HttpGet, Route("Home/Designation")]
        public IActionResult Designation(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                ViewBag.UserName = AddedBy;
                ViewBag.id = "";
                ViewBag.DesignationName = "";
                ViewBag.Heading = "Add Designation";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = _context.DesignationMasters.Where(x => x.Id == id).FirstOrDefault();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.DesignationName = data.DesignationName;
                        ViewBag.Heading = "Update Designation";
                        ViewBag.btnText = "Update";
                    }
                }
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
            string AddedBy = HttpContext.Session.GetString("UserName");
            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
            ViewBag.UserName = AddedBy;

            if (model == null)
            {
                ModelState.Clear();
                return View();
            }
            if (model.Id != 0)
            {
                var Department = await _ICrmrpo.updateDesignation(model);
                if (Department != null)
                {
                    TempData["Message"] = "Data Update Successfully.";
                    return RedirectToAction("Designation", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Record not found for update.");
                    return View(model);
                }
            }
            else
            {
                DesignationMaster master = new DesignationMaster
                {
                    DesignationName = model.DesignationName,
                };
                _context.DesignationMasters.Add(master);
                _context.SaveChanges(); TempData["Message"] = "Data Added Successfully.";
                return RedirectToAction("Department", "Home");
            }
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
        public JsonResult Product(int? id)
        {
            var data = (from pm in _context.ProductMasters
                        join gm in _context.GstMasters on Convert.ToInt32(pm.Gst) equals gm.Id
                        //join cs in _context.CustomerRegistrations on pm.Id equals Convert.ToInt32(cs.ProductDetails)
                        where pm.Id == id && pm.IsDeleted == false
                        select new Customer
                        {
                            Scgst = gm.Scgst,
                            Cgst = gm.Cgst,
                            Igst = gm.Igst,
                            Price = pm.Price,
                            HsnSacCode = pm.HsnSacCode,
                            //State = cs.State,
                        }).FirstOrDefault();

            var result = new
            {
                Data = data,
            };

            return new JsonResult(result);
        }

        public JsonResult EditWorkLocation(int id)
        {
            var loc = new WorkLocation1();
            var data = _ICrmrpo.GetWorkLocationById(id);
            loc.Id = data.Id;
            loc.CityId = data.CityId;
            loc.StateId = data.StateId;
            loc.Commissoninpercentage = data.Commissoninpercentage;

            var result = new
            {
                loc = loc,
            };
            return new JsonResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditWorkLocation(WorkLocation1 model)
        {
            try
            {
                var Location = await _ICrmrpo.updateWorkLocation(model);
                if (Location != null)
                {
                    TempData["ErrorMessage"] = "Work Location update Successfully.";
                    return RedirectToAction("WorkLocationlist", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Work Location not update.";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        public IActionResult employeerTDS()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                ViewBag.CustomerName = _context.CustomerRegistrations
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                ViewBag.Message = TempData["ErrorMessage"];
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> employeerTDS(EmployeerTd model)
        {
            try
            {
                if (model != null)
                {
                    EmployeerTd master = new EmployeerTd
                    {
                        Amount = model.Amount,
                        CustomerId = model.CustomerId,
                        WorkLocationId = model.WorkLocationId,
                        CreateDate = DateTime.Now.Date,
                        Isactive = true,
                        Tdspercentage = model.Tdspercentage,
                    };
                    _context.EmployeerTds.Add(master);
                    _context.SaveChanges();
                    TempData["ErrorMessage"] = "Employeer TDS Add";
                    return RedirectToAction("employeerTDS");
                }
                else
                {
                    TempData["ErrorMessage"] = "data not save";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        [HttpPost]
        public IActionResult GetLocationsByCustomer(string customerId)
        {
            if (!string.IsNullOrEmpty(customerId))
            {
                var locations = _context.CustomerRegistrations
                    .AsEnumerable()
                    .Select(c => new
                    {
                        c.Id,
                        WorkLocations = c.WorkLocation?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(loc => _context.Cities.FirstOrDefault(w => w.Id == Convert.ToInt32(loc)))
                                                    .Where(w => w != null)
                    })
                    .FirstOrDefault(x => x.Id == Convert.ToInt32(customerId));

                if (locations != null && locations.WorkLocations != null)
                {
                    var locationList = locations.WorkLocations.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.City1
                    }).ToList();

                    return Json(locationList);
                }
            }

            return Json(new List<SelectListItem>());
        }


        [HttpGet]
        public List<State> BindStateDetails(int CountryId)
        {

            List<State> stateDetail = new List<State>();
            try
            {
                stateDetail = _ICrmrpo.BindState();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return stateDetail;
        }
        [HttpGet]
        public List<City> BindCityDetails(int stateId)
        {
            List<City> cityDetail = new List<City>();
            try
            {
                cityDetail = _ICrmrpo.BindCity(stateId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return cityDetail;
        }
        [Route("Home/addcontribution")]
        [HttpGet]
        public IActionResult addcontribution()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                ViewBag.CustomerName = _context.CustomerRegistrations
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                ViewBag.Message = TempData["ErrorMessage"];
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> addcontribution(Additonalcontribution model)
        {
            try
            {
                if (model != null)
                {
                    Additonalcontribution ac = new Additonalcontribution
                    {
                        ContributionName = model.ContributionName,
                        CustomerId = model.CustomerId,
                        WorkLocationId = model.WorkLocationId,
                        CreatedDate = DateTime.Now.Date,
                        Isactive = true,
                    };
                    _context.Additonalcontributions.Add(ac);
                    _context.SaveChanges();
                    TempData["ErrorMessage"] = "Additonalcontribution  Add";
                    return RedirectToAction("addcontribution");
                }
                else
                {
                    TempData["ErrorMessage"] = "data not save";
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
