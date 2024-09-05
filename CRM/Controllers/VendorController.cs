using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.Controllers
{
    public class VendorController : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;

        public VendorController(ICrmrpo _ICrmrpo, admin_NDCrMContext _context)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
        }
        [Route("Home/Customer")]
        [HttpGet]
        public IActionResult VendorCustomer(int id = 0)
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
        public async Task<IActionResult> VendorCustomer(Customer model)
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
    }
}
