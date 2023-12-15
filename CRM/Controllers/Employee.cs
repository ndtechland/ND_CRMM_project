using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CRM.Controllers
{
    public class Employee : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        

        public Employee(ICrmrpo _ICrmrpo, admin_NDCrMContext _context)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
          
        }
        public IActionResult EmployeeRegistration()
        {
            
            if (HttpContext.Session.GetString("UserName") != null)
            {
                //var emp = new EmployeeRegistration();
                //string AddedBy = HttpContext.Session.GetString("UserName");
                //ViewBag.UserName = AddedBy;
                ////WorkLocation dropdown 
                //ViewBag.WorkLocation = _context.WorkLocations
                //.Select(w => new SelectListItem
                //{
                //    Value = w.Id.ToString(),
                //    Text = w.AddressLine1
                //})
                // .ToList();
                ////Gender dropdown 
                //ViewBag.Gender = _context.GenderMasters
                //.Select(w => new SelectListItem {
                // Value = w.Id.ToString(),
                // Text = w.GenderName })
                // .ToList();
                ////Department dropdown 
                //ViewBag.Department = _context.DepartmentMasters.Select(w => new SelectListItem
                //{
                //    Value = w.Id.ToString(),
                //    Text = w.DepartmentName

                //}).ToList();
                ////Designation dropdown 
                //ViewBag.Designation = _context.DesignationMasters.Select(w => new SelectListItem
                //{
                //    Value = w.Id.ToString(),
                //    Text = w.DesignationName

                //}).ToList();
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
        public  IActionResult EmployeeBasicinfo()
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
                if(data != null)
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
                if(data!=null)
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
        public JsonResult Edit (int id)
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
            var department=_context.DepartmentMasters.ToList();
            var MonthlyCTC = _context.EmployeeSalaryDetails.Where(x =>x.EmployeeId == data.EmployeeId).FirstOrDefault();
            var result = new
            {
                Data = data,
                Gender = gender,
                Worklocation = worklocation,
                Designation=designation,
                Department=department,
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
                EmployeeSalaryDetail empd = new EmployeeSalaryDetail();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                if (data != null)
                {
                    empd.EmployeeId = data.EmployeeId;
                    empd.EmployeeName = data.FirstName;

                }
                return View(empd);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Gengeneratesalary(Updatesalary model)
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

        //[HttpGet("Employee/EmployeeBankDetail")]
        //public IActionResult EmployeeBankDetail(int? id)
        //{
        //    if (HttpContext.Session.GetString("UserName") != null)
        //    {                
        //        string AddedBy = HttpContext.Session.GetString("UserName");
        //        ViewBag.UserName = AddedBy;
        //        var data = _context.EmployeeRegistrations.Find(id);
        //        EmployeeBankDetail empd = new EmployeeBankDetail();
        //        ViewBag.AccountType = _context.AccountTypeMasters
        //           .Select(w => new SelectListItem
        //           {
        //               Value = w.Id.ToString(),
        //               Text = w.AccountType
        //           })
        //            .ToList();
        //        if (data != null)
        //        {
        //            empd.EmployeeRegistrationId = data.Id;
        //        }               
        //        return View(empd);
        //}
        //    else
        //    {
        //        return RedirectToAction("Login", "Admin");
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> EmployeeBankDetail(EmployeeBankDetail model)
        //{
        //    try
        //    {
        //        var response = await _ICrmrpo.EmployeeBankDetail(model);
        //        if (response != null)
        //        {

        //            return RedirectToAction("Employeelist", "Employee");
        //            ViewBag.Message = "registration Successfully.";
        //        }
        //        else
        //        {
        //            ModelState.Clear();
        //            return View();
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw new Exception("Error:" + Ex.Message);
        //    }
        //}
        public async Task<IActionResult> salarydetail()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var response = await _ICrmrpo.salarydetail();
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

            if (customers == null)
            {
                return Json(new { success = false, message = "No data received." });
            }

            foreach (var item in customers)
            {

                if (customers.Count > 0)
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


            return Json(new { success = true, message = "Data saved successfully." });
        }
   
    }
}
