﻿using CRM.IUtilities;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    public class Vendor : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Vendor(ICrmrpo _ICrmrpo, admin_NDCrMContext _context, IWebHostEnvironment hostingEnvironment)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
            _webHostEnvironment = hostingEnvironment;
        }
        [HttpGet, Route("Vendor/VendorRegistration")]
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
                    if (response > 0)
                    {
                        TempData["Message"] = "Registration Successfully.";
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

        [HttpGet,Route("Vendor/VendorProfile")]
        public async Task<IActionResult> VendorProfile()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                string id = Convert.ToString(HttpContext.Session.GetString("UserId")); 
                ViewBag.UserName = AddedBy;
                var data = await _ICrmrpo.GetVendorProfile(id);
                ViewBag.vendorid = data.Id;
                ViewBag.FilePathDetail = data.CompanyImage;
                ViewBag.id = id;
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

        [HttpGet,Route("Vendor/ApprovedPresnolInfo")]
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
                    ViewBag.Heading = "Add PersonalInfo";
                    ViewBag.BtnText = "SAVE";

                    if (iId != 0)
                    {
                        var ApprovedPresnolInfo = _context.ApprovedPresnolInfos.Find(iId);
                        if (ApprovedPresnolInfo != null)
                        {
                            ViewBag.id = ApprovedPresnolInfo.Id;
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
                        var aadharTwoImageName = fileOperation.SaveBase64Image("img1",model.Aadharbase64[1], allowedExtensions);

                        if (aadharOneImageName != "not allowed")
                        {
                            model.AadharOne =  aadharOneImageName;
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
                    var emp = await _context.EmployeePersonalDetails.FirstOrDefaultAsync(x => x.EmpRegId == item.EmployeeId);
                    if (emp != null)
                    {
                        emp.PersonalEmailAddress = item.PersonalEmailAddress;
                        emp.MobileNumber =Convert.ToString(item.MobileNumber);
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
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CompanyImage");
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
    }
}
