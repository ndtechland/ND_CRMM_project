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
using System.Threading.Tasks;

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
                        FullName = x.FirstName,
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
                                             select new LeaveTypeValue
                                             {
                                                 Id = lm.LeavetypeId,
                                                 leavetype = lty.Leavetype1,
                                                 leaveValue = (decimal)lm.Value,
                                                 Isactive = lm.IsActive
                                             }).ToList();

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
                        TypeOfLeave.Value -= CountLeave;
                        await _context.SaveChangesAsync();
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
                        TypeOfLeave.Value -= CountLeave;
                        await _context.SaveChangesAsync();
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
                    Isapprove = false
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
        public async Task<EmployeeCheckIn> Empcheckin(EmpCheckIn model, bool checkIn)
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
                EmployeeCheckIn empcheck = new EmployeeCheckIn()
                {
                    EmployeeId = emp.EmployeeId,
                    CurrentLat = model.CurrentLat,
                    Currentlong = model.Currentlong,
                    Currentdate = DateTime.Now,
                };
                empcheck.Breakin = model.Breakin;
                empcheck.Breakout = model.Breakout;
                checkIn = model.Breakin == true && model.Breakout == false ? false : checkIn;
                if (!checkIn)
                {
                    empcheck.CheckIn = checkIn;
                    empcheck.CheckOutTime = DateTime.Now;

                }
                else
                {
                    empcheck.CheckInTime = DateTime.Now;
                    empcheck.CheckIn = checkIn;
                }
                await _context.EmployeeCheckIns.AddAsync(empcheck);
                await _context.SaveChangesAsync();

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

                if (todayCheckInsCount == 1)
                {
                    EmployeeCheckInRecord empch = new()
                    {
                        EmpId = emp.EmployeeId,
                        CheckIntime = fgdfd.CheckInTime,
                        CurrentDate = DateTime.Now,
                        Isactive = true,
                        Workinghour = TimeSpan.Zero
                    };
                    await _context.EmployeeCheckInRecords.AddAsync(empch);
                    await _context.SaveChangesAsync();
                }

                if (!checkIn)
                {
                    var checkInRecord = await _context.EmployeeCheckInRecords
                        .Where(x => x.EmpId == emp.EmployeeId && x.CheckIntime.Value.Date == DateTime.Now.Date)
                        .FirstOrDefaultAsync();

                    if (checkInRecord != null)
                    {
                        TimeSpan workingHours = (fgdfd.CheckInTime.Value.Date - DateTime.Now).Duration();
                        checkInRecord.Workinghour = workingHours;
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
        public async Task<EmployeeCheckIn> Empcheckout(EmpCheckIn model, bool checkIn)
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
                var newEmpCheckInRecord = new EmployeeCheckIn
                {
                    EmployeeId = emp.EmployeeId,
                    CurrentLat = model.CurrentLat,
                    Currentlong = model.Currentlong,
                    CheckIn = checkIn,
                    Currentdate = DateTime.Now,
                    CheckInTime = null,
                    CheckOutTime = DateTime.Now,
                    Breakin = false,
                    Breakout = false
                };

                _context.EmployeeCheckIns.Add(newEmpCheckInRecord);
                if (!checkIn)
                {
                    var empCheckInRecord = await _context.EmployeeCheckIns
                        .Where(x => x.EmployeeId == emp.EmployeeId && x.CheckInTime.Value.Date == DateTime.Now.Date)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefaultAsync();

                    if (empCheckInRecord != null)
                    {
                        TimeSpan workingHours = (empCheckInRecord.CheckInTime.Value.Date - DateTime.Now).Duration();
                        var checkInRecord = await _context.EmployeeCheckInRecords
                            .Where(x => x.EmpId == emp.EmployeeId && x.CheckIntime.Value.Date == DateTime.Now.Date)
                            .FirstOrDefaultAsync();

                        if (checkInRecord != null)
                        {
                            checkInRecord.Workinghour = workingHours;
                            checkInRecord.CheckOuttime = DateTime.Now;
                        }
                        else
                        {
                            EmployeeCheckInRecord newCheckInRecord = new EmployeeCheckInRecord
                            {
                                EmpId = emp.EmployeeId,
                                CheckIntime = empCheckInRecord.CheckInTime,
                                CheckOuttime = DateTime.Now,
                                Workinghour = workingHours,
                                CurrentDate = DateTime.Now,
                                Isactive = true
                            };
                            _context.EmployeeCheckInRecords.Add(newCheckInRecord);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return newEmpCheckInRecord;
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
        public async Task<Empattendancedatail> GetEmpattendance(string userid)
        {
            try
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    var empAttendanceDetails = await _context.EmployeeRegistrations
                        .Where(x => x.EmployeeId == userid)
                        .Select(x => new
                        {
                            OfficeShiftId = x.OfficeshiftTypeid,
                            EmployeeId = x.EmployeeId
                        })
                        .FirstOrDefaultAsync();

                    if (empAttendanceDetails != null)
                    {
                        if (empAttendanceDetails.OfficeShiftId.HasValue)
                        {
                            var officeShift = await _context.Officeshifts.FindAsync((int)empAttendanceDetails.OfficeShiftId.Value);
                            if (officeShift == null)
                            {
                                throw new Exception("Office shift not found.");
                            }
                            if (!DateTime.TryParse(officeShift.Endtime, out DateTime shiftEndTime))
                            {
                                throw new Exception("Invalid shift end time format.");
                            }
                            var checkIns = await _context.EmployeeCheckIns
    .Where(g => g.EmployeeId == empAttendanceDetails.EmployeeId && g.CheckInTime.HasValue && g.CheckInTime.Value.Date == DateTime.Now.Date)
    .OrderBy(g => g.Id)
    .ToListAsync();

                            var checkInTime = checkIns
                                .Select(g => g.CheckInTime.HasValue ? g.CheckInTime.Value.ToString("hh:mm tt") : "N/A")
                                .FirstOrDefault();

                            var currentDate = checkIns
                                .Select(g => g.Currentdate.HasValue ? g.Currentdate.Value.ToString("dd-MM-yyyy") : "N/A")
                                .FirstOrDefault();
                            var checkOutTime = checkIns
                                .OrderByDescending(g => g.Id)
                                .Select(g => g.CheckOutTime.HasValue ? g.CheckOutTime.Value.ToString("hh:mm tt") : "N/A")
                                .FirstOrDefault();
                            var loginActivities = await GetLoginActivities(empAttendanceDetails.EmployeeId);
                            var latestLoginStatus = loginActivities.Any() ? loginActivities.Last().LoginStatus : "No Activity";
                            var attendanceDetail = new Empattendancedatail
                            {
                                OfficeHour = $"{officeShift.Starttime} - {officeShift.Endtime}",
                                CheckInTime = checkInTime,
                                CheckOutTime = checkOutTime,
                                StartOverTime = await CalculateStartOverTime(empAttendanceDetails.EmployeeId, (int)empAttendanceDetails.OfficeShiftId.Value, _context),
                                FinishOverTime = await CalculateFinishOverTime(empAttendanceDetails.EmployeeId, (int)empAttendanceDetails.OfficeShiftId.Value, _context),
                                OvertimeWorkingHours = await CalculateMonthlyOvertimeHours(empAttendanceDetails.EmployeeId, (int)empAttendanceDetails.OfficeShiftId.Value),
                                TotalWorkingHours = await CalculateTotalWorkingHours(empAttendanceDetails.EmployeeId),
                                MonthlyWorkingHours = await CalculateMonthlyWorkingHours(empAttendanceDetails.EmployeeId),
                                Presencepercentage = await CalculatePresencePercentage(empAttendanceDetails.EmployeeId),
                                absencepercentage = await CalculateAbsencePercentage(empAttendanceDetails.EmployeeId),
                                Currentdate = currentDate,
                                LoginStatus = latestLoginStatus,
                                loginactivities = await GetLoginActivities(empAttendanceDetails.EmployeeId)
                            };

                            return attendanceDetail;
                        }
                        else
                        {
                            throw new Exception("Office shift ID is missing.");
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        private async Task<List<Loginactivity>> GetLoginActivities(string employeeId)
        {
            var currentDate = DateTime.Now.Date;

            var attendanceRecordsCheckIn = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId && g.CheckIn == true && g.CheckInTime.HasValue && g.CheckInTime.Value.Date == currentDate)
                .OrderBy(g => g.CheckInTime)
                .ToListAsync();
            var attendanceRecordsCheckOut = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId && g.CheckIn == false && g.CheckOutTime.HasValue && g.CheckOutTime.Value.Date == currentDate)
                .OrderByDescending(g => g.CheckOutTime)
                .ToListAsync();

            var loginActivities = new List<Loginactivity>();
            int index = 0;

            foreach (var CheckInrecord in attendanceRecordsCheckIn)
            {
                Loginactivity loginActivity = new Loginactivity
                {
                    CheckIN = CheckInrecord.CheckInTime.Value.ToString("hh:mm tt"),
                    CheckOut = "N/A",
                    LoginStatus = "Check-In",
                    loginactivities = null,
                };

                if (index < attendanceRecordsCheckOut.Count)
                {
                    var CheckOutrecord = attendanceRecordsCheckOut[index];
                    loginActivity.CheckOut = CheckOutrecord.CheckOutTime.HasValue
                        ? CheckOutrecord.CheckOutTime.Value.ToString("hh:mm tt")
                        : "N/A";
                    loginActivity.LoginStatus = "Check-Out";
                    loginActivity.loginactivities = null;
                }

                loginActivities.Add(loginActivity);
                index++;
            }
            return loginActivities;
        }
        private async Task<string> CalculatePresencePercentage(string employeeId)
        {
            var emp = await _context.EmployeeRegistrations
                .Where(x => x.EmployeeId == employeeId)
                .FirstOrDefaultAsync();

            if (emp == null)
            {
                return "0";
            }

            var Attendancedays = await _context.Attendancedays
                .Where(x => x.Vendorid == emp.Vendorid)
                .FirstOrDefaultAsync();

            if (Attendancedays == null || string.IsNullOrEmpty(Attendancedays.Nodays) || !int.TryParse(Attendancedays.Nodays, out int totalAttendanceDays) || totalAttendanceDays <= 0)
            {
                return "0";
            }

            var totalDaysWorked = await _context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId && g.CurrentDate.Value.Month == DateTime.Now.Month && g.CurrentDate.Value.Year == DateTime.Now.Year)
                .CountAsync();
            decimal presencePercentage = (decimal)totalDaysWorked / totalAttendanceDays * 100;

            return Math.Round(presencePercentage).ToString() + "%";
        }

        private async Task<string> CalculateAbsencePercentage(string employeeId)
        {
            var emp = await _context.EmployeeRegistrations
                .Where(x => x.EmployeeId == employeeId)
                .FirstOrDefaultAsync();

            if (emp == null)
            {
                return "0";
            }

            var Attendancedays = await _context.Attendancedays
                .Where(x => x.Vendorid == emp.Vendorid)
                .FirstOrDefaultAsync();

            if (Attendancedays == null || string.IsNullOrEmpty(Attendancedays.Nodays) || !int.TryParse(Attendancedays.Nodays, out int totalAttendanceDays) || totalAttendanceDays <= 0)
            {
                return "0";
            }

            var totalDaysWorked = await _context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId && g.CurrentDate.Value.Month == DateTime.Now.Month && g.CurrentDate.Value.Year == DateTime.Now.Year)
                .CountAsync();
            decimal presencePercentage = (100 - (decimal)totalDaysWorked / totalAttendanceDays * 100);

            return Math.Round(presencePercentage).ToString() + "%";
        }
        private static async Task<string> CalculateStartOverTime(string employeeId, int officeShiftId, admin_NDCrMContext context)
        {

            var checkInData = await context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId && g.CheckIntime.Value.Date == DateTime.Now.Date)
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
        private static async Task<string> CalculateFinishOverTime(string employeeId, int officeShiftId, admin_NDCrMContext context)
        {
            var officeHour = await context.Officeshifts
                .Where(h => h.Id == officeShiftId)
                .Select(h => h.Endtime)
                .FirstOrDefaultAsync();

            if (DateTime.TryParse(officeHour, out DateTime endTime))
            {
                var checkInData = await context.EmployeeCheckInRecords
                    .Where(g => g.EmpId == employeeId && g.CheckIntime.Value.Date == DateTime.Now.Date)
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
        private async Task<string> CalculateTotalWorkingHours(string employeeId)
        {
            var checkIns = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId && g.Currentdate.HasValue && g.Currentdate.Value.Date == DateTime.Now.Date)
                .OrderBy(g => g.CheckInTime)
                .ToListAsync();

            double totalHours = 0;

            for (int i = 0; i < checkIns.Count; i++)
            {
                var checkInRecord = checkIns[i];
                if (checkInRecord.CheckIn == true && checkInRecord.CheckInTime.HasValue)
                {
                    var checkOutRecord = checkIns.FirstOrDefault(g => g.CheckIn == false && g.CheckOutTime.HasValue && g.CheckOutTime > checkInRecord.CheckInTime);
                    if (checkOutRecord != null)
                    {
                        totalHours += (checkOutRecord.CheckOutTime.Value - checkInRecord.CheckInTime.Value).TotalHours;
                        checkIns.Remove(checkOutRecord);
                    }
                }
            }
            return FormatHours(totalHours);
        }

        private async Task<string> CalculateMonthlyWorkingHours(string employeeId)
        {
            var checkIns = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId && g.Currentdate.HasValue && g.Currentdate.Value.Month == DateTime.Now.Month)
                .OrderBy(g => g.CheckInTime)
                .ToListAsync();

            double monthlyHours = 0;

            for (int i = 0; i < checkIns.Count; i++)
            {
                var checkInRecord = checkIns[i];
                if (checkInRecord.CheckIn == true && checkInRecord.CheckInTime.HasValue)
                {
                    var checkOutRecord = checkIns.FirstOrDefault(g => g.CheckIn == false && g.CheckOutTime.HasValue && g.CheckOutTime > checkInRecord.CheckInTime);

                    if (checkOutRecord != null)
                    {
                        monthlyHours += (checkOutRecord.CheckOutTime.Value - checkInRecord.CheckInTime.Value).TotalHours;
                        checkIns.Remove(checkOutRecord);
                    }
                }
            }

            return FormatHours(monthlyHours);
        }

        private async Task<string> CalculateMonthlyOvertimeHours(string employeeId, int officeShiftId)
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
                .Where(g => g.EmpId == employeeId && g.CheckIntime.HasValue && g.CheckIntime.Value.Month == DateTime.Now.Month)
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
        public async Task<Loginactivity> GetEmpLoginactivity(string userid)
        {
            try
            {
                if (string.IsNullOrEmpty(userid))
                {
                    throw new ArgumentException("User ID cannot be null or empty.");
                }

                Loginactivity loginactivity = new Loginactivity();
                var currentDate = DateTime.Now.Date;
                var attendanceRecords = await _context.EmployeeCheckIns
                    .Where(g => g.EmployeeId == userid &&
                                (g.CheckInTime.Value.Date == currentDate ||
                                 (g.CheckOutTime.HasValue && g.CheckOutTime.Value.Date == currentDate)))
                    .OrderBy(g => g.CheckInTime)
                    .ToListAsync();

                if (!attendanceRecords.Any())
                {
                    return loginactivity;
                }
                string checkInTime = attendanceRecords
                    .Where(g => g.CheckIn == true)
                    .OrderBy(g => g.CheckInTime)
                    .Select(g => g.CheckInTime.Value.ToString("hh:mm tt"))
                    .FirstOrDefault() ?? "N/A";

                string checkOutTime = attendanceRecords
                    .Where(g => g.CheckOutTime.HasValue)
                    .OrderByDescending(g => g.CheckOutTime)
                    .Select(g => g.CheckOutTime.Value.ToString("hh:mm tt"))
                    .FirstOrDefault() ?? "N/A";

                DateTime? lastBreakOutTime = null;
                foreach (var record in attendanceRecords)
                {
                    Loginactivity dd = new Loginactivity();

                    if (record.CheckIn == false )
                    {
                        lastBreakOutTime = record.CheckOutTime.Value;
                        dd.CheckOut = lastBreakOutTime.Value.ToString("hh:mm tt");
                        dd.CheckIN = "N/A";
                        dd.LoginStatus = "Check-Out";
                        dd.loginactivities = null;
                    }
                    else if (record.CheckIn == true)
                    {
                        if (!lastBreakOutTime.HasValue || record.CheckInTime > lastBreakOutTime.Value)
                        {
                            dd.CheckIN = record.CheckInTime.Value.ToString("hh:mm tt");
                            dd.CheckOut = "N/A";
                            dd.LoginStatus = "Check-In";
                            dd.loginactivities = null;
                        }
                    }
                    loginactivity.CheckIN = checkInTime;
                    loginactivity.CheckOut = checkOutTime;
                    loginactivity.LoginStatus = record.CheckIn == true ? "Check-In" : "Check-Out";
                    loginactivity.loginactivities.Add(dd);
                }

                return loginactivity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving employee login activity: " + ex.Message);
            }
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
                                        StatusName = status.StatusName
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
                var attendanceRecordsCheckIn = await _context.EmployeeCheckIns
                    .Where(g => g.EmployeeId == userid && g.CheckIn == true)
                    .OrderBy(g => g.CheckInTime).OrderByDescending(g => g.CheckInTime)
                    .ToListAsync();

                var attendanceRecordsCheckOut = await _context.EmployeeCheckIns
                    .Where(g => g.EmployeeId == userid && g.CheckIn == false)
                    .OrderByDescending(g => g.CheckOutTime)
                    .ToListAsync();

                if (!attendanceRecordsCheckIn.Any())
                {
                    return new List<WebLoginactivity>();
                }

                List<WebLoginactivity> loginActivities = new List<WebLoginactivity>();
                int index = 0;
                foreach (var CheckInrecord in attendanceRecordsCheckIn)
                {
                    if (index < attendanceRecordsCheckOut.Count)
                    {
                        var CheckOutrecord = attendanceRecordsCheckOut[index];

                        WebLoginactivity loginActivity = new WebLoginactivity
                        {
                            CheckIN = CheckInrecord.CheckInTime.Value.ToString("hh:mm tt"),
                            CheckOut = CheckOutrecord.CheckOutTime.HasValue
                                                    ? CheckOutrecord.CheckOutTime.Value.ToString("hh:mm tt")
                                                    : "N/A",
                            Currentdate = CheckInrecord.Currentdate.HasValue
                                                    ? CheckInrecord.Currentdate.Value.ToString("dd-MM-yyyy")
                                                    : "N/A",
                            LoginStatus = CheckInrecord.CheckIn == true ? "Check-In" : "Check-Out"
                        };

                        loginActivities.Add(loginActivity);
                    }
                    index++;
                }
                return loginActivities;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving employee login activity: " + ex.Message);
            }
        }
        public async Task<List<officeEventsDto>> GetOfficeEvents(string userid)
        {
            try
            {
                var empdetail = await _context.EmployeeRegistrations
                    .Where(x => x.EmployeeId == userid)
                    .FirstOrDefaultAsync();
                var taskassign = await _context.OfficeEvents
                    .Where(x => x.Vendorid == empdetail.Vendorid).Select(x => new officeEventsDto
                    {
                        Subtittle = x.Subtittle,
                        Tittle = x.Tittle,
                        Date = x.Date.Value.Date,
                    }).ToListAsync();
                return taskassign;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<Empattendancedatail> GetFilterattendance(string userid, DateTime Currentdate)
        {
            try
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    var empAttendanceDetails = await _context.EmployeeRegistrations
                        .Where(x => x.EmployeeId == userid)
                        .Select(x => new
                        {
                            OfficeShiftId = x.OfficeshiftTypeid,
                            EmployeeId = x.EmployeeId
                        })
                        .FirstOrDefaultAsync();

                    if (empAttendanceDetails != null && empAttendanceDetails.OfficeShiftId.HasValue)
                    {
                        var officeShift = await _context.Officeshifts.FindAsync((int)empAttendanceDetails.OfficeShiftId.Value);
                        if (officeShift == null)
                            throw new Exception("Office shift not found.");

                        // Parse the end time only once for the calculations below
                        if (!DateTime.TryParse(officeShift.Endtime, out DateTime shiftEndTime))
                            throw new Exception("Invalid shift end time format.");

                        var checkIns = await _context.EmployeeCheckIns
                            .Where(g => g.EmployeeId == empAttendanceDetails.EmployeeId && g.CheckInTime.HasValue && g.CheckInTime.Value.Date == Currentdate.Date)
                            .ToListAsync();
                        var checkInTime = checkIns.OrderBy(g => g.Id).Select(g => g.CheckInTime?.ToString("hh:mm tt") ?? "N/A").FirstOrDefault();
                        var checkOutTime = checkIns.OrderByDescending(g => g.Id).Select(g => g.CheckOutTime?.ToString("hh:mm tt") ?? "N/A").FirstOrDefault();
                        var date = checkIns.OrderBy(g => g.Id).Select(g => g.Currentdate?.ToString("dd-MM-yyyy")).FirstOrDefault();
                        var totalWorkingHours = await FilterCalculateTotalWorkingHours(empAttendanceDetails.EmployeeId, Currentdate);
                        var monthlyWorkingHours = await FilterCalculateMonthlyWorkingHours(empAttendanceDetails.EmployeeId, Currentdate);

                        var attendanceDetail = new Empattendancedatail
                        {
                            OfficeHour = $"{officeShift.Starttime} - {officeShift.Endtime}",
                            CheckInTime = checkInTime,
                            CheckOutTime = checkOutTime,
                            StartOverTime = shiftEndTime.ToString("hh:mm tt"),
                            FinishOverTime = await FilterCalculateFinishOverTime(empAttendanceDetails.EmployeeId, (int)empAttendanceDetails.OfficeShiftId.Value, Currentdate),
                            OvertimeWorkingHours = await FilterCalculateOvertimeHours(empAttendanceDetails.EmployeeId, (int)empAttendanceDetails.OfficeShiftId.Value, Currentdate),
                            TotalWorkingHours = totalWorkingHours,
                            MonthlyWorkingHours = monthlyWorkingHours,
                            Presencepercentage = await FilterCalculatePresencePercentage(empAttendanceDetails.EmployeeId, Currentdate),
                            absencepercentage = await FilterCalculateAbsencePercentage(empAttendanceDetails.EmployeeId, Currentdate),
                            Currentdate = date,
                            loginactivities = await FilterGetLoginActivities(empAttendanceDetails.EmployeeId, Currentdate)
                        };

                        return attendanceDetail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public async Task<string> FilterCalculateTotalWorkingHours(string employeeId, DateTime Currentdate)
        {
            var checkIns = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId && g.Currentdate.Value.Date == Currentdate.Date)
                .ToListAsync();

            var totalHours = checkIns.Sum(g =>
            {
                if (g.CheckOutTime.HasValue && g.CheckInTime.HasValue)
                {
                    return (g.CheckOutTime.Value - g.CheckInTime.Value).TotalHours;
                }
                return 0.0;
            });

            return FormatHours1(totalHours);
        }

        public async Task<string> FilterCalculatePresencePercentage(string employeeId, DateTime Currentdate)
        {
            var emp = await _context.EmployeeRegistrations
                .Where(x => x.EmployeeId == employeeId)
                .FirstOrDefaultAsync();

            if (emp == null) return "0";

            var attendanceDays = await _context.Attendancedays
                .Where(x => x.Vendorid == emp.Vendorid)
                .FirstOrDefaultAsync();

            if (attendanceDays == null || !int.TryParse(attendanceDays.Nodays, out int totalAttendanceDays) || totalAttendanceDays <= 0)
                return "0";

            var totalDaysWorked = await _context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId && g.CheckIntime.HasValue && g.CheckIntime.Value.Month == Currentdate.Month && g.CheckIntime.Value.Year == Currentdate.Year)
                .CountAsync();

            decimal presencePercentage = (decimal)totalDaysWorked / totalAttendanceDays * 100;

            return Math.Round(presencePercentage).ToString() + "%";
        }
        public async Task<string> FilterCalculateAbsencePercentage(string employeeId, DateTime Currentdate)
        {
            var emp = await _context.EmployeeRegistrations
                .Where(x => x.EmployeeId == employeeId)
                .FirstOrDefaultAsync();

            if (emp == null) return "0";

            var attendanceDays = await _context.Attendancedays
                .Where(x => x.Vendorid == emp.Vendorid)
                .FirstOrDefaultAsync();

            if (attendanceDays == null || !int.TryParse(attendanceDays.Nodays, out int totalAttendanceDays) || totalAttendanceDays <= 0)
                return "0";

            var totalDaysWorked = await _context.EmployeeCheckInRecords
                .Where(g => g.EmpId == employeeId && g.CheckIntime.HasValue && g.CheckIntime.Value.Month == Currentdate.Month && g.CheckIntime.Value.Year == Currentdate.Year)
                .CountAsync();

            decimal absencePercentage = 100 - (decimal)totalDaysWorked / totalAttendanceDays * 100;

            return Math.Round(absencePercentage).ToString() + "%";
        }
        public async Task<List<Loginactivity>> FilterGetLoginActivities(string employeeId, DateTime currentDate)
        {
            var attendanceRecords = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId && g.Currentdate.Value.Date == currentDate.Date)
                .ToListAsync();

            var loginActivities = new List<Loginactivity>();
            HashSet<string> existingCheckOuts = new HashSet<string>();

            foreach (var record in attendanceRecords)
            {
                if (record.CheckInTime.HasValue)
                {
                    loginActivities.Add(new Loginactivity
                    {
                        CheckIN = record.CheckInTime.Value.ToString("hh:mm tt"),
                        CheckOut = "N/A",
                        LoginStatus = "Check-In",
                        loginactivities = null // Assuming loginactivities is not needed
                    });
                }
                if (record.CheckOutTime.HasValue)
                {
                    string checkOutTime = record.CheckOutTime.Value.ToString("hh:mm tt");
                    if (!existingCheckOuts.Contains(checkOutTime))
                    {
                        loginActivities.Add(new Loginactivity
                        {
                            CheckIN = "N/A",
                            CheckOut = checkOutTime,
                            LoginStatus = "Check-Out",
                            loginactivities = null
                        });

                        existingCheckOuts.Add(checkOutTime);
                    }
                }
            }

            return loginActivities;
        }

        public async Task<string> FilterCalculateFinishOverTime(string employeeId, int officeShiftId, DateTime Currentdate)
        {
            var officeHour = await _context.Officeshifts
                .Where(h => h.Id == officeShiftId)
                .Select(h => h.Endtime)
                .FirstOrDefaultAsync();

            if (DateTime.TryParse(officeHour, out DateTime endTime))
            {
                var checkInData = await _context.EmployeeCheckIns
                    .Where(g => g.EmployeeId == employeeId && g.CheckInTime.HasValue && g.CheckInTime.Value.Date == Currentdate.Date)
                    .OrderByDescending(g => g.Id)
                    .FirstOrDefaultAsync();

                if (checkInData?.CheckOutTime.HasValue == true && checkInData.CheckOutTime > endTime)
                    return checkInData.CheckOutTime.Value.ToString("hh:mm tt");
            }

            return "";
        }

        public async Task<string> FilterCalculateOvertimeHours(string employeeId, int officeShiftId, DateTime date)
        {
            var officeShift = await _context.Officeshifts
                .Where(x => x.Id == officeShiftId)
                .Select(x => x.Endtime)
                .FirstOrDefaultAsync();

            if (DateTime.TryParse(officeShift, out DateTime shiftEndTime))
            {
                var checkIns = await _context.EmployeeCheckIns
                    .Where(x => x.EmployeeId == employeeId && x.Currentdate.HasValue && x.Currentdate.Value.Date == date.Date)
                    .ToListAsync();

                var overtimeHours = checkIns.Sum(x =>
                {
                    if (x.CheckOutTime.HasValue && x.CheckOutTime > shiftEndTime)
                    {
                        return (x.CheckOutTime.Value - shiftEndTime).TotalHours;
                    }
                    return 0.0;
                });

                return FormatHours1(overtimeHours);
            }

            return "";
        }

        public async Task<string> FilterCalculateMonthlyWorkingHours(string employeeId, DateTime Currentdate)
        {
            var checkIns = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId
                            && g.CheckInTime.HasValue
                            && g.CheckInTime.Value.Month == Currentdate.Month
                            && g.CheckInTime.Value.Year == Currentdate.Year)
                .ToListAsync();

            var totalMonthlyHours = checkIns.Sum(g =>
            {
                if (g.CheckOutTime.HasValue && g.CheckInTime.HasValue)
                {
                    return (g.CheckOutTime.Value - g.CheckInTime.Value).TotalHours;
                }
                return 0.0;
            });

            return FormatHours1(totalMonthlyHours);
        }

        private string FormatHours1(double hours)
        {
            TimeSpan t = TimeSpan.FromHours(hours);
            return string.Format("{0:D2}h:{1:D2}m", t.Hours, t.Minutes);
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
                    !int.TryParse(attendancedays.Nodays, out int totalAttendanceDays) || totalAttendanceDays <= 0)
                {
                    return new Monthlyattendancedatail
                    {
                        TotalWorkingDays = 0,
                        TotalPresentDays = 0,
                        TotalAbsentDays = 0,
                        Attendance = "0%"
                    };
                }
                var totalDaysWorked = await _context.EmployeeCheckInRecords
                    .Where(g => g.EmpId == userid && g.CurrentDate.Value.Month == DateTime.Now.Month && g.CurrentDate.Value.Year == DateTime.Now.Year)
                    .CountAsync();
                decimal presencePercentage = (decimal)totalDaysWorked / totalAttendanceDays * 100;
                int totalAbsentDays = totalAttendanceDays - totalDaysWorked;

                return new Monthlyattendancedatail
                {
                    TotalWorkingDays = totalAttendanceDays,
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
                    .Where(p => p.UserId == userid && p.Isapprove == true)
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
                        TypeofLeaveid =p.TypeOfLeaveId,
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
                    var leaveType = GetLeaveType(l.StartLeaveId, l.EndeaveId, totalFullday);
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

        private static string? GetLeaveType(int startLeaveId, int endLeaveId, decimal totalFullday)
        {
            int halfDayCount = 0;
            int fullDayCount = (int)totalFullday;

            if (startLeaveId == 1 || startLeaveId == 2) halfDayCount++;
            if (startLeaveId == 3) fullDayCount++;

            if (endLeaveId == 1 || endLeaveId == 2) halfDayCount++;
            if (endLeaveId == 3) fullDayCount++;

            List<string> leaveTypes = new List<string>();
            if (halfDayCount > 0)
            {
                leaveTypes.Add($"{halfDayCount} Half Day{(halfDayCount > 1 ? "s" : "")}");
            }
            if (fullDayCount > 0)
            {
                leaveTypes.Add($"{fullDayCount} Full Day{(fullDayCount > 1 ? "s" : "")}");
            }

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
                }).FirstOrDefault();
                return getleave;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message, ex);
            }
        }
        public async Task<List<getattendancegraph>> GetEmpGraph(string userid)
        {
            try
            {
                int currentYear = DateTime.Now.Year;

                var graph = await _context.EmployeeCheckInRecords
                    .Where(x => x.EmpId == userid && x.CurrentDate.HasValue && x.CurrentDate.Value.Year == currentYear)
                    .GroupBy(x => new
                    {
                        Year = x.CurrentDate.Value.Year,
                        Month = x.CurrentDate.Value.Month
                    })
                    .Select(g => new getattendancegraph
                    {
                        Year = g.Key.Year.ToString(),
                        Month = getMonthName(g.Key.Month),
                        Value = g.Count()
                    }).ToListAsync();

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
                                        where taskList.EmployeeId == userid && taskList.Emptaskid == id
                                        select new TasksassignlistDto
                                        {
                                            Id = taskList.Id,
                                            TasksubTittle = taskList.Taskname,
                                            TaskStatus = taskStatus.StatusName,
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
    }
}
