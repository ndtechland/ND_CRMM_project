using CRM.Controllers;
using CRM.IUtilities;
using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Utilities;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebPlatform;
using MimeKit.Encodings;
using NuGet.Versioning;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using Syncfusion.Pdf.Graphics;
using System;
using System.Globalization;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using DinkToPdf;
namespace CRM.Repository
{
    public class Employee : IEmployee
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly admin_NDCrMContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Employee(admin_NDCrMContext context, IWebHostEnvironment hostingEnvironment)
        {
            this._context = context;
            _webHostEnvironment = hostingEnvironment;
        }
        public async Task<EmployeeBasicInfo> GetEmployeeById(string userid)
        {
            try
            {
                if (userid != null)
                {
                    var empid = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).Select(x => new EmployeeBasicInfo
                    {
                        Userid = x.Id,
                        FullName = x.FirstName + "" + x.MiddleName + "  " + x.LastName,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        WorkEmail = x.WorkEmail,
                        MobileNumber = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => g.MobileNumber).First(),
                        DateOfBirth = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => g.DateOfBirth.Value.ToString("dd-MM-yyyy")).First(),
                        Stateid = x.StateId,
                        FatherName = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => g.FatherName).First(),
                        Cityid = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => Convert.ToInt16(g.City)).First(),
                        StateName = _context.States.Where(g => g.Id == x.StateId).Select(g => g.SName).First(),
                        CityName = _context.Cities.Where(g => g.Id == Convert.ToInt16(x.WorkLocationId)).Select(g => g.City1).First(),
                        Address1 = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => g.AddressLine1).First(),
                        Address2 = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => g.AddressLine2).First(),
                        Pincode = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => g.Pincode).First(),
                        PersonalEmailAddress = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => g.PersonalEmailAddress).First(),
                        DateOfJoining = String.Format("{0:dd-MM-yyyy}", x.DateOfJoining),
                        DepartmentName = _context.DepartmentMasters.Where(g => g.Id == Convert.ToInt16(x.DepartmentId)).Select(g => g.DepartmentName).First().Trim(),
                        DesignationName = _context.DesignationMasters.Where(g => g.Id == Convert.ToInt16(x.DepartmentId)).Select(g => g.DesignationName).First().Trim(),
                        CompanyName = _context.VendorRegistrations.Where(g => g.Id == x.Vendorid).Select(g => g.CompanyName).First(),
                        CompanyLocationName = _context.VendorRegistrations.Where(g => g.Id == x.Vendorid).Select(g => g.Location).First(),
                        EmployeeId = x.EmployeeId,
                        AadharNo = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => g.Aadharcard).First(),
                        PanNo = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => g.Pan).First(),
                        EmpProfile = "/EmpProfile/" + x.EmpProfile,
                        AadharOne = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => "/img1/" + g.AadharOne).First(),
                        AadharTwo = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => "/img1/" + g.AadharTwo).First(),
                        Panimg = _context.EmployeePersonalDetails.Where(g => g.EmpId == x.Id).Select(g => "/img1/" + g.Panimg).First(),
                        ShiftTime = _context.Officeshifts.Where(g => g.Id == x.OfficeshiftTypeid).Select(g => $"{g.Starttime} - {g.Endtime}").First(),
                        ShiftType = _context.Officeshifts.Where(g => g.Id == x.OfficeshiftTypeid).First().ShiftTypeid,
                    }).FirstOrDefaultAsync();
                    return empid;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
        public async Task<Approvedbankdetail> Bankdetail(bankdetail model, string userid)
        {
            try
            {
                string ChequeImagePath = "";
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                if (model.Chequebase64 != null)
                {
                    if (model.Chequebase64.Length > 10 * 1024 * 1024)
                    {
                        throw new Exception("Pan file size exceeds the 10 MB limit.");
                    }
                    ChequeImagePath = fileOperation.SaveBase64Image("ChequeImage", model.Chequebase64, allowedExtensions);
                    if (ChequeImagePath == "not allowed")
                    {
                        throw new Exception("File upload not allowed.");
                    }
                }
                var emp = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).FirstOrDefaultAsync();
                var empbank = await _context.Approvedbankdetails.Where(x => x.EmployeeId == userid).FirstOrDefaultAsync();
                if (empbank != null)
                {
                    empbank.Vendorid = emp.Vendorid;
                    empbank.AccountHolderName = model.AccountHolderName;
                    empbank.BankName = model.BankName;
                    empbank.AccountNumber = model.AccountNumber;
                    empbank.ReEnterAccountNumber = model.ReEnterAccountNumber;
                    empbank.Ifsc = model.Ifsc;
                    empbank.AccountTypeId = Convert.ToInt32(model.AccountTypeId);
                    empbank.EpfNumber = model.EpfNumber;
                    empbank.Nominee = model.Nominee;
                    if (!string.IsNullOrEmpty(ChequeImagePath))
                    {
                        empbank.Chequeimage = ChequeImagePath;
                    }
                    empbank.UpdateDate = DateTime.Now.Date;
                    empbank.IsApproved = false;
                    await _context.SaveChangesAsync();
                    return empbank;
                }
                else
                {
                    Approvedbankdetail empB = new Approvedbankdetail
                    {
                        Vendorid = emp.Vendorid,
                        AccountHolderName = model.AccountHolderName,
                        BankName = model.BankName,
                        AccountNumber = model.AccountNumber,
                        ReEnterAccountNumber = model.ReEnterAccountNumber,
                        Ifsc = model.Ifsc,
                        EmployeeId = userid,
                        AccountTypeId = Convert.ToInt32(model.AccountTypeId),
                        EpfNumber = model.EpfNumber,
                        Nominee = model.Nominee,
                        UpdateDate = DateTime.Now.Date,
                        IsApproved = false,
                    };
                    ChequeImagePath = fileOperation.SaveBase64Image("ChequeImage", model.Chequebase64, allowedExtensions);
                    empB.Chequeimage = ChequeImagePath;
                    _context.Approvedbankdetails.Add(empB);
                    await _context.SaveChangesAsync();

                    return empB;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<List<City>> getcity(int stateid)
        {
            try
            {
                var cities = await _context.Cities.Where(x => x.StateId == stateid).ToListAsync();

                return cities;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<List<State>> Getstate()
        {
            try
            {
                var state = await _context.States.ToListAsync();
                return state;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<bankdetail> GetBankdetail(string userid)
        {
            try
            {
                if (userid != null)
                {
                    var result = await _context.EmployeeBankDetails.Where(x => x.EmpId == userid && x.IsDeleted == false).Select(x => new bankdetail
                    {
                        AccountHolderName = x.AccountHolderName,
                        BankName = x.BankName,
                        AccountNumber = x.AccountNumber,
                        ReEnterAccountNumber = x.ReEnterAccountNumber,
                        Ifsc = x.Ifsc,
                        AccountTypeId = x.AccountTypeId,
                        EpfNumber = x.EpfNumber,
                        Nominee = x.Nominee,
                        Chequeimage = "/ChequeImage/" + x.Chequeimage,
                    }).FirstOrDefaultAsync();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Error :" + ex.Message);
            }
        }
        public async Task<salarydetails> Getsalarydetails(string userid)
        {
            try
            {
                if (userid != null)
                {
                    var result = await _context.EmployeeSalaryDetails.Where(x => x.EmployeeId == userid && x.IsDeleted == false).Select(x => new salarydetails
                    {
                        Basic = x.Basic,
                        HouseRentAllowance = x.HouseRentAllowance,
                        TravellingAllowance = x.TravellingAllowance,
                        Esic = x.Esic,
                        Epf = x.Epf,
                        MonthlyGrossPay = x.MonthlyGrossPay,
                        MonthlyCtc = x.MonthlyCtc,
                        Professionaltax = x.Professionaltax,
                        SpecialAllowance = x.SpecialAllowance,
                        Gross = x.Gross,
                    }).FirstOrDefaultAsync();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Error :" + ex.Message);
            }
        }
        public async Task<ApprovedPresnolInfo> PersonalDetail(EmpPersonalDetail model, string userid)
        {
            try
            {
                string panImagePath = "";
                string aadharImagePath1 = "";
                string aadharImagePath2 = "";
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                if (model.PanbaseImage != null)
                {
                    if (model.PanbaseImage.Length > 10 * 1024 * 1024)
                    {
                        throw new Exception("Pan file size exceeds the 10 MB limit.");
                    }
                    panImagePath = fileOperation.SaveBase64Image("img1", model.PanbaseImage, allowedExtensions);
                    if (panImagePath == "not allowed")
                    {
                        throw new Exception("File upload not allowed.");
                    }
                }
                if (model.Aadhar1 != null)
                {
                    if (model.Aadhar1.Length > 10 * 1024 * 1024)
                    {
                        throw new Exception("Pan file size exceeds the 10 MB limit.");
                    }
                    aadharImagePath1 = fileOperation.SaveBase64Image("img1", model.Aadhar1, allowedExtensions);
                    if (aadharImagePath1 == "not allowed")
                    {
                        throw new Exception("File upload not allowed.");
                    }
                }
                if (model.Aadhar2 != null)
                {
                    if (model.Aadhar2.Length > 10 * 1024 * 1024)
                    {
                        throw new Exception("Pan file size exceeds the 10 MB limit.");
                    }
                    aadharImagePath2 = fileOperation.SaveBase64Image("img1", model.Aadhar2, allowedExtensions);
                    if (aadharImagePath2 == "not allowed")
                    {
                        throw new Exception("File upload not allowed.");
                    }
                }
                var emp = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).FirstOrDefaultAsync();
                var apppersonal = await _context.ApprovedPresnolInfos.Where(x => x.EmployeeId == userid).FirstOrDefaultAsync();
                if (apppersonal != null)
                {
                    apppersonal.Vendorid = emp.Vendorid;
                    apppersonal.PersonalEmailAddress = model.PersonalEmailAddress;
                    apppersonal.MobileNumber = model.MobileNumber;
                    apppersonal.DateOfBirth = model.DateOfBirth == null ? _context.EmployeePersonalDetails.Where(x => x.EmpRegId == userid && x.IsDeleted == false).First().DateOfBirth : Convert.ToDateTime(model.DateOfBirth);
                    apppersonal.Pan = model.PanNo;
                    apppersonal.AddressLine1 = model.Address1;
                    apppersonal.AddressLine2 = model.Address2;
                    apppersonal.City = Convert.ToString(model.Cityid);
                    apppersonal.StateId = Convert.ToString(model.Stateid);
                    apppersonal.Pincode = model.Pincode;
                    apppersonal.AadharNo = model.AadharNo;
                    apppersonal.UpdateDate = DateTime.Now.Date;
                    apppersonal.IsApproved = false;
                    apppersonal.FullName = model.FullName;
                    apppersonal.FatherName = model.FatherName;
                    if (!string.IsNullOrEmpty(panImagePath))
                    {
                        apppersonal.Panimg = fileOperation.SaveBase64Image("img1", model.PanbaseImage, allowedExtensions);
                    }
                    if (!string.IsNullOrEmpty(aadharImagePath1))
                    {
                        apppersonal.AadharOne = fileOperation.SaveBase64Image("img1", model.Aadhar1, allowedExtensions);
                    }
                    if (!string.IsNullOrEmpty(aadharImagePath2))
                    {
                        apppersonal.AadharTwo = fileOperation.SaveBase64Image("img1", model.Aadhar2, allowedExtensions);
                    }
                    if (model.Empprofile != null)
                    {
                        string EmpprofileImagePath = fileOperation.SaveBase64Image("EmpProfile", model.Empprofile, allowedExtensions);
                        emp.EmpProfile = EmpprofileImagePath;
                    }

                    await _context.SaveChangesAsync();
                    return apppersonal;
                }
                else
                {

                    ApprovedPresnolInfo empP = new ApprovedPresnolInfo();
                    {

                        empP.FullName = model.FullName;
                        empP.Vendorid = emp.Vendorid;
                        empP.EmployeeId = userid;
                        empP.PersonalEmailAddress = model.PersonalEmailAddress;
                        empP.MobileNumber = model.MobileNumber;
                        empP.DateOfBirth = model.DateOfBirth == null ? _context.EmployeePersonalDetails.Where(x => x.EmpRegId == userid && x.IsDeleted == false).First().DateOfBirth : Convert.ToDateTime(model.DateOfBirth);
                        empP.AddressLine1 = model.Address1;
                        empP.AddressLine2 = model.Address2;
                        empP.City = Convert.ToString(model.Cityid);
                        empP.StateId = Convert.ToString(model.Stateid);
                        empP.Pincode = model.Pincode;
                        empP.Pan = model.PanNo;
                        empP.AadharNo = model.AadharNo;
                        empP.FatherName = model.FatherName;
                        empP.UpdateDate = DateTime.Now.Date;
                        empP.IsApproved = false;
                        if (model.Aadhar1 != null)
                        {
                            aadharImagePath1 = fileOperation.SaveBase64Image("img1", model.Aadhar1, allowedExtensions);
                            empP.AadharOne = aadharImagePath1;
                        }
                        if (model.Aadhar2 != null)
                        {
                            aadharImagePath2 = fileOperation.SaveBase64Image("img1", model.Aadhar2, allowedExtensions);
                            empP.AadharTwo = aadharImagePath2;
                        }
                        if (model.PanbaseImage != null)
                        {
                            panImagePath = fileOperation.SaveBase64Image("img1", model.PanbaseImage, allowedExtensions);
                            empP.Panimg = panImagePath;
                        }
                        if (model.Empprofile != null)
                        {
                            string EmpprofileImagePath = fileOperation.SaveBase64Image("EmpProfile", model.Empprofile, allowedExtensions);
                            emp.EmpProfile = EmpprofileImagePath;
                        }
                    }

                    _context.ApprovedPresnolInfos.Add(empP);
                    await _context.SaveChangesAsync();

                    return empP;
                }


                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PersonalDetail: {ex.Message}");
                return null;
            }
        }
        public async Task<leavedto> LeaveType(string userid)
        {

            leavedto GetleaveList = new leavedto();
            GetleaveList.GetLeaveTypeList = (from lm in _context.Leavemasters
                                             join lty in _context.LeaveTypes on lm.LeavetypeId equals lty.Id
                                             where lm.EmpId == userid && lm.IsActive == true  
                                             && lm.Vendorid == lty.Vendorid
                                             select new LeaveTypeValue
                                             {
                                                 Id = lm.LeavetypeId,
                                                 leavetype = lty.Leavetype1 ,
                                                 leaveValue = lm.Value ?? 0m ,
                                                 Isactive = lm.IsActive
                                             }).ToList();
            if (!GetleaveList.GetLeaveTypeList.Any())
            {
                GetleaveList.GetLeaveTypeList.Add(new LeaveTypeValue
                {
                    leaveValue = 0m,         
                });
            }
            GetleaveList.GetLeaveList = _context.Leaves.Where(x => x.Isactive == true).ToList();
            return GetleaveList;
        }
        #region no use
        //public async Task<bool> ApplyLeave(ApplyLeave model, string userid)
        //{
        //    try
        //    {
        //        List<Leave> FH = new List<Leave>();
        //        List<Leave> SH = new List<Leave>();
        //        List<Leave> FD = new List<Leave>();
        //        decimal totalLeaveRequested = 0.00M;
        //        var TypeOfLeave = await _context.Leavemasters
        //                                        .Where(x => x.LeavetypeId == model.TypeOfLeaveId && x.EmpId == userid)
        //                                        .FirstOrDefaultAsync();

        //        if (TypeOfLeave == null)
        //        {
        //            throw new Exception("Leave type not found for the given employee.");
        //        }
        //        if (model.EndDate != model.StartDate)
        //        {
        //            totalLeaveRequested = (model.EndDate - model.StartDate).Days;
        //        }
        //        if (model.StartLeaveId == 1)
        //        {
        //            FH = await _context.Leaves.Where(x => x.Id == model.StartLeaveId).ToListAsync();
        //        }
        //        else if (model.StartLeaveId == 2)
        //        {
        //            SH = await _context.Leaves.Where(x => x.Id == model.EndeaveId).ToListAsync();
        //        }
        //        else
        //        {
        //            FD = await _context.Leaves.Where(x => x.Id == model.EndeaveId).ToListAsync();
        //        }
        //        if (totalLeaveRequested == 0)
        //        {
        //            totalLeaveRequested = (FH.Count() > 0 ? 0.50M : 0) +
        //                                  (SH.Count() > 0 ? 0.50M : 0) +
        //                                  (FD.Count() > 0 ? 1.00M : 0);
        //        }
        //        else
        //        {
        //            totalLeaveRequested += (FH.Count() > 0 ? 0.50M : 0) +
        //                                   (SH.Count() > 0 ? 0.50M : 0);
        //        }
        //        decimal leaveBalance = (decimal)TypeOfLeave.Value;
        //        decimal remainingLeave = totalLeaveRequested - leaveBalance;
        //        if (leaveBalance >= totalLeaveRequested)
        //        {
        //            ApplyLeaveNews applyLeaveNews = new ApplyLeaveNews()
        //            {
        //                UserId = userid,
        //                StartLeaveId = model.StartLeaveId,
        //                EndeaveId = model.EndeaveId,
        //                TypeOfLeaveId = model.TypeOfLeaveId,
        //                StartDate = model.StartDate,
        //                EndDate = model.EndDate,
        //                CreatedDate = DateTime.Now,
        //                Reason = model.Reason,
        //                CountLeave = totalLeaveRequested,
        //                Isapprove = false,
        //                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month)
        //            };

        //            await _context.ApplyLeaveNews.AddAsync(applyLeaveNews);
        //            await _context.SaveChangesAsync();
        //            TypeOfLeave.Value -= totalLeaveRequested;
        //            if (TypeOfLeave.Value < 0)
        //            {
        //                TypeOfLeave.Value = 0.00M;
        //            }
        //            _context.Entry(TypeOfLeave).State = EntityState.Modified;
        //            await _context.SaveChangesAsync();

        //            return true;
        //        }
        //        else if (leaveBalance > 0)
        //        {
        //            ApplyLeaveNews applyLeaveNews = new ApplyLeaveNews()
        //            {
        //                UserId = userid,
        //                StartLeaveId = model.StartLeaveId,
        //                EndeaveId = model.EndeaveId,
        //                TypeOfLeaveId = model.TypeOfLeaveId,
        //                StartDate = model.StartDate,
        //                EndDate = model.EndDate,
        //                CreatedDate = DateTime.Now,
        //                Reason = model.Reason,
        //                CountLeave = leaveBalance,
        //                Isapprove = false,
        //                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month)
        //            };

        //            await _context.ApplyLeaveNews.AddAsync(applyLeaveNews);
        //            await _context.SaveChangesAsync();
        //            TypeOfLeave.Value -= leaveBalance;
        //            _context.Entry(TypeOfLeave).State = EntityState.Modified;
        //            await _context.SaveChangesAsync();
        //            PaidLeavemaster paidLeaveEntry = new PaidLeavemaster()
        //            {
        //                UserId = userid,
        //                StartLeaveId = model.StartLeaveId,
        //                EndeaveId = model.EndeaveId,
        //                TypeOfLeaveId = model.TypeOfLeaveId,
        //                StartDate = model.StartDate,
        //                EndDate = model.EndDate,
        //                CreatedDate = DateTime.Now,
        //                CountLeave = remainingLeave,
        //                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month),
        //                Reason = model.Reason,
        //                Isapprove = false
        //            };

        //            await _context.PaidLeavemasters.AddAsync(paidLeaveEntry);
        //            await _context.SaveChangesAsync();

        //            return true;
        //        }
        //        else
        //        {
        //            throw new Exception("Not enough leave balance.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Message: " + ex.Message);
        //    }
        //}
        #endregion

        public async Task<bool> ApplyLeave(ApplyLeave model, string userid)
        {
            try
            {
                List<Leave> FHFirst = new List<Leave>();
                List<Leave> SHFirst = new List<Leave>();
                List<Leave> FDFirst = new List<Leave>();
                List<Leave> FHSecond = new List<Leave>();
                List<Leave> SHSecond = new List<Leave>();
                List<Leave> FDSecond = new List<Leave>();
                decimal total = (decimal)0.00;
                decimal CountLeave = (decimal)0.00;
                decimal PaidCountLeave = (decimal)0.00;
                decimal Balance = (decimal)0.00;

                var TypeOfLeave = _context.Leavemasters.Where(x => x.LeavetypeId == model.TypeOfLeaveId && x.EmpId == userid).FirstOrDefault();

                if (model.EndDate != model.StartDate)
                {
                    total = (model.EndDate - model.StartDate).Days - 1;
                }
                if (model.EndDate == model.StartDate)
                {
                    if (model.StartLeaveId == 1)
                    {
                        FHFirst = await _context.Leaves.Where(x => x.Id == model.StartLeaveId).ToListAsync();
                    }
                    if (model.StartLeaveId == 2)
                    {
                        SHFirst = await _context.Leaves.Where(x => x.Id == model.StartLeaveId).ToListAsync();
                    }
                    if (model.StartLeaveId == 3)
                    {
                        FDFirst = await _context.Leaves.Where(x => x.Id == model.StartLeaveId).ToListAsync();
                    }
                }
                else
                {
                    if (model.StartLeaveId == 1)
                    {
                        FHFirst = await _context.Leaves.Where(x => x.Id == model.StartLeaveId).ToListAsync();
                    }
                    if (model.StartLeaveId == 2)
                    {
                        SHFirst = await _context.Leaves.Where(x => x.Id == model.StartLeaveId).ToListAsync();
                    }
                    if (model.StartLeaveId == 3)
                    {
                        FDFirst = await _context.Leaves.Where(x => x.Id == model.StartLeaveId).ToListAsync();
                    }
                    if (model.EndeaveId == 1)
                    {
                        FHSecond = await _context.Leaves.Where(x => x.Id == model.EndeaveId).ToListAsync();
                    }
                    if (model.EndeaveId == 2)
                    {
                        SHSecond = await _context.Leaves.Where(x => x.Id == model.EndeaveId).ToListAsync();
                    }
                    if (model.EndeaveId == 3)
                    {
                        FDSecond = await _context.Leaves.Where(x => x.Id == model.EndeaveId).ToListAsync();
                    }
                }


                if (total == 0)
                {
                    decimal TotalLeaveFirst = (FHFirst.Count() == 0 ? 0 : (decimal)(FHFirst.Count() * 0.50)) + (SHFirst.Count() == 0 ? 0 : (decimal)(SHFirst.Count() * 0.50)) + (FDFirst.Count() == 0 ? 0 : (decimal)(FDFirst.Count() * 1.00));
                    decimal TotalLeaveSecond = (FHSecond.Count() == 0 ? 0 : (decimal)(FHSecond.Count() * 0.50)) + (SHSecond.Count() == 0 ? 0 : (decimal)(SHSecond.Count() * 0.50)) + (FDSecond.Count() == 0 ? 0 : (decimal)(FDSecond.Count() * 1.00));
                    total = Math.Max(0, total + (TotalLeaveFirst + TotalLeaveSecond));
                    decimal currentLeaveValue = TypeOfLeave.Value ?? 0m;
                    if (currentLeaveValue > 0)
                    {
                        CountLeave = Math.Min(total, currentLeaveValue);
                        //TypeOfLeave.Value -= CountLeave;
                        //await _context.SaveChangesAsync();
                        Balance = CountLeave;
                        PaidCountLeave = total - CountLeave;
                    }
                    else
                    {
                        CountLeave = Math.Max(total, currentLeaveValue);
                        PaidCountLeave = CountLeave;
                        Balance = Math.Min(total, currentLeaveValue);
                    }
                }
                else
                {
                    decimal TotalLeaveFirst = (FHFirst.Count() == 0 ? 0 : (decimal)(FHFirst.Count() * 0.50)) + (SHFirst.Count() == 0 ? 0 : (decimal)(SHFirst.Count() * 0.50)) + (FDFirst.Count() == 0 ? 0 : (decimal)(FDFirst.Count() * 1.00));
                    decimal TotalLeaveSecond = (FHSecond.Count() == 0 ? 0 : (decimal)(FHSecond.Count() * 0.50)) + (SHSecond.Count() == 0 ? 0 : (decimal)(SHSecond.Count() * 0.50)) + (FDSecond.Count() == 0 ? 0 : (decimal)(FDSecond.Count() * 1.00));


                    total = Math.Max(0, total + (TotalLeaveFirst + TotalLeaveSecond));
                    decimal currentLeaveValue = TypeOfLeave.Value ?? 0m;
                    if (currentLeaveValue > 0)
                    {
                        CountLeave = Math.Min(total, currentLeaveValue);
                        //TypeOfLeave.Value -= CountLeave;
                        //await _context.SaveChangesAsync();
                        Balance = CountLeave;
                        PaidCountLeave = total - CountLeave;
                    }
                    else
                    {
                        CountLeave = Math.Max(total, currentLeaveValue);
                        PaidCountLeave = CountLeave;
                        Balance = Math.Min(total, currentLeaveValue);
                    }

                }
                int month = DateTime.Now.Month;
                ApplyLeaveNews apply = new ApplyLeaveNews()
                {
                    UserId = userid,
                    StartLeaveId = model.StartLeaveId,
                    EndeaveId = model.EndeaveId,
                    TypeOfLeaveId = model.TypeOfLeaveId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CreatedDate = DateTime.Now,
                    Reason = model.Reason,
                    CountLeave = Balance,
                    PaidCountLeave = PaidCountLeave,
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    Isapprove = 3
                };
                await _context.ApplyLeaveNews.AddAsync(apply);
                await _context.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {

                throw new Exception("Error Message : e " + ex);
            }
        }

        public async Task<List<EmpattendanceDto>> GetAllEmpsalaryslip(string userid)
        {
            try
            {
                if (userid != null)
                {
                    var result = _context.Empattendances.Where(x => x.EmployeeId == userid).Select(x => new EmpattendanceDto
                    {
                        Id = x.Id,
                        SalarySlipPath = "/EMPpdfs/" + x.SalarySlip,
                        Month = x.Month,
                        Year = x.Year,
                        SalarySlipName = getMonthName(x.Month) + '-' + x.Year
                    }).ToList();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Error :" + ex.Message);
            }
        }
        public static String getMonthName(int? monthNumber)
        {
            switch (monthNumber)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
                default:
                    return "Invalid month";
            }
        }
        public async Task<EmpattendanceDto> Getsalarydetails(string userid, int month, int year)
        {
            try
            {
                if (userid != null)
                {
                    var result = _context.Empattendances.Where(x => x.EmployeeId == userid && x.Month == month && x.Year == year).Select(x => new EmpattendanceDto
                    {
                        SalarySlipPath = "/EMPpdfs/" + x.SalarySlip,
                        Month = x.Month,
                        Year = x.Year,
                        SalarySlipName = getMonthName(x.Month) + '-' + x.Year
                    }).FirstOrDefault();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Error :" + ex.Message);
            }
        }

        public async Task<CompanyLoctionDto> GetCompanyLoction(string userid)
        {
            try
            {
                if (userid != null)
                {
                    var empid = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).Select(x => new CompanyLoctionDto
                    {
                        CompanyOfficeLocation = _context.VendorRegistrations.Where(g => g.Id == x.Vendorid).Select(g => g.Location).First(),
                        Radious = _context.VendorRegistrations.Where(g => g.Id == x.Vendorid).Select(g => g.Radious).First(),
                    }).FirstOrDefaultAsync();
                    return empid;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
        public async Task<EmployeeCheckIn> Empcheckin(CRM.Models.APIDTO.EmpCheckIn model, bool checkIn)
        {
            try
            {
                var emp = await _context.EmployeeRegistrations
                    .Where(x => x.Id == model.Userid)
                    .FirstOrDefaultAsync();
                if (emp == null)
                {
                    throw new Exception("Employee not found.");
                }
                var officeShift = await _context.Officeshifts
    .Where(s => s.Id == emp.OfficeshiftTypeid)
    .FirstOrDefaultAsync();

                if (officeShift == null)
                {
                    throw new Exception("Employee shift information not found.");
                }
                EmployeeCheckIn empcheck = new EmployeeCheckIn()
                {
                    EmployeeId = emp.EmployeeId,
                    CurrentLat = model.CurrentLat,
                    Currentlong = model.Currentlong,
                    Currentdate = DateTime.Now,
                    Updatedate = DateTime.Now,
                };
                empcheck.Breakin = model.Breakin;
                empcheck.Breakout = model.Breakout;
                checkIn = model.Breakin == true && model.Breakout == false ? true : checkIn;
                if (!checkIn)
                {
                    empcheck.CheckIn = checkIn;
                    empcheck.CheckOutTime = DateTime.Now;
                    empcheck.Updatedate = DateTime.Now;

                }
                else
                {
                    empcheck.CheckInTime = DateTime.Now;
                    empcheck.CheckIn = checkIn;
                    empcheck.Updatedate = DateTime.Now;

                }
                await _context.EmployeeCheckIns.AddAsync(empcheck);
                await _context.SaveChangesAsync();
                var fgdfdin = await _context.EmpCheckIns
                    .Where(x => x.EmployeeId == emp.EmployeeId && x.Currentdate.Value.Date == DateTime.Now.Date)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                if (fgdfdin == null)
                {
                    CRM.Models.Crm.EmpCheckIn empcheckin = new CRM.Models.Crm.EmpCheckIn()
                    {
                        EmployeeId = emp.EmployeeId,
                        CurrentLat = model.CurrentLat,
                        Currentlong = model.Currentlong,
                        Currentdate = DateTime.Now,
                        CheckInTime = DateTime.Now,
                        CheckIn = true,
                    };

                    await _context.EmpCheckIns.AddAsync(empcheckin);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (fgdfdin.CheckIn == true && !checkIn)
                    {
                        CRM.Models.Crm.EmpCheckIn empcheckout = new CRM.Models.Crm.EmpCheckIn()
                        {
                            EmployeeId = emp.EmployeeId,
                            CurrentLat = model.CurrentLat,
                            Currentlong = model.Currentlong,
                            Currentdate = DateTime.Now,
                            CheckOutTime = DateTime.Now,
                            CheckIn = false,
                        };

                        await _context.EmpCheckIns.AddAsync(empcheckout);
                        await _context.SaveChangesAsync();
                    }
                }




                var fgdfd = await _context.EmployeeCheckIns
                    .Where(x => x.EmployeeId == emp.EmployeeId && x.CheckInTime.Value.Date == DateTime.Now.Date)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                if (fgdfd == null)
                {
                    throw new Exception("Check-in record not found.");
                }

                var todayCheckInsCount = await _context.EmployeeCheckIns
                .CountAsync(x => x.EmployeeId == emp.EmployeeId && x.CheckInTime.Value.Date == DateTime.Now.Date);
                if (todayCheckInsCount > 0)
                {
                    var existingCheckInRecord = await _context.EmployeeCheckInRecords
                        .Where(x => x.EmpId == emp.EmployeeId && x.CheckIntime.Value.Date == DateTime.Now.Date)
                        .FirstOrDefaultAsync();

                    if (existingCheckInRecord != null)
                    {
                        existingCheckInRecord.CheckOuttime = null;
                        TimeSpan workingHours = (existingCheckInRecord.CheckIntime.Value - DateTime.Now).Duration();
                        existingCheckInRecord.Workinghour = workingHours;
                        existingCheckInRecord.ShiftId = officeShift.Id;
                        _context.EmployeeCheckInRecords.Update(existingCheckInRecord);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        EmployeeCheckInRecord empch = new()
                        {
                            EmpId = emp.EmployeeId,
                            CheckIntime = fgdfd.CheckInTime,
                            CurrentDate = DateTime.Now,
                            Isactive = true,
                            Workinghour = TimeSpan.Zero,
                            ShiftId = officeShift.Id,


                        };
                        await _context.EmployeeCheckInRecords.AddAsync(empch);
                        await _context.SaveChangesAsync();
                    }
                }

                if (!checkIn)
                {
                    var checkInRecord = await _context.EmployeeCheckInRecords
                        .Where(x => x.EmpId == emp.EmployeeId && x.CheckIntime.Value.Date == DateTime.Now.Date)
                        .FirstOrDefaultAsync();

                    if (checkInRecord != null)
                    {
                        TimeSpan workingHours = (fgdfd.CheckInTime.Value - checkInRecord.CheckIntime.Value).Duration();
                        checkInRecord.Workinghour = workingHours;
                        checkInRecord.CheckOuttime = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                }
                return empcheck;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<ApprovedPresnolInfo> webPersonalDetail(webPersonalDetail model, string userid)
        {
            try
            {
                string panImagePath = "";
                string aadharImagePath1 = "";
                string aadharImagePath2 = "";
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                if (model.PanbaseImage != null)
                {
                    if (model.PanbaseImage.Length > 10 * 1024 * 1024)
                    {
                        throw new Exception("Pan file size exceeds the 10 MB limit.");
                    }
                    panImagePath = fileOperation.SaveBase64Image("img1", model.PanbaseImage, allowedExtensions);
                    if (panImagePath == "not allowed")
                    {
                        throw new Exception("File upload not allowed.");
                    }
                }
                if (model.Aadhar1 != null)
                {
                    if (model.Aadhar1.Length > 10 * 1024 * 1024)
                    {
                        throw new Exception("Pan file size exceeds the 10 MB limit.");
                    }
                    aadharImagePath1 = fileOperation.SaveBase64Image("img1", model.Aadhar1, allowedExtensions);
                    if (aadharImagePath1 == "not allowed")
                    {
                        throw new Exception("File upload not allowed.");
                    }
                }
                if (model.Aadhar2 != null)
                {
                    if (model.Aadhar2.Length > 10 * 1024 * 1024)
                    {
                        throw new Exception("Pan file size exceeds the 10 MB limit.");
                    }
                    aadharImagePath2 = fileOperation.SaveBase64Image("img1", model.Aadhar2, allowedExtensions);
                    if (aadharImagePath2 == "not allowed")
                    {
                        throw new Exception("File upload not allowed.");
                    }
                }
                var emp = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).FirstOrDefaultAsync();
                var apppersonal = await _context.ApprovedPresnolInfos.Where(x => x.EmployeeId == userid).FirstOrDefaultAsync();
                if (apppersonal != null)
                {
                    apppersonal.Vendorid = emp.Vendorid;
                    apppersonal.PersonalEmailAddress = model.PersonalEmailAddress;
                    apppersonal.MobileNumber = model.MobileNumber;
                    apppersonal.DateOfBirth = model.DateOfBirth == null ? _context.EmployeePersonalDetails.Where(x => x.EmpRegId == userid && x.IsDeleted == false).First().DateOfBirth : Convert.ToDateTime(model.DateOfBirth);
                    apppersonal.Pan = model.PanNo;
                    apppersonal.AddressLine1 = model.Address1;
                    apppersonal.AddressLine2 = model.Address2;
                    apppersonal.City = Convert.ToString(model.Cityid);
                    apppersonal.StateId = Convert.ToString(model.Stateid);
                    apppersonal.Pincode = model.Pincode;
                    apppersonal.AadharNo = model.AadharNo;
                    apppersonal.UpdateDate = DateTime.Now.Date;
                    apppersonal.IsApproved = false;
                    apppersonal.FullName = model.FullName;
                    apppersonal.FatherName = model.FatherName;
                    if (!string.IsNullOrEmpty(panImagePath))
                    {
                        apppersonal.Panimg = fileOperation.SaveBase64Image("img1", model.PanbaseImage, allowedExtensions);
                    }
                    if (!string.IsNullOrEmpty(aadharImagePath1))
                    {
                        apppersonal.AadharOne = fileOperation.SaveBase64Image("img1", model.Aadhar1, allowedExtensions);
                    }
                    if (!string.IsNullOrEmpty(aadharImagePath2))
                    {
                        apppersonal.AadharTwo = fileOperation.SaveBase64Image("img1", model.Aadhar2, allowedExtensions);
                    }
                    return apppersonal;
                }
                else
                {

                    ApprovedPresnolInfo empP = new ApprovedPresnolInfo();
                    {

                        empP.FullName = model.FullName;
                        empP.Vendorid = emp.Vendorid;
                        empP.EmployeeId = userid;
                        empP.PersonalEmailAddress = model.PersonalEmailAddress;
                        empP.MobileNumber = model.MobileNumber;
                        empP.DateOfBirth = model.DateOfBirth == null ? _context.EmployeePersonalDetails.Where(x => x.EmpRegId == userid && x.IsDeleted == false).First().DateOfBirth : Convert.ToDateTime(model.DateOfBirth);
                        empP.AddressLine1 = model.Address1;
                        empP.AddressLine2 = model.Address2;
                        empP.City = Convert.ToString(model.Cityid);
                        empP.StateId = Convert.ToString(model.Stateid);
                        empP.Pincode = model.Pincode;
                        empP.Pan = model.PanNo;
                        empP.AadharNo = model.AadharNo;
                        empP.FatherName = model.FatherName;
                        empP.UpdateDate = DateTime.Now.Date;
                        empP.IsApproved = false;
                        if (model.Aadhar1 != null)
                        {
                            aadharImagePath1 = fileOperation.SaveBase64Image("img1", model.Aadhar1, allowedExtensions);
                            empP.AadharOne = aadharImagePath1;
                        }
                        if (model.Aadhar2 != null)
                        {
                            aadharImagePath2 = fileOperation.SaveBase64Image("img1", model.Aadhar2, allowedExtensions);
                            empP.AadharTwo = aadharImagePath2;
                        }
                        if (model.PanbaseImage != null)
                        {
                            panImagePath = fileOperation.SaveBase64Image("img1", model.PanbaseImage, allowedExtensions);
                            empP.Panimg = panImagePath;
                        }
                    }

                    _context.ApprovedPresnolInfos.Add(empP);
                    await _context.SaveChangesAsync();

                    return empP;
                }


                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PersonalDetail: {ex.Message}");
                return null;
            }
        }
        public async Task<EmployeeCheckIn> Empcheckout(CRM.Models.APIDTO.EmpCheckIn model, bool checkIn)
        {
            try
            {
                var emp = await _context.EmployeeRegistrations
                    .FirstOrDefaultAsync(x => x.Id == model.Userid);

                if (emp == null)
                {
                    throw new Exception("Employee not found.");
                }

                var officeShift = await _context.Officeshifts
                    .FirstOrDefaultAsync(s => s.Id == emp.OfficeshiftTypeid);

                if (officeShift == null)
                {
                    throw new Exception("Employee shift information not found.");
                }

                var empCheckInRecord = await _context.EmpCheckIns
                    .Where(x => x.EmployeeId == emp.EmployeeId && x.Currentdate.Value.Date == DateTime.Now.Date)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();
                if (empCheckInRecord == null || empCheckInRecord.CheckIn == true)
                {
                    var empcheckin = new CRM.Models.Crm.EmpCheckIn
                    {
                        EmployeeId = emp.EmployeeId,
                        CurrentLat = model.CurrentLat,
                        Currentlong = model.Currentlong,
                        Currentdate = DateTime.Now,
                        CheckOutTime = DateTime.Now,
                        CheckIn = false,
                    };

                    await _context.EmpCheckIns.AddAsync(empcheckin);
                }

                var existingCheckIn = await _context.EmployeeCheckIns
                    .Where(x => x.EmployeeId == emp.EmployeeId && x.Currentdate.Value.Date == DateTime.Now.Date)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();


                if (existingCheckIn == null || existingCheckIn.CheckIn == true)
                {
                    var empcheckinDetail = new EmployeeCheckIn
                    {
                        EmployeeId = emp.EmployeeId,
                        CurrentLat = model.CurrentLat,
                        Currentlong = model.Currentlong,
                        Currentdate = DateTime.Now,
                        CheckOutTime = DateTime.Now,
                        CheckIn = false,
                        Breakin = false,
                        Breakout = false,
                        Updatedate = DateTime.Now,
                    };

                    await _context.EmployeeCheckIns.AddAsync(empcheckinDetail);
                }
                else
                {
                    if (existingCheckIn != null && existingCheckIn.CheckIn == false)
                    {
                        existingCheckIn.CurrentLat = model.CurrentLat;
                        existingCheckIn.Currentlong = model.Currentlong;
                        existingCheckIn.CheckOutTime = DateTime.Now;
                        existingCheckIn.CheckIn = false;
                        existingCheckIn.Updatedate = DateTime.Now;
                        _context.EmployeeCheckIns.Update(existingCheckIn);


                    }
                }
                var detailedCheckInRecord = await _context.EmployeeCheckInRecords
                       .FirstOrDefaultAsync(x => x.EmpId == emp.EmployeeId && x.CheckIntime.Value.Date == DateTime.Now.Date);

                if (detailedCheckInRecord != null)
                {
                    TimeSpan workingHours = (DateTime.Now - detailedCheckInRecord.CheckIntime.Value).Duration();
                    detailedCheckInRecord.Workinghour = workingHours;
                    detailedCheckInRecord.CheckOuttime = DateTime.Now;
                    detailedCheckInRecord.ShiftId = officeShift.Id;

                    _context.EmployeeCheckInRecords.Update(detailedCheckInRecord);
                }
                else
                {
                    var newDetailedRecord = new EmployeeCheckInRecord
                    {
                        EmpId = emp.EmployeeId,
                        CheckIntime = existingCheckIn.CheckInTime,
                        CheckOuttime = DateTime.Now,
                        Workinghour = (DateTime.Now - existingCheckIn.CheckInTime.Value).Duration(),
                        CurrentDate = DateTime.Now,
                        Isactive = true,
                        ShiftId = officeShift.Id
                    };
                    await _context.EmployeeCheckInRecords.AddAsync(newDetailedRecord);
                }
                await _context.SaveChangesAsync();
                return existingCheckIn;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public async Task<EmployeeRegistration> Updateprofilepicture(profilepicture model, string userid)
        {
            try
            {
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                var emp = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid).FirstOrDefaultAsync();

                if (emp != null)
                {

                    if (model.Empprofile != null)
                    {
                        string EmpprofileImagePath = fileOperation.SaveBase64Image("EmpProfile", model.Empprofile, allowedExtensions);
                        emp.EmpProfile = EmpprofileImagePath;
                    }


                    await _context.SaveChangesAsync();
                    return emp;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PersonalDetail: {ex.Message}");
                return null;
            }
        }
        public async Task<Empattendancedatail> GetEmpattendance(string userid, DateTime Currentdate)
        {
            try
            {
                if (string.IsNullOrEmpty(userid))
                {
                    return null;
                }

                var empAttendanceDetails = await _context.EmployeeRegistrations
                    .Where(x => x.EmployeeId == userid)
                    .Select(x => new
                    {
                        OfficeShiftId = x.OfficeshiftTypeid,
                        EmployeeId = x.EmployeeId
                    }).FirstOrDefaultAsync();
                if (empAttendanceDetails == null)
                {
                    throw new Exception("Employee not found.");
                }
                var wfhCount = await _context.EmpApplywfhs
                .Where(g => g.UserId == userid && g.Iswfh == 2 && g.Currentdate.Value.Month == Currentdate.Month
                            && g.Currentdate.Value.Year == Currentdate.Year)
                .CountAsync();
                var matchingRecord = await _context.EmpApplywfhs
    .Where(g => g.UserId == userid && g.Iswfh == 2
        && g.Startdate.Value.Date <= DateTime.Now.Date
        && g.EndDate.Value.Date >= DateTime.Now.Date)
    .OrderByDescending(g => g.Id)
    .FirstOrDefaultAsync();

                string WFHStatus = matchingRecord != null ? "WFH-In" : "WFH-Out";


                var checkIns = await _context.EmployeeCheckInRecords
                    .Where(g => g.EmpId == userid && g.CurrentDate.HasValue && g.CurrentDate.Value.Date == Currentdate)
                    .OrderBy(g => g.Id)
                    .ToListAsync();
                if (!checkIns.Any())
                {
                    var shiftId1 = empAttendanceDetails.OfficeShiftId;
                    var currentofficeShift1 = await _context.Officeshifts.FindAsync((int)shiftId1);

                    if (currentofficeShift1 == null)
                    {
                        throw new Exception("Office shift not found.");
                    }

                    return new Empattendancedatail
                    {
                        OfficeHour = $"{currentofficeShift1.Starttime} - {currentofficeShift1.Endtime}",
                        CheckInTime = "N/A",
                        CheckOutTime = "N/A",
                        StartOverTime = await CalculateStartOverTime(userid, (int)shiftId1, _context, Currentdate),
                        FinishOverTime = await CalculateFinishOverTime(userid, (int)shiftId1, _context, Currentdate),
                        OvertimeWorkingHours = await CalculateMonthlyOvertimeHours(userid, (int)shiftId1, Currentdate),
                        TotalWorkingHours = await CalculateTotalWorkingHours(userid, Currentdate),
                        MonthlyWorkingHours = await CalculateMonthlyWorkingHours(userid, Currentdate),
                        Presencepercentage = await CalculatePresencePercentage(userid, Currentdate),
                        absencepercentage = await CalculateAbsencePercentage(userid, Currentdate),
                        Currentdate = Currentdate.ToString("dd-MM-yyyy"),
                        LoginStatus = "Check-Out",
                        loginactivities = await GetLoginBreakActivities(userid, Currentdate),
                        Ontime = "",
                        dayPart = GetPartOfDay(DateTime.Now.TimeOfDay),
                        WFHStatus = WFHStatus,
                        NumberofWFH = wfhCount
                    };
                }

                var firstCheckIn = checkIns.First();
                var shiftId = firstCheckIn.ShiftId ?? empAttendanceDetails.OfficeShiftId;
                var currentofficeShift = await _context.Officeshifts.FindAsync((int)shiftId);

                if (currentofficeShift == null)
                {
                    throw new Exception("Office shift not found.");
                }

                var checkInTime = firstCheckIn.CheckIntime.HasValue ? firstCheckIn.CheckIntime.Value.ToString("hh:mm tt") : "N/A";
                var checkOutTime = checkIns.LastOrDefault()?.CheckOuttime?.ToString("hh:mm tt") ?? "N/A";

                var loginActivities = await _context.EmployeeCheckIns
                    .Where(g => g.EmployeeId == userid && g.Currentdate.HasValue && g.Currentdate.Value.Date == DateTime.Now.Date)
                    .OrderBy(g => g.Id)
                    .ToListAsync();

                string latestLoginStatus = loginActivities.LastOrDefault()?.CheckIn == true ? "Check-In" : "Check-Out";
                string lateness = "On Time";
                if (checkIns.Any() && checkInTime != "N/A")
                {
                    DateTime scheduledStartTime = DateTime.Parse(currentofficeShift.Starttime);
                    DateTime actualCheckInTime = DateTime.Parse(checkInTime);
                    if (actualCheckInTime > scheduledStartTime)
                    {
                        TimeSpan delay = actualCheckInTime - scheduledStartTime;
                        lateness = $"{delay.Hours}h{delay.Minutes}m Late";
                    }
                }



                return new Empattendancedatail
                {
                    OfficeHour = $"{currentofficeShift.Starttime} - {currentofficeShift.Endtime}",
                    CheckInTime = checkInTime,
                    CheckOutTime = checkOutTime,
                    StartOverTime = await CalculateStartOverTime(userid, (int)shiftId, _context, Currentdate),
                    FinishOverTime = await CalculateFinishOverTime(userid, (int)shiftId, _context, Currentdate),
                    OvertimeWorkingHours = await CalculateMonthlyOvertimeHours(userid, (int)shiftId, Currentdate),
                    TotalWorkingHours = await CalculateTotalWorkingHours(userid, Currentdate),
                    MonthlyWorkingHours = await CalculateMonthlyWorkingHours(userid, Currentdate),
                    Presencepercentage = await CalculatePresencePercentage(userid, Currentdate),
                    absencepercentage = await CalculateAbsencePercentage(userid, Currentdate),
                    Currentdate = Currentdate.ToString("dd-MM-yyyy"),
                    LoginStatus = latestLoginStatus,
                    loginactivities = await GetLoginBreakActivities(userid, Currentdate),
                    Ontime = lateness,
                    dayPart = GetPartOfDay(DateTime.Now.TimeOfDay),
                    WFHStatus = WFHStatus,
                    NumberofWFH = wfhCount,

                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetEmpattendance: " + ex.Message);
            }
        }

        private async Task<List<Breakactivity>> GetLoginBreakActivities(string employeeId, DateTime Currentdate)
        {

            var breakRecordsIn = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId && g.Breakin == true && g.CheckInTime.HasValue && g.CheckInTime.Value.Date == Currentdate.Date)
                .OrderBy(g => g.CheckInTime)
                .ToListAsync();

            var breakRecordsOut = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId && g.Breakout == true && g.CheckInTime.HasValue && g.CheckInTime.Value.Date == Currentdate.Date)
                .OrderBy(g => g.CheckInTime)
                .ToListAsync();

            var loginActivities = new List<Breakactivity>();
            int index = 0;

            foreach (var breakInRecord in breakRecordsIn)
            {
                var loginActivity = new Breakactivity
                {
                    BreakIN = breakInRecord.CheckInTime.HasValue
                        ? breakInRecord.CheckInTime.Value.ToString("hh:mm tt")
                        : "N/A",
                    BreakOut = "N/A",
                    LoginStatus = "Check-In",
                    loginactivities = null
                };
                if (index < breakRecordsOut.Count)
                {
                    var breakOutRecord = breakRecordsOut[index];

                    if (breakOutRecord.Breakout == true)
                    {
                        loginActivity.BreakOut = breakOutRecord.CheckInTime.HasValue
                            ? breakOutRecord.CheckInTime.Value.ToString("hh:mm tt")
                            : "N/A";
                        if (breakOutRecord.CheckIn == false)
                        {
                            loginActivity.LoginStatus = "Check-Out";
                        }
                        else
                        {
                            loginActivity.LoginStatus = "Check-In";
                        }
                    }
                }

                loginActivities.Add(loginActivity);
                index++;
            }
            if (loginActivities.Any())
            {
                var lastActivity = loginActivities.Last();
                if (lastActivity.BreakOut == "N/A" && breakRecordsIn.Last().CheckOutTime == null)
                {
                    lastActivity.LoginStatus = "Check-In";
                }
            }

            return loginActivities;
        }

        private async Task<string> CalculatePresencePercentage(string employeeId, DateTime currentDate)
        {
            var emp = await _context.EmployeeRegistrations
                .Where(x => x.EmployeeId == employeeId)
                .FirstOrDefaultAsync();

            if (emp == null)
            {
                return "0%";
            }

            var attendanceDays = await _context.Attendancedays
                .Where(x => x.Vendorid == emp.Vendorid)
                .FirstOrDefaultAsync();

            if (attendanceDays == null || string.IsNullOrEmpty(attendanceDays.Nodays)
                || !int.TryParse(attendanceDays.Nodays, out int totalAttendanceDays) || totalAttendanceDays <= 0)
            {
                return "0%";
            }

            int totalDaysWorked = await _context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId
                            && g.CurrentDate.Value.Month == currentDate.Month
                            && g.CurrentDate.Value.Year == currentDate.Year)
                .CountAsync();

            decimal presencePercentage = (decimal)totalDaysWorked / totalAttendanceDays * 100;
            return Math.Round(presencePercentage).ToString() + "%";
        }

        private async Task<string> CalculateAbsencePercentage(string employeeId, DateTime currentDate)
        {
            var emp = await _context.EmployeeRegistrations
                .Where(x => x.EmployeeId == employeeId)
                .FirstOrDefaultAsync();

            if (emp == null)
            {
                return "0%";
            }

            var attendanceDays = await _context.Attendancedays
                .Where(x => x.Vendorid == emp.Vendorid)
                .FirstOrDefaultAsync();

            if (attendanceDays == null || string.IsNullOrEmpty(attendanceDays.Nodays)
                || !int.TryParse(attendanceDays.Nodays, out int totalAttendanceDays) || totalAttendanceDays <= 0)
            {
                return "0%";
            }

            int totalDaysWorked = await _context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId
                            && g.CurrentDate.Value.Month == currentDate.Month
                            && g.CurrentDate.Value.Year == currentDate.Year)
                .CountAsync();

            int daysMissed = totalAttendanceDays - totalDaysWorked;
            decimal absencePercentage = (decimal)daysMissed / totalAttendanceDays * 100;
            return Math.Round(absencePercentage).ToString() + "%";
        }

        private static async Task<string> CalculateStartOverTime(string employeeId, int officeShiftId, admin_NDCrMContext context, DateTime Currentdate)
        {

            var checkInData = await context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId && g.CheckIntime.Value.Date == Currentdate.Date)
                .OrderByDescending(g => g.Id)
                .FirstOrDefaultAsync();

            var officeHour = await context.Officeshifts
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
        private static async Task<string> CalculateFinishOverTime(string employeeId, int officeShiftId, admin_NDCrMContext context, DateTime Currentdate)
        {
            var officeHour = await context.Officeshifts
                .Where(h => h.Id == officeShiftId)
                .Select(h => h.Endtime)
                .FirstOrDefaultAsync();

            if (DateTime.TryParse(officeHour, out DateTime endTime))
            {
                var checkInData = await context.EmployeeCheckInRecords
                    .Where(g => g.EmpId == employeeId && g.CheckIntime.Value.Date == Currentdate.Date)
                    .OrderByDescending(g => g.Id)
                    .FirstOrDefaultAsync();

                if (checkInData != null && checkInData.CheckOuttime.HasValue)
                {
                    var checkOutTime = checkInData.CheckOuttime.Value;
                    if (checkOutTime > endTime)
                    {
                        return checkOutTime.ToString("hh:mm tt");
                    }
                    else
                    {
                        return "N/A";
                    }
                }
            }

            return "N/A";
        }
        //private async Task<string> CalculateTotalWorkingHours(string employeeId, DateTime currentdate)
        //{
        //    var checkIns = await _context.EmployeeCheckInRecords
        //        .Where(g => g.EmpId == employeeId && g.CurrentDate.HasValue && g.CurrentDate.Value.Date == currentdate.Date)
        //        .ToListAsync();

        //    double totalHours = 0;

        //    foreach (var checkInRecord in checkIns.ToList())
        //    {
        //        if (checkInRecord.CheckIntime.HasValue)
        //        {
        //            if (currentdate.Date != DateTime.Now.Date)
        //            {
        //                var checkOutRecord = checkIns.FirstOrDefault(g => g.CheckOuttime.HasValue && g.CheckOuttime > checkInRecord.CheckIntime);
        //                if (checkOutRecord != null)
        //                {
        //                    totalHours += (checkOutRecord.CheckOuttime.Value - checkInRecord.CheckIntime.Value).TotalHours;
        //                    checkIns.Remove(checkOutRecord);
        //                }
        //            }
        //            else
        //            {
        //                var checkRecord = await _context.EmployeeCheckIns
        //                    .Where(x => x.EmployeeId == employeeId && x.Currentdate.Value.Date == DateTime.Now.Date)
        //                    .OrderByDescending(x => x.Id)
        //                    .FirstOrDefaultAsync();

        //                if (checkRecord.CheckIn == true)
        //                {
        //                    totalHours += (DateTime.Now - checkInRecord.CheckIntime.Value).TotalHours;
        //                }
        //                else
        //                {
        //                    var checkOutRecord = checkIns.FirstOrDefault(g => g.CheckOuttime.HasValue && g.CheckOuttime > checkInRecord.CheckIntime);
        //                    if (checkOutRecord != null)
        //                    {
        //                        totalHours += (checkOutRecord.CheckOuttime.Value - checkInRecord.CheckIntime.Value).TotalHours;
        //                        checkIns.Remove(checkOutRecord);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return FormatHours(totalHours);
        //}
        private async Task<string> CalculateTotalWorkingHours(string employeeId, DateTime currentdate)
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
                .Where(g => g.EmpId == employeeId && g.CurrentDate.HasValue && g.CurrentDate.Value.Date == currentdate.Date)
                .OrderBy(g => g.CheckIntime)
                .ToListAsync();

            double totalHours = 0;
            double totalBreakHours = 0;
            DateTime? lastBreakInTime = null;
            var latestCheckRecord = await _context.EmployeeCheckIns
                .Where(x => x.EmployeeId == employeeId && x.Currentdate.HasValue && x.Currentdate.Value.Date == DateTime.Now.Date)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            foreach (var checkInRecord in checkIns)
            {
                if (checkInRecord.CheckIntime.HasValue)
                {
                    if (currentdate.Date != DateTime.Now.Date)
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
                            .Where(g => g.EmployeeId == employeeId && g.Currentdate.HasValue && g.Currentdate.Value.Date == currentdate.Date)
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
                                if (breakRecord.CheckInTime.HasValue && breakRecord.CheckInTime.Value.Date == currentdate.Date)
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
        private async Task<string> CalculateMonthlyWorkingHours(string employeeId, DateTime Currentdate)
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
                .Where(g => g.EmpId == employeeId && g.CurrentDate.Value.Month == Currentdate.Month)
                .OrderBy(g => g.CheckIntime)
                .ToListAsync();

            double totalHours = 0;
            double totalBreakHours = 0;
            DateTime? lastBreakInTime = null;


            var breakRecords = await _context.EmployeeCheckIns
                           .Where(g => g.EmployeeId == employeeId && g.Currentdate.HasValue && g.Currentdate.Value.Month == Currentdate.Month)
                           .Where(g => g.Breakin == true || g.Breakout == true)
                           .OrderBy(g => g.CheckInTime)
                           .ToListAsync();

            double monthlyHours = 0;

            for (int i = 0; i < checkIns.Count; i++)
            {
                var checkInRecord = checkIns[i];
                if (checkInRecord.CheckIntime.HasValue)
                {
                    var checkOutRecord = checkIns.FirstOrDefault(g => g.CheckIntime == checkInRecord.CheckIntime && g.CheckOuttime.HasValue && g.CheckOuttime > checkInRecord.CheckIntime);
                    if (checkOutRecord != null)
                    {
                        monthlyHours += (checkOutRecord.CheckOuttime.Value - checkInRecord.CheckIntime.Value).TotalHours;
                    }
                }
                foreach (var breakRecord in breakRecords)
                {
                    if (breakRecord.Breakin == true && checkInRecord.CheckIntime.HasValue)
                    {
                        lastBreakInTime = breakRecord.CheckInTime;
                    }
                    else if (breakRecord.Breakout == true && lastBreakInTime.HasValue)
                    {
                        if (breakRecord.CheckInTime.HasValue && breakRecord.CheckInTime.Value.Month == Currentdate.Month)
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
                double netWorkingHours = monthlyHours - totalBreakHours;

            }

            return FormatHours(monthlyHours);
        }

        private async Task<string> CalculateMonthlyOvertimeHours(string employeeId, int officeShiftId, DateTime Currentdate)
        {
            var officeHour = await _context.Officeshifts
                .Where(h => h.Id == officeShiftId)
                .FirstOrDefaultAsync();

            if (officeHour == null || !DateTime.TryParse(officeHour.Starttime, out DateTime startTime) || !DateTime.TryParse(officeHour.Endtime, out DateTime endTime))
            {
                return "Invalid shift times";
            }


            TimeSpan workingHours = endTime - startTime;
            decimal totalWorkingHours = (decimal)workingHours.TotalHours;
            decimal formattedWorkingHours = Math.Round(totalWorkingHours, 2);

            var checkIns = await _context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId && g.CheckIntime.HasValue && g.CheckIntime.Value.Month == Currentdate.Month)
                .ToListAsync();

            decimal totalWorkedHours = 0;

            foreach (var checkInRecord in checkIns)
            {
                if (checkInRecord.CheckIntime.HasValue && checkInRecord.CheckOuttime.HasValue)
                {
                    decimal workedHours = (decimal)(checkInRecord.CheckOuttime.Value - checkInRecord.CheckIntime.Value).TotalHours - (formattedWorkingHours);
                    totalWorkedHours += workedHours;
                }
            }
            decimal totalOvertimeHours = totalWorkedHours;

            if (totalOvertimeHours > 0)
            {

                return FormatHoursd(totalOvertimeHours > 0 ? totalOvertimeHours : 0);
            }
            else
            {
                return "N/A";
            }
        }
        private string FormatHoursd(decimal hours)
        {
            int wholeHours = (int)hours;
            decimal minutes = (hours - wholeHours) * 60;
            return $"{wholeHours}h{Math.Round(minutes)}m";
        }

        private string FormatHours(double totalHours)
        {
            int hours = (int)totalHours;
            int minutes = (int)((totalHours - hours) * 60);
            return $"{hours}h{minutes}m";
        }
        public async Task<List<TasksassignDto>> GetEmpTasksassign(string userid)
        {
            try
            {
                var taskassign = await _context.EmployeeTasks.Where(x => x.EmployeeId == userid).Select(x => new TasksassignDto
                {
                    Id = x.Id,
                    TaskName = x.Task,
                    TaskTittle = x.Tittle,
                    taskstartdate = x.Startdate,
                    taskEnddate = x.Enddate,
                    TaskDescription = x.Description,
                    TaskStatus = _context.TaskStatuses.Where(a => a.Id == x.Status).Select(status => status.StatusName).FirstOrDefault(),
                }).ToListAsync();

                return taskassign;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<TasksassignnameDto> GetEmpTasksassignname(string userid, int id)
        {
            try
            {
                var result = await (from emp in _context.EmployeeTasks
                                    join status in _context.TaskStatuses on emp.Status equals status.Id
                                    where emp.Id == id && emp.EmployeeId == userid
                                    select new
                                    {
                                        emp.Id,
                                        emp.Tittle,
                                        emp.Description,
                                        StatusName = status.StatusName,
                                        Duration = $"{emp.Startdate.Value.ToString("dd/MM/yyyy")} - {emp.Enddate.Value.ToString("dd/MM/yyyy")}"
                                    }).FirstOrDefaultAsync();

                if (result == null)
                {
                    throw new Exception("Task not found.");
                }
                var taskassign = await (from taskList in _context.EmployeeTasksLists
                                        join taskStatus in _context.TaskStatuses on taskList.TaskStatus equals taskStatus.Id
                                        where taskList.EmployeeId == userid && taskList.Emptaskid == id
                                        select new TasksassignlistDto
                                        {
                                            Id = taskList.Id,
                                            TasksubTittle = taskList.Taskname,
                                            TaskStatus = result.StatusName == "Completed" ? "Completed" : taskStatus.StatusName
                                        }).ToListAsync();
                var tasksAssignNameDto = new TasksassignnameDto
                {
                    Id = result.Id,
                    TaskTittle = result.Tittle,
                    TaskDescription = result.Description,
                    Status = result.StatusName,
                    Duration = result.Duration,
                    Empdata = taskassign
                };

                return tasksAssignNameDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message, ex);
            }
        }
        public async Task<EmpTasksList> CompletedempTasksassign(EmpTasksListDto model, string userid)
        {
            try
            {
                var empsubtask = await _context.EmployeeTasks
                    .FirstOrDefaultAsync(x => x.Id == model.taskid && x.EmployeeId == userid);

                if (empsubtask == null)
                {
                    throw new Exception("Subtask not found for the given employee.");
                }

                var emptyTasks = await _context.EmployeeTasksLists
                    .Where(x => x.Emptaskid == empsubtask.Id && x.EmployeeId == userid)
                    .ToListAsync();

                if (!emptyTasks.Any())
                {
                    throw new Exception("No empty tasks found for the given employee.");
                }

                var createdTasksList = new List<EmpTasksList>();
                foreach (var task in emptyTasks)
                {
                    var subtasksList = new EmpTasksList
                    {
                        Subtaskid = empsubtask.Id,
                        Taskid = task.Id,
                        Taskstatus = 3,
                        IsApprove = false,
                        EmployeeId = userid,
                        Replydate = DateTime.Now.Date,
                    };

                    _context.EmpTasksLists.Add(subtasksList);
                    createdTasksList.Add(subtasksList);
                    task.TaskStatus = 3;

                    var entry1 = _context.Entry(task);
                    if (entry1.State == EntityState.Detached)
                    {
                        _context.EmployeeTasksLists.Attach(task);
                    }
                }
                empsubtask.Status = 3;
                var entry = _context.Entry(empsubtask);
                if (entry.State == EntityState.Detached)
                {
                    _context.EmployeeTasks.Attach(empsubtask);
                }
                await _context.SaveChangesAsync();

                return createdTasksList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message, ex);
            }
        }
        public async Task<EmpTasksList> UnCompletedempTasksassign(EmpSubTasksListDto model, string userid)
        {
            try
            {
                var empttask = await _context.EmployeeTasksLists
                    .FirstOrDefaultAsync(x => x.Id == model.subtaskid && x.EmployeeId == userid);

                if (empttask != null)
                {
                    var tasksList = new EmpTasksList
                    {

                        Subtaskid = empttask.Emptaskid,
                        Taskid = model.subtaskid,
                        Taskreason = model.Taskreason,
                        Taskstatus = 6,
                        IsApprove = false,
                        EmployeeId = userid,
                        Replydate = DateTime.Now.Date,
                    };
                    _context.EmpTasksLists.Add(tasksList);
                    await _context.SaveChangesAsync();
                    return tasksList;
                }
                throw new Exception("Task or subtask not found for the given employee.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message, ex);
            }
        }
        public async Task<EmpTasksList> Tasksassign(TasksListDto model, string userid)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "TasksListDto cannot be null.");
            }

            List<EmpTasksList> tasksToAdd = new List<EmpTasksList>();

            try
            {
                if (model.taskid != null && model.taskid.Length > 0 && !string.IsNullOrWhiteSpace(model.taskid[0]))
                {
                    var taskIds = model.taskid[0]
                        .Replace("[", string.Empty)
                        .Replace("]", string.Empty)
                        .Split(",")
                        .Select(id => id.Trim())
                        .Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .ToList();

                    if (taskIds.Any())
                    {
                        var emptTasks = await _context.EmployeeTasksLists
                            .Where(x => taskIds.Contains(x.Id) && x.EmployeeId == userid)
                            .ToListAsync();

                        foreach (var emptTask in emptTasks)
                        {
                            var tasksList = new EmpTasksList
                            {
                                Subtaskid = emptTask.Emptaskid,
                                Taskid = emptTask.Id,
                                Taskreason = model.Taskreason,
                                Taskstatus = GetTaskStatus(model.Taskstatus),
                                IsApprove = false,
                                EmployeeId = userid,
                                Replydate = DateTime.UtcNow.Date,
                            };

                            tasksToAdd.Add(tasksList);
                            emptTask.TaskStatus = GetTaskStatus(model.Taskstatus);

                            var entry = _context.Entry(emptTask);
                            if (entry.State == EntityState.Detached)
                            {
                                _context.EmployeeTasksLists.Attach(emptTask);
                            }
                            if (taskIds.Any())
                            {
                                foreach (var subtaskid in taskIds)
                                {
                                    var empSubtask = await _context.EmployeeTasks
                                        .FirstOrDefaultAsync(x => x.Id == subtaskid && x.EmployeeId == userid);

                                    if (empSubtask != null)
                                    {
                                        var existingTask = await _context.EmployeeTasksLists
                                            .Where(x => x.Emptaskid == empSubtask.Id && x.EmployeeId == userid && x.TaskStatus == 3)
                                            .ToListAsync();

                                        empSubtask.Status = existingTask.Count > 0 ? 3 : 1;

                                        var empSubtaskEntry = _context.Entry(empSubtask);
                                        if (empSubtaskEntry.State == EntityState.Detached)
                                        {
                                            _context.EmployeeTasks.Attach(empSubtask);
                                        }
                                        empSubtaskEntry.State = EntityState.Modified;
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
                }
                if (model.subtaskid.HasValue && tasksToAdd.Count == 0)
                {
                    var empSubtask = await _context.EmployeeTasks
                        .FirstOrDefaultAsync(x => x.Id == model.subtaskid.Value && x.EmployeeId == userid);

                    if (empSubtask != null)
                    {
                        var emptyTasks = await _context.EmployeeTasksLists
                            .Where(x => x.Emptaskid == empSubtask.Id && x.EmployeeId == userid)
                            .ToListAsync();

                        if (emptyTasks.Any())
                        {
                            foreach (var task in emptyTasks)
                            {
                                var subtasksList = new EmpTasksList
                                {
                                    Subtaskid = model.subtaskid.Value,
                                    Taskid = task.Id,
                                    Taskreason = model.Taskreason,
                                    Taskstatus = GetTaskStatus(model.Taskstatus),
                                    IsApprove = false,
                                    EmployeeId = userid,
                                    Replydate = DateTime.UtcNow.Date,
                                };
                                tasksToAdd.Add(subtasksList);
                                task.TaskStatus = GetTaskStatus(model.Taskstatus);

                                var entry = _context.Entry(task);
                                if (entry.State == EntityState.Detached)
                                {
                                    _context.EmployeeTasksLists.Attach(task);
                                }
                            }
                        }
                    }
                }
                if (tasksToAdd.Count > 0)
                {
                    await _context.EmpTasksLists.AddRangeAsync(tasksToAdd);
                }

                await _context.SaveChangesAsync();
                if (model.subtaskid.HasValue)
                {
                    var empSubtask = await _context.EmployeeTasks
                        .FirstOrDefaultAsync(x => x.Id == model.subtaskid.Value && x.EmployeeId == userid);

                    if (empSubtask != null)
                    {
                        var existingTask = await _context.EmpTasksLists
                            .Where(x => x.Subtaskid == empSubtask.Id && x.EmployeeId == userid && x.Taskstatus == 3)
                            .ToListAsync();

                        empSubtask.Status = existingTask.Count > 0 ? 3 : 1;

                        var empSubtaskEntry = _context.Entry(empSubtask);
                        if (empSubtaskEntry.State == EntityState.Detached)
                        {
                            _context.EmployeeTasks.Attach(empSubtask);
                        }
                        empSubtaskEntry.State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                return tasksToAdd.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private int GetTaskStatus(string status)
        {
            return status switch
            {
                "Completed" => 3,
                "UnCompleted" => 6,
                _ => 0,
            };
        }
        public async Task<EmpTasksList> SubTaskCompletedempTasksassign(EmpSubTasksDto model, string userid)
        {
            try
            {
                var empttask = await _context.EmployeeTasksLists
                    .FirstOrDefaultAsync(x => x.Id == model.subtaskid && x.EmployeeId == userid);

                if (empttask != null)
                {
                    var tasksList = new EmpTasksList
                    {

                        Subtaskid = empttask.Emptaskid,
                        Taskid = model.subtaskid,
                        Taskstatus = 3,
                        IsApprove = false,
                        EmployeeId = userid,
                        Replydate = DateTime.Now.Date,
                    };
                    _context.EmpTasksLists.Add(tasksList);
                    await _context.SaveChangesAsync();
                    return tasksList;
                }
                throw new Exception("Task or subtask not found for the given employee.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message, ex);
            }
        }
        public async Task<List<WebLoginactivity>> GetWebEmpLoginactivity(string userid)
        {
            try
            {
                if (string.IsNullOrEmpty(userid))
                {
                    throw new ArgumentException("User ID cannot be null or empty.");
                }
                var attendanceRecords = await _context.EmployeeCheckInRecords
                   .Where(g => g.EmpId == userid).OrderByDescending(g => g.Id)
                   .ToListAsync();
                if (!attendanceRecords.Any())
                {
                    return new List<WebLoginactivity>();
                }

                List<WebLoginactivity> loginActivities = new List<WebLoginactivity>();


                foreach (var checkInRecord in attendanceRecords)
                {
                    var checkOutRecord = attendanceRecords
                        .FirstOrDefault(co => co.CurrentDate == checkInRecord.CurrentDate);

                    WebLoginactivity loginActivity = new WebLoginactivity
                    {
                        CheckIN = checkInRecord.CheckIntime.HasValue
                            ? checkInRecord.CheckIntime.Value.ToString("hh:mm tt")
                            : "N/A",
                        CheckOut = checkOutRecord?.CheckOuttime.HasValue == true
                            ? checkOutRecord.CheckOuttime.Value.ToString("hh:mm tt")
                            : "N/A",
                        Currentdate = checkInRecord.CurrentDate.HasValue
                            ? checkInRecord.CurrentDate.Value.ToString("dd-MM-yyyy")
                            : "N/A",
                        LoginStatus = checkInRecord.CheckOuttime == null ? "Check-In" : "Check-Out"
                    };

                    loginActivities.Add(loginActivity);
                }

                return loginActivities;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving employee login activity: " + ex.Message);
            }
        }

        public async Task<MeetEventsAndHolidayDto> GetholidayandEvents(string userid)
        {
            try
            {
                MeetEventsAndHolidayDto meetEventsAndHoliday = new MeetEventsAndHolidayDto();
                var empdetail = await _context.EmployeeRegistrations
                    .Where(x => x.EmployeeId == userid)
                    .FirstOrDefaultAsync();
                meetEventsAndHoliday.officeEventsDtos = await _context.OfficeEvents
                    .Where(x => x.Vendorid == empdetail.Vendorid).Select(x => new officeEventsDto
                    {
                        Subtittle = x.Subtittle,
                        Tittle = x.Tittle,
                        Date = x.Date.Value.Date,
                    }).ToListAsync();
                meetEventsAndHoliday.meetEventsDtos = await _context.EventsmeetSchedulers
                    .Where(x => x.Vendorid == empdetail.Vendorid).Select(x => new MeetEventsDto
                    {
                        EventTittle = x.Tittle,
                        Eventdate = x.ScheduleDate.Value.Date,
                        EventType = x.IsEventsmeet == true ? "Meet" : "Event",
                        EventTime = x.Time,
                        EventDescription = ExtractMeetingLink(x.Description)/*x.Description*/,
                    }).ToListAsync();
                return meetEventsAndHoliday;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<List<officeEventsDto>> GetOfficeEvents(string userid)
        {
            try
            {
                var empdetail = await _context.EmployeeRegistrations
                    .Where(x => x.EmployeeId == userid)
                    .FirstOrDefaultAsync();
                var officeEventsDtos = await _context.OfficeEvents
                    .Where(x => x.Vendorid == empdetail.Vendorid).Select(x => new officeEventsDto
                    {
                        Subtittle = x.Subtittle,
                        Tittle = x.Tittle,
                        Date = x.Date.Value.Date,
                    }).ToListAsync();

                return officeEventsDtos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<Monthlyattendancedatail> GetMonthAttanceDetails(string userid)
        {
            try
            {
                var emp = await _context.EmployeeRegistrations
                    .Where(x => x.EmployeeId == userid)
                    .FirstOrDefaultAsync();

                if (emp == null)
                {
                    return new Monthlyattendancedatail
                    {
                        TotalWorkingDays = 0,
                        TotalPresentDays = 0,
                        TotalAbsentDays = 0,
                        Attendance = "0%"
                    };
                }

                var attendancedays = await _context.Attendancedays
                    .Where(x => x.Vendorid == emp.Vendorid)
                    .FirstOrDefaultAsync();

                if (attendancedays == null || string.IsNullOrEmpty(attendancedays.Nodays) ||
                    !int.TryParse(attendancedays.Nodays, out int monthlyWorkingDays) || monthlyWorkingDays <= 0)
                {
                    return new Monthlyattendancedatail
                    {
                        TotalWorkingDays = 0,
                        TotalPresentDays = 0,
                        TotalAbsentDays = 0,
                        Attendance = "0%"
                    };
                }
                int daysPassedInMonth = DateTime.Now.Day;
                int totalAttendanceDays = Math.Min(daysPassedInMonth, monthlyWorkingDays);

                //var totalDaysWorked = await _context.EmployeeCheckInRecords
                //    .Where(g => g.EmpId == userid && g.CurrentDate.Value.Month == DateTime.Now.Month && g.CurrentDate.Value.Year == DateTime.Now.Year)
                //    .CountAsync();

                //decimal presencePercentage = (decimal)totalDaysWorked / monthlyWorkingDays * 100;
                //int totalAbsentDays = totalAttendanceDays - totalDaysWorked;
                var totalDaysWorked = (await _context.EmployeeCheckInRecords
                  .Where(g => g.EmpId == userid
                              && g.CurrentDate.Value.Month == DateTime.Now.Month
                              && g.CurrentDate.Value.Year == DateTime.Now.Year)
                  .ToListAsync())
                  .Count();

                var leaveRecords = await _context.ApplyLeaveNews
      .Where(l => l.UserId == userid
                  && l.StartDate.Month == DateTime.Now.Month
                  && l.StartDate.Year == DateTime.Now.Year
                  && l.StartDate.Day <= DateTime.Now.Day
                  && l.Isapprove == 2)
      .ToListAsync();


                int leaveDays = leaveRecords.Sum(l => (l.EndDate - l.StartDate).Days + 1);

                decimal presencePercentage = (decimal)totalDaysWorked / monthlyWorkingDays * 100;

                int totalAbsentDays = leaveDays;
                return new Monthlyattendancedatail
                {
                    TotalWorkingDays = monthlyWorkingDays,
                    TotalPresentDays = totalDaysWorked,
                    TotalAbsentDays = totalAbsentDays,
                    Attendance = Math.Round(presencePercentage).ToString() + "%"
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<TotalLeave> Getleavelist(string userid)
        {
            try
            {
                decimal totalFullday = 0.00m;
                var toleave = await _context.ApplyLeaveNews
                    .Where(p => p.UserId == userid && p.Isapprove == 2)
                    .Select(p => new
                    {
                        Id = p.Id,
                        CountLeave = p.CountLeave,
                        PaidCountLeave = p.PaidCountLeave,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        StartLeaveId = p.StartLeaveId,
                        EndeaveId = p.EndeaveId,
                        Reason = p.Reason,
                        CreatedDate = p.CreatedDate,
                        TypeofLeaveid = p.TypeOfLeaveId,
                    }).ToListAsync();

                if (!toleave.Any())
                {
                    throw new Exception("No leave application found for the specified user.");
                }
                TotalLeave fd = new TotalLeave();
                decimal totalLeaves = toleave.Sum(x => (decimal)(x.CountLeave + x.PaidCountLeave));

                fd.TotalLeaves = totalLeaves;
                fd.Type = new List<TotalLeavelist>();

                foreach (var l in toleave)
                {
                    totalFullday = (l.EndDate - l.StartDate).Days - (l.EndDate != l.StartDate ? 1 : 0);
                    totalFullday = Math.Max(totalFullday, 0);
                    var leaveType = GetLeaveType(l.StartLeaveId, l.EndeaveId, totalFullday, l.CountLeave, (decimal)l.PaidCountLeave);
                    var TypeofLeave = await _context.LeaveTypes.Where(s => s.Id == l.TypeofLeaveid)
                                .Select(s => s.Leavetype1)
                                .FirstOrDefaultAsync() ?? "Unknown";
                    fd.Type.Add(new TotalLeavelist
                    {
                        id = l.Id,
                        Leavedate = l.CreatedDate,
                        Reason = l.Reason,
                        Nodays = (decimal)(l.CountLeave + l.PaidCountLeave),
                        LeaveType = leaveType,
                        Leaveapplydate = l.CreatedDate.ToString("dd MMMM yyyy"),
                        LeaveSearchdate = l.CreatedDate.ToString("MMM-yy"),
                        TypeofLeave = TypeofLeave,
                    });
                }

                return fd;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        private static string? GetLeaveType(int startLeaveId, int endLeaveId, decimal totalFullday, decimal countLeave, decimal paidCountLeave)
        {
            int halfDayCount = 0;
            int fullDayCount = (int)totalFullday;

            if (startLeaveId == endLeaveId)
            {
                if (startLeaveId == 1 || startLeaveId == 2)
                {
                    halfDayCount++;
                }
                else if (startLeaveId == 3)
                {
                    fullDayCount++;
                }
            }
            else
            {
                if (startLeaveId == 1 || startLeaveId == 2) halfDayCount++;
                if (startLeaveId == 3) fullDayCount++;

                if (endLeaveId == 1 || endLeaveId == 2) halfDayCount++;
                if (endLeaveId == 3) fullDayCount++;
            }

            List<string> leaveTypes = new List<string>();

            if (halfDayCount > 0)
            {
                leaveTypes.Add($"{halfDayCount} Half Day{(halfDayCount > 1 ? "s" : "")}");
            }
            if (fullDayCount > 0)
            {
                leaveTypes.Add($"{fullDayCount} Full Day{(fullDayCount > 1 ? "s" : "")}");
            }

            decimal totalLeave = countLeave + paidCountLeave;
            leaveTypes.Add($"(Total Leaves: {totalLeave})");

            return string.Join(", ", leaveTypes);
        }
        public async Task<getTotalLeave> GetEmptotalleave(string userid, int id)
        {
            try
            {
                var getleave = _context.ApplyLeaveNews.Where(x => x.UserId == userid && x.Id == id).Select(x => new getTotalLeave
                {
                    Reason = x.Reason,
                    Totaldays = x.CountLeave + x.PaidCountLeave,
                    PaidLeave = x.CountLeave,
                    UnPaidLeave = x.PaidCountLeave,
                    LeaveStartdate = x.StartDate.ToString("dd MMM yyyy"),
                    LeaveEnddate = x.EndDate.ToString("dd MMM yyyy"),

                }).FirstOrDefault();
                return getleave;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message, ex);
            }
        }
        public async Task<getattendancegraph> GetEmpGraph(string userid)
        {
            try
            {
                int currentYear = DateTime.Now.Year;

                var monthlyData = await _context.EmployeeCheckInRecords
                    .Where(x => x.EmpId == userid && x.CurrentDate.HasValue && x.CurrentDate.Value.Year == currentYear)
                    .GroupBy(x => new
                    {
                        Year = x.CurrentDate.Value.Year,
                        Month = x.CurrentDate.Value.Month
                    })
                    .Select(g => new getattendancegraphlist
                    {
                        Month = getMonthName(g.Key.Month),
                        Value = g.Count()
                    }).ToListAsync();

                var graph = new getattendancegraph
                {
                    Year = currentYear.ToString(),
                    graphlist = monthlyData
                };

                return graph;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving employee attendance graph: " + ex.Message, ex);
            }
        }

        public async Task<getTasklist> gettasklist(string userid)
        {
            try
            {
                var employeeTasks = await (from task in _context.EmployeeTasks
                                           join status in _context.TaskStatuses
                                           on task.Status equals status.Id
                                           where task.EmployeeId == userid
                                           select new
                                           {
                                               task.Id,
                                               task.Task,
                                               StatusName = status.StatusName,
                                               task.Startdate,
                                               task.Enddate
                                           }).ToListAsync();
                var reassignedTasks = new List<getReassignedTasklist>();
                var completedTasks = new List<getCompletedTasklist>();
                var uncompletedTasks = new List<getUnCompletedTasklist>();

                foreach (var task in employeeTasks)
                {
                    if (task.StatusName == "Reassigned")
                    {
                        reassignedTasks.Add(new getReassignedTasklist
                        {
                            id = task.Id,
                            Taskname = task.Task,
                            Duration = $"{task.Startdate.Value.ToString("dd/MM/yyyy")} - {task.Enddate.Value.ToString("dd/MM/yyyy")}",
                            status = task.StatusName
                        });
                    }
                    else if (task.StatusName == "Completed")
                    {
                        completedTasks.Add(new getCompletedTasklist
                        {
                            id = task.Id,
                            Taskname = task.Task,
                            Duration = $"{task.Startdate.Value.ToString("dd/MM/yyyy")} - {task.Enddate.Value.ToString("dd/MM/yyyy")}",
                            status = task.StatusName
                        });
                    }
                    else if (task.StatusName == "UnCompleted")
                    {
                        uncompletedTasks.Add(new getUnCompletedTasklist
                        {
                            id = task.Id,
                            Taskname = task.Task,
                            Duration = $"{task.Startdate.Value.ToString("dd/MM/yyyy")} - {task.Enddate.Value.ToString("dd/MM/yyyy")}",
                            status = task.StatusName
                        });
                    }
                }

                return new getTasklist
                {
                    Reassigned = reassignedTasks,
                    Completed = completedTasks,
                    UnCompleted = uncompletedTasks
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving employee tasks: " + ex.Message, ex);
            }
        }
        public async Task<List<TasksassignlistDto>> getSubtasklist(string userid, int? id)
        {
            try
            {
                var taskassign = await (from taskList in _context.EmployeeTasksLists
                                        join taskStatus in _context.TaskStatuses on taskList.TaskStatus equals taskStatus.Id
                                        join task in _context.EmployeeTasks on taskList.Emptaskid equals task.Id
                                        where taskList.EmployeeId == userid && taskList.Emptaskid == id
                                        select new TasksassignlistDto
                                        {
                                            Id = taskList.Id,
                                            TaskTittle = task.Tittle,
                                            TasksubTittle = taskList.Taskname,
                                            TaskStatus = taskStatus.StatusName,
                                            TaskDescription = task.Description,
                                            Duration = $"{task.Startdate.Value.ToString("dd/MM/yyyy")} - {task.Enddate.Value.ToString("dd/MM/yyyy")}"
                                        }).ToListAsync();


                return taskassign;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message, ex);
            }
        }
        public async Task<List<AppFaq>> Getappfaq()
        {
            try
            {
                var faq = await _context.AppFaqs.ToListAsync();
                return faq;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        private static string ExtractMeetingLink(string htmlDescription)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlDescription);
            var linkNode = doc.DocumentNode.SelectSingleNode("//a[@href]");
            return linkNode?.GetAttributeValue("href", null) ?? string.Empty;
        }

        public async Task<bool> Overtimeapply(string userid)
        {
            try
            {
                var employeeOvertime = new EmployeeOvertime()
                {
                    EmployeeId = userid,
                    ApprovalDate = DateTime.Now,
                    Approved = false
                };
                _context.Add(employeeOvertime);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public string GetPartOfDay(TimeSpan timeOfDay)
        {
            if (timeOfDay >= new TimeSpan(5, 0, 0) && timeOfDay < new TimeSpan(12, 0, 0))
            {
                return "Morning";
            }
            else if (timeOfDay >= new TimeSpan(12, 0, 0) && timeOfDay < new TimeSpan(17, 0, 0))
            {
                return "Afternoon";
            }
            else if (timeOfDay >= new TimeSpan(17, 0, 0) && timeOfDay < new TimeSpan(21, 0, 0))
            {
                return "Evening";
            }
            else
            {
                return "Night";
            }
        }
        public async Task<EmployeeCheckIn> UpdateEmpLocation(CRM.Models.APIDTO.EmpCheckIn model, bool checkIn)
        {
            try
            {
                var emp = await _context.EmployeeRegistrations
                    .FirstOrDefaultAsync(x => x.Id == model.Userid)
                    ?? throw new Exception("Employee not found.");
                var officeShift = await _context.Officeshifts
                    .FirstOrDefaultAsync(s => s.Id == emp.OfficeshiftTypeid)
                    ?? throw new Exception("Employee shift information not found.");

                var today = DateTime.Now.Date;
                await HandleEmployeeCheckIns(model, emp.EmployeeId, today, checkIn);
                await HandleEmpCheckIns(model, emp.EmployeeId, today, checkIn);
                await HandleEmployeeCheckInRecords(emp.EmployeeId, officeShift.Id, today, checkIn);

                // await transaction.CommitAsync();

                return new EmployeeCheckIn
                {
                    EmployeeId = emp.EmployeeId,
                    CurrentLat = model.CurrentLat,
                    Currentlong = model.Currentlong,
                    Currentdate = DateTime.Now,
                    CheckInTime = checkIn ? DateTime.Now : null,
                    CheckIn = checkIn
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving employee check-in data: " + ex.Message, ex);
            }
        }

        private async Task HandleEmployeeCheckIns(CRM.Models.APIDTO.EmpCheckIn model, string employeeId, DateTime today, bool checkIn)
        {
            var existingCheckInRecord = await _context.EmployeeCheckIns
                .Where(x => x.EmployeeId == employeeId && x.Currentdate.Value.Date == today)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (existingCheckInRecord != null)
            {
                if (checkIn)
                {
                    if (existingCheckInRecord?.CheckIn == true)
                    {
                        existingCheckInRecord.CurrentLat = model.CurrentLat;
                        existingCheckInRecord.Currentlong = model.Currentlong;
                        existingCheckInRecord.Updatedate = DateTime.Now;
                        _context.EmployeeCheckIns.Update(existingCheckInRecord);
                    }
                    else
                    {
                        existingCheckInRecord.Updatedate = DateTime.Now;
                        _context.EmployeeCheckIns.Update(existingCheckInRecord);
                    }
                }
                if (!checkIn)
                {
                    if (existingCheckInRecord?.CheckIn == false)
                    {
                        existingCheckInRecord.Updatedate = DateTime.Now;
                        _context.EmployeeCheckIns.Update(existingCheckInRecord);
                    }
                    else
                    {
                        await _context.EmployeeCheckIns.AddAsync(new EmployeeCheckIn
                        {
                            EmployeeId = employeeId,
                            CurrentLat = model.CurrentLat,
                            Currentlong = model.Currentlong,
                            Currentdate = DateTime.Now,
                            CheckOutTime = DateTime.Now,
                            CheckIn = false,
                            Breakin = false,
                            Breakout = false,
                            Updatedate = DateTime.Now
                        });
                    }
                }


                await _context.SaveChangesAsync();

            }
        }

        private async Task HandleEmpCheckIns(CRM.Models.APIDTO.EmpCheckIn model, string employeeId, DateTime today, bool checkIn)
        {
            var empCheckInRecord = await _context.EmpCheckIns
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Currentdate.Value.Date == today);

            if (empCheckInRecord == null)
            {
                await _context.EmpCheckIns.AddAsync(new CRM.Models.Crm.EmpCheckIn
                {
                    EmployeeId = employeeId,
                    CurrentLat = model.CurrentLat,
                    Currentlong = model.Currentlong,
                    Currentdate = DateTime.Now,
                    CheckInTime = checkIn ? DateTime.Now : null,
                    CheckOutTime = !checkIn ? DateTime.Now : null,
                    CheckIn = checkIn
                });
            }
            else
            {
                empCheckInRecord.CurrentLat = model.CurrentLat;
                empCheckInRecord.Currentlong = model.Currentlong;
                empCheckInRecord.CheckIn = checkIn;
                empCheckInRecord.CheckOutTime = !checkIn ? DateTime.Now : empCheckInRecord.CheckOutTime;
            }
            await _context.SaveChangesAsync();
        }

        private async Task HandleEmployeeCheckInRecords(string employeeId, int shiftId, DateTime today, bool checkIn)
        {
            var checkInRecord = await _context.EmployeeCheckInRecords
                .FirstOrDefaultAsync(x => x.EmpId == employeeId && x.CheckIntime.HasValue && x.CheckIntime.Value.Date == today);

            if (!checkIn)
            {
                if(checkInRecord.CheckOuttime == null)
                {
                    checkInRecord.CheckOuttime = DateTime.Now;
                    checkInRecord.Workinghour = (checkInRecord.CheckOuttime.Value - checkInRecord.CheckIntime.Value).Duration();

                }
                else if(checkInRecord.CheckOuttime != null)
                {
                    checkInRecord.Isactive = true;
                }
            }
            else
            {
                checkInRecord.Isactive = true;
            }
            _context.EmployeeCheckInRecords.Update(checkInRecord);
            await _context.SaveChangesAsync();

        }

        public async Task<bool> ApplyWfh(EmpApplyWfhDto model, string userid)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(userid))
                {
                    throw new ArgumentException("Invalid input data.");
                }

                var apply = new EmpApplywfh
                {
                    UserId = userid,
                    Startdate = model.Startdate,
                    EndDate = model.EndDate,
                    Currentdate = DateTime.Now,
                    Reason = model.Reason,
                    Iswfh = 3
                };

                await _context.EmpApplywfhs.AddAsync(apply);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred: " + ex.Message, ex);
            }
        }
        public async Task<EmployeeCheckIn> GetLATLONG(int id)
        {
            try
            {
                var empdata = _context.EmployeeLogins.Where(x => x.Id == id).FirstOrDefault();

                var LatLongdata = _context.EmployeeCheckIns.Where(x => x.EmployeeId == empdata.EmployeeId).OrderByDescending(x => x.Updatedate).FirstOrDefault();
                return LatLongdata;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message, ex);
            }
        }

    }
}
