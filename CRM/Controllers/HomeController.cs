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


        [Route("Home/Customer")]
        [HttpGet]
        public IActionResult Customer(int id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");

                if (id != 0)
                {
                    ViewBag.UserName = AddedBy;
                    var data = _ICrmrpo.GetCustomerById(id);
                    if (data != null)
                    {
                        ViewBag.ProductDetails = _context.ProductMasters.Where(x => x.IsDeleted == false)
                            .Select(p => new SelectListItem
                            {
                                Value = p.Id.ToString(),
                                Text = p.ProductName,
                            })
                            .ToList();
                        ViewBag.SelectedStateId = data.StateId;
                        ViewBag.SelectedCityId = data.WorkLocation;
                        ViewBag.state = data.State;
                        ViewBag.startDate = ((DateTime)data.StartDate).ToString("yyyy-MM-dd");
                        ViewBag.renewDate = ((DateTime)data.RenewDate).ToString("yyyy-MM-dd");
                        return View(data);
                    }
                }
                ViewBag.UserName = AddedBy;
                ViewBag.SelectedStateId = null;
                ViewBag.SelectedCityId = null;
                ViewBag.ProductDetails = _context.ProductMasters.Where(x => x.IsDeleted == false)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.ProductName,
                    })
                    .ToList();
                return View();
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
                if (model.Id != null)
                {
                    var data = await _ICrmrpo.updateCustomerReg(model);
                    return RedirectToAction("CustomerList", "Home");
                    TempData["msg"] = "Update Successfully.";
                }
                if (response != null)
                {
                    return RedirectToAction("CustomerList", "Home");
                    TempData["msg"] = "Registration Successfully.";
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
                ViewBag.ProductDetails = _context.ProductMasters.Where(x => x.IsDeleted == false)
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
            var Productdata = _context.ProductMasters.Where(x => x.IsDeleted == false).ToList();
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

        public JsonResult EditDesignation(int id)
        {
            var loc = new DesignationMaster();
            var data = _ICrmrpo.GetDesignationById(id);
            loc.Id = data.Id;
            loc.DesignationName = data.DesignationName;
            var result = new
            {
                loc = loc,
            };
            return new JsonResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditDesignation(DesignationMaster model)
        {
            try
            {
                var Designation = await _ICrmrpo.updateDesignation(model);
                if (Designation != null)
                {
                    TempData["ErrorMessage"] = "Designation update Successfully.";
                    return RedirectToAction("Designationlist", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Designation not update.";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        public JsonResult EditDepartment(int id)
        {
            var loc = new DepartmentMaster();
            var data = _ICrmrpo.GetDepartmentById(id);
            loc.Id = data.Id;
            loc.DepartmentName = data.DepartmentName;
            var result = new
            {
                loc = loc,
            };
            return new JsonResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditDepartment(DepartmentMaster model)
        {
            try
            {
                var Department = await _ICrmrpo.updateDepartment(model);
                if (Department != null)
                {
                    TempData["ErrorMessage"] = "Department update Successfully.";
                    return RedirectToAction("Departmentlist", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Department not update.";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var data = _context.CustomerRegistrations.Find(id);
                if (data != null)
                {
                    _context.CustomerRegistrations.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("CustomerList");
            }
            catch (Exception ex)
            {
                throw new Exception();
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

        [Route("Home/CustomerProfile")]
        [HttpGet]
        public async Task<IActionResult> CustomerProfile()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                string id = Convert.ToString(HttpContext.Session.GetString("UserId")); ;
                ViewBag.UserName = AddedBy;
                ViewBag.id = id;
                var data = await _ICrmrpo.GetCustomerProfile(id);
                ViewBag.Message = TempData["ErrorMessage"];
                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CustomerProfile(CustomerRegistration model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int id = Convert.ToInt32(HttpContext.Session.GetString("UserId")); ;
                ViewBag.UserName = AddedBy;
                if (id != null)
                {
                    var data = await _ICrmrpo.UpdateCustomerProfile(model, AddedBy);
                    TempData["ErrorMessage"] = "Update Successfully.";
                    return RedirectToAction("CustomerProfile");
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
        [Route("Home/Changepassword")]
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
                if (string.IsNullOrEmpty(AddedBy) || (string.IsNullOrEmpty(userId)))
                {
                    TempData["Message"] = "Session expired. Please login again.";
                    return RedirectToAction("Login", "Admin");
                }

                ViewBag.UserName = AddedBy;

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

                if (!string.IsNullOrEmpty(userId))
                {
                    int id = Convert.ToInt32(userId);
                    var data = await _ICrmrpo.UpdateChangepassword(model, AddedBy, id);
                    TempData["Message"] = "Password updated successfully.";
                }
                else
                {
                    TempData["Message"] = "Invalid session data.";
                    return RedirectToAction("Changepassword");
                }

                return RedirectToAction("Logout", "Home");
            }
            catch (Exception Ex)
            {
                TempData["Error"] = "An error occurred while changing the password. Please try again later.";
                return RedirectToAction("Changepassword");
            }
        }
    }

}
