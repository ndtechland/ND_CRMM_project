using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using CRM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Components.RenderTree;
using System.IO;
using Org.BouncyCastle.Asn1.Mozilla;
using SelectPdf;
using static CRM.Controllers.Employee;
using System.Globalization;
using System.Data;
using MimeKit.Encodings;
using Fingers10.ExcelExport.ActionResults;
using System.Net.Mail;
using MimeKit;
using Microsoft.AspNetCore.Hosting;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Diagnostics.Metrics;
using NuGet.Versioning;
using DocumentFormat.OpenXml.Drawing.Charts;
using DataTable = System.Data.DataTable;
using DocumentFormat.OpenXml.Office.CustomUI;
using Humanizer;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using DocumentFormat.OpenXml.InkML;
using CRM.IUtilities;
using Microsoft.VisualStudio.Services.Commerce;

namespace CRM.Controllers
{
    public class Employee : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _IEmailService;

        public Employee(ICrmrpo _ICrmrpo, admin_NDCrMContext _context, IConfiguration configuration, IEmailService _IEmailService)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
            this._IEmailService = _IEmailService;
            _configuration = configuration;


        }
        [HttpGet, Route("Employee/EmployeeRegistration")]
        public async Task<IActionResult> EmployeeRegistration(string id)
        {

            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.userId = adminlogin.Vendorid;
                ViewBag.WorkLocation = _context.Cities
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.City1
                })
                 .ToList();
                ViewBag.Cities = _context.Cities
               .Select(w => new SelectListItem
               {
                   Value = w.Id.ToString(),
                   Text = w.City1
               })
                .ToList();
                ViewBag.stes = _context.States
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.SName
                })
                 .ToList();
                ViewBag.Department = _context.DepartmentMasters.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.DepartmentName

                }).ToList();
                //Designation dropdown 
                ViewBag.Designation = _context.DesignationMasters.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.DesignationName
                }).ToList();
                //States dropdown
                ViewBag.States = _context.States.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.SName
                })
                 .ToList();
                //CustomerName dropdown
                ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                //Offerletter dropdown
                ViewBag.Offerletter = _context.Offerletters.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();
                ViewBag.officeshift = _context.Officeshifts.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Starttime} - {x.Endtime} - {x.ShiftTypeid}" 
                }).ToList();

                ViewBag.Emp_Reg_Code = "";
                ViewBag.First_Name = "";
                ViewBag.Middle_Name = "";
                ViewBag.Last_Name = "";
                ViewBag.Date_Of_Joining = "";
                ViewBag.Work_Email = "";
                ViewBag.Gender_ID = "";
                ViewBag.Work_Location_ID = "";
                ViewBag.Designation_ID = "";
                ViewBag.Department_ID = "";
                ViewBag.CustomerID = "";
                ViewBag.Basic = "";
                ViewBag.AnnualCTC = "";
                ViewBag.HouseRentAllowance = "";
                ViewBag.TravellingAllowance = "";
                ViewBag.ESIC = "";
                ViewBag.EPF = "";
                ViewBag.MonthlyGrossPay = "";
                ViewBag.MonthlyCTC = "";
                ViewBag.Personal_Email_Address = "";
                ViewBag.Mobile_Number = "";
                ViewBag.Date_Of_Birth = "";
                ViewBag.Age = "";
                ViewBag.Father_Name = "";
                ViewBag.PAN = "";
                ViewBag.Address_Line_1 = "";
                ViewBag.Address_Line_2 = "";
                ViewBag.City = "";
                ViewBag.State_ID = "";
                ViewBag.Pincode = "";
                ViewBag.Account_Holder_Name = "";
                ViewBag.Bank_Name = "";
                ViewBag.Account_Number = "";
                ViewBag.Re_Enter_Account_Number = "";
                ViewBag.IFSC = "";
                ViewBag.Account_Type_ID = "";
                ViewBag.EPF_Number = "";
                ViewBag.Deduction_Cycle = "";
                ViewBag.Employee_Contribution_Rate = "";
                ViewBag.Professionaltax = "";
                ViewBag.nominee = "";
                ViewBag.servicecharge = "";
                ViewBag.specialallowance = "";
                ViewBag.gross = "";
                ViewBag.Amount = "";
                ViewBag.Tdspercentage = "";
                ViewBag.Offerletters = "";
                ViewBag.shifttype = "";
                ViewBag.statesy = "";
                ViewBag.btnText = "SAVE";

                if (id != null)
                {
                    DataTable dtEmployeeRecord = _ICrmrpo.GetEmployDetailById(id, Userid);

                    if (dtEmployeeRecord != null && dtEmployeeRecord.Rows.Count > 0)
                    {
                        DataRow row = dtEmployeeRecord.Rows[0] as DataRow; // Explicit cast to DataRow
                        if (row != null)
                        {

                            ViewBag.First_Name = row["First_Name"].ToString();
                            ViewBag.Middle_Name = row["Middle_Name"].ToString();
                            ViewBag.Last_Name = row["Last_Name"].ToString();
                            ViewBag.Date_Of_Joining = ((DateTime)row["Date_Of_Joining"]).ToString("yyyy-MM-dd");
                            ViewBag.Work_Email = row["Work_Email"].ToString();
                            ViewBag.Gender_ID = row["Gender_ID"].ToString();
                            ViewBag.Work_Location_ID = row["Work_Location_ID"].ToString();
                            ViewBag.Designation_ID = row["Designation_ID"].ToString();
                            ViewBag.Department_ID = row["Department_ID"].ToString();
                            ViewBag.CustomerID = row["Vendorid"].ToString();
                            ViewBag.Basic = row["Basic"].ToString();
                            ViewBag.AnnualCTC = row["AnnualCTC"].ToString();
                            ViewBag.HouseRentAllowance = row["HouseRentAllowance"].ToString();
                            ViewBag.TravellingAllowance = row["TravellingAllowance"].ToString();
                            ViewBag.ESIC = row["ESIC"].ToString();
                            ViewBag.EPF = row["EPF"].ToString();
                            ViewBag.MonthlyGrossPay = row["MonthlyGrossPay"].ToString();
                            ViewBag.MonthlyCTC = row["MonthlyCTC"].ToString();
                            ViewBag.Personal_Email_Address = row["Personal_Email_Address"].ToString();
                            ViewBag.Mobile_Number = row["Mobile_Number"].ToString();
                            ViewBag.Date_Of_Birth = ((DateTime)row["Date_Of_Birth"]).ToString("yyyy-MM-dd");
                            ViewBag.Age = row["Age"].ToString();
                            ViewBag.Father_Name = row["Father_Name"].ToString();
                            ViewBag.PAN = row["PAN"].ToString();
                            ViewBag.Address_Line_1 = row["Address_Line_1"].ToString();
                            ViewBag.Address_Line_2 = row["Address_Line_2"].ToString();
                            ViewBag.City = row["City"].ToString();
                            ViewBag.State_ID = row["State_ID"].ToString();
                            ViewBag.Pincode = row["Pincode"].ToString();
                            ViewBag.Account_Holder_Name = row["Account_Holder_Name"].ToString();
                            ViewBag.Bank_Name = row["Bank_Name"].ToString();
                            ViewBag.Account_Number = row["Account_Number"].ToString();
                            ViewBag.Re_Enter_Account_Number = row["Re_Enter_Account_Number"].ToString();
                            ViewBag.IFSC = row["IFSC"].ToString();
                            ViewBag.Account_Type_ID = row["Account_Type_ID"].ToString();
                            ViewBag.EPF_Number = row["EPF_Number"].ToString();
                            ViewBag.Deduction_Cycle = row["Deduction_Cycle"].ToString();
                            ViewBag.Employee_Contribution_Rate = row["Employee_Contribution_Rate"].ToString();
                            ViewBag.Professionaltax = row["Professionaltax"].ToString();
                            ViewBag.nominee = row["nominee"].ToString();
                            ViewBag.servicecharge = row["servicecharge"].ToString();
                            ViewBag.specialallowance = row["SpecialAllowance"].ToString();
                            ViewBag.gross = row["gross"].ToString();
                            ViewBag.Amount = row["Amount"].ToString();
                            ViewBag.Tdspercentage = row["tdspercentage"].ToString();
                            ViewBag.Offerletters = row["offerletterid"].ToString();
                            ViewBag.statesy = row["stateId"].ToString();
                            ViewBag.shifttype = row["officeshiftTypeid"].ToString();
                            ViewBag.Emp_Reg_Code = id;
                            ViewBag.btnText = "UPDATE";

                        }

                    }
                }
                return View();

            }

            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> EmployeeRegistration(EmpMultiform model)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                string Mode = "INS";
                string Empid = "";
                if (!string.IsNullOrEmpty(model.Emp_Reg_ID))
                {
                    Mode = "UPD";
                    Empid = model.Emp_Reg_ID;
                }
                else
                {
                    Empid = GenerateEmployeeId();
                    model.EmployeeId = Empid;
                    var existingEmployee = _context.EmployeeRegistrations.FirstOrDefault(x => x.WorkEmail == model.WorkEmail);
                    if (existingEmployee != null)
                    {
                        TempData["Message"] = "WorkEmail already exists";
                        return View();
                    }
                }
                if (Userid != null)
                {
                    if (Mode == "INS")
                    {
                        TempData["Message"] = "Employee Registration successful";
                        var response = await _ICrmrpo.EmpRegistration(model, Mode, Empid, Userid);
                        ModelState.Clear();
                        return RedirectToAction("EmployeeRegistration");
                    }
                    if (Mode == "UPD")
                    {
                        TempData["Message"] = "Employee Data Update successful";
                        var response = await _ICrmrpo.EmpRegistration(model, Mode, Empid, Userid);
                        ModelState.Clear();
                        return RedirectToAction("EmployeeRegistration");
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
                return View();
            }
        }
        [HttpGet, Route("Employee/Employeelist")]
        public async Task<IActionResult> Employeelist()
        {
            try
            {
                List<EmployeeImportExcel> response = new List<EmployeeImportExcel>();
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    string userIdString = HttpContext.Session.GetString("UserId");
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id ==Convert.ToInt16(userIdString)).FirstOrDefaultAsync();
                    ViewBag.officeshift = _context.Officeshifts.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = $"{x.Starttime} - {x.Endtime} - {x.ShiftTypeid}"
                    }).ToList();
                    if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int id))
                    {
                        if (id == 1)
                        {
                            response = await _ICrmrpo.EmployeeList();
                        }
                        else
                        {
                            response = await _ICrmrpo.CustomerEmployeeList(id);
                            foreach(var item in response)
                            {
                                ViewBag.shiftlist = item.ShiftTypeid;
                            }
                        }

                        ViewBag.UserName = HttpContext.Session.GetString("UserName");
                        return View(response);
                    }
                    response = await _ICrmrpo.EmployeeList();
                    ViewBag.UserName = HttpContext.Session.GetString("UserName");
                    return View(response);
                }
                return RedirectToAction("Login", "Admin");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }


        public static int CalculatAge(DateTime DOB)
        {
            DateTime currentDate = DateTime.Now;
            int age = currentDate.Year - DOB.Year;

            // Check if the birthday for this year has occurred yet
            if (currentDate.Month < DOB.Month || (currentDate.Month == DOB.Month && currentDate.Day < DOB.Day))
            {
                age--;
            }

            return age;

        }

        public async Task<IActionResult> DeleteEmployee(string Emp_Reg_ID)
        {
            try
            {
                var data = _context.EmployeeRegistrations.SingleOrDefault(e => e.EmployeeId == Emp_Reg_ID);
                var data1 = _context.EmployeePersonalDetails.SingleOrDefault(e => e.EmpRegId == Emp_Reg_ID);
                var data2 = _context.EmployeeSalaryDetails.SingleOrDefault(e => e.EmployeeId == Emp_Reg_ID);
                var data3 = _context.EmployeeBankDetails.SingleOrDefault(e => e.EmpId == Emp_Reg_ID);

                if (data != null && data1 != null && data2 != null && data3 != null)
                {
                    data.IsDeleted = true;
                    data1.IsDeleted = true;
                    data2.IsDeleted = true;
                    data3.IsDeleted = true;
                    _context.SaveChanges();
                }

                return RedirectToAction("Employeelist");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the employee:" + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteBasicEmp(int id)
        {
            try
            {
                var data = _context.EmployeePersonalDetails.Find(id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    _context.SaveChanges();
                }
                return RedirectToAction("EmployeeBasicinfoList");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the BasicEmployee:" + ex.Message);
            }
        }
        [HttpGet]
        public JsonResult Edit(int id)
        {
            var emp = _ICrmrpo.GetempPersonalDetailById(id);
            var statedata = _context.StateMasters.ToList();
            var result = new
            {
                Emp = emp,
                StateData = statedata,

            };
            return new JsonResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> update(EmployeePersonalDetail model)
        {
            try
            {
                var response = await _ICrmrpo.Iupdate(model);
                if (response != null)
                {

                    return RedirectToAction("EmployeeBasicinfoList", "Employee");
                    TempData["msg"] = "EmployeeBasicinfo Successfully.";
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
        public JsonResult EditEmployee(int id)
        {
            var data = _context.EmployeeRegistrations.Where(e => e.Id == id).SingleOrDefault();
            var gender = _context.GenderMasters.ToList();
            var worklocation = _context.WorkLocations.ToList();
            var designation = _context.DesignationMasters.ToList();
            var department = _context.DepartmentMasters.ToList();
            var MonthlyCTC = _context.EmployeeSalaryDetails.Where(x => x.EmployeeId == data.EmployeeId).FirstOrDefault();
            var result = new
            {
                Data = data,
                Gender = gender,
                Worklocation = worklocation,
                Designation = designation,
                Department = department,
                MonthlyCTC = MonthlyCTC,

            };
            return new JsonResult(result);

        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(EmployeeList model)
        {
            try
            {
                var product = await _ICrmrpo.updateEmployee(model);
                if (product != null)
                {
                    return RedirectToAction("Employeelist", "Employee");
                    TempData["msg"] = "product update Successfully.";
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
        [HttpGet]
        [Route("Employee/salarydetail")]
        public async Task<IActionResult> salarydetail()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var attt = await _context.Attendancedays.Where(x => x.Vendorid == adminlogin.Vendorid).FirstOrDefaultAsync();
                ViewBag.Nodays = attt.Nodays;
                ViewBag.UserName = AddedBy;
                TempData["UserName"] = AddedBy;
                if (!string.IsNullOrEmpty(AddedBy))
                {
                    HttpContext.Session.SetString("UserName", AddedBy);
                }
                var response = await _ICrmrpo.salarydetail(Userid);
                decimal total = 0.00M;
                foreach (var item in response)
                {
                    total += (decimal)item.MonthlyCtc;
                }
                ViewBag.TotalAmmount = total;
                ViewBag.CustomerName = _context.CustomerRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }


        [HttpPost]
        public async Task<JsonResult> Empattendance(List<Empattendance> customers)
        {
            bool isActive = false;

            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.UserName = AddedBy;
                foreach (Empattendance empattendance in customers)
                {
                    var month = await _context.Empattendances.Where(x => x.Month == DateTime.Now.Month && x.EmployeeId == empattendance.EmployeeId).ToListAsync();
                    if (month.Count > 0)
                    {
                        isActive = true;
                    }
                }
                if (!isActive)
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {

                            foreach (var item in customers)
                            {
                                var ctc = await _context.EmployeeSalaryDetails
                                    .Where(x => x.EmployeeId == item.EmployeeId)
                                    .FirstOrDefaultAsync();
                                var attt = await _context.Attendancedays.Where(x => x.Vendorid == adminlogin.Vendorid).FirstOrDefaultAsync();
                                if (ctc.Incentive != null && ctc.TravellingAllowance != null)
                                {
                                    if (ctc.Incentive != null && ctc.Incentive >= 0 && ctc.TravellingAllowance != null && ctc.TravellingAllowance >= 0)
                                    {
                                        ctc.Incentive = 0;
                                        ctc.TravellingAllowance = 0;
                                        await _context.SaveChangesAsync();
                                    }
                                }

                                if (item.Id != 0)
                                {
                                    Empattendance emp = new Empattendance
                                    {
                                        EmployeeId = item.EmployeeId,
                                        Month = DateTime.Now.Month,
                                        Year = DateTime.Now.Year,
                                        Attendance = item.Attendance,
                                        Entry = DateTime.Now,
                                        Incentive = item.Incentive,
                                        TravellingAllowance = item.TravellingAllowance,
                                        GenerateSalary = decimal.Round((decimal)(ctc.MonthlyCtc /Convert.ToDecimal(attt.Nodays) * item.Attendance), 2) + item.Incentive,
                                        Lop = (decimal)(ctc.MonthlyCtc) - decimal.Round((decimal)(ctc.MonthlyCtc / Convert.ToDecimal(attt.Nodays) * item.Attendance), 2),
                                    };

                                    _context.Empattendances.Add(emp);
                                    await _context.SaveChangesAsync();
                                }
                            }

                            transaction.Commit();

                            foreach (var item in customers)
                            {
                                SendPDF(item.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                            return Json(new { success = false, message = "Error occurred while saving data." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                return Json(new { success = false, message = "Error occurred while checking existing data." });
            }
            return Json(new { success = true, message = "Data saved successfully.", Data = isActive });
        }
        [HttpGet, Route("Employee/GenerateSalary")]
        public async Task<IActionResult> GenerateSalary()
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.UserName = AddedBy;
                ViewBag.CustomerName = _context.CustomerRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                return View();

            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
        [HttpPost]
        public IActionResult GetLocationsByCustomer(string customerId)
        {
            var locationsJsonblank = "";
            if (!string.IsNullOrEmpty(customerId))
            {
                var locations = _context.CustomerRegistrations.FirstOrDefault(x => x.Id == Convert.ToInt32(customerId));
                string[] strlocation = locations.WorkLocation?.Split(new string[] { "," }, StringSplitOptions.None);
                List<City> locationlist = new List<City>();

                foreach (var loc in strlocation)
                {
                    locationlist.Add(_context.Cities.FirstOrDefault(x => x.Id == Convert.ToInt32(loc)));
                }


                var locationsJson = locationlist.Select(x => new SelectListItem
                {
                    Text = x.Id.ToString(),
                    Value = x.City1
                }).ToList();

                return Json(locationsJson);
            }

            return Json(locationsJsonblank);

        }
        [HttpPost]
        public async Task<IActionResult> GenerateSalary(int Month, int year)
        {
            try
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();

                ViewBag.monthid = Month;
                ViewBag.yearid = year;

                if (Month != 0 && year != 0)
                {
                    // Fetch customer information
                    ViewBag.CustomerName = _context.CustomerRegistrations
                        .Where(x => x.Vendorid == adminlogin.Vendorid)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.CompanyName
                        }).ToList();

                    // Call to generate salary
                    GenerateSalary salary = new GenerateSalary();
                    salary.GeneratedSalaries = await _ICrmrpo.GenerateSalary(Month, year, Userid);

                    // Check if data is found
                    if (salary.GeneratedSalaries.Count > 0)
                    {
                        return View(salary);
                    }
                    else
                    {
                        TempData["Message"] = "No data found for the selected month and year.";
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("GenerateSalary");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "An error occurred: " + ex.Message;
                return RedirectToAction("GenerateSalary");
            }
        }
        [Route("Employee/SalarySlipInPDF")]
        public async Task<IActionResult> SalarySlipInPDF(int id)
        {
            try
            {
                string AddedBy = HttpContext.Session.Id;
                ViewBag.UserName = AddedBy;

                var result = await (from emp in _context.EmployeeRegistrations
                                    join empsalary in _context.EmployeeSalaryDetails on emp.EmployeeId equals empsalary.EmployeeId into empsalaryGroup
                                    from empsalary in empsalaryGroup.DefaultIfEmpty()
                                    join empbank in _context.EmployeeBankDetails on emp.EmployeeId equals empbank.EmpId into empbankGroup
                                    from empbank in empbankGroup.DefaultIfEmpty()
                                    join empatt in _context.Empattendances on emp.EmployeeId equals empatt.EmployeeId into empattGroup
                                    from empatt in empattGroup.DefaultIfEmpty()
                                    join worklocation in _context.Cities on emp.WorkLocationId equals worklocation.Id.ToString() into worklocationGroup
                                    from worklocation in worklocationGroup.DefaultIfEmpty()
                                    join designation in _context.DesignationMasters on emp.DesignationId equals designation.Id.ToString() into designationGroup
                                    from designation in designationGroup.DefaultIfEmpty()
                                    join tds in _context.EmployeerTds on emp.Vendorid equals tds.CustomerId into tdsGroup
                                    from tds in tdsGroup.DefaultIfEmpty()
                                    join vrs in _context.VendorRegistrations on emp.Vendorid equals (int?)vrs.Id into vrsGroup
                                    from vrs in vrsGroup.DefaultIfEmpty()
                                    where emp.Id == id
                                    select new SalarySlipDetails
                                    {
                                        Id = emp.Id,
                                        Employee_ID = emp.EmployeeId,
                                        First_Name = emp.FirstName,
                                        Address_Line_1 = worklocation.City1,
                                        Epf = empsalary.Epf,
                                        Designation_Name = designation.DesignationName,
                                        Bank_Name = empbank.BankName,
                                        Account_Number = empbank.AccountNumber,
                                        Basic = empsalary.Basic,
                                        EPF_Number = empbank.EpfNumber,
                                        Month = getMonthName(Convert.ToInt32(empatt.Month)),
                                        Year = empatt.Year,
                                        HouseRentAllowance = empsalary.HouseRentAllowance,
                                        Lop = empatt.Lop,
                                        Professionaltax = empsalary.Professionaltax,
                                        TravellingAllowance = empatt.TravellingAllowance,
                                        SpecialAllowance = empsalary.SpecialAllowance,
                                        Esic = empsalary.Esic,
                                        Amount = tds.Amount,
                                        CompanyName = vrs.CompanyName,
                                        CompanyImage = vrs.CompanyImage
                                    }).FirstOrDefaultAsync();

                if (result != null)
                {
                    return View(result);
                }
                else
                {
                    return View();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public IActionResult Employer()
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    string AddedBy = HttpContext.Session.GetString("UserName");
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin =  _context.AdminLogins.Where(x => x.Id ==  Userid).FirstOrDefault();
                    ViewBag.UserName = AddedBy;
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Employer(EmployeerModelEPF model)
        {
            try
            {
                var response = await _ICrmrpo.Employer(model);

                ModelState.Clear();
                return RedirectToAction("Employer");

            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        [HttpGet]
        [Route("Employee/Employee_list")]
        public async Task<IActionResult> Employee_list(string Deduction_Cycle)
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            int? userId = HttpContext.Session.GetString("UserId") != null
                ? Convert.ToInt32(HttpContext.Session.GetString("UserId"))
                : (int?)null;

            if (userId == null)
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Login");
            }

            if (!string.IsNullOrEmpty(Deduction_Cycle))
            {
                var adminLogin = await _context.AdminLogins.Where(x => x.Id == userId).FirstOrDefaultAsync();
                var response = await _ICrmrpo.EmployerList(Deduction_Cycle);

                if (response.Count > 0)
                {
                    return View(response);
                }
                else
                {
                    ViewBag.ErrorMessage = "No details found.";
                    return View();
                }
            }
            else
            {
                ModelState.Clear();
                return View();
            }
        }

        //  send  pdf and mail //

        public IActionResult DocPDF(int id)
        {
            try
            {
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                HtmlToPdf converter = new HtmlToPdf();
                string SlipURL = $"{schema}://{host}/Employee/SalarySlipInPDF?id={id}";
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }
                string uniqueFileName = $"SalarySlip_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string filePath = Path.Combine(wwwRootPath, uniqueFileName);
                doc.Save(filePath);
                byte[] pdf = System.IO.File.ReadAllBytes(filePath);
                doc.Close();

                var result = (from emp in _context.EmployeeRegistrations
                              where emp.Id == id
                              select new SalarySlipDetails
                              {
                                  Id = emp.Id,
                                  Employee_ID = emp.EmployeeId,
                                  First_Name = emp.FirstName,
                                  Email_Id = emp.WorkEmail
                              }).FirstOrDefault();

                if (result != null)
                {
                    string fileName = $"{result.Employee_ID}_SalarySlip.pdf";
                    string emailSubject = $"Salary Slip for {result.Employee_ID}";
                    string emailBody = $"Hello {result.First_Name} ({result.Employee_ID}), please find your attached salary slip.";
                    _IEmailService.SendEmailAsync(result.Email_Id, emailSubject, emailBody, pdf, fileName, "application/pdf");

                    return Json(new { success = true, message = "Salary Slip has been Sent successfully.", fileName = fileName, pdf = Convert.ToBase64String(pdf) });
                }

                return Json(new { success = false, message = "Error: Employee not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        public void SendPDF(int id)
        {
            try
            {
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                HtmlToPdf converter = new HtmlToPdf();
                string SlipURL = $"{schema}://{host}/Employee/SalarySlipInPDF?id={id}";
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }
                string uniqueFileName = $"SalarySlip_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string filePath = Path.Combine(wwwRootPath, uniqueFileName);
                doc.Save(filePath);
                byte[] pdf = System.IO.File.ReadAllBytes(filePath);
                doc.Close();
                string savedFileName = uniqueFileName;
                var result = (from emp in _context.EmployeeRegistrations
                              where emp.Id == id
                              select new SalarySlipDetails
                              {
                                  Id = emp.Id,
                                  Employee_ID = emp.EmployeeId,
                                  First_Name = emp.FirstName,
                                  Email_Id = emp.WorkEmail
                              }).FirstOrDefault();

                if (result == null)
                {
                    throw new Exception("Employee not found.");
                }
                var empAttendance = _context.Empattendances.FirstOrDefault(e => e.EmployeeId == result.Employee_ID && e.Month == DateTime.Now.Month && e.Year == DateTime.Now.Year);
                if (empAttendance != null)
                {
                    empAttendance.SalarySlip = savedFileName;
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Attendance record not found for the employee.");
                }
                string Email_Subject = $"Salary Slip for {result.Employee_ID}";
                string Email_body = $"Hello {result.First_Name} ({result.Employee_ID}), please find your attached salary slip.";
                _IEmailService.SendEmailAsync(result.Email_Id, Email_Subject, Email_body, pdf, "SalarySlip.pdf", "application/pdf");

                Console.WriteLine($"Salary slip for Employee ID {result.Employee_ID} has been saved and the Empattendance table has been updated.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getMonthName(int monthValue)
        {
            string monthName;

            switch (monthValue)
            {
                case 1:
                    monthName = "January";
                    break;
                case 2:
                    monthName = "February";
                    break;
                case 3:
                    monthName = "March";
                    break;
                case 4:
                    monthName = "April";
                    break;
                case 5:
                    monthName = "May";
                    break;
                case 6:
                    monthName = "June";
                    break;
                case 7:
                    monthName = "July";
                    break;
                case 8:
                    monthName = "August";
                    break;
                case 9:
                    monthName = "September";
                    break;
                case 10:
                    monthName = "October";
                    break;
                case 11:
                    monthName = "November";
                    break;
                case 12:
                    monthName = "December";
                    break;
                default:
                    monthName = "Invalid Month";
                    break;
            }

            return monthName;
        }
        [HttpGet]
        [Route("Employee/Invoice")]
        public async Task<IActionResult> Invoice(string customerId, int Month, int year, string WorkLocation)
        {
            try
            {
                if (customerId != null && Month != null && year != null && WorkLocation != null)
                {
                    ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                    List<Invoice> invoice = new List<Invoice>();

                    invoice = await _ICrmrpo.GenerateInvoice(customerId, Month, year, WorkLocation);
                    if (invoice.Count > 0)
                    {
                        return View(invoice[0]);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No data found";
                        return RedirectToAction("GenerateSalary");
                    }
                }
                else
                {
                    return RedirectToAction("GenerateSalary");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }


        }

        //-----ImportToExcelEmployeeList
        public IActionResult ImportToExcelEmployeeList()
        {
            try
            {
                var response = _ICrmrpo.EmployeeListForExcel();
                return File(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee_List.xlsx");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }
        public JsonResult EditSalaryDetails(string EmployeeId)
        {
            var empSalaryDetail = new EmployeeSalaryDetail();
            var data = _ICrmrpo.GetempSalaryDetailtById(EmployeeId);
            empSalaryDetail.EmployeeId = data.EmployeeId;
            empSalaryDetail.AnnualCtc = data.AnnualCtc;
            empSalaryDetail.Esic = data.Esic;
            empSalaryDetail.TravellingAllowance = data.TravellingAllowance;
            empSalaryDetail.Professionaltax = data.Professionaltax;
            empSalaryDetail.Basic = data.Basic;
            empSalaryDetail.HouseRentAllowance = data.HouseRentAllowance;
            empSalaryDetail.Epf = data.Epf;
            empSalaryDetail.MonthlyCtc = data.MonthlyCtc;
            empSalaryDetail.MonthlyGrossPay = data.MonthlyGrossPay;
            empSalaryDetail.Incentive = data.Incentive;
            empSalaryDetail.TravellingAllowance = data.TravellingAllowance;
            var result = new
            {
                empSalaryDetail = empSalaryDetail,

            };
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditSalaryDetails(EmployeeSalaryDetail model)
        {
            try
            {
                var Salary = await _ICrmrpo.updateSalaryDetail(model);
                if (Salary != null)
                {
                    ViewBag.Message = "salarydetail update Successfully.";
                    return RedirectToAction("salarydetail", "Employee");
                }
                else
                {
                    ModelState.Clear();
                    return RedirectToAction("salarydetail", "Employee");
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        [HttpGet]
        [Route("/Employee/ESCDownloadExcel")]
        public async Task<IActionResult> ESCDownloadExcel(string customerId, string WorkLocation)
        {
            try
            {
                var employeeList = await _ICrmrpo.ESCExcel(customerId, WorkLocation).ConfigureAwait(false);
                if (employeeList.Count != 0)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("ESC");
                        var currentRow = 1;

                        worksheet.Cell(currentRow, 1).Value = "Sr.No.";
                        worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 2).Value = "Employee ID";
                        worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 3).Value = "Employee Name";
                        worksheet.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 4).Value = "Account Number";
                        worksheet.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 5).Value = "IFSC";
                        worksheet.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 6).Value = "netpayment";
                        worksheet.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.Yellow;

                        currentRow++;

                        var index = 1;
                        foreach (var item in employeeList)
                        {
                            worksheet.Cell(currentRow, 1).Value = index++;
                            worksheet.Cell(currentRow, 2).Value = item.EmployeeId;
                            worksheet.Cell(currentRow, 3).Value = item.FirstName;
                            worksheet.Cell(currentRow, 4).Value = item.AccountNumber;
                            worksheet.Cell(currentRow, 5).Value = item.Ifsc;
                            worksheet.Cell(currentRow, 6).Value = item.netpayment;
                            currentRow++;
                        }


                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            var fileBytes = stream.ToArray();
                            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Ecsreport.xlsx");
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "No salary details found.";
                    return RedirectToAction("salarydetail", "Employee");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        [Route("/Employee/GenerateSalaryReport")]
        public IActionResult GenerateSalaryReport()
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
                return View();

            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSalaryReport(string customerId, int Month, int year, string WorkLocation)
        {
            try
            {
                ViewBag.custid = customerId;
                ViewBag.locid = WorkLocation;
                ViewBag.monthid = Month;
                ViewBag.yearid = year;
                if (customerId != null && Month != null && year != null && WorkLocation != null)
                {
                    ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                    GenerateSalaryReportDTO salary = new GenerateSalaryReportDTO();



                    salary.GenerateSalaryReports = await _ICrmrpo.GenerateSalaryReport(customerId, Month, year, WorkLocation);
                    decimal total = 0.00M;
                    foreach (var item in salary.GenerateSalaryReports)
                    {
                        total += (decimal)item.GenerateSalary;
                    }
                    ViewBag.TotalAmmount = total;
                    if (salary.GenerateSalaryReports.Count > 0)

                    {
                        return View(salary);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No data found";
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("GenerateSalaryReport");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }


        }

        public IActionResult EPFReport()
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
                return View();

            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> EPFReport(string customerId, int Month, int year, string WorkLocation)

        {
            try
            {
                ViewBag.custid = customerId;
                ViewBag.locid = WorkLocation;
                ViewBag.monthid = Month;
                ViewBag.yearid = year;
                if (customerId != null && Month != null && year != null && WorkLocation != null)
                {
                    ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                    EPFReportDTO salary = new EPFReportDTO();



                    salary.EPFReports = await _ICrmrpo.EPFReport(customerId, Month, year, WorkLocation);
                    if (salary.EPFReports.Count > 0)

                    {
                        return View(salary);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No data found";
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("EPFReport");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }


        }

        public IActionResult ESIReport()
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
                return View();

            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ESIReport(string customerId, int Month, int year, string WorkLocation)

        {
            try
            {
                ViewBag.custid = customerId;
                ViewBag.locid = WorkLocation;
                ViewBag.monthid = Month;
                ViewBag.yearid = year;
                if (customerId != null && Month != null && year != null && WorkLocation != null)
                {
                    ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                    EPFReportDTO salary = new EPFReportDTO();



                    salary.EPFReports = await _ICrmrpo.ESIReport(customerId, Month, year, WorkLocation);
                    if (salary.EPFReports.Count > 0)

                    {
                        return View(salary);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No data found";
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("EPFReport");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }


        }
        private string GenerateEmployeeId()
        {
            var data = _context.EmployeeRegistrations
                              .OrderByDescending(x => x.Id)
                              .FirstOrDefault();
            string EmpID = string.Empty;
            int numericValue = 1001;

            if (data != null && !string.IsNullOrEmpty(data.EmployeeId))
            {
                string[] parts = data.EmployeeId.Split('-');
                if (parts.Length > 1 && int.TryParse(parts.Last(), out numericValue))
                {
                    numericValue++;
                }
            }
            EmpID = $"NDT-{numericValue:D4}";

            return EmpID;
        }
        public async Task<IActionResult> DeleteEmployer(int id)
        {
            try
            {
                var data = _context.EmployeerEpfs.Find(id);
                if (data != null)
                {
                    _context.EmployeerEpfs.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("Employee_list");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public JsonResult EditEmployer(int id)
        {
            var epf = new EmployeerEpf();
            var data = _ICrmrpo.GetEmployer(id);
            epf.Id = data.Id;
            epf.EpfNumber = data.EpfNumber;
            epf.DeductionCycle = data.DeductionCycle;
            epf.EmployerContributionRate = data.EmployerContributionRate;
            var result = new
            {
                epf = epf,
            };
            return new JsonResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditEmployer(EmployeerEpf model)
        {
            try
            {
                var Location = await _ICrmrpo.updateEmployer(model);
                if (Location != null)
                {
                    TempData["ErrorMessage"] = "Employer update Successfully.";
                    return RedirectToAction("Employee_list", "Employee");
                }
                else
                {
                    TempData["ErrorMessage"] = "Employer not update.";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
            //}
        }
        public JsonResult tdsDetails(int CustomerId)
        {
            var employeerTd = new EmployeerTd();
            if (CustomerId > 0)
            {
                var data = _ICrmrpo.tdsDetails(CustomerId);
                if (data != null)
                {
                    employeerTd.Tdspercentage = data.Tdspercentage;
                    employeerTd.Amount = data.Amount;
                }
                var result = new
                {
                    employeerTd = employeerTd,
                };
                return new JsonResult(result);
            }
            var errorResult = new JsonResult(new
            {
                error = "Invalid CustomerId. CustomerId must be greater than 0."
            });
            return errorResult;
        }
        public JsonResult Epfesilist()
        {
            var employeerTd = new EmployeerEpf();
            var data = _context.EmployeerEpfs.Where(e => e.IsActive == true).ToList();
            var result = new
            {
                data = data,
            };
            return new JsonResult(result);
        }

        //public IActionResult ImportToExcelAttendance(string customerId, string WorkLocation)
        //{
        //    try
        //    {
        //        var data = _ICrmrpo.salarydetail(customerId, WorkLocation).Result;
        //        var response = _ICrmrpo.ImportToExcelAttendance(data);
        //        return File(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee_Attandence_List.xlsx");
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }

        //}

        [HttpPost]
        public ActionResult ImportProductionExcel(IFormFile upload)
        {

            if (ModelState.IsValid)
            {
                if (upload != null && upload.Length > 0)
                {
                    Stream stream = upload.OpenReadStream();

                    IXLWorkbook workbook = null;

                    try
                    {
                        // Load workbook using ClosedXML
                        workbook = new XLWorkbook(stream);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("File", "Unable to Upload file!");
                        return View();
                    }

                    IXLWorksheet worksheet = workbook.Worksheets.First();

                    DataTable dt = new DataTable();

                    try
                    {
                        foreach (IXLCell cell in worksheet.Row(1).Cells())
                        {
                            string columnName = cell.Value.ToString();

                            // Add the column to the DataTable
                            dt.Columns.Add(columnName);
                        }


                        for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                        {
                            DataRow newRow = dt.Rows.Add();
                            for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                            {
                                newRow[col - 1] = worksheet.Cell(row, col).Value.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("File", "Error processing Excel file!");
                        return View();
                    }

                    DataSet result = new DataSet();
                    result.Tables.Add(dt);
                    var rcount = 0;

                    var attendance = new List<salarydetail>();

                    foreach (DataRow row in dt.Rows)
                    {
                        var attend = new salarydetail();
                        rcount++;

                        var ccount = 0;
                        foreach (DataColumn col in dt.Columns)
                        {
                            try
                            {
                                ccount++;
                                Console.WriteLine(row[col]);
                                if (col.Caption == "Sr.No.")
                                {


                                    attend.Id = Convert.ToInt32(row[col].ToString());
                                }
                                else if (col.Caption == "Employee Name")
                                {
                                    string financialYearValue = row[col].ToString().Trim();

                                    // Check if the financial year value exists in the list of FinYear names
                                    attend.FirstName = row[col].ToString();


                                }
                                else if (col.Caption == "Father Name")
                                {
                                    attend.FatherName = row[col].ToString();
                                }
                                else if (col.Caption == "Employee Id")
                                {
                                    attend.EmployeeId = row[col].ToString();
                                }
                                else if (col.Caption == "Monthly CTC")
                                {
                                    attend.MonthlyCtc = Convert.ToInt32(row[col].ToString());
                                }
                                else if (col.Caption == "Attendance")
                                {
                                    attend.Attendance = Convert.ToInt32(row[col].ToString());
                                }


                            }
                            catch (Exception ex)
                            {
                                Console.Write($" row number: {rcount}:{dt.Columns[ccount - 1]}");
                                //  throw ex;
                                continue;
                            }



                        }


                        attendance.Add(attend);
                    }
                    return Json(attendance);


                }


            }
            else
            {
                ModelState.AddModelError("File", "Please Upload Your file");
            }

            return null;
        }
        [HttpGet, Route("/Employee/MonthlysalaryReport")]
        public async Task<IActionResult> MonthlysalaryReport(string customerId, int Month, int year, string WorkLocation)
        {
            try
            {
                var response = await _ICrmrpo.monthlysalaryReport(customerId, Month, year, WorkLocation);
                if (response.Count != 0)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("monthlyReport");
                        var currentRow = 1;

                        worksheet.Cell(currentRow, 1).Value = "Sr.No.";
                        worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 2).Value = "First Name";
                        worksheet.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 3).Value = "Middle Name";
                        worksheet.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 4).Value = "Company Name";
                        worksheet.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 5).Value = "Date Of Joining";
                        worksheet.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 6).Value = "Work Email";
                        worksheet.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 7).Value = "Gender";
                        worksheet.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 8).Value = "Work Location";
                        worksheet.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 9).Value = "Designation";
                        worksheet.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 10).Value = "Department";
                        worksheet.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 11).Value = "Emp_Reg_ID";
                        worksheet.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 12).Value = "Annual CTC";
                        worksheet.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 13).Value = "Basic";
                        worksheet.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 14).Value = "HouseRent Allowance";
                        worksheet.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 15).Value = "Travelling Allowance";
                        worksheet.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 16).Value = "ESIC";
                        worksheet.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 17).Value = "EPF";
                        worksheet.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 18).Value = "Monthly Gross Pay";
                        worksheet.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 19).Value = "Monthly CTC";
                        worksheet.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 20).Value = "Special Allowance";
                        worksheet.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 21).Value = "Gross";
                        worksheet.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 22).Value = "Personal Email Address";
                        worksheet.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 23).Value = "Mobile";
                        worksheet.Cell(currentRow, 24).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 24).Value = "Date Of Birth";
                        worksheet.Cell(currentRow, 25).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 25).Value = "Age";
                        worksheet.Cell(currentRow, 26).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 26).Value = "Father Name";
                        worksheet.Cell(currentRow, 27).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 27).Value = "PAN";
                        worksheet.Cell(currentRow, 28).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 28).Value = "Address Line 1";
                        worksheet.Cell(currentRow, 29).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 29).Value = "Address Line 2";
                        worksheet.Cell(currentRow, 30).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 30).Value = "City";
                        worksheet.Cell(currentRow, 31).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 31).Value = "State";
                        worksheet.Cell(currentRow, 32).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 32).Value = "PinCode";
                        worksheet.Cell(currentRow, 33).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 33).Value = "Account Holder Name";
                        worksheet.Cell(currentRow, 34).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 34).Value = "Bank Name";
                        worksheet.Cell(currentRow, 35).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 35).Value = "Account Number";
                        worksheet.Cell(currentRow, 36).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 36).Value = "Re-enter Account Number";
                        worksheet.Cell(currentRow, 37).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 37).Value = "IFSC";
                        worksheet.Cell(currentRow, 38).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 38).Value = "Account Type";
                        worksheet.Cell(currentRow, 39).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 39).Value = "EPF Number";
                        worksheet.Cell(currentRow, 40).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 40).Value = "Deduction Cycle";
                        worksheet.Cell(currentRow, 41).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 41).Value = "Employee Contribution Rate";
                        worksheet.Cell(currentRow, 42).Style.Fill.BackgroundColor = XLColor.Yellow;
                        worksheet.Cell(currentRow, 42).Value = "Nominee";


                        currentRow++;

                        var index = 1;
                        foreach (var item in response)
                        {
                            worksheet.Cell(currentRow, 1).Value = index++;
                            worksheet.Cell(currentRow, 2).Value = item.FirstName;
                            worksheet.Cell(currentRow, 3).Value = item.MiddleName == null ? "" + item.FirstName + " " + "" + ' ' + "" + "" + item.LastName + "" : "" + item.FirstName + "" + "" + ' ' + "" + "" + item.MiddleName + "" + "" + ' ' + "" + "" + item.LastName + "";
                            worksheet.Cell(currentRow, 4).Value = item.CustomerName;
                            worksheet.Cell(currentRow, 5).Value = item.DateOfJoining;
                            worksheet.Cell(currentRow, 6).Value = item.WorkEmail;
                            worksheet.Cell(currentRow, 7).Value = item.Gender;
                            worksheet.Cell(currentRow, 8).Value = item.WorkLocation;
                            worksheet.Cell(currentRow, 9).Value = item.DesignationName;
                            worksheet.Cell(currentRow, 10).Value = item.DepartmentName;
                            worksheet.Cell(currentRow, 11).Value = item.Emp_Reg_ID;
                            worksheet.Cell(currentRow, 12).Value = item.AnnualCTC;
                            worksheet.Cell(currentRow, 13).Value = item.Basic;
                            worksheet.Cell(currentRow, 14).Value = item.HouseRentAllowance;
                            worksheet.Cell(currentRow, 15).Value = item.TravellingAllowance;
                            worksheet.Cell(currentRow, 16).Value = item.ESIC;
                            worksheet.Cell(currentRow, 17).Value = item.EPF;
                            worksheet.Cell(currentRow, 18).Value = item.MonthlyGrossPay;
                            worksheet.Cell(currentRow, 19).Value = item.MonthlyCTC;
                            worksheet.Cell(currentRow, 20).Value = item.SpecialAllowance;
                            worksheet.Cell(currentRow, 21).Value = item.gross;
                            worksheet.Cell(currentRow, 22).Value = item.PersonalEmailAddress;
                            worksheet.Cell(currentRow, 23).Value = item.Mobile;
                            worksheet.Cell(currentRow, 24).Value = item.DateOfBirth;
                            worksheet.Cell(currentRow, 25).Value = item.Age;
                            worksheet.Cell(currentRow, 26).Value = item.FatherName;
                            worksheet.Cell(currentRow, 27).Value = item.PAN;
                            worksheet.Cell(currentRow, 28).Value = item.AddressLine1;
                            worksheet.Cell(currentRow, 29).Value = item.AddressLine2;
                            worksheet.Cell(currentRow, 30).Value = item.City;
                            worksheet.Cell(currentRow, 31).Value = item.State;
                            worksheet.Cell(currentRow, 32).Value = item.Pincode;
                            worksheet.Cell(currentRow, 33).Value = item.AccountHolderName;
                            worksheet.Cell(currentRow, 34).Value = item.BankName;
                            worksheet.Cell(currentRow, 35).Value = item.AccountNumber;
                            worksheet.Cell(currentRow, 36).Value = item.ReEnterAccountNumber;
                            worksheet.Cell(currentRow, 37).Value = item.IFSC;
                            worksheet.Cell(currentRow, 38).Value = item.AccountType;
                            worksheet.Cell(currentRow, 39).Value = item.EPF_Number;
                            worksheet.Cell(currentRow, 40).Value = item.Deduction_Cycle;
                            worksheet.Cell(currentRow, 41).Value = item.Employee_Contribution_Rate;
                            worksheet.Cell(currentRow, 42).Value = item.nominee;

                            currentRow++;
                        }


                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            var fileBytes = stream.ToArray();
                            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "monthlyReport.xlsx");
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "No salary details found.";
                    return RedirectToAction("GenerateSalary", "Employee");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet, Route("/Employee/EmployeeOfferletter")]
        public async Task<IActionResult> EmployeeOfferletter(int? Id = 0, int? Userid = 0)
        {
            try
            {
                //int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                if (adminlogin == null)
                {
                    return BadRequest("Admin login not found");
                }
                var vendorinfo = _context.VendorRegistrations.Where(v => v.Id == adminlogin.Vendorid).FirstOrDefault();
                var offerletter = await _context.Offerletters
                    .Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false && x.Id == Id)
                    .FirstOrDefaultAsync();
                if (offerletter == null)
                {
                    return BadRequest("Offer letter not found");
                }
                var result = new getempOfferletter
                {
                    Id = offerletter.Id,
                    Name = offerletter.Name,
                    MonthlyCtc = offerletter.MonthlyCtc,
                    AnnualCtc = offerletter.AnnualCtc,
                    CandidateAddress = offerletter.CandidateAddress,
                    CandidatePincode = offerletter.CandidatePincode,
                    HrName = offerletter.HrName,
                    HrJobTitle = offerletter.HrJobTitle,
                    HrSignature = offerletter.HrSignature,
                    Currentdate = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    StateName = _context.States.Where(g => g.Id == offerletter.StateId).Select(g => g.SName).FirstOrDefault(),
                    CityName = _context.Cities.Where(g => g.Id == offerletter.CityId).Select(g => g.City1).FirstOrDefault(),
                    DateOfJoining = offerletter.DateOfJoining?.ToString("dd/MM/yyyy"),
                    DepartmentName = _context.DepartmentMasters.Where(g => g.Id == Convert.ToInt16(offerletter.DepartmentId)).Select(g => g.DepartmentName).FirstOrDefault()?.Trim(),
                    DesignationName = _context.DesignationMasters.Where(g => g.Id == Convert.ToInt16(offerletter.DesignationId)).Select(g => g.DesignationName).FirstOrDefault()?.Trim(),
                    Validdate = offerletter.Validdate?.ToString("dd/MM/yyyy"),
                    CompanyImage = _context.VendorRegistrations.Where(g => g.Id == offerletter.Vendorid).Select(g => g.CompanyImage).FirstOrDefault(),
                    CompanyName = _context.VendorRegistrations.Where(g => g.Id == offerletter.Vendorid).Select(g => g.CompanyName).FirstOrDefault(),
                    OfficeLocation = _context.VendorRegistrations.Where(g => g.Id == offerletter.Vendorid).Select(g => g.WorkLocation).FirstOrDefault()
                };
                return View(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        public async Task<IActionResult> OfferletterDocPDF(int? Id = 0)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                if (adminlogin == null)
                {
                    return BadRequest("Admin login not found");
                }
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                string SlipURL = $"{schema}://{host}/Employee/EmployeeOfferletter?Id={Id}&userid={Userid}";
                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }
                var result = (from off in _context.Offerletters
                              where off.Id == Id && off.Vendorid == adminlogin.Vendorid
                              select new getempOfferletter
                              {
                                  Id = off.Id,
                                  Name = off.Name,
                                  CandidateEmail = off.CandidateEmail
                              }).FirstOrDefault();
                string uniqueFileName = $"Offerletter_{Id}.pdf";
                string filePath = Path.Combine(wwwRootPath, uniqueFileName);
                doc.Save(filePath);
                byte[] pdf = System.IO.File.ReadAllBytes(filePath);

                

                if (result == null)
                {
                    return BadRequest("Employee not found.");
                }

                var empoff = _context.Offerletters.FirstOrDefault(e => e.Id == result.Id);
                if (empoff != null)
                {
                    empoff.OfferletterFile = uniqueFileName;
                    _context.SaveChanges();
                    string emailSubject = $"Offerletter for {result.Name}";
                    string emailBody = $"Hello {result.Name}, please find your attached offer letter.";
                  await  _IEmailService.SendEmailAsync(result.CandidateEmail, emailSubject, emailBody, pdf, uniqueFileName, "application/pdf");
                  return Json(new { success = true, message = "Offer letter  has been Sent successfully.", fileName = uniqueFileName });

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
        [HttpGet, Route("/Employee/Appointmentletter")]
        public async Task<IActionResult> Appointmentletter(int? Id = 0,int? Userid = 0)
        {
            try
            {
                //int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                if (adminlogin == null)
                {
                    return BadRequest("Admin login not found");
                }
                var empdetail = await _context.EmployeeRegistrations
                    .Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false && x.Id == Id)
                    .FirstOrDefaultAsync();
                if (empdetail == null)
                {
                    return BadRequest("Appointment letter not found");
                }
                var result = new EmpAppointmentletter
                {
                    Empcode = empdetail.EmployeeId,
                    First_Name = empdetail.FirstName,
                    MonthlyCtc = _context.EmployeeSalaryDetails.Where(x => x.EmployeeId == empdetail.EmployeeId).Select(x => x.MonthlyCtc).FirstOrDefault(),
                    AnnualCtc = _context.EmployeeSalaryDetails.Where(x => x.EmployeeId == empdetail.EmployeeId).Select(x => x.AnnualCtc).FirstOrDefault(),
                    CompanyName = _context.VendorRegistrations.Where(g => g.Id == empdetail.Vendorid).Select(g => g.CompanyName).FirstOrDefault(),
                    Designation_Name = _context.DesignationMasters.Where(g => g.Id == Convert.ToInt16(empdetail.DesignationId)).Select(g => g.DesignationName).FirstOrDefault()?.Trim(),
                    Department_Name = _context.DepartmentMasters.Where(g => g.Id == Convert.ToInt16(empdetail.DepartmentId)).Select(g => g.DepartmentName).FirstOrDefault()?.Trim(),
                    Joiningdate = empdetail.DateOfJoining?.ToString("dd/MM/yyyy"),
                    OfccityId = _context.Cities.Where(g => g.Id == Convert.ToInt16(empdetail.WorkLocationId)).Select(g => g.City1).FirstOrDefault(),
                    Basic = _context.EmployeeSalaryDetails.Where(x => x.EmployeeId == empdetail.EmployeeId).Select(x => x.Basic).FirstOrDefault(),
                    HouseRentAllowance = _context.EmployeeSalaryDetails.Where(x => x.EmployeeId == empdetail.EmployeeId).Select(x => x.HouseRentAllowance).FirstOrDefault(),
                    EmployeeESIC = _context.EmployeeSalaryDetails.Where(x => x.EmployeeId == empdetail.EmployeeId).Select(x => x.Esic).FirstOrDefault(),
                    EmployeeEPF = _context.EmployeeSalaryDetails.Where(x => x.EmployeeId == empdetail.EmployeeId).Select(x => x.Epf).FirstOrDefault(),
                    CurrentDate = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    CompanyImage = _context.VendorRegistrations.Where(g => g.Id == empdetail.Vendorid).Select(g => g.CompanyImage).FirstOrDefault(),
                    Location = _context.VendorRegistrations.Where(g => g.Id == empdetail.Vendorid).Select(g => g.Location).FirstOrDefault(),
                };
                return View(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        public async Task<IActionResult> AppointmentletterDocPDF(int? Id = 0)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var emp = await _context.EmployeeRegistrations.Where(x => x.Id == Id && adminlogin.Vendorid == x.Vendorid).FirstOrDefaultAsync();
                if (adminlogin == null)
                {
                    return BadRequest("Admin login not found");
                }
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                string SlipURL = $"{schema}://{host}/Employee/Appointmentletter?Id={Id}&userid={Userid}";
                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }
                string uniqueFileName = $"Appointmentletter_{Id}.pdf";
                string filePath = Path.Combine(wwwRootPath, uniqueFileName);
                doc.Save(filePath);
                byte[] pdf = System.IO.File.ReadAllBytes(filePath);

                var result = await _context.EmployeePersonalDetails.Where(x => x.EmpRegId == emp.EmployeeId).FirstOrDefaultAsync();

                if (result == null)
                {
                    return BadRequest("Employee not found.");
                }

                var empoff = _context.EmployeeRegistrations.FirstOrDefault(e => e.EmployeeId == result.EmpRegId);
                if (empoff != null)
                {
                    empoff.Appoinmentletter = uniqueFileName;
                    _context.SaveChanges();
                    string emailSubject = $"Appointmentletter for {emp.FirstName}";
                    string emailBody = $"Hello {emp.FirstName}, please find your attached Appointment letter.";
                    await _IEmailService.SendEmailAsync(result.PersonalEmailAddress, emailSubject, emailBody, pdf, uniqueFileName, "application/pdf");

                    return Json(new { success = true, message = "Appointment letter has been Sent successfully.", fileName = uniqueFileName });
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

        [HttpGet, Route("/Employee/AddOfferletterdetail")]
        public async Task<IActionResult> AddOfferletterdetail(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                ViewBag.Department = await _context.DepartmentMasters.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.DepartmentName
                }).ToListAsync();

                ViewBag.Designation = await _context.DesignationMasters.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.DesignationName
                }).ToListAsync();

                ViewBag.States = await _context.States.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.SName
                }).ToListAsync();

                ViewBag.Cities = await _context.Cities.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.City1
                }).ToListAsync();

                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.userId = adminlogin.Vendorid;
                ViewBag.Heading = "Add Offerletter Detail";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = await _ICrmrpo.GetOfferletterbyid(id);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Name = data.Name;
                        ViewBag.CandidatePincode = data.CandidatePincode;
                        ViewBag.CandidateAddress = data.CandidateAddress;
                        ViewBag.DepartmentId = data.DepartmentId;
                        ViewBag.DesignationId = data.DesignationId;
                        ViewBag.MonthlyCtc = data.MonthlyCtc;
                        ViewBag.AnnualCtc = data.AnnualCtc;
                        ViewBag.StateId = data.StateId;
                        ViewBag.CityId = data.CityId;
                        ViewBag.DateOfJoining = data.DateOfJoining?.ToString("yyyy-MM-dd");
                        ViewBag.Validdate = data.Validdate?.ToString("yyyy-MM-dd");
                        ViewBag.HrJobTitle = data.HrJobTitle;
                        ViewBag.HrSignature = data.HrSignature;
                        ViewBag.HrName = data.HrName;
                        ViewBag.CandidateEmail = data.CandidateEmail;

                        ViewBag.Heading = "Update Offerletter Detail";
                        ViewBag.btnText = "UPDATE";
                    }
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOfferletterdetail(Offerletters model)
        {
            try
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                if (model.Id > 0)
                {
                    var data = await _ICrmrpo.updateOfferletterdetail(model);
                    if (data > 0)
                    {
                        TempData["Message"] = "Data Update Successfully.";
                        return RedirectToAction("AddOfferletterdetail", "Employee");
                    }
                    else
                    {
                        TempData["Message"] = "Update Failed.";
                        return View(model);
                    }
                }
                else
                {
                    var response = await _ICrmrpo.AddOfferletterdetail(model, Userid);
                    if (response > 0)
                    {
                        TempData["Message"] = "Data Add Successfully.";
                        return RedirectToAction("AddOfferletterdetail", "Employee");
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
        public async Task<IActionResult> OfferletterList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var response = await _ICrmrpo.OfferletterdetailList(Userid);

                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }
        public async Task<IActionResult> DeleteOfferletter(int id)
        {
            try
            {
                var data = _context.Offerletters.Find(id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    _context.Offerletters.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("OfferletterList");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        [HttpGet]
        public IActionResult DeletHrSignature(string FilePath, int id)
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
                    var img = _context.Offerletters.FirstOrDefault(s => s.HrSignature == FilePath && s.Id == id);
                    if (img != null)
                    {
                        img.HrSignature = null;
                        _context.SaveChangesAsync();
                    }

                }
            }
            return Json(success);
        }

        [HttpGet, Route("/Employee/Appointmentletterlist")]
        public async Task<IActionResult> Appointmentletterlist()
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    string AddedBy = HttpContext.Session.GetString("UserName");
                    ViewBag.UserName = AddedBy;
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                    var response = await _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new Appointmentdetail
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        DateOfJoining = x.DateOfJoining.GetValueOrDefault(),
                        MiddleName = x.MiddleName,
                        WorkEmail = x.WorkEmail,
                        Emp_Reg_ID = x.EmployeeId,
                        Appoinmentletter = x.Appoinmentletter
                    }).ToListAsync();

                    return View(response);
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        [HttpPost]
        public JsonResult UpdateShiftType(int OfficeshiftTypeid, string EmployeeId)
        {
            var emp = _context.EmployeeRegistrations.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
            emp.OfficeshiftTypeid = OfficeshiftTypeid;
            _context.SaveChanges();
            return Json(new { success = true, message = "Shift type updated successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> EmpLeavemaster(int id = 0)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    ViewBag.UserName = HttpContext.Session.GetString("UserName");
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                    ViewBag.EmployeeId = _context.EmployeeRegistrations.Where(x =>x.Vendorid == adminlogin.Vendorid).Select(D => new SelectListItem
                    {
                        Value = D.EmployeeId.ToString(),
                        Text = D.EmployeeId

                    }).ToList();
                    ViewBag.leavetype = _context.LeaveTypes.Select(D => new SelectListItem
                    {
                        Value = D.Id.ToString(),
                        Text = D.Leavetype1

                    }).ToList();
                    ViewBag.id = 0;
                    ViewBag.LeavetypeId = "";
                    ViewBag.Value = "";
                    ViewBag.EmpId = "";
                    ViewBag.Status = "";
                    ViewBag.heading = "Add Leavemaster :";
                    ViewBag.btnText = "SAVE";
                    if (id != null && id != 0)
                    {
                        var data = _context.Leavemasters.Find(id);
                        if (data != null)
                        {

                            ViewBag.id = data.Id;
                            ViewBag.LeavetypeId = data.LeavetypeId;
                            ViewBag.Value = data.Value;
                            ViewBag.EmpId = data.EmpId;
                            ViewBag.Status = data.IsActive;
                            ViewBag.btnText = "UPDATE";
                            ViewBag.heading = "Update Leavemaster :";

                        }
                    }
                    LeavemasterDto response = new LeavemasterDto();
                    response.lmd = await _ICrmrpo.getLeavemaster(Userid);
                    return View(response);
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
        [Route("/Employee/EmpLeavemaster")]
        public async Task<IActionResult> EmpLeavemaster(LeavemasterDto model)
        {

            if (HttpContext.Session.GetString("UserName") != null)
            {
                var options = _context.EmployeeRegistrations
                 .Select(D => new SelectListItem
                 {
                     Value = D.EmployeeId.ToString(),
                     Text = D.EmployeeId.ToString()
                 }).ToList();

                ViewBag.EmployeeId = options;
                ViewBag.leavetype = _context.LeaveTypes.Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.Leavetype1

                }).ToList();
                int AddedByid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserLogin = AddedBy;
                if (model.id > 0)
                {


                    var existingEntity = _context.Leavemasters.Find(model.id);
                    if (existingEntity != null)
                    {

                        existingEntity.LeavetypeId = Convert.ToInt16(model.LeavetypeId);
                        existingEntity.Value = model.Value;
                        existingEntity.EmpId = model.EmpId;
                        existingEntity.IsActive = model.IsActive;
                        existingEntity.LeaveUpdateDate = DateTime.Now;
                        _context.SaveChanges();
                        TempData["Message"] = "Records has Update successfully.";
                    }

                }
                else
                {

                    var newRecord = new Leavemaster
                    {
                        LeavetypeId = Convert.ToInt16(model.LeavetypeId),
                        EmpId = model.EmpId,
                        Value = model.Value,
                        IsActive = true,
                        Createddate = DateTime.Now.Date,
                    };


                    _context.Leavemasters.Add(newRecord);
                    _context.SaveChanges();
                    TempData["Message"] = "Records has added successfully.";
                }

                return RedirectToAction("EmpLeavemaster");
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public async Task<IActionResult> DeleteEmpLeavemaster(int id)
        {
            var LeavemasterDelete = _context.Leavemasters.Find(id);
            if (LeavemasterDelete != null)
            {
                _context.Leavemasters.Remove(LeavemasterDelete);
                _context.SaveChanges();
                return RedirectToAction("EmpLeavemaster");
            }
            else
            {
                return NotFound();
            }
        }

    }
}





