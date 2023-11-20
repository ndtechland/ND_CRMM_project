using CRM.Models.Crm;
using CRM.Repository;
using Microsoft.AspNetCore.Mvc;

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
            return View();
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
