using CRM.Controllers;
using CRM.IUtilities;
using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Utilities;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Services.Account;
using MimeKit.Encodings;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using Syncfusion.Pdf.Graphics;
using System;
using System.Globalization;

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
                        CompanyLocationName = _context.Cities.Where(g => g.Id == Convert.ToInt16(x.WorkLocationId)).Select(g => g.City1).First(),
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
                string aadharImagePath = "";
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
                    if (model.AadharImage != null && model.AadharImage.Count > 0)
                    {
                        for (int i = 0; i < model.AadharImage.Count; i++)
                        {
                            if (model.AadharImage[i] != null)
                            {
                                aadharImagePath = fileOperation.SaveBase64Image("img1", model.AadharImage[i], allowedExtensions);
                                if (aadharImagePath == "not allowed")
                                {
                                    throw new Exception("File upload not allowed.");
                                }
                                if (i == 0)
                                {
                                    apppersonal.AadharOne = aadharImagePath; ;
                                }
                                else if (i == 1)
                                {
                                    apppersonal.AadharTwo = aadharImagePath;
                                }
                            }
                        }
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
                        if (model.AadharImage != null && model.AadharImage.Count > 0)
                        {
                            for (int i = 0; i < model.AadharImage.Count; i++)
                            {
                                aadharImagePath = fileOperation.SaveBase64Image("img1", model.AadharImage[i], allowedExtensions);

                                if (i == 0)
                                {
                                    empP.AadharOne = aadharImagePath;
                                }
                                else if (i == 1)
                                {
                                    empP.AadharTwo = aadharImagePath;
                                }
                            }
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
                                             where lm.EmpId == userid
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

        public async Task<bool> ApplyLeave(ApplyLeave model, string userid)
        {
            try
            {
                List<Leave> FH = new List<Leave>();
                List<Leave> SH = new List<Leave>();
                List<Leave> FD = new List<Leave>();
                decimal total = (decimal)0.00;
                var TypeOfLeave = _context.Leavemasters.Where(x => x.LeavetypeId == model.TypeOfLeaveId && x.EmpId == userid).FirstOrDefault();
                if (model.EndDate != model.StartDate)
                {
                    total = (model.EndDate - model.StartDate).Days;
                }
                if (model.StartLeaveId == 1)
                {
                    FH = await _context.Leaves.Where(x => x.Id == model.StartLeaveId).ToListAsync();
                }
                else if (model.StartLeaveId == 2)
                {
                    SH = await _context.Leaves.Where(x => x.Id == model.EndeaveId).ToListAsync();
                }
                else
                {
                    FD = await _context.Leaves.Where(x => x.Id == model.EndeaveId).ToListAsync();
                }
                if (total == 0)
                {
                    decimal TotalLeave = (FH.Count() == 0 ? 0 : (decimal)(FH.Count() * 0.50)) + (SH.Count() == 0 ? 0 : (decimal)(SH.Count() * 0.50)) + (FD.Count() == 0 ? 0 : (decimal)(FD.Count() * 1.00));
                    total = TotalLeave;
                }
                else
                {
                    decimal TotalLeave = (FH.Count() == 0 ? 0 : (decimal)(FH.Count() * 0.50)) + (SH.Count() == 0 ? 0 : (decimal)(SH.Count() * 0.50));
                    total -= TotalLeave;
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
                    CountLeave = total,
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)
                };
                await _context.ApplyLeaveNews.AddAsync(apply);
                await _context.SaveChangesAsync();
                TypeOfLeave.Value -= total;
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
                        SalarySlipPath = "~/EMPpdfs/" + x.SalarySlip,
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
                        SalarySlipPath = "~/EMPpdfs/" + x.SalarySlip,
                        Month = x.Month,
                        Year = x.Year
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
    }
}
