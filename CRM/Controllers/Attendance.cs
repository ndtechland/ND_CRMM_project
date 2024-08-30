using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    public class Attendance : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult clock()
        {
            return View();
        }
    }
}
