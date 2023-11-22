using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CRM.Controllers
{
    public class Employee : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        private readonly admin_NDCrM _dbcontext;

        public Employee(ICrmrpo _ICrmrpo, admin_NDCrMContext _context, admin_NDCrM dbcontext)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
            _dbcontext = dbcontext;
        }
        public IActionResult EmployeeRegistration()
        {
            
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var emp = new EmployeeRegistration();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = AddedBy;
                //WorkLocation dropdown 
                ViewBag.WorkLocation = _context.WorkLocations
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.AddressLine1
                })
                 .ToList();
                //Gender dropdown 
                ViewBag.Gender = _context.GenderMasters
                .Select(w => new SelectListItem {
                 Value = w.Id.ToString(),
                 Text = w.GenderName })
                 .ToList();
                //Department dropdown 
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
                return View(emp);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> EmployeeRegistration(EmployeeRegistration model)
        {
            try
            {
                var response = await _ICrmrpo.EmpRegistration(model);
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
    }
}
