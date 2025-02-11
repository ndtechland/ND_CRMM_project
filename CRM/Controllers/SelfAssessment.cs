using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using DinkToPdf.Contracts;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    public class SelfAssessment : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        private readonly IEmailService _IEmailService;
        private readonly IConverter _converter;

        public SelfAssessment(admin_NDCrMContext context, ICrmrpo ICrmrpo, IEmailService iEmailService, IConverter _IConverter)
        {
            _context = context;
            _ICrmrpo = ICrmrpo;
            _IEmailService = iEmailService;
            _converter = _IConverter;

        }
        [HttpGet, Route("/SelfAssessment/Assessment")]
        public async Task<IActionResult> Assessment(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
            List<Selfassesstmentadmin> response = _context.Selfassesstmentadmins.OrderByDescending(x=>x.Id).ToList();

                ViewBag.id = 0;
                ViewBag.Tittle = "";
                ViewBag.SubTittle = "";
                ViewBag.Heading = "Add Assessment";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = _context.Selfassesstmentadmins.Where(x => x.Id == id).FirstOrDefault();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Tittle;
                        ViewBag.SubTittle = data.SubTittle;
                        ViewBag.Heading = "Update Assessment";
                        ViewBag.btnText = "Update";
                    }
                }
                return View("Assessment");

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Assessment([FromBody] SelfassesstmentadminDTO model)
        {
            try
            {
                if (model == null || model.SelfAssessmentList == null || !model.SelfAssessmentList.Any())
                {
                    return BadRequest("No data received.");
                }

                List<Selfassesstmentadmin> assessmentsToAdd = new List<Selfassesstmentadmin>();

                foreach (var assessment in model.SelfAssessmentList)
                {
                    Selfassesstmentadmin entity = new Selfassesstmentadmin();
                    {
                        entity.Tittle = assessment.Tittle;
                        entity.SubTittle = assessment.SubTittle;
                        entity.Ispoint = assessment.Ispoint;
                        entity.Pointname = assessment.Pointname;
                        entity.Isactive = true;
                        entity.Isdelete = false;
                        entity.CreatedDate = DateTime.Now;
                    }

                    _context.Selfassesstmentadmins?.Add(entity);
                    //_context.SaveChanges();
                }
                _context.SaveChanges();


                return View("Assessment");
                return Json(new { success = true, message = "Data saved successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }


    }
}
