using CRM.IUtilities;
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
using SelectPdf;
using System.Data.SqlClient;

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
                string AddedBy = HttpContext.Session.GetString("UserName");
                var items = _context.States.ToList();
                ViewBag.StateItems = new SelectList(items, "Id", "SName");
                if (id != 0)
                {
                    ViewBag.Heading = "Vendor Registration";
                    ViewBag.btnText = "Update";
                    var vendor = _context.VendorRegistrations.Where(x => x.Id == id).FirstOrDefault();
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
                        return View(data);
                    }
                }
                ViewBag.Heading = "Vendor Registration";
                ViewBag.btnText = "SAVE";
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
                    var response = await _ICrmrpo.Vendorreg(model);
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

        [HttpGet, Route("Vendor/VendorProfile")]
        public async Task<IActionResult> VendorProfile()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                string id = Convert.ToString(HttpContext.Session.GetString("UserId"));
                var items = _context.States.ToList();
                ViewBag.StateItems = new SelectList(items, "Id", "SName");
                ViewBag.UserName = AddedBy;
                var data = await _ICrmrpo.GetVendorProfile(id);
                ViewBag.vendorid = data.Id;
                ViewBag.SelectedCityId = data.CityId;
                ViewBag.SelectedBillingCityId = data.BillingCityId;
                ViewBag.SelectedBillingStateId = data.BillingStateId;
                ViewBag.FilePathDetail = data.CompanyImage;
                ViewBag.id = id;
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
                string AddedBy = HttpContext.Session.GetString("UserName");
                int id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                ViewBag.UserName = AddedBy;
                if (id != null)
                {
                    var data = await _ICrmrpo.UpdateVendorProfile(model, id);
                    TempData["Message"] = "Update Successfully.";
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
                    var addedBy = HttpContext.Session.GetString("UserName");
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    ViewBag.UserName = addedBy;
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
                    var model = new EmployeePresnolInfoList
                    {
                        ApprovedPresnolInfos = EmployeePresnolInfo
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
                    string AddedBy = HttpContext.Session.GetString("UserName");
                    ViewBag.UserName = AddedBy;
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
                    var addedBy = HttpContext.Session.GetString("UserName");
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    ViewBag.UserName = addedBy;
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
                    var model = new ApprovedbankdetailList
                    {
                        Approvedbankdetails = Approvedbankdetail
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
                    string AddedBy = HttpContext.Session.GetString("UserName");
                    ViewBag.UserName = AddedBy;
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
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                List<Attendanceday> response = _context.Attendancedays.Where(x => x.Vendorid == adminlogin.Vendorid).ToList();
                ViewBag.UserName = AddedBy;
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
                    var existingData = await _context.Attendancedays.FindAsync(model.Id);
                    if (existingData != null)
                    {
                        existingData.Nodays = model.Nodays;
                        existingData.Createdate = DateTime.Now.Date;
                        existingData.Vendorid = adminlogin.Vendorid;

                        await _context.SaveChangesAsync();
                        TempData["Message"] = "Data Update Successfully.";
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
                    TempData["Message"] = "Data Added Successfully.";
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
                }
                return RedirectToAction("VendorAttendancedayslist");
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
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                List<Officeshift> response = await _context.Officeshifts.Where(x => x.Vendorid == adminlogin.Vendorid).ToListAsync();

                ViewBag.UserName = AddedBy;
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
                    var existingData = await _context.Officeshifts.FindAsync(model.Id);
                    if (existingData != null)
                    {
                        existingData.Starttime = model.Starttime;
                        existingData.Endtime = model.Endtime;
                        existingData.ShiftTypeid = model.ShiftTypeid;
                        existingData.Createdate = DateTime.Now.Date;
                        existingData.Vendorid = adminlogin.Vendorid;

                        await _context.SaveChangesAsync();
                        TempData["Message"] = "Data Update Successfully.";
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
                    TempData["Message"] = "Data Added Successfully.";
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
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
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
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;

                bool check = await _ICrmrpo.AddVendorCategory(model, vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "Category added successfully!";
                        return RedirectToAction("VendorCategory");
                    }
                    else
                    {
                        TempData["msg"] = "Category updated successfully!";
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
                TempData["msg"] = "Category deleted successfully!";
                return RedirectToAction("VendorCategory");

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("Vendor/VendorInvoice")]
        public async Task<IActionResult> VendorInvoice(int ID)
        {
            try
            {
                if (ID != null)
                {
                    Invoice invoice = new Invoice();

                    invoice = await _ICrmrpo.GenerateInvoice(ID);
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
        public async Task<IActionResult> UpdateVendorActiveStatus(int Id)
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
                var pdfResult = await VendorInvoiceDocPDF(vendor.Id);
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

        public async Task<IActionResult> VendorInvoiceDocPDF(int? Id = 0)
        {
            try
            {
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                string SlipURL = $"{schema}://{host}/Vendor/VendorInvoice?Id={Id}";
                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "VendorInvoicefile");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }
                var result = (from vr in _context.VendorRegistrations
                              where vr.Id == Id
                              select new
                              {
                                  Id = vr.Id,
                                  CompanyName = vr.CompanyName,
                                  Email = vr.Email
                              }).FirstOrDefault();
                string uniqueFileName = $"Invoice_{Guid.NewGuid()}_{Id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string filePath = System.IO.Path.Combine(wwwRootPath, uniqueFileName);
                doc.Save(filePath);
                byte[] pdf = System.IO.File.ReadAllBytes(filePath);



                if (result == null)
                {
                    return BadRequest("Vendor not found.");
                }

                var empoff = _context.VendorRegistrations.FirstOrDefault(e => e.Id == result.Id);
                if (empoff != null)
                {
                    empoff.Invoicefile = uniqueFileName;
                    _context.SaveChanges();
                    string emailSubject = $"Invoice for {result.CompanyName}";
                    string emailBody = $"Hello {result.CompanyName}";
                    await _emailService.SendEmailAsync(result.Email, emailSubject, emailBody, pdf, uniqueFileName, "application/pdf");
                    return Json(new { success = true, message = "Invoice  has been Sent successfully.", fileName = uniqueFileName });

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
                string AddedBy = HttpContext.Session.GetString("UserName");
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
                ViewBag.UserName = AddedBy;
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
                        TempData["Message"] = "Data Update Successfully.";
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
                    TempData["Message"] = "Data Added Successfully.";
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
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                List<OfficeBreakstatus> response = await _context.OfficeBreakstatuses.Where(x => x.Vendorid == adminlogin.Vendorid).ToListAsync();
                ViewBag.UserName = AddedBy;
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
                    var existingData = await _context.OfficeBreakstatuses.FindAsync(model.Id);
                    if (existingData != null)
                    {
                        existingData.Breakstatus = model.Breakstatus;
                        existingData.Createdate = DateTime.Now.Date;
                        existingData.Vendorid = adminlogin.Vendorid;

                        await _context.SaveChangesAsync();
                        TempData["Message"] = "Data Update Successfully.";
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
                    TempData["Message"] = "Data Added Successfully.";
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
                string AddedBy = HttpContext.Session.GetString("UserName");
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
               }).ToListAsync();
                ViewBag.TaskStatus = await _context.TaskStatuses.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.StatusName,
                }).ToListAsync();
                ViewBag.EmployeeId = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(D => new SelectListItem
                {
                    Value = D.EmployeeId.ToString(),
                    Text = D.EmployeeId

                }).ToList();
                ViewBag.UserName = AddedBy;
                ViewBag.Task = "";
                ViewBag.Tittle = "";
                ViewBag.Startdate = "";
                ViewBag.Enddate = "";
                ViewBag.Description = "";
                ViewBag.Status = "";
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
                    ViewBag.Status = data.Status;
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
                    var existingData = await _context.EmployeeTasks.FindAsync(model.Id);
                    if (existingData != null)
                    {
                        existingData.Task = model.Task;
                        existingData.Tittle = model.Tittle;
                        existingData.Startdate = model.Startdate;
                        existingData.Enddate = model.Enddate;
                        existingData.Description = model.Description;
                        existingData.Status = Convert.ToInt16(model.Status);
                        existingData.EmployeeId = model.EmployeeId;
                        existingData.Vendorid = adminlogin.Vendorid;

                        await _context.SaveChangesAsync();
                        TempData["Message"] = "Data Update Successfully.";
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
                        Status = Convert.ToInt16(model.Status),
                        EmployeeId = model.EmployeeId,
                        Vendorid = adminlogin.Vendorid,
                    };

                    _context.EmployeeTasks.Add(data);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Data Added Successfully.";
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
                }
                return RedirectToAction("EmpTasksassignment");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteList:" + ex.Message);
            }
        }

        [HttpGet, Route("Vendor/EmpTaskslist")]
        //public async Task<IActionResult> EmpTaskslist(int? id = 0)
        //{
        //    if (HttpContext.Session.GetString("UserName") != null)
        //    {
        //        string AddedBy = HttpContext.Session.GetString("UserName");
        //        int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
        //        var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
        //        var emplist = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).FirstOrDefault();
        //        List<EmpTasknameDto> response = await _context.EmployeeTasksLists.OrderByDescending(x => x.Id)
        //       .Select(x => new EmpTasknameDto
        //       {
        //           Id = x.Id,
        //           Emptask = _context.EmployeeTasks.Where(a => a.Id == x.Emptaskid).Select(t => t.Task).FirstOrDefault(),
        //           Taskname = x.Taskname,
        //           TaskStatus = _context.TaskStatuses.Where(a => a.Id == x.TaskStatus).Select(status => status.StatusName).FirstOrDefault(),
        //           EmployeeId = x.EmployeeId,
        //       }).ToListAsync();
        //        ViewBag.EmployeeId = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(D => new SelectListItem
        //        {
        //            Value = D.EmployeeId.ToString(),
        //            Text = D.EmployeeId
        //        }).ToList();
        //        ViewBag.UserName = AddedBy;
        //        ViewBag.Taskname = "";
        //        ViewBag.Emptaskid = "";
        //        ViewBag.EmpId = "";
        //        ViewBag.Heading = "Add TaskName";
        //        ViewBag.BtnText = "SAVE";
        //        if (id != 0)
        //        {
        //            var data = await _context.EmployeeTasksLists.Where(x => x.Id == id).FirstOrDefaultAsync();
        //            ViewBag.id = data.Id;
        //            ViewBag.Taskname = data.Taskname;
        //            ViewBag.Emptaskid = data.Emptaskid;
        //            ViewBag.EmpId = data.EmployeeId;
        //            ViewBag.Heading = "Update TaskName";
        //            ViewBag.BtnText = "UPDATE";
        //        }
        //        return View(response);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Admin");
        //    }
        //}
        public async Task<IActionResult> EmpTaskslist(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == Userid);
 
                EmployeeTaskModel model = new EmployeeTaskModel();
                model.EmpTaskList = await _context.EmployeeTasksLists.OrderByDescending(x => x.Id)
                    .Select(x => new EmpTasknameDto
                    {
                        Id = _context.EmployeeTasks.Where(a => a.Id == x.Emptaskid).Select(t => t.Id).FirstOrDefault(),
                        Emptask = _context.EmployeeTasks.Where(a => a.Id == x.Emptaskid).Select(t => t.Task).FirstOrDefault(),
                        Taskname = x.Taskname,
                        TaskStatusId = x.TaskStatus,
                        TaskStatus = _context.TaskStatuses.Where(a => a.Id == x.TaskStatus).Select(status => status.StatusName).FirstOrDefault(),
                        EmployeeId = x.EmployeeId,
                    }).ToListAsync();

                
                ViewBag.EmployeeId = await _context.EmployeeRegistrations
                    .Where(x => x.Vendorid == adminlogin.Vendorid)
                    .Select(D => new SelectListItem
                    {
                        Value = D.EmployeeId.ToString(),
                        Text = D.EmployeeId
                    }).ToListAsync();

                 
                ViewBag.UserName = AddedBy;
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
                     

                    // Remove existing services
                    var existingServices = await _context.EmployeeTasksLists
                        .Where(s => s.Emptaskid == model.Id)
                        .ToListAsync();
                    _context.EmployeeTasksLists.RemoveRange(existingServices);

                    // Add new 
                    //foreach (var taskName in Taskname)
                    //{
                    //    foreach (var status in TaskStatusId)
                    //    {
                    //        if (!string.IsNullOrWhiteSpace(taskName))
                    //        {
                    //            EmployeeTasksList task = new EmployeeTasksList()
                    //            {
                    //                Taskname = taskName,
                    //                Emptaskid = Convert.ToInt16(model.Emptask),
                    //                EmployeeId = model.EmployeeId,
                    //                TaskStatus = status,
                    //            };
                    //            await _context.EmployeeTasksLists.AddAsync(task);
                    //        }
                    //        break;
                    //    }
                    //}
                    foreach (var taskName in Taskname)
                    {
                        
                        if (!string.IsNullOrWhiteSpace(taskName))
                        {
                            EmployeeTasksList task = new EmployeeTasksList()
                            {
                                Taskname = taskName,
                                Emptaskid = Convert.ToInt16(model.Emptask),
                                EmployeeId = model.EmployeeId,
                                TaskStatus = 1,
                            };
                            await _context.EmployeeTasksLists.AddAsync(task);
                        }
                    }

                   
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Data Update Successfully.";
                    return RedirectToAction("EmpTaskslist", "Vendor");
                    //var existingData = await _context.EmployeeTasksLists.FindAsync(model.Id);
                    //if (existingData != null)
                    //{
                    //    existingData.Emptaskid = Convert.ToInt16(model.Emptask);
                    //    existingData.EmployeeId = model.EmployeeId;
                    //    existingData.Taskname = Taskname.FirstOrDefault(); 
                    //    await _context.SaveChangesAsync();

                    //    TempData["Message"] = "Data Update Successfully.";
                    //    return RedirectToAction("EmpTaskslist", "Vendor");
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("", "Record not found for update.");
                    //    return View(model);
                    //}
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
                    TempData["Message"] = "Data Added Successfully.";
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
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
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
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                var existingBankDetail = await _context.VendorBankDetails
            .Where(b => b.VendorId == vendorid)
            .FirstOrDefaultAsync();
                if(model.Id==0)
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
                    if(model.Id==0)
                    {
                        TempData["Message"] = "Bank detail added successfully..";
                    }
                    else
                    {
                        TempData["Message"] = "Bank detail updated successfully..";
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
                TempData["Message"] = "Deleted successfully!";
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
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;
                List<OfficeEvent> events = _context.OfficeEvents.Where(e => e.Vendorid == vendorid).OrderBy(x=>x.Date).ToList();        
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
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                int vendorid = (int)adminlogin.Vendorid;

                bool check = await _ICrmrpo.AddOfficeEvents(model, vendorid);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "Event added successfully!";
                        return RedirectToAction("Events");
                    }
                    else
                    {
                        TempData["msg"] = "Event updated successfully!";
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
                TempData["msg"] = "Deleted successfully!";
                return RedirectToAction("Events");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult UpdateTaskStatus(int Taskstatusid, int Id)
        {
            var emp = _context.EmployeeTasks.Where(x => x.Id == Id).FirstOrDefault();
            emp.Status = Taskstatusid;
            _context.SaveChanges();
            return Json(new { success = true, message = "Task status updated successfully!" });
        }
    }
}

