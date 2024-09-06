using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.Controllers
{
    public class Vendor : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;

        public Vendor(ICrmrpo _ICrmrpo, admin_NDCrMContext _context)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
        }
        [Route("Vendor/VendorRegistration")]
        [HttpGet]
        public IActionResult VendorRegistration(int id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");

                if (id != 0)
                {
                    ViewBag.UserName = AddedBy;
                    var data = _ICrmrpo.GetVendorById(id);
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
        public async Task<IActionResult> VendorRegistration(VendorDto model)
        {
            try
            {
                if (model.Id > 0)
                {
                    var data = await _ICrmrpo.updateVendorreg(model);
                    if (data > 0)
                    {
                        TempData["msg"] = "Update Successfully.";
                        return RedirectToAction("VendorList", "Vendor");
                    }
                    else
                    {
                        TempData["msg"] = "Update Failed.";
                        return View(model);
                    }
                }
                else
                {
                    var response = await _ICrmrpo.Vendorreg(model);
                    if (response > 0)
                    {
                        TempData["msg"] = "Registration Successfully.";
                        return RedirectToAction("VendorList", "Vendor");
                    }
                    else
                    {
                        TempData["msg"] = "Registration Failed.";
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
        public async Task<IActionResult> VendorList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = await _ICrmrpo.VendorList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }
        public async Task<IActionResult> DeleteVendor(int id)
        {
            try
            {
                var data = _context.VendorRegistrations.Find(id);
                if (data != null)
                {
                    _context.VendorRegistrations.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("VendorList");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        [Route("Vendor/VendorProfile")]
        [HttpGet]
        public async Task<IActionResult> VendorProfile()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                string id = Convert.ToString(HttpContext.Session.GetString("UserId")); ;
                ViewBag.UserName = AddedBy;
                ViewBag.id = id;
                var data = await _ICrmrpo.GetVendorProfile(id);
                ViewBag.Message = TempData["ErrorMessage"];
                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> VendorProfile(VendorRegistration model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int id = Convert.ToInt32(HttpContext.Session.GetString("UserId")); ;
                ViewBag.UserName = AddedBy;
                if (id != null)
                {
                    var data = await _ICrmrpo.UpdateVendorProfile(model, AddedBy);
                    TempData["ErrorMessage"] = "Update Successfully.";
                    return RedirectToAction("VendorProfile");
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
