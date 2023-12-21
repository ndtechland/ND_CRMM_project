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
using DinkToPdf;
using IronPdf;
using IronPdf.Engines.Chrome;
using IronPdf.Rendering;


//using Microsoft.TeamFoundation.WorkItemTracking.Internals;

namespace CRM.Controllers
{
    public class Employee : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        private readonly PdfService _pdfService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;



        public Employee(ICrmrpo _ICrmrpo, admin_NDCrMContext _context, PdfService pdfService, IConfiguration configuration)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
            this._pdfService = pdfService;
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

        [HttpGet("Employee/Gengeneratesalary")]
        public IActionResult Gengeneratesalary(int? id, string name)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var data = _context.EmployeeRegistrations.Find(id);
                //EmpId

                EmployeeSalaryDetail empd = new EmployeeSalaryDetail();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                if (data != null)
                {
                    empd.EmployeeId = data.EmployeeId;
                    empd.EmployeeName = data.FirstName;
                    empd.EmpId = data.Id;

                }
                return View(empd);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Gengeneratesalary(EmployeeSalaryDetail model)
        {
            try
            {
                var response = await _ICrmrpo.Gengeneratesalary(model);
                if (response != null)
                {

                    return RedirectToAction("Employeelist", "Employee");
                    ViewBag.Message = "registration Successfully.";
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


        //public IActionResult DownloadPdf(int id, int oid)
        //{
        //    //GeneratePdf(id, oid);
        //    string filePath = "../savedPDF/index.html";

        //    if (System.IO.File.Exists(filePath))
        //    {
        //        // Read the file bytes
        //        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

        //        // Set the Content-Disposition header to suggest the filename to browsers
        //        var contentDisposition = new System.Net.Mime.ContentDisposition
        //        {
        //            FileName = "Input.pdf",
        //            Inline = false // Force the browser to prompt for download
        //        };
        //        Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

        //        // Set the content type to PDF
        //        Response.ContentType = "application/pdf";

        //        // Return the file as a FileResult
        //        return File(fileBytes, "application/pdf");
        //    }
        //    else
        //    {
        //        // If the file doesn't exist, return a 404 Not Found response
        //        return null;
        //    }
        //}

        //public void GeneratePdf(int id, int oid)
        //{
        //    using (WebClient client = new WebClient())
        //    {
        //        // Define the URL to the HTML content
        //        string url = $"https://admin.organicdeal.in/Home/Invoice?oid={oid}";

        //        // Download the HTML content from the URL
        //        string htmlContent = client.DownloadString(url);

        //        // Specify the file path where you want to save the HTML content
        //        string htmlFilePath = "../savedHTML/index.html";

        //        // Save the HTML content to the specified file path
        //        System.IO.File.WriteAllText(htmlFilePath, htmlContent);
        //    }

        //    // instantiate the html to pdf converter
        //    HtmlToPdf converter = new HtmlToPdf();

        //    // get html file path
        //    var htmlFilePath = "../savedHTML/index.html";

        //    // convert the html to pdf
        //    PdfDocument doc = converter.ConvertHtmlString(System.IO.File.ReadAllText(htmlFilePath));

        //    // specify the file path for saving the PDF
        //    var pdfFilePath = "../savedPDF/index.pdf";

        //    // save the pdf document
        //    doc.Save(pdfFilePath);

        //    // close the pdf document
        //    doc.Close();
        //}


        public IActionResult SalarySlipInPDF()
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

        public IActionResult DownloadSalarySlip()
        {
            // Generate HTML content for Salary Slip dynamically
            var salarySlipHtml = GenerateSalarySlipHtml("hello");

            // Generate and return the PDF file
            var pdfBytes = _pdfService.GeneratePdf(salarySlipHtml);

            return File(pdfBytes, "application/pdf", "SalarySlip.pdf");
        }

        private string GenerateSalarySlipHtml(string username)
        {
            // Dynamically generate HTML content for Salary Slip based on your data
            // This is just a placeholder example; you should customize it based on your actual requirements
            var htmlContent = @"
            <html>
            <head>
                <style>
                    /* Add your CSS styles here */
                    body {
                        font-family: Arial, sans-serif;
                    }
                    .salary-slip {
                        padding: 20px;
                        border: 1px solid #ccc;
                    }
                </style>
            </head>
            <body>
                <div class='salary-slip'>
                    <h1>Salary Slip</h1>
                    <p>Employee: John Doe</p>
                    <p>Salary: ${username}</p>
                    <!-- Add more details as needed -->
                </div>
            </body>
            </html>";

            return htmlContent;
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


        public IActionResult DocPDF()
        {
            try
            {

                var rendere = new ChromePdfRenderer();
                WebClient client = new WebClient();
                // Create a PDF from a HTML string using C#
                string SlipURL = _configuration.GetValue<string>("URL") + "/Employee/SalarySlipInPDF";

                var pdf = rendere.RenderHtmlAsPdf(client.DownloadString(SlipURL));

                // Export to a file or Stream
                pdf.SaveAs("output.pdf");

                return File(pdf.Stream, "application/pdf", "SalarySlip.pdf");
            }
            catch (Exception)
            {

                throw;
            }

        }
    }

}       


  

