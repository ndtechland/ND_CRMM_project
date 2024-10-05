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
                    CheckIn = checkIn,
                    CheckInTime = DateTime.Now,
                    CheckOutTime = checkIn ? (DateTime?)null : DateTime.Now,
                    Currentdate = DateTime.Now,
                };

                await _context.EmployeeCheckIns.AddAsync(empcheck);
                await _context.SaveChangesAsync();

                // Get the latest check-in for this employee on the current date
                var fgdfd = await _context.EmployeeCheckIns
                    .Where(x => x.EmployeeId == emp.EmployeeId && x.CheckInTime.Date == DateTime.Now.Date)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                if (fgdfd == null)
                {
                    throw new Exception("Check-in record not found.");
                }

                // Check if this is the first check-in for today
                var todayCheckInsCount = await _context.EmployeeCheckIns
                    .CountAsync(x => x.EmployeeId == emp.EmployeeId && x.CheckInTime.Date == DateTime.Now.Date);

                if (todayCheckInsCount == 1)
                {
                    // Add a new EmployeeCheckInRecord for the first check-in of the day
                    EmployeeCheckInRecord empch = new()
                    {
                        EmpId = emp.EmployeeId,
                        CheckIntime = fgdfd.CheckInTime,
                        CurrentDate = DateTime.Now,
                        Isactive = true,
                        Workinghour = TimeSpan.Zero  // Initially zero, as the user just checked in
                    };
                    await _context.EmployeeCheckInRecords.AddAsync(empch);
                    await _context.SaveChangesAsync();
                }

                if (!checkIn)
                {
                    // User is checking out; calculate working hours
                    var checkInRecord = await _context.EmployeeCheckInRecords
                        .Where(x => x.EmpId == emp.EmployeeId && x.CheckIntime.Value.Date == DateTime.Now.Date)
                        .FirstOrDefaultAsync();

                    if (checkInRecord != null)
                    {
                        // Calculate working hours based on CheckInTime and current time (CheckOutTime)
                        TimeSpan workingHours = (fgdfd.CheckInTime - DateTime.Now).Duration();

                        // Update the record with the working hours and save
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
                var empCheckInRecord = await _context.EmployeeCheckIns
                    .Where(x => x.EmployeeId == emp.EmployeeId && x.CheckInTime.Date == DateTime.Now.Date)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                if (empCheckInRecord == null)
                {
                    empCheckInRecord = new EmployeeCheckIn
                    {
                        EmployeeId = emp.EmployeeId,
                        CurrentLat = model.CurrentLat,
                        Currentlong = model.Currentlong,
                        CheckIn = checkIn,
                        CheckInTime = DateTime.Now,
                        Currentdate = DateTime.Now
                    };
                    _context.EmployeeCheckIns.Add(empCheckInRecord);
                }
                else if (!checkIn)
                {
                    empCheckInRecord.CheckOutTime = DateTime.Now;

                    TimeSpan workingHours = (empCheckInRecord.CheckInTime - DateTime.Now).Duration();
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

                await _context.SaveChangesAsync(); return empCheckInRecord;
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
                                .Where(g => g.EmployeeId == empAttendanceDetails.EmployeeId && g.CheckInTime.Date == DateTime.Now.Date)
                                .ToListAsync();

                            var checkInTime = checkIns.OrderBy(g => g.Id)
                                .Select(g => g.CheckInTime.ToString("hh:mm tt"))
                                .FirstOrDefault();

                            var checkOutTime = checkIns.OrderByDescending(g => g.Id)
                                .Select(g => g.CheckOutTime)
                                .FirstOrDefault();
                            var attendanceDetail = new Empattendancedatail
                            {
                                OfficeHour = $"{officeShift.Starttime} - {officeShift.Endtime}",
                                CheckInTime = checkInTime,
                                CheckOutTime = checkOutTime.HasValue ? checkOutTime.Value.ToString("hh:mm tt") : "",
                                StartOverTime = shiftEndTime.ToString("hh:mm tt"),
                                FinishOverTime = await CalculateFinishOverTime(empAttendanceDetails.EmployeeId, (int)empAttendanceDetails.OfficeShiftId.Value, _context),
                                OvertimeWorkingHours = await CalculateOvertimeHours(empAttendanceDetails.EmployeeId, (int)empAttendanceDetails.OfficeShiftId.Value),
                                TotalWorkingHours = await CalculateTotalWorkingHours(empAttendanceDetails.EmployeeId),
                                MonthlyWorkingHours = await CalculateMonthlyWorkingHours(empAttendanceDetails.EmployeeId),
                                Presencepercentage = await CalculatePresencePercentage(empAttendanceDetails.EmployeeId),
                                absencepercentage = await CalculateAbsencePercentage(empAttendanceDetails.EmployeeId)
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
                .Where(g => g.EmpId == employeeId && g.CheckIntime.Value.Month == DateTime.Now.Month && g.CheckIntime.Value.Year == DateTime.Now.Year)
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
                .Where(g => g.EmpId == employeeId && g.CheckIntime.Value.Month == DateTime.Now.Month && g.CheckIntime.Value.Year == DateTime.Now.Year)
                .CountAsync();
            decimal presencePercentage = (100 - (decimal)totalDaysWorked / totalAttendanceDays * 100);

            return Math.Round(presencePercentage).ToString() + "%";
        }
        private static async Task<string> CalculateFinishOverTime(string employeeId, int officeShiftId, admin_NDCrMContext context)
        {
            var officeHour = await context.Officeshifts
                .Where(h => h.Id == officeShiftId)
                .Select(h => h.Endtime)
                .FirstOrDefaultAsync();

            if (DateTime.TryParse(officeHour, out DateTime endTime))
            {
                var checkInData = await context.EmployeeCheckIns
                    .Where(g => g.EmployeeId == employeeId && g.CheckInTime.Date == DateTime.Now.Date)
                    .OrderByDescending(g => g.Id)
                    .FirstOrDefaultAsync();

                if (checkInData != null && checkInData.CheckOutTime.HasValue)
                {
                    var checkOutTime = checkInData.CheckOutTime.Value;
                    if (checkOutTime > endTime)
                    {
                        return checkOutTime.ToString("hh:mm tt");
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            return "";
        }
        private async Task<string> CalculateTotalWorkingHours(string employeeId)
        {
            var now = DateTime.Now;

            var checkIns = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId)
                .ToListAsync();

            var totalHours = checkIns
                .Where(g => g.CheckInTime.Month == now.Month && g.CheckInTime.Year == now.Year)
                .Sum(g =>
                {
                    if (g.CheckOutTime.HasValue)
                    {
                        return (g.CheckOutTime.Value - g.CheckInTime).TotalHours;
                    }
                    return 0.0;
                });

            return FormatHours(totalHours);
        }

        private async Task<string> CalculateMonthlyWorkingHours(string employeeId)
        {
            var checkIns = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId && g.CheckInTime.Month == DateTime.Now.Month && g.CheckInTime.Year == DateTime.Now.Year)
                .ToListAsync();

            var monthlyHours = checkIns
                .Sum(g => g.CheckOutTime.HasValue
                        ? (g.CheckOutTime.Value - g.CheckInTime).TotalHours
                        : 0.0);

            return FormatHours(monthlyHours);
        }

        private async Task<string> CalculateOvertimeHours(string employeeId, int officeShiftId)
        {
            var checkIns = await _context.EmployeeCheckIns
                .Where(g => g.EmployeeId == employeeId && g.CheckInTime.Date == DateTime.Now.Date)
                .ToListAsync();

            var shiftEndTime = await _context.Officeshifts
                .Where(h => h.Id == officeShiftId)
                .Select(h => h.Endtime)
                .FirstOrDefaultAsync();

            if (!DateTime.TryParse(shiftEndTime, out DateTime parsedEndTime))
            {
                return "N/A";
            }

            double totalOvertimeHours = checkIns.Sum(g =>
            {
                if (g.CheckOutTime.HasValue && g.CheckOutTime > parsedEndTime)
                {
                    return (g.CheckOutTime.Value.TimeOfDay - parsedEndTime.TimeOfDay).TotalHours;
                }
                return 0.0;
            });

            return FormatHours(Math.Abs(totalOvertimeHours));
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

                var empAttendanceDetails = await _context.EmployeeRegistrations
                    .Where(x => x.EmployeeId == userid)
                    .Select(x => new
                    {
                        OfficeShiftId = x.OfficeshiftTypeid,
                        EmployeeId = x.EmployeeId
                    })
                    .FirstOrDefaultAsync();

                if (empAttendanceDetails == null)
                {
                    throw new Exception("Employee not found.");
                }

                if (!empAttendanceDetails.OfficeShiftId.HasValue)
                {
                    throw new Exception("Office shift ID is missing.");
                }

                var officeShift = await _context.Officeshifts.FindAsync((int)empAttendanceDetails.OfficeShiftId.Value);
                if (officeShift == null)
                {
                    throw new Exception("Office shift not found.");
                }

                if (!DateTime.TryParse(officeShift.Endtime, out DateTime shiftEndTime))
                {
                    throw new Exception("Invalid shift end time format.");
                }

                var breakInRecords = await _context.EmployeeCheckIns
                    .Where(g => g.EmployeeId == userid
                                 && g.CheckInTime.Day == DateTime.Now.Day
                                 && g.CheckInTime.Day == DateTime.Now.Day
                                 && g.CheckIn == true)
                    .ToListAsync();
                var breakoutRecords = await _context.EmployeeCheckIns
                   .Where(g => g.EmployeeId == userid
                                && g.CheckOutTime.Value.Day == DateTime.Now.Day
                                && g.CheckOutTime.Value.Day == DateTime.Now.Day
                                && g.CheckIn == false)
                   .ToListAsync();
                var breakInTimes = breakInRecords.Select(b => b.CheckInTime.ToString("hh:mm tt")).ToList();
                var breakInString = breakInTimes.Count > 0 ? string.Join(", ", breakInTimes) : "No BreakIn Records";
                var breakOutTimes = breakoutRecords.Select(b => b.CheckOutTime?.ToString("hh:mm tt")).ToList();
                var breakoutString = breakOutTimes.Count > 0 ? string.Join(", ", breakOutTimes) : "No BreakOut Records";

                var attendanceDetail = new Loginactivity
                {
                    OfficeHour = $"{officeShift.Starttime} - {officeShift.Endtime}",
                    BreakIN = breakInString,
                    BreakOut = breakoutString
                };

                return attendanceDetail;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving employee login activity: " + ex.Message);
            }
        }

    }
}
