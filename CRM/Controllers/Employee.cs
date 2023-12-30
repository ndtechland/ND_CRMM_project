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
using Microsoft.TeamFoundation.Test.WebApi;
using NuGet.Protocol;
using Microsoft.Extensions.Primitives;

namespace CRM.Controllers
{
    public class Employee : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;



        public Employee(ICrmrpo _ICrmrpo, admin_NDCrMContext _context, IConfiguration configuration)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
            _configuration = configuration;

        }



        public IActionResult EmployeeRegistration()
        {

            if (HttpContext.Session.GetString("UserName") != null)
            {
                //var emp = new EmpMultiform();
                //string AddedBy = HttpContext.Session.GetString("UserName");
                //ViewBag.UserName = AddedBy;
                ////WorkLocation dropdown 
                ViewBag.WorkLocation = _context.WorkLocations
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.AddressLine1
                })
                 .ToList();
                ////Gender dropdown 
                //ViewBag.Gender = _context.GenderMasters
                //.Select(w => new SelectListItem
                //{
                //    Value = w.Id.ToString(),
                //    Text = w.GenderName
                //})
                // .ToList();
                ////Department dropdown 
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

                ViewBag.States = _context.StateMasters.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.StateName
                }).ToList();
                ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
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
                var response = await _ICrmrpo.EmpRegistration(model);
                //if (response != null)
                //{

                //    return RedirectToAction("Employeelist", "Employee");
                //    ViewBag.Message = "registration Successfully.";
                //}
                //else
                //{
                ModelState.Clear();
                return View();
                //}
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }

        public async Task<IActionResult> Employeelist()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = await _ICrmrpo.EmployeeList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
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

        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var data = _context.EmployeeRegistrations.Find(id);
                if (data != null)
                {
                    data.IsDeleted = true;
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
        public async Task<IActionResult> salarydetail(string customerId, string WorkLocation)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;

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
                var month = await _context.Empattendances
                    .Where(x => x.Month == DateTime.Now.Month)
                    .ToListAsync();

                if (month.Count > 0)
                {
                    isActive = true;
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
                                if (item.Id != 0)
                                {
                                    Empattendance emp = new Empattendance
                                    {
                                        EmployeeId = item.EmployeeId,
                                        Month = DateTime.Now.Month,
                                        Year = DateTime.Now.Year,
                                        Attendance = item.Attendance,
                                        Entry = DateTime.Now,
                                        GenerateSalary = decimal.Round((decimal)(ctc.MonthlyCtc / 30 * item.Attendance), 2),
                                        Lop = (decimal)(ctc.MonthlyCtc) - decimal.Round((decimal)(ctc.MonthlyCtc / 30 * item.Attendance), 2),
                                    };

                                    _context.Empattendances.Add(emp);
                                    await _context.SaveChangesAsync();
                                }
                            }

                            transaction.Commit();
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
        [HttpPost]
        public async Task<IActionResult> GenerateSalary(string customerId, int Month, int year)
        {
            try
            {
                if(customerId != null && Month != null && year !=null)
                {
                    ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                    GenerateSalary salary = new GenerateSalary();

                    salary.GeneratedSalaries = await _ICrmrpo.GenerateSalary(customerId, Month, year);
                    return View(salary);
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

        public IActionResult sendmail()
        {
            return View();
        }

        [Route("Employee/SalarySlipInPDF")]
        public IActionResult SalarySlipInPDF(int? id)
        {
            try
            {               
                    var result = (from emp in _context.EmployeeRegistrations
                                  join empsalary in _context.EmployeeSalaryDetails on emp.Id equals empsalary.EmpId
                                  join empbank in _context.EmployeeBankDetails on emp.Id equals empbank.EmployeeRegistrationId
                                  join empatt in _context.Empattendances on emp.EmployeeId equals empatt.EmployeeId
                                  join worklocation in _context.WorkLocations on emp.WorkLocationId equals worklocation.Id.ToString()
                                  join designation in _context.DesignationMasters on emp.DesignationId equals designation.Id.ToString()
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
                                  }).FirstOrDefault();
                if(result!= null)
                {
                    return View(result);
                }
                else
                {
                    return RedirectToAction("GenerateSalary");
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
                return View();
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Employer(Employeer_EPF model)
        {
            try
            {
                var response = await _ICrmrpo.Employer(model);

                ModelState.Clear();
                return View();

            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }

        public async Task<IActionResult> Employee_list()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = await _ICrmrpo.EmployerList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }


        }

        public IActionResult DocPDF(int id)
        {
            HtmlToPdf converter = new HtmlToPdf();
           
            WebClient client = new WebClient();
            string SlipURL = _configuration.GetValue<string>("URL") + "/Employee/SalarySlipInPDF?id="+id+"";
            PdfDocument doc = converter.ConvertUrl(SlipURL);
            if(doc != null)
            {
                byte[] pdf = doc.Save();

                doc.Close();

                FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                fileResult.FileDownloadName = "SalarySlip.pdf";
                return fileResult;
            }
            return RedirectToAction("GenerateSalary");

        }
        public static string getMonthName(int monthValue)
        {
            string monthName = "";

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
        public async Task<IActionResult> Invoice(string customerId, int Month, int year ,string WorkLocation)
        {
            try
            {

                if (customerId != null && Month != null && year != null && WorkLocation!=null)
                {
                    ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                    List<Invoice> invoice = new List<Invoice>();

                    invoice = await _ICrmrpo.GenerateInvoice(customerId, Month, year,WorkLocation);
                    if(invoice.Count > 0)
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

    

    }
}

             
                
















