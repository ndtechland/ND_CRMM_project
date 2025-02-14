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
        //public async Task<IActionResult> Assessment(int? id = 0)
        //{
        //    if (HttpContext.Session.GetString("UserName") != null)
        //    {
        //        SelfassesstmentadminDTO model = new SelfassesstmentadminDTO();
        //        model.SelfAssessmentList = _context.Selfassesstmentadmins.Where(x=>x.Isdelete == false).OrderByDescending(x=>x.Id).ToList();
        //        var Titlequesdata = _context.Selfassesstmentadmins.Where(a => a.Isdelete == false).FirstOrDefault();
        //        var SubTitlequesdata = _context.Selfassesstmentadmins.Where(a => a.Isdelete == false && a.Tittle== Titlequesdata.Tittle).ToList();
        //        ViewBag.Tittle = Titlequesdata.Tittle;
        //        ViewBag.SubTittle = SubTitlequesdata;
        //        ViewBag.id = 0;
        //        //ViewBag.Tittle = "";
        //        ViewBag.SubTittle = "";
        //        ViewBag.Ispoint = "";
        //        ViewBag.Pointname = "";
        //        ViewBag.Heading = "Add Assessment";
        //        ViewBag.btnText = "SAVE";
        //        if (id != 0)
        //        {
        //            var data = _context.Selfassesstmentadmins.Where(x => x.Id == id).FirstOrDefault();
        //            if (data != null)
        //            {
        //                ViewBag.id = data.Id;
        //                ViewBag.Tittle = data.Tittle;
        //                ViewBag.SubTittle = data.SubTittle;
        //                ViewBag.Ispoint = data.Ispoint;
        //                ViewBag.Pointname = data.Pointname;
        //                ViewBag.Heading = "Update Assessment";
        //                ViewBag.btnText = "Update";
        //            }
        //        }
        //        return View(model);

        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Admin");
        //    }
        //}
        public async Task<IActionResult> Assessment(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                SelfassesstmentadminDTO model = new SelfassesstmentadminDTO();
                model.SelfAssessmentList = _context.Selfassesstmentadmins.Where(x => x.Isdelete == false).OrderByDescending(x => x.Id).ToList();

                var Titlequesdata = _context.Selfassesstmentadmins.FirstOrDefault(a => a.Isdelete == false);
                
                if (Titlequesdata != null)
                {
                    var SubTitlequesdata = _context.Selfassesstmentadmins
                        .Where(a => a.Isdelete == false && a.Tittle == Titlequesdata.Tittle)
                        .Select(a => a.SubTittle)
                        .ToList();

                    ViewBag.Tittle = Titlequesdata.Tittle;
                    ViewBag.SubTittleList = SubTitlequesdata; 
                }

                ViewBag.id = 0;
                ViewBag.Ispoint = "";
                ViewBag.Pointname = "";
                ViewBag.Heading = "Add Assessment";
                ViewBag.btnText = "SAVE";

                if (id != 0)
                {
                    var data = _context.Selfassesstmentadmins.FirstOrDefault(x => x.Id == id);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Tittle;
                        ViewBag.SubTittle = data.SubTittle; 
                        ViewBag.Ispoint = data.Ispoint;
                        ViewBag.Pointname = data.Pointname;
                        ViewBag.Heading = "Update Assessment";
                        ViewBag.btnText = "Update";
                    }
                }

                return View(model);
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

                var assessmentIds = model.SelfAssessmentList.Select(a => a.Id).ToList();

                var existingRecords = _context.Selfassesstmentadmins
                    .Where(x => assessmentIds.Contains(x.Id))
                    .ToList();

                bool isUpdated = false, isAdded = false;

                foreach (var assessment in model.SelfAssessmentList)
                {
                    var existingRecord = existingRecords.FirstOrDefault(x => x.Id == assessment.Id);

                    if (existingRecord != null)
                    {
                        existingRecord.Tittle = assessment.Tittle;
                        existingRecord.SubTittle = assessment.SubTittle;
                        existingRecord.Ispoint = assessment.Ispoint;
                        existingRecord.Pointname = assessment.Pointname;
                        isUpdated = true;
                    }
                    else
                    {
                        Selfassesstmentadmin newEntity = new Selfassesstmentadmin
                        {
                            Tittle = assessment.Tittle,
                            SubTittle = assessment.SubTittle,
                            Ispoint = assessment.Ispoint,
                            Pointname = assessment.Pointname,
                            Isactive = true,
                            Isdelete = false,
                            CreatedDate = DateTime.Now
                        };
                        _context.Selfassesstmentadmins?.Add(newEntity);
                        isAdded = true;
                    }
                }

                await _context.SaveChangesAsync();

                string message = isUpdated && isAdded ? "Data updated and saved successfully!": isUpdated
                        ? "Data updated successfully!"
                        : "Data saved successfully!";

                return Json(new { success = true, message = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal Server Error: " + ex.Message });
            }
        }

        public async Task<IActionResult> DeleteAssessment(int id)
        {
            try
            {
                var data = _context.Selfassesstmentadmins.Find(id);
                if (data != null)
                {
                    data.Isdelete = true;
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("Assessment");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

    }
}
