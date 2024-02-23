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

        public IActionResult EmployeeRegistration(string id)
        {

            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
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
                ViewBag.statesy = "";
                ViewBag.btnText = "SAVE";

                if (id != null)
                {
                    DataTable dtEmployeeRecord = _ICrmrpo.GetEmployDetailById(id);

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
                            ViewBag.CustomerID = row["CustomerID"].ToString();
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
                            ViewBag.statesy = row["stateId"].ToString();
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
                        ViewBag.Message = "WorkEmail already exists";
                        return View();
                    }
                }
                
                var response = await _ICrmrpo.EmpRegistration(model, Mode, Empid);
                ModelState.Clear();
                ViewBag.Message = "Registration successful"; 
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
                return View();
            }
        }
        [Route("Employee/Employeelistg")]
        [Route("Employee/Employeelists")]
        [Route("Employee/Employeelist")]
        public async Task<IActionResult> Employeelist()
        {
            try
            {
                List<EmployeeImportExcel> response = new List<EmployeeImportExcel>();
                if (HttpContext.Session.GetString("UserName") != null)
                {

                    response = await _ICrmrpo.EmployeeList();
                    ViewBag.UserName = HttpContext.Session.GetString("UserName");
                    return View(response);
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception Ex)
            {

                throw new Exception("Error:" + Ex.Message);
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
        public async Task<IActionResult> salarydetail(string customerId, string WorkLocation)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                TempData["UserName"] = AddedBy;
                TempData["custid"] = customerId;
                TempData["locid"] = WorkLocation;
                //
                if (AddedBy != null)
                { HttpContext.Session.SetString("UserName", AddedBy); }
                else
                {
                    AddedBy = HttpContext.Session.GetString("UserName");

                }
                if (customerId != null)
                {
                    HttpContext.Session.SetString("custid", customerId);
                }
                else
                {
                    customerId = HttpContext.Session.GetString("custid");
                }

                if (WorkLocation != null)
                {
                    HttpContext.Session.SetString("locid", WorkLocation);
                }
                else
                {
                    WorkLocation = HttpContext.Session.GetString("locid");
                }
                //

                var response = await _ICrmrpo.salarydetail(customerId, WorkLocation);
                decimal total = 0.00M;
                foreach (var item in response)
                {
                    total += (decimal)item.MonthlyCtc;
                }
                ViewBag.TotalAmmount = total;
                ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                ViewBag.Message = TempData["ErrorMessage"];
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
                                        GenerateSalary = decimal.Round((decimal)(ctc.MonthlyCtc / 25 * item.Attendance), 2) + item.Incentive,
                                        Lop = (decimal)(ctc.MonthlyCtc) - decimal.Round((decimal)(ctc.MonthlyCtc / 25 * item.Attendance), 2),
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

        public IActionResult GenerateSalary()
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
        public IActionResult GetLocationsByCustomer(string customerId)
        {
            var locationsJsonblank = "";
            if (!string.IsNullOrEmpty(customerId))
            {
                var locations = _context.CustomerRegistrations.FirstOrDefault(x => x.Id == Convert.ToInt32(customerId));
                string[] strlocation = locations.WorkLocation?.Split(new string[] { "," }, StringSplitOptions.None);
                List<WorkLocation> locationlist = new List<WorkLocation>();

                foreach (var loc in strlocation)
                {
                    locationlist.Add(_context.WorkLocations.FirstOrDefault(x => x.Id == Convert.ToInt32(loc)));
                }


                var locationsJson = locationlist.Select(x => new SelectListItem
                {
                    Text = x.Id.ToString(),
                    Value = x.AddressLine1
                }).ToList();

                return Json(locationsJson);
            }

            return Json(locationsJsonblank);

        }
        [HttpPost]

        public async Task<IActionResult> GenerateSalary(string customerId, int Month, int year, string WorkLocation)

        {
            try
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
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
                    GenerateSalary salary = new GenerateSalary();



                    salary.GeneratedSalaries = await _ICrmrpo.GenerateSalary(customerId, Month, year, WorkLocation);
                    if (salary.GeneratedSalaries.Count > 0)

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
                    return RedirectToAction("GenerateSalary");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }


        }

        [Route("Employee/SalarySlipInPDF")]
        public IActionResult SalarySlipInPDF(int? id)
        {
            try
            {
                //if (HttpContext.Session.GetString("UserName") != null)
                // {
                string AddedBy = HttpContext.Session.Id;
                ViewBag.UserName = AddedBy;
                var result = (from emp in _context.EmployeeRegistrations
                              join empsalary in _context.EmployeeSalaryDetails on emp.Id equals empsalary.EmpId
                              join empbank in _context.EmployeeBankDetails on emp.Id equals empbank.EmployeeRegistrationId
                              join empatt in _context.Empattendances on emp.EmployeeId equals empatt.EmployeeId
                              join worklocation in _context.WorkLocations on emp.WorkLocationId equals worklocation.Id.ToString()
                              join designation in _context.DesignationMasters on emp.DesignationId equals designation.Id.ToString()
                              join tds in _context.EmployeerTds on emp.CustomerId equals tds.CustomerId
                              where emp.Id == id
                              select new SalarySlipDetails
                              {
                                  Id = emp.Id,
                                  Employee_ID = emp.EmployeeId,
                                  First_Name = emp.FirstName,
                                  Address_Line_1 = worklocation.AddressLine1,
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
                                  Professionaltax=empsalary.Professionaltax,
                                  TravellingAllowance=empatt.TravellingAllowance,
                                  SpecialAllowance=empsalary.SpecialAllowance,
                                  Esic=empsalary.Esic,
                                  Amount=tds.Amount,
                              }).FirstOrDefault();

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
            if (Deduction_Cycle != null)
            {
                var response = await _ICrmrpo.EmployerList(Deduction_Cycle);
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                if (response.Count > 0)
                {
                    return View(response);

                }
                else
                {
                    TempData["ErrorMessage"] = "No details found.";
                    return RedirectToAction("Employee_list");

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
            string schema = Request.Scheme;
            string host = Request.Host.Value;
            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            WebClient client = new WebClient();
            // Create a PDF from a HTML string using C#
            string SlipURL = $"{schema}://{host}/Employee/SalarySlipInPDF?id={id}";
            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertUrl(SlipURL);

            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "SalarySlip.pdf";

            var result = (from emp in _context.EmployeeRegistrations

                          where emp.Id == id
                          select new SalarySlipDetails
                          {
                              Id = emp.Id,
                              Employee_ID = emp.EmployeeId,
                              First_Name = emp.FirstName,
                              Email_Id = emp.WorkEmail

                          }).FirstOrDefault();
            string Email_Subject = "Salary Slip for " + result.Employee_ID + "";
            string Email_body = "Hello " + result.First_Name + " (" + result.Employee_ID + ") please find your attached salary slip....";

            if (result != null)
            {
                _IEmailService.SendEmailAsync(result.Email_Id, Email_Subject, Email_body, pdf, "SalarySlip.pdf", "application/pdf");
                return fileResult;

            }
            else
            {
                return View();
            }

        }

        public void SendPDF(int id)
        {
            string schema = Request.Scheme;
            string host = Request.Host.Value;

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            WebClient client = new WebClient();
            // Create a PDF from a HTML string using C#
            string SlipURL = $"{schema}://{host}/Employee/SalarySlipInPDF?id={id}";
            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertUrl(SlipURL);

            // save pdf document
            //doc.Save("Sample.pdf");

            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "SalarySlip.pdf";

            var result = (from emp in _context.EmployeeRegistrations

                          where emp.Id == id
                          select new SalarySlipDetails
                          {
                              Id = emp.Id,
                              Employee_ID = emp.EmployeeId,
                              First_Name = emp.FirstName,
                              Email_Id = emp.WorkEmail

                          }).FirstOrDefault();
            string Email_Subject = "Salary Slip for " + result.Employee_ID + "";
            string Email_body = "Hello " + result.First_Name + " (" + result.Employee_ID + ") please find your attached salary slip....";

            _IEmailService.SendEmailAsync(result.Email_Id, Email_Subject, Email_body, pdf, "SalarySlip.pdf", "application/pdf");

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
            var data = _context.EmployeeRegistrations.OrderByDescending(x => x.Id).FirstOrDefault();
            string EmpID = string.Empty;

            if (data != null && !string.IsNullOrEmpty(data.EmployeeId))
            {
                string[] parts = data.EmployeeId.Split('-');

                if (parts.Length > 1 && int.TryParse(parts[2], out int numericValue))
                {
                    numericValue++;
                    EmpID = $"ND-{DateTime.Now.Month}/{DateTime.Now.Year}-{numericValue}";

                }
            }
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
                if(data !=null)
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

        public IActionResult ImportToExcelAttendance(string customerId, string WorkLocation)
        {
            try
            {
                var data = _ICrmrpo.salarydetail(customerId, WorkLocation).Result;
                var response = _ICrmrpo.ImportToExcelAttendance(data);
                return File(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee_Attandence_List.xlsx");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }

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
        [HttpGet]
        [Route("/Employee/MonthlysalaryReport")]
        public async Task<IActionResult> MonthlysalaryReport(string customerId, int Month, int year, string WorkLocation)
        {
            try
            {
                var response = await  _ICrmrpo.monthlysalaryReport(customerId, Month, year, WorkLocation);
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

    }
}





