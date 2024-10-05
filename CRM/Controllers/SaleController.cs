using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    public class SaleController : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        public SaleController(admin_NDCrMContext context, ICrmrpo ICrmrpo)
        {
            _context = context;
            _ICrmrpo = ICrmrpo;
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
                    //var customerData = _context.CustomerInvoices.Where(c => c.CustomerId == id).ToList();
                    

                    if (id != 0)
                    {
                        ViewBag.UserName = AddedBy;
                        ViewBag.Heading = "Invoice";
                        ViewBag.btnText = "Update";
                        var data = _context.CustomerInvoices.Where(c=>c.CustomerId==id).FirstOrDefault();
                        if (data != null)
                        {

                            ViewBag.ProductDetails = _context.ProductMasters.Where(x => x.IsDeleted == false)
                                .Select(p => new SelectListItem
                                {
                                    Value = p.Id.ToString(),
                                    Text = p.ProductName,
                                })
                                .ToList();
                            ViewBag.ProductId = data.ProductId;
                            ViewBag.Price = data.ProductPrice;
                            ViewBag.RenewPrice = data.RenewPrice;
                            ViewBag.NoOfRenewMonth = data.NoOfRenewMonth;
                            ViewBag.HsnSacCode = data.Hsncode;
                            ViewBag.StartDate = data.StartDate;
                            ViewBag.RenewDate = data.RenewDate;
                            ViewBag.IGST = data.Igst;
                            ViewBag.SGST = data.Sgst;
                            ViewBag.CGST = data.Cgst;
                            return View();
                        }
                    }
                    ViewBag.UserName = AddedBy;
                    ViewBag.Heading = "Invoice";
                    ViewBag.btnText = "SAVE";
                    ViewBag.ProductId =null;
                    ViewBag.Price = 0;
                    ViewBag.RenewPrice = null;
                    ViewBag.NoOfRenewMonth = null;
                    ViewBag.HsnSacCode = null;
                    ViewBag.StartDate = null;
                    ViewBag.RenewDate = null;
                    ViewBag.IGST = 0;
                    ViewBag.SGST = 0;
                    ViewBag.CGST = 0;
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
        [HttpPost]
        public async Task<IActionResult> Invoice( List<ProductDetail> model)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();

                 
                    bool data = await _ICrmrpo.CustomerInvoice(model, (int)adminlogin.Vendorid);
                if (data)
                {
                    TempData["Message"] = "Added Successfully.";
                    TempData.Keep("Message");
                
                    var model1 =
                        new
                        {
                            path = "/Sale/CustomerInvoiceList"
                        };
                    return Ok(model1);
                        //return RedirectToAction("CustomerInvoiceList", "Sale");
                    }
                    else
                    {
                        TempData["Message"] = "Add Failed.";
                        return View(model);
                    }
                 
                 
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetCustomerNames(string searchTerm)
        {
            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();

            var customers = _context.CustomerRegistrations
                                    .Where(c => c.CompanyName.Contains(searchTerm) && c.Vendorid==adminlogin.Vendorid)
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
            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();

            var customer = (from c in _context.CustomerRegistrations
                          join so in _context.States on c.StateId equals so.Id
                          join sb in _context.States on c.BillingStateId equals sb.Id
                          join co in _context.Cities on c.CityId equals co.Id
                          join cb in _context.Cities on c.BillingCityId equals cb.Id
                          where c.Id == id && c.Vendorid== adminlogin.Vendorid
                            select new
                            {
                              BillingAddress = c.BillingAddress,
                              Location = c.Location,
                              OfficeState = so.SName, 
                              OfficeCity = co.City1,    
                              BillingState = sb.SName,  
                              BillingStateId = c.BillingStateId,
                                MobileNumber = c.MobileNumber,
                                Email = c.Email,
                                GstNumber = c.GstNumber,  
                              BillingCity = cb.City1    
                          })
               .FirstOrDefault();

            return Json(customer);
        }        

        public async Task<IActionResult> CustomerInvoiceList()
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();

                List<CustomerInvoiceDTO> data = await _ICrmrpo.GetCustometInvoiceList((int)adminlogin.Vendorid);
                return View(data);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteCustomerInvoice(int id)
        {
            try
            { 
                var dlt = await _context.CustomerInvoices.Where(c => c.CustomerId == id).ToListAsync();  
                 
                if (dlt.Any())
                {                     
                    _context.CustomerInvoices.RemoveRange(dlt); // Use RemoveRange for multiple deletions
                    await _context.SaveChangesAsync(); 
                    TempData["Message"] = "Deleted Successfully.";
                }
                else
                {
                    TempData["Message"] = "No invoices found for deletion.";
                }

                return RedirectToAction("CustomerInvoiceList");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                TempData["Error"] = $"Error deleting invoices: {ex.Message}";
                return RedirectToAction("CustomerInvoiceList");
            }
        }

    }
}
