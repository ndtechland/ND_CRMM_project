using ClosedXML.Excel;
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
        private readonly IEmailService _emailService;

        public CustomerController(ICrmrpo _ICrmrpo, admin_NDCrMContext _context, IEmailService emailService)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
            _emailService = emailService;
        }
        [Route("Customer/Customer")]
        [HttpGet]
        public IActionResult Customer(int id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();

                ViewBag.checkvendorbillingstateid = _context.VendorRegistrations.Where(v => v.Id == adminlogin.Vendorid).FirstOrDefault().BillingStateId;
                var items = _context.States.ToList();
                ViewBag.StateItems = new SelectList(items, "Id", "SName");
                if (id != 0)
                {
    
                    ViewBag.Heading = "Customer Registration";
                    ViewBag.btnText = "Update";
                    var data =  _ICrmrpo.GetCustomerById(id);
                    if (data != null)
                    {

                        ViewBag.ProductDetails = _context.ProductMasters.Where(x => x.IsDeleted == false)
                            .Select(p => new SelectListItem
                            {
                                Value = p.Id.ToString(),
                                Text = p.ProductName,
                            }).ToList();

                        ViewBag.SelectedStateId = data.StateId;
                        ViewBag.SelectedCityId = data.CityId;
                        ViewBag.state = data.BillingStateId;
                        ViewBag.BillingCityId = data.BillingCityId;
                        ViewBag.CheckIsSameAddress = data.IsSameAddress;
                        ViewBag.NoOfRenewMonth = data.NoOfRenewMonth;
                        ViewBag.Renewprice = data.Renewprice;
                        ViewBag.startDate = ((DateTime)data.StartDate).ToString("yyyy-MM-dd");
                        ViewBag.renewDate = ((DateTime)data.RenewDate).ToString("yyyy-MM-dd");
                        return View(data);
                    }
                }

                ViewBag.Heading = "Customer Registration";
                ViewBag.btnText = "SAVE";
                ViewBag.SelectedStateId = null;
                ViewBag.SelectedCityId = null;
                ViewBag.BillingCityId = null;
                ViewBag.CheckIsSameAddress =null;
                ViewBag.NoOfRenewMonth = null;
                ViewBag.Renewprice = null;
                ViewBag.ProductDetails = _context.ProductMasters.Where(x => x.IsDeleted == false)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.ProductName,
                    }).ToList();
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
                        TempData["Message"] = "updok";
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
                        TempData["Message"] = "ok";
                        await _emailService.CustomerWelcomeEmail(model.Email, model.CompanyName);
                        return RedirectToAction("Customer", "Customer");
                    }
                    else
                    {
                        TempData["Message"] = "Failed.";
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
                
                int Adminid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Adminid).FirstOrDefaultAsync();
                var response = await _ICrmrpo.CustomerList((int)adminlogin.Vendorid);

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
                    TempData["Message"] = "dltok";
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
                
                string id = Convert.ToString(HttpContext.Session.GetString("UserId")); ;

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
                
                int id = Convert.ToInt32(HttpContext.Session.GetString("UserId")); ;

                if (id != null)
                {
                    var data = await _ICrmrpo.UpdateCustomerProfile(model, id);
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

        public async Task<IActionResult> GetCityByStateId(int stateid)
        {
            var dist = await _context.Cities
                .Where(s => s.StateId == stateid)
                .Select(s => new { id = s.Id, name = s.City1 }).ToListAsync();

            return Json(dist);
        }
        public async Task<IActionResult> ExportCustomerList()
        {
            try
            {
                int Adminid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Adminid).FirstOrDefaultAsync();
                var customerList = await _ICrmrpo.CustomerList((int)adminlogin.Vendorid);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Customer Details");

                    // Adding Header Row
                    int currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Sl. No.";
                    worksheet.Cell(currentRow, 2).Value = "First Name";
                    worksheet.Cell(currentRow, 3).Value = "Last Name";
                    worksheet.Cell(currentRow, 4).Value = "Company Name";
                    worksheet.Cell(currentRow, 5).Value = "Mobile Number";
                    worksheet.Cell(currentRow, 6).Value = "Alternate Number";
                    worksheet.Cell(currentRow, 7).Value = "Email ID";
                    worksheet.Cell(currentRow, 8).Value = "GST Number";
                    worksheet.Cell(currentRow, 9).Value = "Office Location";
                    worksheet.Cell(currentRow, 10).Value = "Office City";
                    worksheet.Cell(currentRow, 11).Value = "Office State";
                    worksheet.Cell(currentRow, 12).Value = "Billing Location";
                    worksheet.Cell(currentRow, 13).Value = "Billing City";
                    worksheet.Cell(currentRow, 14).Value = "Billing State";

                    // Applying style to header
                    for (int col = 1; col <= 14; col++)
                    {
                        worksheet.Cell(currentRow, col).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, col).Style.Font.Bold = true;
                    }

                    // Adding Data Rows
                    int serialNumber = 1;
                    foreach (var item in customerList)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = serialNumber++;
                        worksheet.Cell(currentRow, 2).Value = item.FirstName;
                        worksheet.Cell(currentRow, 3).Value = item.LastName;
                        worksheet.Cell(currentRow, 4).Value = item.CompanyName;
                        worksheet.Cell(currentRow, 5).Value = item.MobileNumber;
                        worksheet.Cell(currentRow, 6).Value = item.AlternateNumber;
                        worksheet.Cell(currentRow, 7).Value = item.Email;
                        worksheet.Cell(currentRow, 8).Value = item.GstNumber;
                        worksheet.Cell(currentRow, 9).Value = item.Location;
                        worksheet.Cell(currentRow, 10).Value = item.OfficeCity;
                        worksheet.Cell(currentRow, 11).Value = item.OfficeState;
                        worksheet.Cell(currentRow, 12).Value = item.BillingAddress;
                        worksheet.Cell(currentRow, 13).Value = item.BillingCity;
                        worksheet.Cell(currentRow, 14).Value = item.BillingState;


                    }

                    // Adjust columns to content
                    worksheet.Columns().AdjustToContents();

                    // Save and return the Excel file
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileContent = stream.ToArray();
                        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CustomerList.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
