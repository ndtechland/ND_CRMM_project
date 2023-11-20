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

        public Employee(ICrmrpo _ICrmrpo, admin_NDCrMContext _context)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
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

                    return RedirectToAction("EmployeeRegistration", "Employee");
                    TempData["msg"] = "registration Successfully.";
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
    }
}
