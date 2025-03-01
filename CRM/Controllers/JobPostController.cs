using CRM.Data;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Models.Jobcontext;
using CRM.Repository;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core;

namespace CRM.Controllers
{
    public class JobPostController : Controller
    {
        private readonly Jobforindia_HireJobContext _Jobcontext;
        private readonly admin_NDCrMContext _context;

        private readonly ICrmrpo _ICrmrpo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;
        public JobPostController(ICrmrpo _ICrmrpo, Jobforindia_HireJobContext _Jobcontext, IWebHostEnvironment hostingEnvironment, IEmailService emailService, admin_NDCrMContext context)
        {
            this._context = context;
            this._ICrmrpo = _ICrmrpo;
            _webHostEnvironment = hostingEnvironment;
            _emailService = emailService;
            this._Jobcontext = _Jobcontext;
        }
        [HttpGet, Route("JobPost/AddJobPost")]
        public IActionResult AddJobPost(int? id)
        {
            int iId = (int)(id == null ? 0 : id);
            if (HttpContext.Session.GetString("UserName") != null)
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();

                ViewBag.Organization = HttpContext.Session.GetString("UserName");

                ViewBag.Department = _context.DepartmentMasters.Where(x => x.AdminLoginId == Userid).Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.DepartmentName

                }).ToList();
                ViewBag.Designation = _context.DesignationMasters.Where(x => x.AdminLoginId == Userid).Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.DesignationName
                }).ToList();
                ViewBag.Qualification = _Jobcontext.Qualifications.Where(x => x.IsDelete == false).Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.Qualificationame

                }).ToList();
                ViewBag.Packagesss = _Jobcontext.Salaries.Where(x => x.IsDelete == false).Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.SalaryPackage

                }).ToList();
                ViewBag.PostedBy = _Jobcontext.Postedbies.Where(x => x.IsDelete == false).Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.Postedtype

                }).ToList();
                ViewBag.WorkModeId = _Jobcontext.WorkModes.Where(x => x.IsDelete == false).Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.WorkModeName

                }).ToList();
                ViewBag.city = _context.Cities
              .Select(w => new SelectListItem
              {
                  Value = w.Id.ToString(),
                  Text = w.City1
              })
               .ToList();
                ViewBag.state = _context.States
             .Select(w => new SelectListItem
             {
                 Value = w.Id.ToString(),
                 Text = w.SName
             }).ToList();
                string AddedBy = HttpContext.Session.GetString("UserName");
                ViewBag.UserLogin = AddedBy;

                ViewBag.id = 0;
                ViewBag.DepartmentId = "";
                ViewBag.DesignationId = "";
                // ViewBag.LocationId = "";
                ViewBag.stateid = "";
                ViewBag.cityid = "";
                ViewBag.NoOfOpenings = "";
                ViewBag.RequiredExpeerience = "";
                //ViewBag.Status = "";
                ViewBag.Skills = "";
                ViewBag.JobDescreption = "";
                ViewBag.Package = "";
                ViewBag.Qualificationid = "";
                ViewBag.PostedById = "";
                ViewBag.WorkMode = "";
                ViewBag.company = "";
                ViewBag.QualificationDescription = "";
                ViewBag.AboutDescription = "";
                ViewBag.ResponsebilitiesDescription = "";
                ViewBag.heading = "Add Job :";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _Jobcontext.CJobOpens.Find(iId);
                    if (data != null)
                    {

                        ViewBag.id = data.Id;
                        ViewBag.DepartmentId = data.Department;
                        ViewBag.DesignationId = data.JobTitle;
                        ViewBag.NoOfOpenings = data.Opening;
                        ViewBag.RequiredExpeerience = data.RequiredExperience;
                       // ViewBag.Status = data.Status == true ? "1" : "0";
                        ViewBag.JobDescreption = data.JobDescription ?? string.Empty;
                        ViewBag.heading = "Update Job :";
                        ViewBag.btnText = "UPDATE";
                        ViewBag.Qualificationid = data.Qualificationid;
                        //ViewBag.Skills = HttpUtility.HtmlDecode(StripHtml(data.Skills)) ?? string.Empty;
                        ViewBag.Skills = data.Skills ?? string.Empty;
                        ViewBag.Package = data.Package;
                        ViewBag.PostedById = data.PostedById;
                        ViewBag.EmployeementType = data.EmployeementType;
                        ViewBag.stateid = data.Stateid;
                        ViewBag.cityid = data.Cityid;
                        ViewBag.WorkMode = data.WorkModeId;
                        ViewBag.company = data.Companyid;
                        ViewBag.QualificationDescription = data.QualificationDescription ?? string.Empty;
                        ViewBag.AboutDescription = data.AboutDescription ?? string.Empty;
                        ViewBag.ResponsebilitiesDescription = data.ResponsebilitiesDescription ?? string.Empty;
                    }
                }

                return View();

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        private string? StripHtml(string? skills)
        {

            // Check for null or empty input
            if (string.IsNullOrEmpty(skills))
                return string.Empty;

            // Regex pattern to match HTML tags
            string pattern = "<.*?>";

            // Replace HTML tags with an empty string
            string result = Regex.Replace(skills, pattern, string.Empty);

            return result;


        }

        [HttpPost, Route("JobPost/AddJobPost")]
        public async Task<IActionResult> Addjob(CJobOpen model)
        {

            if (HttpContext.Session.GetString("UserName") != null)
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                ViewBag.Organization = HttpContext.Session.GetString("UserName");

                ViewBag.Department = _context.DepartmentMasters.Where(x => x.AdminLoginId == Userid).Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.DepartmentName

                }).ToList();
                ViewBag.Designation = _context.DesignationMasters.Where(x => x.AdminLoginId == Userid).Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.DesignationName
                }).ToList();
                ViewBag.Qualification = _Jobcontext.Qualifications.Where(x => x.IsDelete == false).Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.Qualificationame

                }).ToList();
                ViewBag.Packagesss = _Jobcontext.Salaries.Where(x => x.IsDelete == false).Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.SalaryPackage

                }).ToList();
                ViewBag.PostedBy = _Jobcontext.Postedbies.Where(x => x.IsDelete == false).Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.Postedtype

                }).ToList();
                ViewBag.WorkModeId = _Jobcontext.WorkModes.Where(x => x.IsDelete == false).Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.WorkModeName

                }).ToList();
                ViewBag.city = _context.Cities
              .Select(w => new SelectListItem
              {
                  Value = w.Id.ToString(),
                  Text = w.City1
              })
               .ToList();
                ViewBag.state = _context.States
             .Select(w => new SelectListItem
             {
                 Value = w.Id.ToString(),
                 Text = w.SName
             }).ToList();


                if (model.Id > 0)
                {
                    var existingEntity = _Jobcontext.CJobOpens.Find(model.Id);
                    if (existingEntity != null)
                    {
                        existingEntity.JobTitle = model.JobTitle;
                        existingEntity.Qualificationid = model.Qualificationid;
                        existingEntity.Skills = model.Skills;
                        existingEntity.Package = model.Package;
                        existingEntity.EmployeementType = model.EmployeementType;
                        existingEntity.Opening = model.Opening;
                        existingEntity.RequiredExperience = model.RequiredExperience;
                        existingEntity.JobDescription = model.JobDescription;
                        existingEntity.Department = model.Department;
                        existingEntity.PostedById = model.PostedById;
                        existingEntity.WorkModeId = model.WorkModeId;
                        existingEntity.Stateid = model.Stateid;
                        existingEntity.Cityid = model.Cityid;
                        existingEntity.Companyid = (int)adminlogin.Vendorid;
                        existingEntity.QualificationDescription = model.QualificationDescription;
                        existingEntity.AboutDescription = model.AboutDescription;
                        existingEntity.ResponsebilitiesDescription = model.ResponsebilitiesDescription;
                        _Jobcontext.SaveChanges();
                        TempData["Message"] = "updok";
                    }
                }
                else
                {
                    var newRecord = new CJobOpen
                    {
                        JobTitle = model.JobTitle,
                        Opening = model.Opening,
                        Qualificationid = model.Qualificationid,
                        Skills = model.Skills,
                        Package = model.Package,
                        EmployeementType = model.EmployeementType,
                        // Location = model.Location,
                        RequiredExperience = model.RequiredExperience,
                        JobDescription = model.JobDescription.Trim(),
                        Department = model.Department,
                        PostedById = model.PostedById,
                        WorkModeId = model.WorkModeId,
                        Status =  true,
                        AddedBy = (int)adminlogin.Vendorid,
                        AddedOn = DateTime.Now.Date.Date,
                        Isdelete = false,
                        Stateid = model.Stateid,
                        Cityid = model.Cityid,
                        Companyid = (int)adminlogin.Vendorid,
                        QualificationDescription = model.QualificationDescription,
                        AboutDescription = model.AboutDescription,
                        ResponsebilitiesDescription = model.ResponsebilitiesDescription,
                        IsVendor = true,
                        Vendorid = (int)adminlogin.Vendorid,
                    };


                    _Jobcontext.CJobOpens.Add(newRecord);
                    _Jobcontext.SaveChanges();
                    TempData["Message"] = "ok";
                }

                return RedirectToAction("JobList");
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpGet]
        public IActionResult citydrop(int stateid)
        {
            var items = _context.Cities
                .Where(x => x.StateId == stateid)
                .Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = D.City1
                }).ToList();

            return Json(items);
        }
        [HttpGet, Route("JobPost/JobList")]
        public async Task<IActionResult> JobList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                List<Joblist> response = await _ICrmrpo.Getjoblist((int)adminlogin.Vendorid);
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }
        public async Task<IActionResult> DeleteJob(int Id)
        {
            try
            {
                var data = _Jobcontext.CJobOpens.Find(Id);
                if (data != null)
                {
                    data.Isdelete = true;
                    _Jobcontext.SaveChanges();
                    TempData["Message"] = "Job Deleted successfully.";
                }
                return RedirectToAction("JobList");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteCurrentOpening:" + ex.Message);
            }
        }
        public async Task<IActionResult> UpdateJobPostActiveStatus(int Id)
        {
            var jobdetail = await _Jobcontext.CJobOpens.FirstOrDefaultAsync(x => x.Id == Id);

            if (jobdetail == null)
            {
                TempData["msg"] = "Job not found!";
                return RedirectToAction("JobList");
            }
            jobdetail.Status = !jobdetail.Status;
            await _Jobcontext.SaveChangesAsync();
            return RedirectToAction("JobList");
        }
        [HttpGet, Route("JobPost/GetApplyJob")]
        public async Task<IActionResult> GetApplyJob()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.FirstOrDefault(x => x.Id == Userid);

                ViewBag.carrierStatus = _Jobcontext.CarrierStatuses.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Statusname
                }).ToList();
                var cities = await _context.Cities.ToListAsync();
                var states = await _context.States.ToListAsync();
                var createprofiles = await _Jobcontext.CreateProfiles.ToListAsync();
                var applyjobs = await _Jobcontext.Applyjobs.ToListAsync();
                var cJobOpens = await _Jobcontext.CJobOpens.ToListAsync();
                var carrierStatuses = await _Jobcontext.CarrierStatuses.ToListAsync();
                var designation = await _context.DesignationMasters.ToListAsync();
              

                var jobList = (from cp in createprofiles
                               join jb in applyjobs on cp.Id.ToString() equals jb.UserId
                               join job in cJobOpens on jb.JobId equals job.Id.ToString()
                               join cs in carrierStatuses on jb.Status equals cs.Id
                               join ct in cities on job.Cityid equals ct.Id
                               join st in states on job.Stateid equals st.Id
                               join desi in designation on Convert.ToInt32(job.JobTitle) equals desi.Id
                               where job.Vendorid == adminlogin.Vendorid
                               select new GetApplyjob
                               {
                                   Id = Convert.ToInt32(jb.Id),
                                   FullName = cp.FullName,
                                   EmailID = cp.EmailId,
                                   MobileNumber = cp.MobileNumber,
                                   Experience = cp.Experience,
                                   GenderName = cp.GenderName,
                                   Dateofbirth = cp.Dateofbirth.ToString("dd/MM/yyyy"),
                                   Address = cp.Address,
                                   Pincode = cp.Pincode,
                                   carrierlistid = jb.Status,
                                   CarrierStatus = cs.Statusname,
                                   ResumeFilePath = "https://jobapi.ndtechland.com/" + cp.ResumeFilePath,
                                   ProfileImage = "https://jobapi.ndtechland.com/" + cp.ProfileImage,
                                   Designation = desi.DesignationName,
                                   StateName = st.SName,
                                   CityName = ct.City1
                               }).ToList().OrderByDescending(x => x.Id).ToList();
              

                return View(jobList);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetApplyJob(GetApplyjob model, string PositionApplyFor)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.FirstOrDefault(x => x.Id == Userid);

                if (model != null)
                {
                    var appjb = await _Jobcontext.Applyjobs.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                    if (appjb.Status != 5)
                    {
                        appjb.Status = Convert.ToInt32(model.CarrierStatus);
                        await _Jobcontext.SaveChangesAsync();
                        var user = await _Jobcontext.CreateProfiles.Where(x => x.Id == Convert.ToInt32(appjb.UserId)).FirstOrDefaultAsync();

                        if (model.CarrierStatus == "10")
                        {
                            bool checkadd = await _Jobcontext.Carriers.AnyAsync(x => x.Email == user.EmailId && x.MobileNo == user.MobileNumber.ToString());
                            if (!checkadd)
                            {
                                Carrier job = new Carrier()
                                {
                                    FirstName = user.FullName,
                                    LastName = user.FullName,
                                    Gender = user.GenderName,
                                    Email = user.EmailId,
                                    MobileNo = Convert.ToString(model.MobileNumber),
                                    CurrentLocation = user.Address,
                                    Stateid = user.StateId,
                                    Cityid = user.StateId,
                                    FilePath = user.ResumeFilePath,
                                    CarrierStatus = Convert.ToInt32(model.CarrierStatus),
                                    CurrentCtc = user.CurrentCtc,
                                    ExpectedCtc = user.ExpectedCtc,
                                    Password = user.Password,
                                };
                                await _Jobcontext.Carriers.AddAsync(job);
                                await _Jobcontext.SaveChangesAsync();
                            }

                        }
                    }

                }
                return RedirectToAction("GetApplyJob", "JobPost");
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
    }
}
