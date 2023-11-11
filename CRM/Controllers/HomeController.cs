using CRM.Models;
using CRM.Models.Crm;
using CRM.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CRM.Controllers
{
    public class HomeController : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;

        public HomeController(ICrmrpo _ICrmrpo, admin_NDCrMContext _context)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Customer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Customer(CustomerRegistration model)
        {
            try
            {
                var response = await _ICrmrpo.Customer(model);
                if (response != null)
                {

                    return RedirectToAction("Customer", "Home");
                    TempData["msg"] = "Regiter Successfully.";
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

        public async Task<IActionResult> CustomerList()
        {
            var response = await _ICrmrpo.CustomerList();
            return View(response);
        }
        public async Task<IActionResult> ProductList()
        {
            var response = await _ICrmrpo.ProductList();
            return View(response);
        }
    }
}