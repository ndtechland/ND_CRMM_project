using CRM.Models.Crm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.Controllers
{
    public class SaleController : Controller
    {
        private readonly admin_NDCrMContext _context;
        public SaleController(admin_NDCrMContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Invoice(int id = 0)
        {
			try
			{
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    string AddedBy = HttpContext.Session.GetString("UserName");
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                    var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();

                    ViewBag.checkvendorbillingstateid = _context.VendorRegistrations.Where(v => v.Id == adminlogin.Vendorid).FirstOrDefault().BillingStateId;

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
                        ViewBag.Heading = "Customer Registration";
                        ViewBag.btnText = "Update";
                        //var data = _ICrmrpo.GetCustomerById(id);
                        //if (data != null)
                        //{

                        //    ViewBag.ProductDetails = _context.ProductMasters.Where(x => x.IsDeleted == false)
                        //        .Select(p => new SelectListItem
                        //        {
                        //            Value = p.Id.ToString(),
                        //            Text = p.ProductName,
                        //        })
                        //        .ToList();
                        //    ViewBag.SelectedStateId = data.StateId;
                        //    ViewBag.SelectedCityId = data.CityId;
                        //    ViewBag.state = data.BillingStateId;
                        //    ViewBag.BillingCityId = data.BillingCityId;
                        //    ViewBag.CheckIsSameAddress = data.IsSameAddress;
                        //    ViewBag.NoOfRenewMonth = data.NoOfRenewMonth;
                        //    ViewBag.Renewprice = data.Renewprice;
                        //    ViewBag.startDate = ((DateTime)data.StartDate).ToString("yyyy-MM-dd");
                        //    ViewBag.renewDate = ((DateTime)data.RenewDate).ToString("yyyy-MM-dd");
                        //    return View(data);
                        //}
                    }
                    ViewBag.UserName = AddedBy;
                    ViewBag.Heading = "Customer Registration";
                    ViewBag.btnText = "SAVE";
                    ViewBag.SelectedStateId = null;
                    ViewBag.SelectedCityId = null;
                    ViewBag.BillingCityId = null;
                    ViewBag.CheckIsSameAddress = null;
                    ViewBag.NoOfRenewMonth = null;
                    ViewBag.Renewprice = null;
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
			catch (Exception)
			{

				throw;
			}
        }
         
        [HttpGet]
        public IActionResult GetCustomerNames(string searchTerm)
        {
            var customers = _context.CustomerRegistrations
                                    .Where(c => c.CompanyName.Contains(searchTerm))
                                    .Select(c => new
                                    {
                                        Id = c.Id,
                                        CompanyName = c.CompanyName
                                    })
                                    .ToList();

            return Json(customers);
        }

        [HttpGet]
        public IActionResult GetCustomerDetailsById(int id)
        {
            var customer = _context.CustomerRegistrations
                                   .Where(c => c.Id == id)
                                   .Select(c => new
                                   {
                                       BillingAddress = c.BillingAddress,
                                       Location = c.Location,
                                       OfficeStateId = c.StateId,
                                       OfficeCityId = c.CityId,
                                       BillingStateId = c.BillingStateId,
                                       BillingCityId = c.BillingCityId
                                   })
                                   .FirstOrDefault();

            return Json(customer);
        }




    }
}
