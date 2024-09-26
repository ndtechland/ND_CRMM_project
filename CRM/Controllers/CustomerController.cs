using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    public class CustomerController : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;

        public CustomerController(ICrmrpo _ICrmrpo, admin_NDCrMContext _context)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
        }
        [Route("Customer/Customer")]
        [HttpGet]
        public IActionResult Customer(int id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                //ViewBag.StateItems = _context.States
                //    .Select(p => new SelectListItem
                //    {
                //        Value = p.Id.ToString(),
                //        Text = p.SName,
                //    })
                //    .ToList();
                var items = _context.States.ToList();
                ViewBag.StateItems = new SelectList(items, "Id", "SName");
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
                        ViewBag.BillingCityId = data.BillingCityId;
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
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                if (model.Id > 0)
                {
                    var data = await _ICrmrpo.updateCustomerReg(model);
                    if (data > 0)
                    {
                        TempData["Message"] = "Update Successfully.";
                        return RedirectToAction("Customer", "Customer");
                    }
                    else
                    {
                        TempData["Message"] = "Update Failed.";
                        return View(model);
                    }
                }
                else
                {
                    var response = await _ICrmrpo.Customer(model,(int)adminlogin.Vendorid);
                    if (response > 0)
                    {
                        TempData["Message"] = "Registration Successfully.";
                        return RedirectToAction("Customer", "Customer");
                    }
                    else
                    {
                        TempData["Message"] = "Registration Failed.";
                        ModelState.Clear();
                        return View(model);
                    }
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
                string AddedBy = HttpContext.Session.GetString("UserName");
                string userIdString = HttpContext.Session.GetString("UserId");
                var response = await _ICrmrpo.CustomerList(userIdString);
                ViewBag.UserName = AddedBy;
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
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
        [Route("Customer/CustomerProfile")]
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
                    TempData["Message"] = "Update Successfully.";
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


        //new 
        public async Task<IActionResult> GetCityByStateId(int stateid)
        {
            var dist = await _context.Cities
                .Where(s => s.StateId == stateid)
                .Select(s => new { id = s.Id, name = s.City1 }).ToListAsync();

            return Json(dist);
        }
    }
}
