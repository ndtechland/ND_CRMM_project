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
using IronPdf;
using IronPdf.Engines.Chrome;
using IronPdf.Rendering;
using System.Data;
using MimeKit.Encodings;
using Fingers10.ExcelExport.ActionResults;

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

        public IActionResult EmployeeRegistration(string id)
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
                //States dropdown
                ViewBag.States = _context.StateMasters.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.StateName
                }).ToList();
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
                ViewBag.ConveyanceAllowance = "";
                ViewBag.FixedAllowance = "";
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
                            ViewBag.Date_Of_Joining = row["Date_Of_Joining"].ToString();
                            ViewBag.Work_Email = row["Work_Email"].ToString();
                            ViewBag.Gender_ID = row["Gender_ID"].ToString();
                            ViewBag.Work_Location_ID = row["Work_Location_ID"].ToString();
                            ViewBag.Designation_ID = row["Designation_ID"].ToString();
                            ViewBag.Department_ID = row["Department_ID"].ToString();
                            ViewBag.CustomerID = row["CustomerID"].ToString();
                            ViewBag.Basic = row["Basic"].ToString();
                            ViewBag.AnnualCTC = row["AnnualCTC"].ToString();
                            ViewBag.HouseRentAllowance = row["HouseRentAllowance"].ToString();
                            ViewBag.ConveyanceAllowance = row["ConveyanceAllowance"].ToString();
                            ViewBag.FixedAllowance = row["FixedAllowance"].ToString();
                            ViewBag.EPF = row["EPF"].ToString();
                            ViewBag.MonthlyGrossPay = row["MonthlyGrossPay"].ToString();
                            ViewBag.MonthlyCTC = row["MonthlyCTC"].ToString();
                            ViewBag.Personal_Email_Address = row["Personal_Email_Address"].ToString();
                            ViewBag.Mobile_Number = row["Mobile_Number"].ToString();
                            ViewBag.Date_Of_Birth = row["Date_Of_Birth"].ToString();
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
                if (model.Emp_Reg_ID != "" && model.Emp_Reg_ID != null)
                {
                    Mode = "UPD";
                    Empid = model.Emp_Reg_ID;
                }

                var response = await _ICrmrpo.EmpRegistration(model, Mode, Empid);

                ModelState.Clear();
                return View();

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
            List<EmployeeRegistration> response = new List<EmployeeRegistration>();
            if (HttpContext.Session.GetString("UserName") != null)
            {
                response = await _ICrmrpo.EmployeeList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                //ViewBag.UserName = AddedBy;
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


        public IActionResult SalarySlipInPDF()
        {
            try
            {
                var result = (from emp in _context.EmployeeRegistrations
                              join empsalary in _context.EmployeeSalaryDetails on emp.Id equals empsalary.EmpId
                              join empbank in _context.EmployeeBankDetails on emp.Id equals empbank.EmployeeRegistrationId
                              join worklocation in _context.WorkLocations on emp.WorkLocationId equals worklocation.Id.ToString()
                              join designation in _context.DesignationMasters on emp.DesignationId equals designation.Id.ToString()
                              select new SalarySlipDetails
                              {
                                  Id = emp.Id,
                                  EmpCode = emp.EmployeeId,
                                  PFNo = "",
                                  ESINo = "",
                                  Designation = designation.DesignationName,
                                  EMPName = "",
                                  NOD = "",
                                  ModeofPay = "",
                                  AcNo = "",
                                  WorkingBranch = "",
                                  BasicAmount = "",
                                  DA = "",
                                  HRA = "",
                                  WA = "",
                                  CA = "",
                                  CCA = "",
                                  MA = "",
                                  SalesIncentive = "",
                                  LeaveEncashment = "",
                                  HolidayWages = "",
                                  SpecialAllowance = "",
                                  Bonus = "",
                                  IndividualIncentive = "",
                                  TotalEarning = "",
                                  NetPay = "",
                                  InWords = "",
                                  PF = "",
                                  ESI = "",
                                  TDS = "",
                                  LOP = "",
                                  PT = "",
                                  SPLDeduction = "",
                                  EWF = "",
                                  CD = "",
                                  TotalDeductions = "",

                              });

                return View();
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
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
                // ViewBag.UserName = AddedBy;
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
        //-----ImportToExcelEmployeeList
        public IActionResult ImportToExcelEmployeeList()
        {
            try
            {
                var response =  _ICrmrpo.EmployeeListForExcel();
                return File(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee_List.xlsx");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
           
        }
    }
}





       



    


  

