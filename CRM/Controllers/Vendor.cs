using CRM.IUtilities;
using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;

using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using SelectPdf;
using System.Data.SqlClient;
using ClosedXML.Excel;
using Org.BouncyCastle.Ocsp;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.ExtendedProperties;
using Path = System.IO.Path;
using jsreport.AspNetCore;
using jsreport.Types;

namespace CRM.Controllers
{
    public class Vendor : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;
        public Vendor(ICrmrpo _ICrmrpo, admin_NDCrMContext _context, IWebHostEnvironment hostingEnvironment, IEmailService emailService)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
            _webHostEnvironment = hostingEnvironment;
            _emailService = emailService;
        }
        [HttpGet, Route("Vendor/VendorRegistration")]
        public IActionResult VendorRegistration(int id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                var items = _context.States.ToList();
                ViewBag.StateItems = new SelectList(items, "Id", "SName");
                var model = new VendorDto();
                if (id != 0)
                {
                    ViewBag.Heading = "Vendor Registration";
                    ViewBag.btnText = "Update";
                    var vendor = _context.VendorRegistrations.Where(x => x.Id == id).FirstOrDefault();

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
                        ViewBag.PlanPrice = _context.PricingPlans
                         .Where(x => x.IsActive == true)
                          .Select(p => new SelectListItem
                          {
                              Value = p.Id.ToString(),
                              Text = $"{p.PlanName} ({p.Price})"
                          }).ToList();
                        ViewBag.SelectedStateId = data.StateId;
                        ViewBag.Price = vendor.Productprice;
                        ViewBag.Renewprice = data.Renewprice;
                        ViewBag.NoOfRenewMonth = data.NoOfRenewMonth;
                        ViewBag.SelectedCityId = data.CityId;
                        ViewBag.SelectedBillingCityId = data.BillingCityId;
                        ViewBag.SelectedBillingStateId = data.BillingStateId;
                        ViewBag.CheckIsSameAddress = data.IsSameAddress;
                        ViewBag.state = data.State;
                        ViewBag.startDate = ((DateTime)data.StartDate).ToString("yyyy-MM-dd");
                        ViewBag.renewDate = ((DateTime)data.RenewDate).ToString("yyyy-MM-dd");
                        ViewBag.duedate = (data.Duedate).ToString("yyyy-MM-dd");
                        ViewBag.Pricingplan = vendor.PricingPlanid;
                        ViewBag.SelectCompany = vendor.SelectCompany;
                        ViewBag.LandlineNumber = vendor.MobileNumber;
                        ViewBag.Notes = vendor.Notes;
                        ViewBag.Terms = vendor.Terms;
                        return View(data);
                    }
                }
                ViewBag.Heading = "Vendor Registration";
                ViewBag.btnText = "SAVE";

                ViewBag.SelectedStateId = null;
                ViewBag.SelectedCityId = null;
                ViewBag.LandlineNumber = 0;
                ViewBag.ProductDetails = _context.ProductMasters.Where(x => x.IsDeleted == false)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.ProductName,
                    }).ToList();
                ViewBag.PlanPrice = _context.PricingPlans
                        .Where(x => x.IsActive == true)
                         .Select(p => new SelectListItem
                         {
                             Value = p.Id.ToString(),
                             Text = $"{p.PlanName} ({p.Price})"
                         }).ToList();
                return View(model);
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
                string invoiceNo = GenerateInvoiceNumber();

                var existingInvoice = await _context.VendorRegistrations
                                                    .FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNo);
                if (existingInvoice != null)
                {
                    invoiceNo = GenerateInvoiceNumber();
                }
                if (model.Id > 0)
                {
                    var data = await _ICrmrpo.updateVendorreg(model);
                    if (data > 0)
                    {
                        TempData["Message"] = "Update Successfully.";
                        return RedirectToAction("VendorRegistration", "Vendor");
                    }
                    else
                    {
                        TempData["Message"] = "Update Failed.";
                        return View(model);
                    }
                }
                else
                {
                    var response = await _ICrmrpo.Vendorreg(model, invoiceNo);
                    if (response != null)
                    {
                        TempData["Message"] = "Registration Successfully.";
                        await _emailService.SendEmailCredentials(model.Email, model.CompanyName, response.UserName, response.Password);
                        return RedirectToAction("VendorRegistration", "Vendor");
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
        public async Task<IActionResult> VendorList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = await _ICrmrpo.VendorList();


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

        [HttpGet, Route("Vendor/VendorProfile")]
        public async Task<IActionResult> VendorProfile()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                string id = Convert.ToString(HttpContext.Session.GetString("UserId"));
                var items = _context.States.ToList();
                ViewBag.StateItems = new SelectList(items, "Id", "SName");

                var data = await _ICrmrpo.GetVendorProfile(id);
                ViewBag.vendorid = data.Id;
                ViewBag.SelectedCityId = data.CityId;
                ViewBag.SelectedBillingCityId = data.BillingCityId;
                ViewBag.SelectedBillingStateId = data.BillingStateId;
                ViewBag.FilePathDetail = data.CompanyImage;
                ViewBag.id = id;
                ViewBag.professionaltax = data.Isprofessionaltax;
                ViewBag.VendorSingature = data.VendorSingature;
                ViewBag.btnText = "Update";
                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> VendorProfile(VendorRegistrationDto model)
        {
            try
            {

                int id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                if (id != null)
                {
                    var data = await _ICrmrpo.UpdateVendorProfile(model, id);
                    TempData["Message"] = "ok";
                    return RedirectToAction("VendorProfile", "Vendor");
                }
                else
                {
                    ModelState.Clear();
                    return View();
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception Ex)
            {
                throw new Exception("Error: " + Ex.Message);
            }
        }

        [HttpGet, Route("Vendor/ApprovedPresnolInfo")]
        public async Task<IActionResult> ApprovedPresnolInfo(int? id)
        {
            try
            {
                ViewBag.states = _context.States
                    .Select(w => new SelectListItem
                    {
                        Value = w.Id.ToString(),
                        Text = w.SName
                    }).ToList();

                int iId = id ?? 0;

                if (HttpContext.Session.GetString("UserName") != null)
                {
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    ViewBag.id = 0;
                    ViewBag.FullName = "";
                    ViewBag.Personal_Email_Address = "";
                    ViewBag.Mobile_Number = "";
                    ViewBag.Date_Of_Birth = "";
                    ViewBag.Age = "";
                    ViewBag.Father_Name = "";
                    ViewBag.PAN = "";
                    ViewBag.Address_Line_1 = "";
                    ViewBag.Address_Line_2 = "";
                    ViewBag.Pincode = "";
                    ViewBag.AadharNo = "";
                    ViewBag.Stateid = "";
                    ViewBag.City = "";
                    ViewBag.Aadharone = "";
                    ViewBag.Aadhartwo = "";
                    ViewBag.Panimg = "";
                    ViewBag.FatherName = "";
                    ViewBag.Heading = "Add PersonalInfo";
                    ViewBag.BtnText = "SAVE";

                    if (iId != 0)
                    {
                        var ApprovedPresnolInfo = _context.ApprovedPresnolInfos.Find(iId);
                        if (ApprovedPresnolInfo != null)
                        {
                            ViewBag.id = ApprovedPresnolInfo.Id;
                            ViewBag.FullName = ApprovedPresnolInfo.FullName;
                            ViewBag.Personal_Email_Address = ApprovedPresnolInfo.PersonalEmailAddress;
                            ViewBag.Mobile_Number = ApprovedPresnolInfo.MobileNumber;
                            ViewBag.Date_Of_Birth = ApprovedPresnolInfo.DateOfBirth?.ToString("yyyy-MM-dd");
                            ViewBag.PAN = ApprovedPresnolInfo.Pan;
                            ViewBag.Address_Line_1 = ApprovedPresnolInfo.AddressLine1;
                            ViewBag.Address_Line_2 = ApprovedPresnolInfo.AddressLine2;
                            ViewBag.Pincode = ApprovedPresnolInfo.Pincode;
                            ViewBag.AadharNo = ApprovedPresnolInfo.AadharNo;
                            ViewBag.Stateid = ApprovedPresnolInfo.StateId;
                            ViewBag.City = ApprovedPresnolInfo.City;
                            ViewBag.Aadharone = ApprovedPresnolInfo.AadharOne;
                            ViewBag.Aadhartwo = ApprovedPresnolInfo.AadharTwo;
                            ViewBag.Panimg = ApprovedPresnolInfo.Panimg;
                            ViewBag.FatherName = ApprovedPresnolInfo.FatherName;
                            ViewBag.Heading = "Update PersonalInfo";
                            ViewBag.BtnText = "UPDATE";
                        }
                    }
                    List<EmployeeApprovedPresnolInfo> EmployeePresnolInfo = await _ICrmrpo.ApprovedPresnolInfoList(Userid);
                    List<EmployeeApprovedPresnolInfo> PreviousData = await _ICrmrpo.PreviousDataApprovedPresnolInfoList(Userid);

                    var model = new EmployeePresnolInfoList
                    {
                        ApprovedPresnolInfos = EmployeePresnolInfo,
                        PreviousData = PreviousData
                    };
                    return View(model);


                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Message : " + ex.Message, ex);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ApprovedPresnolInfo(EmployeePresnolInfoList model)
        {
            try
            {
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                if (HttpContext.Session.GetString("UserName") != null)
                {
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));


                    if (model.Panbase64 != null)
                    {
                        var panImageName = fileOperation.SaveBase64Image("img1", model.Panbase64, allowedExtensions);
                        if (panImageName != "not allowed")
                        {
                            model.Panimg = panImageName;
                        }
                    }
                    if (model.Aadharbase64 != null && model.Aadharbase64.Count == 2)
                    {
                        var aadharOneImageName = fileOperation.SaveBase64Image("img1", model.Aadharbase64[0], allowedExtensions);
                        var aadharTwoImageName = fileOperation.SaveBase64Image("img1", model.Aadharbase64[1], allowedExtensions);

                        if (aadharOneImageName != "not allowed")
                        {
                            model.AadharOne = aadharOneImageName;
                        }

                        if (aadharTwoImageName != "not allowed")
                        {
                            model.AadharTwo = aadharTwoImageName;
                        }
                    }
                    bool isAddedOrUpdated = await _ICrmrpo.AddApprovedPresnolInfo(model);
                    if (isAddedOrUpdated)
                    {
                        TempData["msg"] = model.id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";
                    }
                    else
                    {
                        TempData["msg"] = "Failed to add or update the record.";
                    }

                    return RedirectToAction("ApprovedPresnolInfo");
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the contact.", ex);
            }
        }

        public async Task<JsonResult> UpdateApprovalStatus(int itemId, bool isApproved)
        {
            try
            {
                var item = await _context.ApprovedPresnolInfos.FindAsync(itemId);
                if (item != null)
                {
                    item.IsApproved = isApproved;
                    await _context.SaveChangesAsync();
                    var empRe = await _context.EmployeeRegistrations.FirstOrDefaultAsync(x => x.EmployeeId == item.EmployeeId);
                    var emp = await _context.EmployeePersonalDetails.FirstOrDefaultAsync(x => x.EmpRegId == item.EmployeeId);
                    if (emp != null)
                    {
                        empRe.FirstName = item.FullName;
                        emp.PersonalEmailAddress = item.PersonalEmailAddress;
                        emp.MobileNumber = Convert.ToString(item.MobileNumber);
                        emp.DateOfBirth = (DateTime)item.DateOfBirth;
                        emp.Pan = item.Pan;
                        emp.AddressLine1 = item.AddressLine1;
                        emp.AddressLine2 = item.AddressLine2;
                        emp.StateId = item.StateId;
                        emp.City = item.City;
                        emp.Pincode = item.Pincode;
                        emp.Aadharcard = item.AadharNo;
                        emp.AadharOne = item.AadharOne;
                        emp.Panimg = item.Panimg;
                        emp.AadharTwo = item.AadharTwo;
                        emp.FatherName = item.FatherName;
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Approved Successfully" });
                    }
                    return Json(new { success = false, message = "Employee not found" });
                }
                return Json(new { success = false, message = "Item not found" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet, Route("Vendor/Approvedbankdetails")]
        public async Task<IActionResult> Approvedbankdetails(int? id)
        {
            try
            {
                int iId = id ?? 0;

                if (HttpContext.Session.GetString("UserName") != null)
                {
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    ViewBag.id = 0;
                    ViewBag.Account_Holder_Name = "";
                    ViewBag.Bank_Name = "";
                    ViewBag.Account_Number = "";
                    ViewBag.Re_Enter_Account_Number = "";
                    ViewBag.IFSC = "";
                    ViewBag.EPF_Number = "";
                    ViewBag.nominee = "";
                    ViewBag.AccountTypeID = "";
                    ViewBag.Chequebase = "";
                    ViewBag.Heading = "Add Bankdetails";
                    ViewBag.BtnText = "SAVE";

                    if (iId != 0)
                    {
                        var Approvedbankdetails = _context.Approvedbankdetails.Find(iId);
                        if (Approvedbankdetails != null)
                        {
                            ViewBag.id = Approvedbankdetails.Id;
                            ViewBag.Account_Holder_Name = Approvedbankdetails.AccountHolderName;
                            ViewBag.Bank_Name = Approvedbankdetails.BankName;
                            ViewBag.Account_Number = Approvedbankdetails.AccountNumber;
                            ViewBag.Re_Enter_Account_Number = Approvedbankdetails.ReEnterAccountNumber;
                            ViewBag.IFSC = Approvedbankdetails.Ifsc;
                            ViewBag.EPF_Number = Approvedbankdetails.EpfNumber;
                            ViewBag.nominee = Approvedbankdetails.Nominee;
                            ViewBag.AccountTypeID = Approvedbankdetails.AccountTypeId;
                            ViewBag.Chequebase = Approvedbankdetails.Chequeimage;
                            ViewBag.Heading = "Update Bankdetails";
                            ViewBag.BtnText = "UPDATE";
                        }
                    }
                    List<ApprovedbankdetailList> Approvedbankdetail = await _ICrmrpo.ApprovedbankdetailList(Userid);
                    List<ApprovedbankdetailList> PreviousData = await _ICrmrpo.PreviousDataApprovedbankdetailList(Userid);

                    var model = new ApprovedbankdetailList
                    {
                        Approvedbankdetails = Approvedbankdetail,
                        PreviousData = PreviousData,
                    };
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Message : " + ex.Message, ex);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Approvedbankdetails(ApprovedbankdetailList model)
        {
            try
            {
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    int AddedByid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));


                    if (model.Chequebase64 != null)
                    {
                        var chequeImageName = fileOperation.SaveBase64Image("ChequeImage", model.Chequebase64, allowedExtensions); ;
                        if (chequeImageName != "not allowed")
                        {
                            model.Chequeimage = chequeImageName;
                        }
                    }
                    bool isAddedOrUpdated = await _ICrmrpo.AddApprovedbankdetail(model);
                    if (isAddedOrUpdated)
                    {
                        TempData["msg"] = model.id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";
                    }
                    else
                    {
                        TempData["msg"] = "Failed to add or update the record.";
                    }

                    return RedirectToAction("Approvedbankdetails");
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the contact.", ex);
            }
        }

        public async Task<JsonResult> UpdatebankStatus(int itemId, bool isApproved)
        {
            try
            {
                var item = await _context.Approvedbankdetails.FindAsync(itemId);
                if (item != null)
                {
                    item.IsApproved = isApproved;
                    await _context.SaveChangesAsync();
                    var emp = await _context.EmployeeBankDetails.FirstOrDefaultAsync(x => x.EmpId == item.EmployeeId);
                    if (emp != null)
                    {
                        emp.AccountHolderName = item.AccountHolderName;
                        emp.BankName = item.BankName;
                        emp.AccountNumber = item.AccountNumber;
                        emp.ReEnterAccountNumber = item.ReEnterAccountNumber;
                        emp.Ifsc = item.Ifsc;
                        emp.AccountTypeId = Convert.ToInt32(item.AccountTypeId);
                        emp.EpfNumber = item.EpfNumber;
                        emp.Nominee = item.Nominee;
                        emp.Chequeimage = item.Chequeimage;
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Approved Successfully" });
                    }
                    return Json(new { success = false, message = "Employee not found" });
                }
                return Json(new { success = false, message = "Item not found" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult DeletCompanyImageFile(string FilePath, int id)
        {
            bool success = false;

            if (FilePath != "")
            {
                string folderPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CompanyImage");
                string folderfilepathPath = folderPath + "//" + FilePath;
                if (Directory.Exists(folderPath))
                {
                    if (System.IO.File.Exists(folderfilepathPath))
                    {
                        System.IO.File.Delete(folderfilepathPath);
                        success = true;
                    }
                    var img = _context.VendorRegistrations.FirstOrDefault(s => s.CompanyImage == FilePath && s.Id == id);
                    if (img != null)
                    {
                        img.CompanyImage = null;
                        _context.SaveChangesAsync();
                    }

                }
            }
            return Json(success);
        }
        [HttpGet, Route("Vendor/VendorAttendancedays")]
        public async Task<IActionResult> VendorAttendancedays(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                List<Attendanceday> response = _context.Attendancedays.Where(x => x.Vendorid == adminlogin.Vendorid).ToList();

                ViewBag.id = 0;
                ViewBag.Nodays = "";
                ViewBag.Heading = "Add Days";
                ViewBag.BtnText = "SAVE";
                if (id != 0)
                {
                    var data = await _context.Attendancedays.Where(x => x.Id == id).FirstOrDefaultAsync();
                    ViewBag.id = data.Id;
                    ViewBag.Nodays = data.Nodays;
                    ViewBag.Heading = "Update Days";
                    ViewBag.BtnText = "UPDATE";
                }
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> VendorAttendancedays(Attendanceday model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var existingdays = await _context.Attendancedays
                       .Where(b => b.Vendorid == adminlogin.Vendorid)
                             .FirstOrDefaultAsync();
                if (model.Id == 0)
                {
                    if (existingdays != null)
                    {
                        TempData["Message"] = "Already exists.";
                        return RedirectToAction("VendorAttendancedays", "Vendor");
                    }
                }

                if (model == null)
                {
                    ModelState.Clear();
                    return View();
                }
                if (model.Id != 0)
                {
                    var existingData = await _context.Attendancedays.FindAsync(model.Id);
                    if (existingData != null)
                    {
                        existingData.Nodays = model.Nodays;
                        existingData.Createdate = DateTime.Now.Date;
                        existingData.Vendorid = adminlogin.Vendorid;

                        await _context.SaveChangesAsync();
                        TempData["Message"] = "updok";
                        return RedirectToAction("VendorAttendancedays", "Vendor");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record not found for update.");
                        return View(model);
                    }
                }
                else
                {
                    var data = new Attendanceday()
                    {
                        Nodays = model.Nodays,
                        Createdate = DateTime.Now.Date,
                        Vendorid = adminlogin.Vendorid,
                    };

                    _context.Attendancedays.Add(data);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "ok";
                    return RedirectToAction("VendorAttendancedays", "Vendor");
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteAttendancedays(int id)
        {
            try
            {
                var data = _context.Attendancedays.Find(id);
                if (data != null)
                {
                    _context.Attendancedays.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("VendorAttendancedays");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteList:" + ex.Message);
            }
        }
        [HttpGet, Route("Vendor/Officeshift")]
        public async Task<IActionResult> Officeshift(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                List<Officeshift> response = await _context.Officeshifts.Where(x => x.Vendorid == adminlogin.Vendorid).ToListAsync();


                ViewBag.Starttime = "";
                ViewBag.Endtime = "";
                ViewBag.ShiftTypeid = "";
                ViewBag.Heading = "Add Office Shift";
                ViewBag.BtnText = "SAVE";
                if (id != 0)
                {
                    var data = await _context.Officeshifts.Where(x => x.Id == id).FirstOrDefaultAsync();
                    ViewBag.id = data.Id;
                    ViewBag.Starttime = data.Starttime;
                    ViewBag.Endtime = data.Endtime;
                    ViewBag.ShiftTypeid = data.ShiftTypeid;
                    ViewBag.Heading = "Update Office Shift";
                    ViewBag.BtnText = "UPDATE";
                }
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Officeshift(Officeshift model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


                if (model == null)
                {
                    ModelState.Clear();
                    return View();
                }
                if (model.Id != 0)
                {
                    var existingData = await _context.Officeshifts.FindAsync(model.Id);
                    if (existingData != null)
                    {
                        existingData.Starttime = model.Starttime;
                        existingData.Endtime = model.Endtime;
                        existingData.ShiftTypeid = model.ShiftTypeid;
                        existingData.Createdate = DateTime.Now.Date;
                        existingData.Vendorid = adminlogin.Vendorid;

                        await _context.SaveChangesAsync();
                        TempData["Message"] = "updok";
                        return RedirectToAction("Officeshift", "Vendor");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record not found for update.");
                        return View(model);
                    }
                }
                else
                {
                    var data = new Officeshift()
                    {
                        Starttime = model.Starttime,
                        Endtime = model.Endtime,
                        ShiftTypeid = model.ShiftTypeid,
                        Createdate = DateTime.Now.Date,
                        Vendorid = adminlogin.Vendorid,
                    };

                    _context.Officeshifts.Add(data);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "ok";
                    return RedirectToAction("Officeshift", "Vendor");
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteOfficeshift(int id)
        {
            try
            {
                var data = _context.Officeshifts.Find(id);
                if (data != null)
                {
                    _context.Officeshifts.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("Officeshift");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteList:" + ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> VendorCategory(int id)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                List<VendorCategoryMaster> Category = await _ICrmrpo.GetVendorCategoryListByVendorId(vendorid)
;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.CategoryName = "";
                ViewBag.heading = "Add Category";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.VendorCategoryMasters.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.CategoryName = data.CategoryName;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Category";

                    }
                }

                return View(Category);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> VendorCategory(VendorCategoryMaster model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;

                bool check = await _ICrmrpo.AddVendorCategory(model, vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("VendorCategory");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("VendorCategory");
                    }
                }
                else
                {
                    return RedirectToAction("VendorCategory");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteVendorCategory(int id)
        {
            try
            {
                var dlt = _context.VendorCategoryMasters.Find(id);
                if (dlt != null)
                {
                    _context.VendorCategoryMasters.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("VendorCategory");

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("Vendor/VendorInvoice")]
        public async Task<IActionResult> VendorInvoice(int ID, bool Ismail)
        {
            try
            {
                if (ID != null)
                {
                    Invoice invoice = new Invoice();

                    invoice = await _ICrmrpo.GenerateInvoice(ID, Ismail);
                    if (invoice != null)
                    {
                        return View(invoice);
                    }
                    else
                    {
                        TempData["msg"] = "No data found";
                        return RedirectToAction("VendorList");
                    }
                }
                else
                {
                    return RedirectToAction("VendorList");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }


        }
        public async Task<IActionResult> UpdateVendorActiveStatus(int Id, bool Ismail)
        {
            var vendor = await _context.VendorRegistrations.FirstOrDefaultAsync(x => x.Id == Id);

            if (vendor == null)
            {
                TempData["msg"] = "Vendor not found!";
                return RedirectToAction("VendorList");
            }
            vendor.Isactive = !vendor.Isactive;
            await _context.SaveChangesAsync();
            if (vendor.Isactive == true)
            {
                var pdfResult = await VendorInvoiceDocPDF(vendor.Id, Ismail);
                if (pdfResult is JsonResult jsonResult && jsonResult.Value is IDictionary<string, object> result &&
                    result.TryGetValue("success", out var success) && (bool)success)
                {
                    TempData["msg"] = "Active status updated successfully and PDF sent!";
                }
                else
                {
                    TempData["msg"] = "Active status updated successfully, but failed to send PDF.";
                }
            }
            return RedirectToAction("VendorList");
        }
        public async Task<IActionResult> VendorInvoiceDocPDF(int? Id = 0, bool Ismail = false)
        {
            try
            {
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                string SlipURL = $"{schema}://{host}/Vendor/VendorInvoice?Id={Id}&Ismail={Ismail}";
                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "VendorInvoicefile");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }

                var result = _context.VendorRegistrations.FirstOrDefault(e => e.Id == Id);

                if (result != null)
                {
                    string sanitizedInvoiceNumber = string.Concat(result.InvoiceNumber.Split(Path.GetInvalidFileNameChars()));
                    string uniqueFileName = $"Invoice_{result.CompanyName}.pdf";
                    string filePath = Path.Combine(wwwRootPath, uniqueFileName);

                    doc.Save(filePath);
                    byte[] pdf = System.IO.File.ReadAllBytes(filePath);

                    result.Invoicefile = uniqueFileName;
                    _context.SaveChanges();

                    string emailSubject = $"Invoice for {result.CompanyName}";
                    string emailBody = $"Hello {result.CompanyName}";
                    await _emailService.SendVendorInvoiceEmailAsync(result.Email, emailSubject, emailBody, pdf, uniqueFileName, "application/pdf");

                    return Json(new { success = true, message = "Invoice has been sent successfully.", fileName = uniqueFileName });
                }
                else
                {
                    return Json(new { BadRequest = 404, message = "Data not found for the employee." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        [HttpGet, Route("Vendor/officeBreak")]
        public async Task<IActionResult> officeBreak(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                List<OfficeBreakDto> response = await _context.OfficeBreaks.Where(x => x.Vendorid == adminlogin.Vendorid)
               .Select(x => new OfficeBreakDto
               {
                   Id = x.Id,
                   Starttime = x.Starttime,
                   Endtime = x.Endtime,
                   Createdate = x.Createdate,
                   Breakstatus = _context.OfficeBreakstatuses.Where(a => a.Id == x.Breakstatusid).Select(status => status.Breakstatus).FirstOrDefault(),
                   ShiftType = _context.Officeshifts.Where(a => a.Id == x.Shiftid).Select(sas => sas.ShiftTypeid).FirstOrDefault(),
               }).ToListAsync();
                ViewBag.Breakstatus = await _context.OfficeBreakstatuses.Where(x => x.Vendorid == adminlogin.Vendorid).Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.Breakstatus
                }).ToListAsync();
                ViewBag.ShiftType = await _context.Officeshifts.Where(x => x.Vendorid == adminlogin.Vendorid).Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.ShiftTypeid
                }).ToListAsync();

                ViewBag.Starttime = "";
                ViewBag.Endtime = "";
                ViewBag.ShiftTypeid = "";
                ViewBag.Breakstatusid = "";
                ViewBag.Heading = "Add Office Break";
                ViewBag.BtnText = "SAVE";
                if (id != 0)
                {
                    var data = await _context.OfficeBreaks.Where(x => x.Id == id).FirstOrDefaultAsync();
                    ViewBag.id = data.Id;
                    ViewBag.Starttime = data.Starttime;
                    ViewBag.Endtime = data.Endtime;
                    ViewBag.ShiftTypeid = data.Shiftid;
                    ViewBag.Breakstatusid = data.Breakstatusid;
                    ViewBag.Heading = "Update Office Break";
                    ViewBag.BtnText = "UPDATE";
                }
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> officeBreak(OfficeBreakDto model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


                if (model == null)
                {
                    ModelState.Clear();
                    return View();
                }
                if (model.Id != 0)
                {
                    var existingData = await _context.OfficeBreaks.FindAsync(model.Id);
                    if (existingData != null)
                    {
                        existingData.Starttime = model.Starttime;
                        existingData.Endtime = model.Endtime;
                        existingData.Shiftid = Convert.ToInt16(model.ShiftType);
                        existingData.Breakstatusid = Convert.ToInt16(model.Breakstatus);
                        existingData.Createdate = DateTime.Now.Date;
                        existingData.Vendorid = adminlogin.Vendorid;

                        await _context.SaveChangesAsync();
                        TempData["Message"] = "updok";
                        return RedirectToAction("officeBreak", "Vendor");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record not found for update.");
                        return View(model);
                    }
                }
                else
                {
                    var data = new OfficeBreak()
                    {
                        Starttime = model.Starttime,
                        Endtime = model.Endtime,
                        Shiftid = Convert.ToInt16(model.ShiftType),
                        Breakstatusid = Convert.ToInt16(model.Breakstatus),
                        Createdate = DateTime.Now.Date,
                        Vendorid = adminlogin.Vendorid,
                    };

                    _context.OfficeBreaks.Add(data);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "ok";
                    return RedirectToAction("officeBreak", "Vendor");
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteofficeBreak(int id)
        {
            try
            {
                var data = _context.OfficeBreaks.Find(id);
                if (data != null)
                {
                    _context.OfficeBreaks.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("officeBreak");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteList:" + ex.Message);
            }
        }
        [HttpGet, Route("Vendor/officeBreakStatus")]
        public async Task<IActionResult> officeBreakStatus(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                List<OfficeBreakstatus> response = await _context.OfficeBreakstatuses.Where(x => x.Vendorid == adminlogin.Vendorid).ToListAsync();

                ViewBag.Breakstatus = "";
                ViewBag.Heading = "Add Office BreakStatus";
                ViewBag.BtnText = "SAVE";
                if (id != 0)
                {
                    var data = await _context.OfficeBreakstatuses.Where(x => x.Id == id).FirstOrDefaultAsync();
                    ViewBag.id = data.Id;
                    ViewBag.Breakstatus = data.Breakstatus;
                    ViewBag.Heading = "Update Office BreakStatus";
                    ViewBag.BtnText = "UPDATE";
                }
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> officeBreakStatus(OfficeBreakstatus model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


                if (model == null)
                {
                    ModelState.Clear();
                    return View();
                }
                if (model.Id != 0)
                {
                    var existingData = await _context.OfficeBreakstatuses.FindAsync(model.Id);
                    if (existingData != null)
                    {
                        existingData.Breakstatus = model.Breakstatus;
                        existingData.Createdate = DateTime.Now.Date;
                        existingData.Vendorid = adminlogin.Vendorid;

                        await _context.SaveChangesAsync();
                        TempData["Message"] = "updok";
                        return RedirectToAction("officeBreakStatus", "Vendor");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record not found for update.");
                        return View(model);
                    }
                }
                else
                {
                    var data = new OfficeBreakstatus()
                    {
                        Breakstatus = model.Breakstatus,
                        Createdate = DateTime.Now.Date,
                        Vendorid = adminlogin.Vendorid,
                    };

                    _context.OfficeBreakstatuses.Add(data);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "ok";
                    return RedirectToAction("officeBreakStatus", "Vendor");
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteofficeBreakStatus(int id)
        {
            try
            {
                var data = _context.OfficeBreakstatuses.Find(id);
                if (data != null)
                {
                    _context.OfficeBreakstatuses.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("officeBreakStatus");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteList:" + ex.Message);
            }
        }
        [HttpGet, Route("Vendor/EmpTasksassignment")]
        public async Task<IActionResult> EmpTasksassignment(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                List<EmpTasksassignDto> response = await _context.EmployeeTasks.Where(x => x.Vendorid == adminlogin.Vendorid).OrderByDescending(x => x.Id)
               .Select(x => new EmpTasksassignDto
               {
                   Id = x.Id,
                   Task = x.Task,
                   Tittle = x.Tittle,
                   Startdate = x.Startdate,
                   Enddate = x.Enddate,
                   Description = x.Description,
                   //Status = _context.TaskStatuses.Where(a => a.Id == x.Status).Select(status => status.StatusName).FirstOrDefault(),
                   TaskStatusId = x.Status,
                   EmployeeId = x.EmployeeId,
                   EmployeeName = _context.EmployeeRegistrations.Where(a => a.EmployeeId == x.EmployeeId).Select(status => status.FirstName).FirstOrDefault(),
               }).ToListAsync();
                ViewBag.TaskStatus = await _context.TaskStatuses.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.StatusName,
                }).ToListAsync();
                ViewBag.EmployeeId = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false).Select(D => new SelectListItem
                {
                    Value = D.EmployeeId.ToString(),
                    Text = $"{D.EmployeeId} {' '} ({D.FirstName})"

                }).ToList();

                ViewBag.Task = "";
                ViewBag.Tittle = "";
                ViewBag.Startdate = "";
                ViewBag.Enddate = "";
                ViewBag.Description = "";
                //ViewBag.Status = "";
                ViewBag.EmpId = "";
                ViewBag.Heading = "Add TaskAssign";
                ViewBag.BtnText = "SAVE";
                if (id != 0)
                {
                    var data = await _context.EmployeeTasks.Where(x => x.Id == id).FirstOrDefaultAsync();
                    ViewBag.id = data.Id;
                    ViewBag.Task = data.Task;
                    ViewBag.Tittle = data.Tittle;
                    ViewBag.Startdate = data.Startdate?.ToString("yyyy-MM-dd");
                    ViewBag.Enddate = data.Enddate?.ToString("yyyy-MM-dd");
                    ViewBag.Description = data.Description;
                    // ViewBag.Status = data.Status;
                    ViewBag.EmpId = data.EmployeeId;
                    ViewBag.Heading = "Update TaskAssign";
                    ViewBag.BtnText = "UPDATE";
                }
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> EmpTasksassignment(EmpTasksassignDto model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


                if (model == null)
                {
                    ModelState.Clear();
                    return View();
                }
                if (model.Id != 0)
                {
                    var existingData = await _context.EmployeeTasks.FindAsync(model.Id);
                    if (existingData != null)
                    {
                        existingData.Task = model.Task;
                        existingData.Tittle = model.Tittle;
                        existingData.Startdate = model.Startdate;
                        existingData.Enddate = model.Enddate;
                        existingData.Description = model.Description;
                        //existingData.Status = Convert.ToInt16(model.Status);
                        existingData.EmployeeId = model.EmployeeId;
                        existingData.Vendorid = adminlogin.Vendorid;

                        await _context.SaveChangesAsync();
                        TempData["Message"] = "updok";
                        return RedirectToAction("EmpTasksassignment", "Vendor");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record not found for update.");
                        return View(model);
                    }
                }
                else
                {
                    var data = new EmployeeTask()
                    {
                        Task = model.Task,
                        Tittle = model.Tittle,
                        Startdate = model.Startdate,
                        Enddate = model.Enddate,
                        Description = model.Description,
                        Status = 1,
                        EmployeeId = model.EmployeeId,
                        Vendorid = adminlogin.Vendorid,
                    };

                    _context.EmployeeTasks.Add(data);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "ok";
                    return RedirectToAction("EmpTasksassignment", "Vendor");
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteEmpTasksassignment(int id)
        {
            try
            {
                var data = _context.EmployeeTasks.Find(id);
                if (data != null)
                {
                    _context.EmployeeTasks.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("EmpTasksassignment");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteList:" + ex.Message);
            }
        }

        [HttpGet, Route("Vendor/EmpTaskslist")]

        public async Task<IActionResult> EmpTaskslist(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == Userid);

                EmployeeTaskModel model = new EmployeeTaskModel();
                model.EmpTaskList = await _ICrmrpo.GetSubTasks((int)adminlogin.Vendorid);

                ViewBag.SubTaskStatus = await _context.TaskStatuses.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.StatusName,
                }).ToListAsync();
                ViewBag.EmployeeId = await _context.EmployeeRegistrations
                    .Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false)
                    .Select(D => new SelectListItem
                    {
                        Value = D.EmployeeId.ToString(),
                        Text = $"{D.EmployeeId} {' '} ({D.FirstName})"

                    }).ToListAsync();



                ViewBag.Heading = "Add Sub Task";
                ViewBag.BtnText = "SAVE";

                if (id != 0)
                {

                    var existingDetail = await _context.EmployeeTasks.FirstOrDefaultAsync(x => x.Id == id);
                    if (existingDetail == null)
                    {
                        return NotFound();
                    }


                    var existingServices = await _context.EmployeeTasksLists
                        .Where(s => s.Emptaskid == existingDetail.Id) // Use Emptaskid to fetch related tasks
                        .ToListAsync();

                    ViewBag.id = existingDetail.Id;
                    ViewBag.Emptaskid = existingDetail.Id;
                    ViewBag.EmpId = existingDetail.EmployeeId;
                    ViewBag.Status = existingDetail.Status;
                    ViewBag.Heading = "Update Sub Task";
                    ViewBag.BtnText = "UPDATE";


                    model.TasksLists = existingServices.Select(s => new TasksList
                    {
                        TaskName = s.Taskname,
                        TaskStatusId = s.TaskStatus
                    }).ToList();
                }

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmpTaskslist(/*EmpTasknameDto*/ EmployeeTaskModel model, string[] Taskname, int[] TaskStatusId)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


                if (model == null)
                {
                    ModelState.Clear();
                    return View();
                }

                //if (model.Id != 0)
                //{


                //    // Remove existing services
                //    var existingServices = await _context.EmployeeTasksLists
                //        .Where(s => s.Emptaskid == model.Id)
                //        .ToListAsync();
                //    _context.EmployeeTasksLists.RemoveRange(existingServices);


                //    foreach (var taskName in Taskname)
                //    {

                //        if (!string.IsNullOrWhiteSpace(taskName))
                //        {
                //            EmployeeTasksList task = new EmployeeTasksList()
                //            {
                //                Taskname = taskName,
                //                Emptaskid = Convert.ToInt16(model.Emptask),
                //                EmployeeId = model.EmployeeId,
                //                TaskStatus = 1,
                //            };
                //            await _context.EmployeeTasksLists.AddAsync(task);
                //        }
                //    }

                //    await _context.SaveChangesAsync();
                //    TempData["Message"] = "updok";
                //    return RedirectToAction("EmpTaskslist", "Vendor");

                //}
                if (model.Id != 0)
                {

                    var existingTasks = await _context.EmployeeTasksLists
                        .Where(s => s.Emptaskid == model.Id)
                        .ToListAsync();


                    var tasksToRemove = existingTasks
                        .Where(t => !Taskname.Contains(t.Taskname))
                        .ToList();
                    _context.EmployeeTasksLists.RemoveRange(tasksToRemove);


                    foreach (var taskName in Taskname)
                    {
                        if (!string.IsNullOrWhiteSpace(taskName))
                        {

                            var existingTask = existingTasks.FirstOrDefault(t => t.Taskname == taskName);

                            if (existingTask != null)
                            {

                                existingTask.Taskname = taskName;
                                existingTask.EmployeeId = model.EmployeeId;
                                _context.EmployeeTasksLists.Update(existingTask);
                            }
                            else
                            {
                                EmployeeTasksList newTask = new EmployeeTasksList()
                                {
                                    Taskname = taskName,
                                    Emptaskid = Convert.ToInt16(model.Emptask),
                                    EmployeeId = model.EmployeeId,
                                    TaskStatus = 1, //pending
                                };
                                await _context.EmployeeTasksLists.AddAsync(newTask);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Message"] = "updok";
                    return RedirectToAction("EmpTaskslist", "Vendor");
                }
                else
                {
                    foreach (var taskName in Taskname)
                    {
                        if (!string.IsNullOrWhiteSpace(taskName))
                        {
                            var data = new EmployeeTasksList()
                            {
                                Emptaskid = Convert.ToInt16(model.Emptask),
                                EmployeeId = model.EmployeeId,
                                Taskname = taskName,
                                TaskStatus = 1,
                            };

                            _context.EmployeeTasksLists.Add(data);
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Message"] = "ok";
                    return RedirectToAction("EmpTaskslist", "Vendor");
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<IActionResult> DeleteEmpTaskslist(int id)
        {
            try
            {
                var data = _context.EmployeeTasksLists.Find(id);
                if (data != null)
                {
                    _context.EmployeeTasksLists.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("EmpTaskslist");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteList:" + ex.Message);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTasksByEmployeeId(string employeeId)
        {
            var tasks = await _context.EmployeeTasks
                .Where(x => x.EmployeeId == employeeId)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Task
                }).ToListAsync();

            return Json(tasks);
        }

        public async Task<IActionResult> AddBankDetail(int id)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;


                List<VendorBankDetail> data = await _ICrmrpo.GetVendorBankDetail(vendorid);

                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.AccountNumber = "";
                ViewBag.BankName = "";
                ViewBag.Ifsc = "";
                ViewBag.AccountHolderName = "";
                ViewBag.BranchAddress = "";
                ViewBag.heading = "Add Bank Detail";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var existdata = _context.VendorBankDetails.Find(iId);
                    if (existdata != null)
                    {
                        ViewBag.id = existdata.Id;
                        ViewBag.AccountNumber = existdata.AccountNumber;
                        ViewBag.BankName = existdata.BankName;
                        ViewBag.Ifsc = existdata.Ifsc;
                        ViewBag.AccountHolderName = existdata.AccountHolderName;
                        ViewBag.BranchAddress = existdata.BranchAddress;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Bank Detail";

                    }
                }
                return View(data);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddBankDetail(VendorBankDetail model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                var existingBankDetail = await _context.VendorBankDetails
            .Where(b => b.VendorId == vendorid)
            .FirstOrDefaultAsync();
                if (model.Id == 0)
                {
                    if (existingBankDetail != null)
                    {
                        TempData["Message"] = "Bank detail already exists.";
                        return RedirectToAction("AddBankDetail", "Vendor");
                    }
                }

                bool check = await _ICrmrpo.AddVendorBankDeatils(model, vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["Message"] = "ok";
                    }
                    else
                    {
                        TempData["Message"] = "updok";
                    }

                    return RedirectToAction("AddBankDetail", "Vendor");
                }
                else
                {
                    TempData["Message"] = "Failed!";
                    return View(model);
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        public async Task<IActionResult> DeleteVendorBankDetail(int id)
        {
            try
            {
                var dlt = _context.VendorBankDetails.Find(id);
                _context.Remove(dlt);
                _context.SaveChanges();
                TempData["Message"] = "dltok";
                return RedirectToAction("AddBankDetail", "Vendor");
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> Events(int id)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                List<OfficeEvent> events = _context.OfficeEvents.Where(e => e.Vendorid == vendorid).OrderBy(x => x.Date).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Tittle = "";
                ViewBag.Subtittle = "";
                ViewBag.description = "";
                ViewBag.Date = "";
                ViewBag.heading = "Add Event";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.OfficeEvents.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Tittle;
                        ViewBag.Subtittle = data.Subtittle;
                        ViewBag.description = data.Description;
                        ViewBag.Date = data.Date.Value.ToString("yyyy-MM-dd");
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Event";

                    }
                }

                return View(events);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Events(OfficeEvent model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;

                bool check = await _ICrmrpo.AddOfficeEvents(model, vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("Events");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("Events");
                    }
                }
                else
                {
                    return RedirectToAction("Events");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> DeleteOfficeEvent(int id)
        {
            try
            {
                var dlt = _context.OfficeEvents.Find(id);
                _context.Remove(dlt);
                _context.SaveChanges();
                TempData["msg"] = "dltok";
                return RedirectToAction("Events");
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateTaskStatus(int Taskstatusid, int Id)
        {
            var emptaslist = await _context.EmployeeTasksLists.Where(x => x.Emptaskid == Id).ToListAsync();
            var emp = _context.EmployeeTasks.Where(x => x.Id == Id).FirstOrDefault();
            if (emp == null)
            {
                return Json(new { success = false, message = "Task not found." });
            }
            emp.Status = Taskstatusid;
            if (Taskstatusid == 3)
            {
                foreach (var sub in emptaslist)
                {
                    sub.TaskStatus = Taskstatusid;
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task status updated successfully!" });
        }

        [HttpGet, Route("Vendor/ApprovedLeaveApply")]
        public async Task<IActionResult> ApprovedLeaveApply(int? id)
        {
            try
            {
                int iId = id ?? 0;

                if (HttpContext.Session.GetString("UserName") != null)
                {
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                    ViewBag.approvestatus = await _context.Approvalmasters.Select(w => new SelectListItem
                    {
                        Value = w.Id.ToString(),
                        Text = w.Status,
                    }).ToListAsync();
                    var Approvedbankdetail = await _ICrmrpo.GetLeaveapplydetailList(adminlogin.Vendorid);
                    if (Approvedbankdetail != null)
                    {
                        return View(Approvedbankdetail);

                    }
                    else
                    {
                        return View();

                    }
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Message : " + ex.Message, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLeaveApplyStatus(int Id, int Isapprove, DateTime StartDate, DateTime EndDate)
        {
            var leave = await _context.ApplyLeaveNews.FirstOrDefaultAsync(x => x.Id == Id);
            if (leave == null)
            {
                return Json(new { success = false, message = "Data not found!" });
            }
            leave.Isapprove = Isapprove;
            await _context.SaveChangesAsync();

            var empinfo = await _context.EmployeeRegistrations.FirstOrDefaultAsync(x => x.EmployeeId == leave.UserId);
            var emppersonalinfo = await _context.EmployeePersonalDetails.FirstOrDefaultAsync(x => x.EmpRegId == leave.UserId);

            string subject;
            string emailBody;

            if (leave.Isapprove == 2)
            {
                subject = "Leave Approval Accepted";
                emailBody = $"Dear {empinfo.FirstName} {empinfo.MiddleName} {empinfo.LastName},Your leave application has been approved.";

                var TypeOfLeave = await _context.Leavemasters
                    .FirstOrDefaultAsync(x => x.LeavetypeId == leave.TypeOfLeaveId && x.EmpId == leave.UserId);

                if (TypeOfLeave != null)
                {
                    decimal totalLeave = leave?.CountLeave ?? 0m;
                    decimal currentLeaveBalance = TypeOfLeave.Value ?? 0m;

                    decimal deductedLeave = Math.Min(totalLeave, currentLeaveBalance);
                    TypeOfLeave.Value -= deductedLeave;
                    leave.CountLeave = deductedLeave;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    TempData["msg"] = "Leave type not found for the employee.";
                    return RedirectToAction("ApprovedLeaveApply");
                }
            }
            else if (leave.Isapprove == 1)
            {
                subject = "Leave Approval Status Update";
                emailBody = $"Dear {empinfo.FirstName} {empinfo.MiddleName} {empinfo.LastName},We regret to inform you that your leave application is still pending. Please reapply with your desired {StartDate.Date} and {EndDate.Date} dates for processing.";
            }
            else
            {
                subject = "Leave Approval Rejected";
                emailBody = $"Dear {empinfo.FirstName} {empinfo.MiddleName} {empinfo.LastName},We regret to inform you that your leave application has been rejected. Please contact HR for further details.";
            }

            var status = await _context.Approvalmasters.Where(x => x.Id == leave.Isapprove).Select(x => x.Status).FirstOrDefaultAsync();

            try
            {
                await _emailService.SendEmpLeaveApprovalEmailAsync(
                    emppersonalinfo.PersonalEmailAddress,
                    empinfo.FirstName,
                    empinfo.MiddleName,
                    empinfo.LastName,
                    subject,
                    emailBody, (int)empinfo.Vendorid
                );
                TempData["msg"] = GetApprovalMessage(status);
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { success = true, message = GetApprovalMessage(status) });
        }

        private string GetApprovalMessage(string status)
        {
            return status switch
            {
                "Complete" => "Approval status updated successfully with a Complete !",
                "Disapprove" => "Approval status updated successfully with a Disapprove!",
                _ => "Approval status updated successfully with a partial status!"
            };
        }

        public async Task<IActionResult> EmployeeEpf(int id)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {

                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                    var epflist = _context.EmployeeEpfPayrollInfos.Where(e => e.Vendorid == adminlogin.Vendorid).OrderByDescending(e => e.Id).ToList();
                    ViewBag.EmployeeItem = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(D => new SelectListItem
                    {
                        Value = D.EmployeeId.ToString(),
                        Text = D.EmployeeId

                    }).ToList();
                    int iId = (int)(id == null ? 0 : id);

                    ViewBag.id = "";
                    ViewBag.EPFPercentage = "";
                    ViewBag.Heading = "Add Employee EPF";
                    ViewBag.BtnText = "SAVE";

                    if (iId != null && iId != 0)
                    {
                        var data = _context.EmployeeEpfPayrollInfos.Find(iId);
                        if (data != null)
                        {
                            ViewBag.id = data.Id;
                            ViewBag.EPFPercentage = data.Epfpercentage;
                            ViewBag.BtnText = "UPDATE";
                            ViewBag.Heading = "Update EPF";

                        }
                    }
                    return View(epflist);
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
        public async Task<IActionResult> EmployeeEpf(EmployeeEpfPayrollInfo model)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var checkexist = _context.EmployeeEpfPayrollInfos.Where(e => e.Id == model.Id).FirstOrDefault();
                bool check = await _ICrmrpo.AddEmployeeEpf(model, (int)adminlogin.Vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        if (checkexist != null)
                        {
                            TempData["Message"] = $"An EPF record  already exists.";
                            return RedirectToAction("EmployeeEpf");
                        }

                        TempData["Message"] = "ok";
                    }
                    else
                    {
                        TempData["Message"] = "updok";
                    }

                    return RedirectToAction("EmployeeEpf");
                }
                else
                {
                    TempData["Message"] = "Failed.";
                    return RedirectToAction("EmployeeEpf");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //[HttpGet]
        //public JsonResult GetEmpEpfNumber(string employeeId)
        //{

        //    var epflist = _context.EmployeeEpfPayrollInfos.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
        //    var employee = _context.EmployeeBankDetails.Where(e => e.EmpId == employeeId)
        //        .Select(e => new
        //        {
        //            EPFNumber = e.EpfNumber
        //        })
        //        .FirstOrDefault();


        //    if (employee != null)
        //    {
        //        return Json(employee);
        //    }


        //    return Json(null);
        //}
        //public JsonResult GetVendorEfp()
        //{

        //    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
        //    var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
        //    var epf = _context.EmployeerEpfs.Where(e => e.AdminLoginId == adminlogin.Id && e.DeductionCycle== "EPF")
        //        .Select(e => new
        //        {
        //            EpfPercentage = e.EpfNumber
        //        })
        //        .FirstOrDefault();


        //    if (epf != null)
        //    {
        //        return Json(epf);
        //    }


        //    return Json(null);
        //}
        public async Task<IActionResult> DeleteEmpEPF(int id)
        {
            try
            {
                var dlt = _context.EmployeeEpfPayrollInfos.Find(id);
                _context.Remove(dlt);
                _context.SaveChanges();
                TempData["Message"] = "dltok";
                return RedirectToAction("EmployeeEpf");
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> EmployeeEsic(int id)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {

                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                    var epflist = _context.EmployeeEsicPayrollInfos.Where(e => e.Vendorid == adminlogin.Vendorid).OrderByDescending(e => e.Id).ToList();
                    //var epfEmployeeList = (from salary in _context.EmployeeSalaryDetails
                    //                       join employee in _context.EmployeeRegistrations
                    //                       on salary.EmployeeId equals employee.EmployeeId
                    //                       where employee.Vendorid == adminlogin.Vendorid && salary.Gross < 21000
                    //                       orderby salary.Id descending
                    //                       select new
                    //                       {
                    //                           Value = employee.EmployeeId.ToString(),
                    //                           Text = employee.EmployeeId
                    //                       }).ToList();


                    //ViewBag.EmployeeItem = epfEmployeeList;

                    //ViewBag.EmployeeItem = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(D => new SelectListItem
                    //{
                    //    Value = D.EmployeeId.ToString(),
                    //    Text = D.EmployeeId

                    //}).ToList();
                    int iId = (int)(id == null ? 0 : id);

                    ViewBag.id = "";
                    ViewBag.ESICPercentage = "";
                    ViewBag.EsicAmount = "";
                    ViewBag.Heading = "Add Employee ESIC";
                    ViewBag.BtnText = "SAVE";
                    if (iId != null && iId != 0)
                    {
                        var data = _context.EmployeeEsicPayrollInfos.Find(iId);
                        if (data != null)
                        {
                            ViewBag.id = data.Id;
                            ViewBag.ESICPercentage = data.Esicpercentage;
                            ViewBag.EsicAmount = data.EsicAmount;
                            ViewBag.BtnText = "UPDATE";
                            ViewBag.Heading = "Update ESIC";

                        }
                    }


                    return View(epflist);
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
        public async Task<IActionResult> EmployeeEsic(EmployeeEsicPayrollInfo model)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var checkexist = _context.EmployeeEsicPayrollInfos.Where(e => e.Id == model.Id).FirstOrDefault();

                bool check = await _ICrmrpo.AddEmployeeEsic(model, (int)adminlogin.Vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        if (checkexist != null)
                        {
                            TempData["Message"] = $"An ESIC record  already exists.";
                            return RedirectToAction("EmployeeEsic");
                        }
                        TempData["Message"] = "ok";
                    }
                    else
                    {
                        TempData["Message"] = "updok";
                    }

                    return RedirectToAction("EmployeeEsic");
                }
                else
                {
                    TempData["Message"] = "Failed.";
                    return RedirectToAction("EmployeeEsic");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public JsonResult GetVendorEsic()
        {

            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
            var epf = _context.EmployeerEpfs.Where(e => e.AdminLoginId == adminlogin.Id && e.DeductionCycle == "ESIC")
                .Select(e => new
                {
                    EsicPercentage = e.EpfNumber
                })
                .FirstOrDefault();


            if (epf != null)
            {
                return Json(epf);
            }


            return Json(null);
        }
        public async Task<IActionResult> DeleteEmpEsic(int id)
        {
            try
            {
                var dlt = _context.EmployeeEsicPayrollInfos.Find(id);
                _context.Remove(dlt);
                _context.SaveChanges();
                TempData["Message"] = "dltok";
                return RedirectToAction("EmployeeEsic");
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> Aboutcompany(int id)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                List<Aboutcompany> company = _context.Aboutcompanies.Where(e => e.Vendorid == vendorid).OrderBy(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Companylink = "";
                ViewBag.heading = "Add About Company";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.Aboutcompanies.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Companylink = data.Companylink;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update About Company";

                    }
                }

                return View(company);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Aboutcompany(Aboutcompany model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                var existingabout = await _context.Aboutcompanies
                .Where(b => b.Vendorid == vendorid)
                  .FirstOrDefaultAsync();
                if (model.Id == 0)
                {
                    if (existingabout != null)
                    {
                        TempData["msg"] = "Already exists.";
                        return RedirectToAction("Aboutcompany");
                    }
                }
                bool check = await _ICrmrpo.Addaddcompany(model, vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("Aboutcompany");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("Aboutcompany");
                    }
                }
                else
                {
                    return RedirectToAction("Aboutcompany");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> DeleteAboutcompany(int id)
        {
            try
            {
                var dlt = _context.Aboutcompanies.Find(id);
                _context.Remove(dlt);
                _context.SaveChanges();
                TempData["msg"] = "dltok";
                return RedirectToAction("Aboutcompany");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSubTasksByEmployeeId(string employeeId)
        {
            var tasks = await _context.EmployeeTasksLists
                .Where(x => x.EmployeeId == employeeId)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Taskname
                }).ToListAsync();

            return Json(tasks);
        }
        [HttpPost]
        public JsonResult UpdateSubTaskStatus(int Taskstatusid, int Id)
        {
            var emp = _context.EmployeeTasksLists.Where(x => x.Id == Id).FirstOrDefault();
            emp.TaskStatus = Taskstatusid;
            _context.SaveChanges();
            return Json(new { success = true, message = "Task status updated successfully!" });
        }
        [HttpGet]
        public async Task<IActionResult> EventsScheduler(int id)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                EventsmeetSchedulerDto es = new EventsmeetSchedulerDto();
                es.Scheduler = _context.EventsmeetSchedulers.Where(e => e.Vendorid == vendorid).OrderBy(x => x.Createddate).ToList();
                ViewBag.EmployeeItem = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(D => new SelectListItem
                {
                    Value = D.EmployeeId.ToString(),
                    Text = $"{D.EmployeeId} {' '} ({D.FirstName})"

                }).ToList();
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Tittle = "";
                ViewBag.description = "";
                ViewBag.ScheduleDate = "";
                ViewBag.EmployeeId = "";
                ViewBag.IsEventsmeet = "";
                ViewBag.IsActive = "";
                ViewBag.Time = "";
                ViewBag.heading = "Add Event Schedule";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.EventsmeetSchedulers.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Tittle;
                        ViewBag.EmployeeId = data.EmployeeId;
                        ViewBag.description = data.Description;
                        ViewBag.IsEventsmeet = data.IsEventsmeet;
                        ViewBag.IsActive = data.IsActive;
                        ViewBag.Time = data.Time;
                        ViewBag.ScheduleDate = data.ScheduleDate.Value.ToString("yyyy-MM-dd");
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Event Schedule";
                    }
                }
                return View(es);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> EventsScheduler(EventsmeetSchedulerDto model)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                string time = Request.Form["Time"];
                string period = Request.Form["Period"];

                model.Time = time + " " + period;
                bool check = await _ICrmrpo.AddEventsScheduler(model, vendorid);
                if (check)
                {
                    if (model.EmployeeId != null && model.EmployeeId.Length > 0)
                    {
                        foreach (var empId in model.EmployeeId)
                        {
                            var employee = await _context.EmployeeRegistrations.Where(e => e.EmployeeId == empId && e.Vendorid == vendorid).FirstOrDefaultAsync();
                            if (employee != null)
                            {
                                string emailBody = $"<p>Dear {employee.FirstName} {employee.LastName},</p>" +
                                              $"<p><strong>Title:</strong> {model.Tittle}</p>" +
                                              $"{model.Description}" +
                                              $"<p><strong>Scheduled On:</strong> {model.ScheduleDate?.ToString("dd MMM yyyy")}</p>" +
                                              $"<p>Thank you.</p>";


                                await _emailService.SendMeetEmailAsync(employee.WorkEmail, employee.FirstName, employee.MiddleName, employee.LastName, emailBody, (int)adminlogin.Vendorid);
                            }
                        }
                    }
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("EventsScheduleList");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("EventsScheduleList");
                    }
                }
                else
                {
                    return RedirectToAction("EventsScheduler");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<IActionResult> DeleteEventScheduler(int id)
        {
            try
            {
                var dlt = _context.EventsmeetSchedulers.Find(id);
                _context.Remove(dlt);
                _context.SaveChanges();
                TempData["msg"] = "dltok";
                return RedirectToAction("EventsScheduleList");
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> EventsScheduleList()
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                EventsmeetSchedulerDto es = new EventsmeetSchedulerDto();
                es.Scheduler = _context.EventsmeetSchedulers.Where(e => e.Vendorid == vendorid).OrderBy(x => x.Createddate).ToList();

                return View(es);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> EmployeeAttendanceList(string EmpId, int? month, int? year)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

                if (adminLogin == null)
                {
                    return RedirectToAction("Login", "Admin");
                }

                // Base query
                var attendanceQuery = from employee in _context.EmployeeRegistrations
                                      join record in _context.EmployeeCheckInRecords
                                      on employee.EmployeeId equals record.EmpId
                                      where employee.Vendorid == adminLogin.Vendorid
                                            && employee.IsDeleted == false
                                            && (string.IsNullOrEmpty(EmpId) || record.EmpId == EmpId) 
                                            && (!month.HasValue || record.CurrentDate.Value.Month == month.Value) 
                                            && (!year.HasValue || record.CurrentDate.Value.Year == year.Value) 
                                      orderby record.CurrentDate descending
                                      select new EmployeeAttendanceDto
                                      {
                                          EmpId = record.EmpId,
                                          EmployeeName = $"{employee.FirstName} " +
                                                         (string.IsNullOrEmpty(employee.MiddleName) ? "" : employee.MiddleName + " ") +
                                                         employee.LastName,
                                          CheckIntime = record.CheckIntime.HasValue
                                              ? record.CheckIntime.Value.ToString("hh:mm tt")
                                              : "N/A",
                                          CheckOuttime = record.CheckOuttime.HasValue
                                              ? record.CheckOuttime.Value.ToString("hh:mm tt")
                                              : "N/A",
                                          CurrentDate = record.CurrentDate.HasValue
                                              ? record.CurrentDate.Value.ToString("dd-MMM-yyyy")
                                              : "N/A",
                                          ShiftId = record.ShiftId,
                                      };

                // Fetch filtered results
                var attendanceRecords = await attendanceQuery.Where(x=>x.EmpId == EmpId).ToListAsync();

                // Fetch shift details & calculate hours
                foreach (var attendanceRecord in attendanceRecords)
                {
                    var shiftDetails = await _context.EmployeeRegistrations
                        .Where(x => x.EmployeeId == attendanceRecord.EmpId)
                        .Join(_context.Officeshifts,
                            emp => emp.OfficeshiftTypeid,
                            ob => ob.Id,
                            (emp, ob) => new { ob.Starttime, ob.Endtime })
                        .FirstOrDefaultAsync();

                    if (shiftDetails != null && DateTime.TryParse(shiftDetails.Starttime, out DateTime startDateTime) &&
                        DateTime.TryParse(shiftDetails.Endtime, out DateTime endDateTime))
                    {
                        TimeSpan startTime = startDateTime.TimeOfDay;
                        TimeSpan endTime = endDateTime.TimeOfDay;
                        double totalHours = (endTime - startTime).TotalHours;
                        attendanceRecord.maxHour = FormatHours(totalHours);
                    }

                    if (DateTime.TryParse(attendanceRecord.CurrentDate, out DateTime parsedDate))
                    {
                        string workingHours = await TotalWorkingHours(attendanceRecord.EmpId, parsedDate);
                        attendanceRecord.Workinghour = workingHours;
                    }
                    else
                    {
                        attendanceRecord.Workinghour = "Invalid Date";
                    }
                }

                var attendanceDto = new EmployeeAttendanceDto
                {
                    detail = attendanceRecords
                };

                ViewBag.month = month;
                ViewBag.year = year;
                return View(attendanceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //[HttpGet]
        //public JsonResult CheckEmployeeExists(string employeeId)
        //{
        //    var epfInfo = _context.EmployeeEpfPayrollInfos.FirstOrDefault(x => x.EmployeeId == employeeId);
        //    if (epfInfo != null)
        //    {
        //        return Json(new { exists = true, message = "This Employee already has an EPF record." });
        //    }

        //    return Json(new { exists = false });
        //}
        //[HttpGet]
        //public JsonResult CheckEmployeeEsicExists(string employeeId)
        //{
        //    var epfInfo = _context.EmployeeEsicPayrollInfos.FirstOrDefault(x => x.EmployeeId == employeeId);
        //    if (epfInfo != null)
        //    {
        //        return Json(new { exists = true, message = "This Employee already has an Esic record." });
        //    }

        //    return Json(new { exists = false });
        //}

        public async Task<IActionResult> EmployeeApplyOvertimeList()
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

                if (adminLogin == null)
                    return StatusCode(404, "Admin login not found");

                var employeeList = await _context.EmployeeRegistrations
                                                 .Where(e => e.Vendorid == adminLogin.Vendorid)
                                                 .OrderByDescending(x => x.EmployeeId)
                                                 .ToListAsync();

                var employeeIds = employeeList.Select(e => e.EmployeeId).ToList();
                var attendanceEntries = await _context.EmployeeOvertimes
                                                      .Where(e => employeeIds.Contains(e.EmployeeId))
                                                      .ToListAsync();

                var overtimeRecords = new List<EmployeeOvertimeDto>();

                foreach (var record in attendanceEntries)
                {
                    var employee = employeeList.FirstOrDefault(e => e.EmployeeId == record.EmployeeId);
                    if (employee == null) continue;

                    string overtimeHours = "N/A";
                    if (record.StartTime.HasValue && record.EndTime.HasValue)
                    {
                        var duration = record.EndTime.Value - record.StartTime.Value;
                        overtimeHours = $"{(int)duration.TotalHours}h{duration.Minutes}m";
                    }
                    var currentDate = DateTime.Now.Date;

                    string startOverTime = "N/A";

                    overtimeRecords.Add(new EmployeeOvertimeDto
                    {
                        Id = record.Id,
                        EmployeeId = record.EmployeeId,
                        EmployeeName = $"{employee.FirstName} " +
                                       (!string.IsNullOrEmpty(employee.MiddleName) ? employee.MiddleName + " " : "") +
                                       employee.LastName,
                        EndTime = record.EndTime?.ToString("dd-MMM-yyyy HH:mm") ?? "N/A",
                        ApprovalDate = record.ApprovalDate?.ToString("dd-MMM-yyyy") ?? "N/A",
                        TotalOvertimeHours = overtimeHours,
                        StartTime = await StartOverTime(employee.EmployeeId, (int)employee.OfficeshiftTypeid, currentDate),
                        Approved = record.Approved
                    });
                }

                return View(overtimeRecords.OrderByDescending(r => r.ApprovalDate).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        public async Task<IActionResult> UpdateApplyOvertimeStatus(int Id)
        {
            var Overtimes = await _context.EmployeeOvertimes.FirstOrDefaultAsync(x => x.Id == Id);
            if (Overtimes == null)
            {
                TempData["msg"] = "Data not found!";
                return RedirectToAction("EmployeeApplyOvertimeList");
            }

            var empinfo = await _context.EmployeeRegistrations.FirstOrDefaultAsync(x => x.EmployeeId == Overtimes.EmployeeId);
            if (empinfo == null)
            {
                TempData["msg"] = "Employee information not found!";
                return RedirectToAction("EmployeeApplyOvertimeList");
            }

            var emppersonalinfo = await _context.EmployeePersonalDetails.FirstOrDefaultAsync(x => x.EmpRegId == Overtimes.EmployeeId);
            if (emppersonalinfo == null)
            {
                TempData["msg"] = "Employee personal information not found!";
                return RedirectToAction("EmployeeApplyOvertimeList");
            }

            var currentDate = DateTime.Now.Date;
            var checkInData = await _context.EmployeeCheckInRecords
                .Where(g => g.EmpId == empinfo.EmployeeId && g.CheckIntime.Value.Date == currentDate)
                .OrderByDescending(g => g.Id)
                .FirstOrDefaultAsync();

            if (checkInData == null)
            {
                TempData["msg"] = "Check-in data not found for today!";
                return RedirectToAction("EmployeeApplyOvertimeList");
            }

            var officeHour = await _context.Officeshifts
                .Where(h => h.Id == (int)empinfo.OfficeshiftTypeid)
                .FirstOrDefaultAsync();

            if (officeHour == null)
            {
                TempData["msg"] = "Office shift data not found!";
                return RedirectToAction("EmployeeApplyOvertimeList");
            }

            TimeSpan timeDifference1 = DateTime.Parse(officeHour.Endtime) - DateTime.Parse(officeHour.Starttime);
            DateTime checkoutHour = checkInData.CheckIntime.Value.Add(timeDifference1);

            Overtimes.StartTime = checkoutHour;
            Overtimes.Approved = !Overtimes.Approved;
            await _context.SaveChangesAsync();

            string subject = (bool)Overtimes.Approved ? "Overtime Application Approved" : "Overtime Application Rejected";
            string emailBody = (bool)Overtimes.Approved
                ? $"Dear {empinfo.FirstName} {empinfo.MiddleName} {empinfo.LastName},\n\nCongratulations! Your overtime application has been approved. Thank you for your dedication and hard work."
                : $"Dear {empinfo.FirstName} {empinfo.MiddleName} {empinfo.LastName},\n\nUnfortunately, your overtime application has been rejected. For more information, please reach out to your supervisor or HR department.";

            try
            {
                await _emailService.SendEmpLeaveApprovalEmailAsync(emppersonalinfo.PersonalEmailAddress, empinfo.FirstName, empinfo.MiddleName, empinfo.LastName, subject, emailBody, (int)empinfo.Vendorid);
                TempData["msg"] = (bool)Overtimes.Approved
                    ? "Approval status updated successfully, and approval email sent!"
                    : "Approval status updated successfully, and rejection email sent!";
            }
            catch (Exception ex)
            {
                TempData["msg"] = (bool)Overtimes.Approved
                    ? "Approval status updated successfully, but failed to send approval email."
                    : "Approval status updated successfully, but failed to send rejection email.";
            }

            return RedirectToAction("EmployeeApplyOvertimeList");
        }

        private async Task<string> StartOverTime(string employeeId, int officeShiftId, DateTime currentDate)
        {
            var checkInData = await _context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId && g.CheckIntime.Value.Date == currentDate.Date)
                .OrderByDescending(g => g.Id)
            .FirstOrDefaultAsync();
            var officeHour = await _context.Officeshifts
                 .Where(h => h.Id == officeShiftId).FirstOrDefaultAsync();

            TimeSpan timeDifference1 = DateTime.Parse(officeHour.Endtime) - DateTime.Parse(officeHour.Starttime);

            if (checkInData != null && checkInData.CheckIntime.HasValue)
            {

                DateTime checkoutHour = checkInData.CheckIntime.Value.Add(timeDifference1);


                return checkoutHour.ToString("hh:mm tt");
            }
            else
            {
                return "N/A";
            }
        }
        public async Task<IActionResult> LeaveTypemaster(int id)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                List<LeaveType> leave = _context.LeaveTypes.Where(e => e.Vendorid == vendorid).OrderBy(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Leavetype = "";
                ViewBag.IsActive = "";
                ViewBag.Leavevalue = "";
                ViewBag.heading = "Add Leave Type";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.LeaveTypes.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Leavetype = data.Leavetype1;
                        ViewBag.Leavevalue = data.Leavevalue;
                        ViewBag.IsActive = data.Isactive;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Leave Type";

                    }
                }

                return View(leave);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> LeaveTypemaster(LeaveType model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;

                bool check = await _ICrmrpo.AddAndUpdateLeaveTypemaster(model, vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("LeaveTypemaster");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("LeaveTypemaster");
                    }
                }
                else
                {
                    return RedirectToAction("LeaveTypemaster");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> DeleteLeaveType(int id)
        {
            try
            {
                var dlt = _context.LeaveTypes.Find(id);
                _context.Remove(dlt);
                _context.SaveChanges();
                TempData["msg"] = "dltok";
                return RedirectToAction("LeaveTypemaster");
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> EmployeeBreakList(string EmpId, int? month, int? year)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

                if (adminLogin == null)
                {
                    return NotFound("Admin not found.");
                }

                var empDetails = await _context.EmployeeRegistrations
                    .Where(x => x.Vendorid == adminLogin.Vendorid && x.IsDeleted == false)
                    .ToListAsync();

                if (!empDetails.Any())
                {
                    return NotFound("No employees found for this vendor.");
                }

                var loginActivities = new List<EmployeeBreakDto>();

                foreach (var employee in empDetails)
                {
                    var breakRecordsIn = await _context.EmployeeCheckIns
                        .Where(g => g.EmployeeId == employee.EmployeeId && g.Breakin == true && g.CheckInTime.HasValue)
                        .Where(g => (!month.HasValue || g.Currentdate.Value.Month == month.Value) &&
                                    (!year.HasValue || g.Currentdate.Value.Year == year.Value))
                        .OrderByDescending(g => g.Currentdate)
                        .ToListAsync();

                    var breakRecordsOut = await _context.EmployeeCheckIns
                        .Where(g => g.EmployeeId == employee.EmployeeId && g.Breakout == true && g.CheckInTime.HasValue)
                        .Where(g => (!month.HasValue || g.Currentdate.Value.Month == month.Value) &&
                                    (!year.HasValue || g.Currentdate.Value.Year == year.Value))
                        .OrderByDescending(g => g.Currentdate)
                        .ToListAsync();

                    int inIndex = 0, outIndex = 0;
                    ViewBag.month = month;
                    ViewBag.year = year;

                    while (inIndex < breakRecordsIn.Count)
                    {
                        var breakInRecord = breakRecordsIn[inIndex];
                        var loginActivity = new EmployeeBreakDto
                        {
                            EmpId = employee.EmployeeId.ToString(),
                            EmployeeName = $"{employee.FirstName} " +
                                           (string.IsNullOrEmpty(employee.MiddleName) ? "" : employee.MiddleName + " ") +
                                           employee.LastName,
                            BreakIntime = breakInRecord.CheckInTime.HasValue
                                ? breakInRecord.CheckInTime.Value.ToString("hh:mm tt")
                                : "N/A",
                            BreakOuttime = "N/A",
                            CurrentDate = breakInRecord.Currentdate.HasValue
                                ? breakInRecord.Currentdate.Value.ToString("dd-MMM-yyyy")
                                : "N/A",
                            Breakhour = "N/A"
                        };

                        if (outIndex < breakRecordsOut.Count)
                        {
                            var breakOutRecord = breakRecordsOut[outIndex];

                            if (breakOutRecord.Currentdate.HasValue &&
                                breakOutRecord.Currentdate.Value.Date == breakInRecord.Currentdate.Value.Date &&
                                breakOutRecord.CheckInTime > breakInRecord.CheckInTime)
                            {
                                loginActivity.BreakOuttime = breakOutRecord.CheckInTime.Value.ToString("hh:mm tt");

                                var breakDuration = breakOutRecord.CheckInTime.Value - breakInRecord.CheckInTime.Value;
                                loginActivity.Breakhour = $"{(int)breakDuration.TotalHours}h {breakDuration.Minutes}m";

                                outIndex++;
                            }
                        }

                        loginActivities.Add(loginActivity);
                        inIndex++;
                    }

                    // **Ensure Employee Appears Even if No Break Records Exist**
                    if (!breakRecordsIn.Any() && !breakRecordsOut.Any())
                    {
                        loginActivities.Add(new EmployeeBreakDto
                        {
                            EmpId = employee.EmployeeId.ToString(),
                            EmployeeName = $"{employee.FirstName} " +
                                           (string.IsNullOrEmpty(employee.MiddleName) ? "" : employee.MiddleName + " ") +
                                           employee.LastName,
                            BreakIntime = "N/A",
                            BreakOuttime = "N/A",
                            CurrentDate = "N/A",
                            Breakhour = "N/A"
                        });
                    }
                }

                // **Apply Employee ID Filter**
                if (!string.IsNullOrEmpty(EmpId))
                {
                    var filteredActivities = loginActivities.Where(x => x.EmpId == EmpId).ToList();

                    // **Ensure Employee Appears Even If No Break Data Exists**
                    if (!filteredActivities.Any())
                    {
                        var employee = empDetails.FirstOrDefault(x => x.EmployeeId == EmpId);
                        if (employee != null)
                        {
                            filteredActivities.Add(new EmployeeBreakDto
                            {
                                EmpId = employee.EmployeeId.ToString(),
                                EmployeeName = $"{employee.FirstName} " +
                                               (string.IsNullOrEmpty(employee.MiddleName) ? "" : employee.MiddleName + " ") +
                                               employee.LastName,
                                BreakIntime = "N/A",
                                BreakOuttime = "N/A",
                                CurrentDate = "N/A",
                                Breakhour = "N/A"
                            });
                        }
                    }

                    loginActivities = filteredActivities;
                }

                var attendanceDto = new EmployeeBreakDto
                {
                    Breakdetail = loginActivities
                };

                return View(attendanceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private async Task<string> TotalWorkingHours(string employeeId, DateTime currentDate)
        {
            var shiftDetails = await _context.EmployeeRegistrations
                .Where(x => x.EmployeeId == employeeId)
                .Join(_context.OfficeBreaks,
                    emp => emp.OfficeshiftTypeid,
                    ob => ob.Shiftid,
                    (emp, ob) => new { ob.Starttime, ob.Endtime })
                .FirstOrDefaultAsync();

            double breakPeriodThreshold = 0;

            if (shiftDetails != null && DateTime.TryParse(shiftDetails.Starttime, out DateTime startDateTime) &&
                DateTime.TryParse(shiftDetails.Endtime, out DateTime endDateTime))
            {
                TimeSpan startTime = startDateTime.TimeOfDay;
                TimeSpan endTime = endDateTime.TimeOfDay;
                breakPeriodThreshold = (endTime - startTime).TotalHours;
            }

            var checkIns = await _context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId && g.CurrentDate.HasValue && g.CurrentDate.Value.Date == currentDate.Date)
                .OrderBy(g => g.CheckIntime)
                .ToListAsync();

            double totalHours = 0;
            double totalBreakHours = 0;
            DateTime? lastBreakInTime = null;
            var latestCheckRecord = await _context.EmployeeCheckIns
               .Where(x => x.EmployeeId == employeeId && x.Currentdate.HasValue && x.Currentdate.Value.Date == currentDate.Date)
               .OrderByDescending(x => x.Id)
               .FirstOrDefaultAsync();
            foreach (var checkInRecord in checkIns)
            {
                if (checkInRecord.CheckIntime.HasValue)
                {
                    if (currentDate.Date != DateTime.Now.Date)
                    {
                        var checkOutRecord = checkIns.FirstOrDefault(g => g.CheckOuttime.HasValue && g.CheckOuttime > checkInRecord.CheckIntime);
                        if (checkOutRecord != null)
                        {
                            totalHours += (checkOutRecord.CheckOuttime.Value - checkInRecord.CheckIntime.Value).TotalHours;
                        }
                    }
                    else
                    {
                        if (latestCheckRecord?.CheckIn == true)
                        {
                            totalHours += (DateTime.Now - checkInRecord.CheckIntime.Value).TotalHours;
                        }
                        else
                        {
                            var checkOutRecord = checkIns.FirstOrDefault(g => g.CheckOuttime.HasValue && g.CheckOuttime > checkInRecord.CheckIntime);
                            if (checkOutRecord != null)
                            {
                                totalHours += (checkOutRecord.CheckOuttime.Value - checkInRecord.CheckIntime.Value).TotalHours;
                            }
                        }
                        var breakRecords = await _context.EmployeeCheckIns
                            .Where(g => g.EmployeeId == employeeId && g.Currentdate.HasValue && g.Currentdate.Value.Date == currentDate.Date)
                            .Where(g => g.Breakin == true || g.Breakout == true)
                            .OrderBy(g => g.CheckInTime)
                            .ToListAsync();

                        foreach (var breakRecord in breakRecords)
                        {
                            if (breakRecord.Breakin == true && checkInRecord.CheckIntime.HasValue)
                            {
                                lastBreakInTime = breakRecord.CheckInTime;
                            }
                            else if (breakRecord.Breakout == true && lastBreakInTime.HasValue)
                            {
                                if (breakRecord.CheckInTime.HasValue && breakRecord.CheckInTime.Value.Date == currentDate.Date)
                                {
                                    double breakDuration = (breakRecord.CheckInTime.Value - lastBreakInTime.Value).TotalHours;

                                    if (breakDuration > breakPeriodThreshold)
                                    {
                                        totalBreakHours += (breakDuration - breakPeriodThreshold);
                                    }

                                    lastBreakInTime = null;
                                }
                            }
                        }
                    }
                }
            }

            double netWorkingHours = totalHours - totalBreakHours;

            return FormatHours(netWorkingHours);
        }

        private string FormatHours(double totalHours)
        {
            int hours = (int)totalHours;
            int minutes = (int)((totalHours - hours) * 60);
            return $"{hours}h {minutes}m";
        }
        public async Task<IActionResult> ExportBreakinReport(string EmpId, int? month, int? year)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

                if (adminLogin == null)
                {
                    return NotFound("Admin not found.");
                }

                var empDetails = await _context.EmployeeRegistrations
                    .Where(x => x.Vendorid == adminLogin.Vendorid && x.EmployeeId == EmpId && x.IsDeleted == false)
                    .ToListAsync();

                if (!empDetails.Any())
                {
                    return NotFound("Employee not found.");
                }

                var loginActivities = new List<EmployeeBreakDto>();

                foreach (var employee in empDetails)
                {
                    var breakRecords = await _context.EmployeeCheckIns
                        .Where(g => g.EmployeeId == employee.EmployeeId && g.CheckInTime.HasValue &&
                                    (!month.HasValue || g.Currentdate.HasValue && g.Currentdate.Value.Month == month.Value) &&
                                    (!year.HasValue || g.Currentdate.HasValue && g.Currentdate.Value.Year == year.Value))
                        .OrderBy(g => g.Currentdate)
                        .ToListAsync();

                    var breakRecordsIn = breakRecords.Where(g => g.Breakin == true).ToList();
                    var breakRecordsOut = breakRecords.Where(g => g.Breakout == true).ToList();

                    if (!breakRecordsIn.Any() && !breakRecordsOut.Any())
                    {
                        continue;
                    }

                    int index = 0;
                    foreach (var breakInRecord in breakRecordsIn)
                    {
                        var loginActivity = new EmployeeBreakDto
                        {
                            EmpId = employee.EmployeeId,
                            EmployeeName = $"{employee.FirstName} " +
                                           (string.IsNullOrEmpty(employee.MiddleName) ? "" : employee.MiddleName + " ") +
                                           employee.LastName,
                            BreakIntime = breakInRecord.CheckInTime.HasValue
                                ? breakInRecord.CheckInTime.Value.ToString("hh:mm tt")
                                : "N/A",
                            BreakOuttime = "N/A",
                            CurrentDate = breakInRecord.Currentdate.HasValue
                                ? breakInRecord.Currentdate.Value.ToString("dd-MMM-yyyy")
                                : "N/A",
                            Breakhour = "N/A"
                        };

                        if (index < breakRecordsOut.Count)
                        {
                            var breakOutRecord = breakRecordsOut[index];

                            if (breakOutRecord.CheckInTime.HasValue)
                            {
                                loginActivity.BreakOuttime = breakOutRecord.CheckInTime.Value.ToString("hh:mm tt");
                                var breakDuration = breakOutRecord.CheckInTime.Value - breakInRecord.CheckInTime.Value;
                                loginActivity.Breakhour = $"{(int)breakDuration.TotalHours}h {breakDuration.Minutes}m";
                            }
                        }

                        loginActivities.Add(loginActivity);
                        index++;
                    }
                }

                if (!loginActivities.Any())
                {
                    return Json("No break records found.");
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Employee Break Report");

                    worksheet.Cell(1, 1).Value = "Sr.No.";
                    worksheet.Cell(1, 2).Value = "Employee ID";
                    worksheet.Cell(1, 3).Value = "Employee Name";
                    worksheet.Cell(1, 4).Value = "Break-In Time";
                    worksheet.Cell(1, 5).Value = "Break-Out Time";
                    worksheet.Cell(1, 6).Value = "Break Hours";
                    worksheet.Cell(1, 7).Value = "Date";

                    int row = 2;
                    foreach (var record in loginActivities)
                    {
                        worksheet.Cell(row, 1).Value = row - 1;
                        worksheet.Cell(row, 2).Value = record.EmpId;
                        worksheet.Cell(row, 3).Value = record.EmployeeName;
                        worksheet.Cell(row, 4).Value = record.BreakIntime;
                        worksheet.Cell(row, 5).Value = record.BreakOuttime;
                        worksheet.Cell(row, 6).Value = record.Breakhour;
                        worksheet.Cell(row, 7).Value = record.CurrentDate;
                        row++;
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee_Break_Report.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public async Task<IActionResult> ExportAttendanceReport(string EmpId, int? month, int? year)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

                if (adminLogin == null)
                    return NotFound("Admin not found.");

                var attendanceRecords = await (from employee in _context.EmployeeRegistrations
                                               join record in _context.EmployeeCheckInRecords
                                               on employee.EmployeeId equals record.EmpId
                                               where employee.Vendorid == adminLogin.Vendorid &&
                                                     employee.IsDeleted == false &&
                                                     (string.IsNullOrEmpty(EmpId) || employee.EmployeeId == EmpId)
                                                     && (!month.HasValue || record.CurrentDate.Value.Month == month.Value)
                                            && (!year.HasValue || record.CurrentDate.Value.Year == year.Value)
                                               orderby record.CurrentDate descending
                                               select new EmployeeAttendanceDto
                                               {
                                                   EmpId = record.EmpId,
                                                   EmployeeName = $"{employee.FirstName} " +
                                                                  (string.IsNullOrEmpty(employee.MiddleName) ? "" : employee.MiddleName + " ") +
                                                                  employee.LastName,
                                                   CheckIntime = record.CheckIntime.HasValue
                                                       ? record.CheckIntime.Value.ToString("hh:mm tt")
                                                       : "N/A",
                                                   CheckOuttime = record.CheckOuttime.HasValue
                                                       ? record.CheckOuttime.Value.ToString("hh:mm tt")
                                                       : "N/A",
                                                   CurrentDate = record.CurrentDate.HasValue
                                                       ? record.CurrentDate.Value.ToString("dd-MMM-yyyy")
                                                       : "N/A",
                                                   Workinghour = "N/A"
                                               }).ToListAsync();
                foreach (var attendanceRecord in attendanceRecords)
                {
                    if (DateTime.TryParse(attendanceRecord.CurrentDate, out DateTime currentDate))
                    {
                        string workingHours = await TotalWorkingHours(attendanceRecord.EmpId, currentDate);
                        attendanceRecord.Workinghour = workingHours;
                    }
                    else
                    {
                        attendanceRecord.Workinghour = "Invalid Date";
                    }
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Employee Attendance Report");

                    int currentRow = 1;
                    string[] headers = { "Sr.No.", "Employee ID", "Employee Name", "Check-In Time", "Check-Out Time", "Working Hours", "Date" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(currentRow, i + 1).Value = headers[i];
                        worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }

                    currentRow++;

                    int row = 1;
                    foreach (var record in attendanceRecords)
                    {
                        worksheet.Cell(currentRow, 1).Value = row++;
                        worksheet.Cell(currentRow, 2).Value = record.EmpId;
                        worksheet.Cell(currentRow, 3).Value = record.EmployeeName;
                        worksheet.Cell(currentRow, 4).Value = record.CheckIntime;
                        worksheet.Cell(currentRow, 5).Value = record.CheckOuttime;
                        worksheet.Cell(currentRow, 6).Value = record.Workinghour;
                        worksheet.Cell(currentRow, 7).Value = record.CurrentDate;
                        currentRow++;
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileContent = stream.ToArray();
                        string fileName = $"{attendanceRecords.FirstOrDefault()?.EmployeeName?.Replace(" ", "")}_Attendance_Report.xlsx";
                        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet, Route("/Vendor/showallemployeeAttendancelist")]
        [HttpGet, Route("/Vendor/showallemployeeBreakinlist")]
        public async Task<IActionResult> showallemployeelist()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

            if (adminLogin == null)
                return Json(new { exists = false, message = "Admin not found." });

            var employeeRegistrations = await _context.EmployeeRegistrations
                .Where(x => x.Vendorid == adminLogin.Vendorid && x.IsDeleted == false && x.Isactive == true)
                .Select(x => new
                {
                    x.EmployeeId,
                    x.FirstName,
                    x.MiddleName,
                    x.LastName,
                    x.DateOfJoining,
                    PersonalDetails = _context.EmployeePersonalDetails
                        .Where(g => g.EmpRegId == x.EmployeeId)
                        .Select(g => new
                        {
                            g.MobileNumber,
                            g.PersonalEmailAddress
                        })
                        .FirstOrDefault()
                }).ToListAsync();

            var epfInfo = employeeRegistrations.Select(x => new showallemployeelisteDto
            {
                EmpId = x.EmployeeId.ToString(),
                EmployeeName = $"{x.FirstName} " +
                               (string.IsNullOrEmpty(x.MiddleName) ? "" : x.MiddleName + " ") +
                               x.LastName,
                MobileNumber = x.PersonalDetails?.MobileNumber,
                EmailId = x.PersonalDetails?.PersonalEmailAddress,
                JoiningDate = x.DateOfJoining.HasValue ? x.DateOfJoining.Value.ToString("dd-MMM-yyyy") : "N/A"
            }).OrderByDescending(x => x.EmpId).ToList();

            var model = new showallemployeelisteDto
            {
                emplist = epfInfo
            };

            return View(model);
        }
        [HttpGet, Route("Vendor/ApprovedWfhApply")]
        public async Task<IActionResult> ApprovedWfhApply(int? id)
        {
            try
            {
                int iId = id ?? 0;

                if (HttpContext.Session.GetString("UserName") != null)
                {
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                    ViewBag.approvestatus = await _context.Approvalmasters.Select(w => new SelectListItem
                    {
                        Value = w.Id.ToString(),
                        Text = w.Status,
                    }).ToListAsync();
                    var Approvedwfhdetail = await _ICrmrpo.GetWfhapplydetailList(adminlogin.Vendorid);
                    if (Approvedwfhdetail != null)
                    {
                        return View(Approvedwfhdetail);
                    }
                    else
                    {
                        return View();

                    }
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Message : " + ex.Message, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWfhApplyStatus(int Id, int Isapprove, DateTime StartDate, DateTime EndDate)
        {
            var applywfh = await _context.EmpApplywfhs.FirstOrDefaultAsync(x => x.Id == Id);
            if (applywfh == null)
            {
                TempData["msg"] = "Data not found!";
                return RedirectToAction("ApprovedWfhApply");
            }

            var empinfo = await _context.EmployeeRegistrations.FirstOrDefaultAsync(x => x.EmployeeId == applywfh.UserId);
            var emppersonalinfo = await _context.EmployeePersonalDetails.FirstOrDefaultAsync(x => x.EmpRegId == applywfh.UserId);

            applywfh.Iswfh = Isapprove;
            await _context.SaveChangesAsync();
            string subject;
            string emailBody;

            if (applywfh.Iswfh == 2)
            {
                subject = "Wfh Approval Accepted";
                emailBody = $"Dear {empinfo.FirstName} {empinfo.MiddleName} {empinfo.LastName},Your Wfh application has been approved.";
            }
            else if (applywfh.Iswfh == 1) // Partial approval (Pending)
            {
                subject = "Wfh Approval Partial";
                emailBody = $"Dear {empinfo.FirstName} {empinfo.MiddleName} {empinfo.LastName},We regret to inform you that your Wfh application is still pending. Please reapply with your desired {StartDate.Date} and {EndDate.Date} dates for processing.";
            }
            else
            {
                subject = "Wfh Approval Rejected";
                emailBody = $"Dear {empinfo.FirstName} {empinfo.MiddleName} {empinfo.LastName},We regret to inform you that your wfh application has been rejected. Please contact HR for further details.";
            }

            var status = await _context.Approvalmasters.Where(x => x.Id == applywfh.Iswfh).Select(x => x.Status).FirstOrDefaultAsync();

            try
            {
                await _emailService.SendEmpLeaveApprovalEmailAsync(
                    emppersonalinfo.PersonalEmailAddress,
                    empinfo.FirstName,
                    empinfo.MiddleName,
                    empinfo.LastName,
                    subject,
                    emailBody, (int)empinfo.Vendorid
                );
                TempData["msg"] = GetwfhApprovalMessage(status);
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { success = true, message = GetwfhApprovalMessage(status) });
        }
        private string GetwfhApprovalMessage(string status)
        {
            return status switch
            {
                "Complete" => "Approval status updated successfully with a Complete !",
                "Disapprove" => "Approval status updated successfully with a Disapprove!",
                _ => "Approval status updated successfully with a partial status!"
            };
        }
        private string GenerateInvoiceNumber()
        {
            var lastInvoice = _context.VendorRegistrations
                                      .OrderByDescending(x => x.Id)
                                      .FirstOrDefault();

            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminLogin = _context.AdminLogins.FirstOrDefault(x => x.Id == userId);

            int numericValue = 10001;

            string companyName = "ND";
            string[] unwantedWords = { "pvt ltd", "private limited", "ltd", "inc", "corporation", "corp" };

            foreach (var word in unwantedWords)
            {
                companyName = companyName.Replace(word, "", StringComparison.OrdinalIgnoreCase).Trim();
            }

            string firstChars;
            var words = companyName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 2 && words[0].Equals(words[1], StringComparison.OrdinalIgnoreCase))
            {
                firstChars = char.ToUpper(words[0][0]).ToString() + char.ToUpper(words[0][1]);
            }
            else if (words.Length >= 2)
            {
                firstChars = string.Concat(words.Select(w => char.ToUpper(w[0])));
            }
            else
            {
                firstChars = companyName.Length >= 3 ? companyName.Substring(0, 3).ToUpper() : companyName.ToUpper();
            }

            DateTime now = DateTime.Now;
            int startYear = now.Month >= 4 ? now.Year : now.Year - 1;
            int endYear = startYear + 1;
            string financialYear = $"{startYear}/{endYear % 100}";

            if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
            {
                string[] parts = lastInvoice.InvoiceNumber.Split('-');
                if (parts.Length > 2 && int.TryParse(parts.Last(), out int lastNumericValue))
                {
                    string lastFinancialYear = parts[1];
                    string lastFirstChars = parts[0];

                    if (lastFinancialYear != financialYear || lastFirstChars != firstChars)
                    {
                        numericValue = 10001;
                    }
                    else
                    {
                        numericValue = lastNumericValue + 1;
                    }
                }
            }

            string invoiceId = $"{firstChars}-{financialYear}-{numericValue:D4}";
            return invoiceId;
        }
        [HttpGet]
        public async Task<IActionResult> salarydeduction(int id)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                List<Salarydeductionmaster> events = _context.Salarydeductionmasters.Where(e => e.Vendorid == vendorid).ToList();
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Deductiontype = "";
                ViewBag.Deductionpercentage = "";
                ViewBag.heading = "Add Salary Deduction ";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.Salarydeductionmasters.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Deductiontype = data.Deductiontype;
                        ViewBag.Deductionpercentage = data.Deductionpercentage;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update";

                    }
                }

                return View(events);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> salarydeduction(Salarydeductionmaster model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;

                bool check = await _ICrmrpo.salarydeduction(model, vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("salarydeduction");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("salarydeduction");
                    }
                }
                else
                {
                    return RedirectToAction("salarydeduction");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> Deletesalarydeduction(int id)
        {
            try
            {
                var dlt = _context.Salarydeductionmasters.Find(id);
                _context.Remove(dlt);
                _context.SaveChanges();
                TempData["msg"] = "dltok";
                return RedirectToAction("salarydeduction");
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSalaryDeductions(decimal annualSalary)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;

                var deductions = await _context.Salarydeductionmasters
                    .Where(e => e.Vendorid == vendorid)
                    .Select(e => new
                    {
                        e.Deductiontype,
                        e.Deductionpercentage
                    }).ToListAsync();
                var calculatedDeductions = deductions.Select(d => new
                {
                    d.Deductiontype,
                    d.Deductionpercentage,
                    CalculatedAmount = Math.Round((annualSalary * (d.Deductionpercentage ?? 0) / 100) /*/ 12*/, 2)
                });

                return Json(calculatedDeductions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet, Route("Vendor/EmpReplyTaskslist")]
        public async Task<IActionResult> EmpReplyTaskslist()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == Userid);
                ViewBag.Heading = "Reply Task";

                var emplist = await _context.EmployeeRegistrations
                            .Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false)
                            .ToListAsync();

                List<string> employeeIds = emplist.Select(emp => emp.EmployeeId).ToList();

                var replytasklist = await (from task in _context.EmpTasksLists
                                           join employee in _context.EmployeeRegistrations
                                               on task.EmployeeId equals employee.EmployeeId
                                           join status in _context.TaskStatuses
                                               on task.Taskstatus equals status.Id
                                           join employeeTask in _context.EmployeeTasks
                                               on task.Subtaskid equals employeeTask.Id into taskDetails
                                           from taskDetail in taskDetails.DefaultIfEmpty()
                                           join subTask in _context.EmployeeTasksLists
                                               on task.Taskid equals subTask.Id into subTaskDetails
                                           from subTaskDetail in subTaskDetails.DefaultIfEmpty()
                                           where employeeIds.Contains(task.EmployeeId)
                                           select new EmptaskReplyListDto
                                           {
                                               id = task.Id,
                                               EmployeeId = task.EmployeeId,
                                               EmployeeName = employee.FirstName,
                                               TaskName = taskDetail != null ? taskDetail.Task : "N/A",
                                               SubTaskName = subTaskDetail != null ? subTaskDetail.Taskname : "N/A",
                                               Replydate = task.Replydate.HasValue
                                                   ? task.Replydate.Value.ToString("dd MMM yy")
                                                   : "N/A",
                                               Taskstatus = status.StatusName,
                                               TaskReason = task.Taskreason
                                           }).OrderByDescending(task => task.id).ToListAsync();


                return View(replytasklist);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpGet]
        public IActionResult DeletSignatureFile(string FilePath, int id)
        {
            bool success = false;

            if (FilePath != "")
            {
                string folderPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CompanyImage");
                string folderfilepathPath = folderPath + "//" + FilePath;
                if (Directory.Exists(folderPath))
                {
                    if (System.IO.File.Exists(folderfilepathPath))
                    {
                        System.IO.File.Delete(folderfilepathPath);
                        success = true;
                    }
                    var img = _context.VendorRegistrations.FirstOrDefault(s => s.VendorSingature == FilePath && s.Id == id);
                    if (img != null)
                    {
                        img.VendorSingature = null;
                        _context.SaveChangesAsync();
                    }

                }
            }
            return Json(success);
        }
        public async Task<IActionResult> ExportApprovedWfhApply()
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

                if (adminLogin == null)
                {
                    return NotFound("Admin not found.");
                }

                var empDetails = await _context.EmployeeRegistrations
                    .Where(x => x.Vendorid == adminLogin.Vendorid && x.IsDeleted == false)
                    .ToListAsync();

                if (empDetails == null || empDetails.Count == 0)
                {
                    return NotFound("Employee not found.");
                }

                var Approvedwfhdetail = await _ICrmrpo.GetWfhapplydetailList(adminLogin.Vendorid);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Employee Break Report");

                    int currentwork = 1;
                    worksheet.Cell(currentwork, 1).Value = "Sr.No.";
                    worksheet.Cell(currentwork, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 2).Value = "Employee ID";
                    worksheet.Cell(currentwork, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 3).Value = "Employee Name";
                    worksheet.Cell(currentwork, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 4).Value = "Mobile Number";
                    worksheet.Cell(currentwork, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 5).Value = "Start Date";
                    worksheet.Cell(currentwork, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 6).Value = "End Date";
                    worksheet.Cell(currentwork, 6).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 7).Value = "Total Days";
                    worksheet.Cell(currentwork, 7).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 9).Value = "Applied Date";
                    worksheet.Cell(currentwork, 9).Style.Fill.BackgroundColor = XLColor.LightGray;

                    currentwork++;

                    var row = 1;
                    foreach (var record in Approvedwfhdetail)
                    {
                        worksheet.Cell(currentwork, 1).Value = row++;
                        worksheet.Cell(currentwork, 2).Value = record.UserId;
                        worksheet.Cell(currentwork, 3).Value = record.EmployeeName;
                        worksheet.Cell(currentwork, 4).Value = record.EmpMobileNumber;
                        worksheet.Cell(currentwork, 5).Value = record.StartDate;
                        worksheet.Cell(currentwork, 6).Value = record.EndDate;
                        worksheet.Cell(currentwork, 7).Value = record.days;
                        worksheet.Cell(currentwork, 8).Value = record.Reason;
                        worksheet.Cell(currentwork, 9).Value = record.CreatedDate;

                        currentwork++;
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileContent = stream.ToArray();
                        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee Break Report.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet, Route("Vendor/AddHRsignature")]
        public async Task<IActionResult> AddHRsignature(int id)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                HrsignatureDto model = new HrsignatureDto();
                model.Hrsignaturlist = _context.Hrsignatures.Where(e => e.Vendorid == vendorid).OrderBy(x => x.Id).ToList();
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.HrJobTitle = "";
                ViewBag.HrSignature = "";
                ViewBag.HrName = "";
                ViewBag.heading = "Add";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.Hrsignatures.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.HrJobTitle = data.HrJobTitle;
                        ViewBag.HrSignature = data.HrSignature1;
                        ViewBag.HrName = data.HrName;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update ";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddHRsignature(HrsignatureDto model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                var existingabout = await _context.Hrsignatures
                .Where(b => b.Vendorid == vendorid)
                  .FirstOrDefaultAsync();
                if (model.Id == 0)
                {
                    if (existingabout != null)
                    {
                        TempData["msg"] = "Already exists.";
                        return RedirectToAction("AddHRsignature");
                    }
                }
                bool check = await _ICrmrpo.Addhrsignature(model, vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("AddHRsignature");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("AddHRsignature");
                    }
                }
                else
                {
                    return RedirectToAction("AddHRsignature");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public IActionResult DeletHrSignature(string FilePath, int id)
        {
            bool success = false;

            if (FilePath != "")
            {
                string folderPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CompanyImage");
                string folderfilepathPath = folderPath + "//" + FilePath;
                if (Directory.Exists(folderPath))
                {
                    if (System.IO.File.Exists(folderfilepathPath))
                    {
                        System.IO.File.Delete(folderfilepathPath);
                        success = true;
                    }
                    var img = _context.Hrsignatures.FirstOrDefault(s => s.HrSignature1 == FilePath && s.Id == id);
                    if (img != null)
                    {
                        img.HrSignature1 = null;
                        _context.SaveChangesAsync();
                    }

                }
            }
            return Json(success);
        }
        [HttpGet]
        [MiddlewareFilter(typeof(JsReportPipeline))]
        public async Task<IActionResult> SendVendorInvoice(int Id = 0, bool Ismail = false)
        {
            try
            {
                Invoice invoice = await _ICrmrpo.GenerateInvoice(Id, Ismail);
                if (invoice == null)
                {
                    TempData["msg"] = "No data found for the invoice.";
                    return RedirectToAction("VendorList");
                }

                ViewBag.Protocol = Request.Scheme;
                ViewBag.Host = Request.Host.Value;

                if (Ismail)
                {
                    var jsReportFeature = HttpContext.JsReportFeature();
                    if (jsReportFeature == null)
                    {
                        throw new Exception("JsReport feature is not available in the current context.");
                    }

                    jsReportFeature
                        .Recipe(Recipe.ChromePdf)
                        .OnAfterRender((r) =>
                        {
                            HttpContext.Response.Headers["Content-Disposition"] = $"attachment; filename=Invoice_{invoice.CompanyName}.pdf";
                        });
                    return View("VendorInvoice", invoice);
                }
                else
                {
                    return View(invoice);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "An error occurred: " + ex.Message;
                return RedirectToAction("VendorList");
            }
        }

    }
}

