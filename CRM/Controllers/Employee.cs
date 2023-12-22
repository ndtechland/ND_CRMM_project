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
//using IronPdf;
//using IronPdf.Engines.Chrome;
//using IronPdf.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
//using IronPdf.Editing;
using System.IO;
using Org.BouncyCastle.Asn1.Mozilla;
using SelectPdf;


//using Microsoft.TeamFoundation.WorkItemTracking.Internals;

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

        [HttpGet]
        public IActionResult EmployeeBasicinfo()
        {

            if (HttpContext.Session.GetString("UserName") != null)
            {
                var emp = new EmployeePersonalDetail();

                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                ViewBag.StateId = _context.StateMasters
              .Select(s => new SelectListItem
              {
                  Value = s.Id.ToString(),
                  Text = s.StateName
              })
               .ToList();
                DateTime dob = DateTime.Now;
                int age = CalculatAge(dob);
                ViewBag.EmployeeAge = age;
                return View(emp);


            }
            else
            {
                return RedirectToAction("Login", "Admin");
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
        [HttpPost]
        public async Task<IActionResult> EmployeeBasicinfo(EmployeePersonalDetail model)
        {
            try
            {
                var response = await _ICrmrpo.EmployeeBasicinfo(model);
                if (response != null)
                {

                    return RedirectToAction("EmployeeBasicinfo", "Employee");
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


        public async Task<IActionResult> EmployeeBasicinfoList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = await _ICrmrpo.EmployeeBasicinfoList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

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
        public async Task<IActionResult> salarydetail()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = await _ICrmrpo.salarydetail();

                decimal total = 0;
                foreach (var item in response)
                {
                    total += (decimal)item.MonthlyCtc;
                }
                ViewBag.TotalAmmount = total;
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }


        [HttpPost]
        public JsonResult Empattendance(List<Empattendance> customers)
        {
            bool IsActive = false;
            var month = _context.Empattendances.Where(x => x.Month == DateTime.Now.Month).ToList();
            if (month.Count > 0)
            {
                //ViewBag.Message = "Your salary already genrated for this month";
                IsActive = true;

            }
            if (IsActive == false)
            {
                foreach (var item in customers)
                {
                    if (item.Id != 0)
                    {

                        Empattendance emp = new Empattendance
                        {
                            EmployeeId = item.EmployeeId,
                            Month = DateTime.Now.Month,
                            Year = DateTime.Now.Year,
                            Attendance = item.Attendance,
                            Entry = DateTime.Now
                        };

                        _context.Empattendances.Add(emp);
                        _context.SaveChanges();

                    }
                }
            }

            return Json(new { success = true, message = "Data saved successfully.", Data = IsActive });
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
                ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                GenerateSalary salary = new GenerateSalary();

                salary.GeneratedSalaries = await _ICrmrpo.GenerateSalary(customerId, Month, year);


                return View(salary);
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
                                  Employee_ID = emp.EmployeeId,
                                  First_Name = emp.FirstName,
                                  Address_Line_1 = worklocation.AddressLine1,
                                  Epf = empsalary.Epf,
                                  Designation_Name = designation.DesignationName,
                                  Bank_Name = empbank.BankName,
                                  Account_Number = empbank.AccountNumber,
                                  Basic = empsalary.Basic,
                                  //EPF_Number =  empbank.EpfNumber

                              }).FirstOrDefault();

                return View(result);
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
            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();
           
            WebClient client = new WebClient();
                   // Create a PDF from a HTML string using C#
            string SlipURL = _configuration.GetValue<string>("URL") + "/Employee/SalarySlipInPDF?id="+id+"";
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
            return fileResult;
            //return File(, "application/pdf", "SalarySlip.pdf");
        }


        public IActionResult Invoice(int id)
        {
            try
            {
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}

             
                
















