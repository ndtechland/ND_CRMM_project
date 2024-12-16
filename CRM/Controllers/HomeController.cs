using CRM.Models;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Users;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

//using System.Web.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.ContactCount = await _context.ContactUs.CountAsync();
                int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                ViewBag.DemoRequest = await _context.DemoRequests.CountAsync();
                ViewBag.HelpCenters = await _context.HelpCenters.CountAsync();
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == UserId).FirstOrDefaultAsync();

                //VendorDashboard
                ViewBag.Professional = await _context.VendorRegistrations.Where(x => x.Id == adminlogin.Vendorid).Select(x => x.Isprofessionaltax).FirstOrDefaultAsync();
                // Customerlist
                ViewBag.Customer = await _context.CustomerRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).CountAsync();
                // VendorInvoice
                ViewBag.VendorInvoice = await _context.VendorRegistrations.Where(x => x.Isactive == true).CountAsync();
                // VendorProduct
                ViewBag.VendorProduct = await _context.VendorProductMasters.Where(x => x.VendorId == adminlogin.Vendorid && x.IsActive == true).CountAsync();
                // CustomerInvoices
                ViewBag.CustomerInvoices = await _context.CustomerInvoices.Where(x => x.VendorId == adminlogin.Vendorid).GroupBy(x => x.InvoiceNumber).CountAsync();

                var emplist = await _context.EmployeeRegistrations
                            .Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false)
                            .ToListAsync();
                //onBreakList
                ViewBag.onBreakList = emplist.Count(x => _context.EmployeeCheckIns
                    .Any(y => y.EmployeeId == x.EmployeeId && y.Breakin == true
                             && y.Currentdate.Value.Date == DateTime.Now.Date));
                //Checkin
                ViewBag.Checkin = emplist.Count(x => _context.EmployeeCheckIns
                 .Any(y => y.EmployeeId == x.EmployeeId && y.CheckIn == true
                          && y.Currentdate.Value.Date == DateTime.Now.Date));

                ViewBag.Vendorlist = await _context.VendorRegistrations.CountAsync();
                //Employee
                ViewBag.Employee = emplist.Count();
                //Holidays
                ViewBag.Holidays = await _context.OfficeEvents
                .Where(x => x.Vendorid == adminlogin.Vendorid && x.Date >= DateTime.Now.Date)
                .Select(x => new
                {
                    Subtittle = x.Subtittle,
                    Date = x.Date
                }).OrderByDescending(x => x.Date).ToListAsync();

                ViewBag.Announcements = await _context.EventsmeetSchedulers
                .Where(x => x.Vendorid == adminlogin.Vendorid)
                .Select(x => new
                {
                    Title = x.Tittle,
                    Date = x.ScheduleDate,
                    time = x.Time
                }).OrderByDescending(x => x.Date).ToListAsync();
                //LeaveList
                ViewBag.onLeaveList = emplist.Count(x => _context.ApplyLeaveNews
               .Any(y => y.UserId == x.EmployeeId
               && y.Isapprove == 2
               && y.StartDate.Date <= DateTime.Now.Date
               && y.EndDate.Date >= DateTime.Now.Date));
               
                var today = DateTime.Today;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

                // Generate all dates from 1st of the month to today
                var allDates = Enumerable.Range(0, (today - firstDayOfMonth).Days + 1)
                    .Select(offset => firstDayOfMonth.AddDays(offset))
                    .ToList();
                var records = _context.EmployeeCheckInRecords
                    .Where(record => record.CurrentDate.HasValue &&
                                     record.CurrentDate.Value.Year == today.Year &&
                                     record.CurrentDate.Value.Month == today.Month &&
                                     record.CurrentDate.Value.Date <= today) 
                    .AsEnumerable()
                    .GroupBy(record => record.CurrentDate.Value.Date)
                    .ToDictionary(group => group.Key, group => group.Sum(record =>
                        (record.CheckOuttime.HasValue && record.CheckIntime.HasValue)
                        ? Math.Max(0, (record.CheckOuttime.Value - record.CheckIntime.Value).TotalHours)
                        : 0));
                var workingHoursDates = allDates.Select(date => date.Day.ToString()).ToList(); 
                var workingHoursData = allDates.Select(date =>
                    records.ContainsKey(date) ? records[date] : 0 
                ).ToList();
                int currentMonth = today.Month;
                ViewBag.WorkingHoursDates = workingHoursDates;
                ViewBag.WorkingHoursData = workingHoursData;

                ViewBag.monthname = getMonthName(currentMonth);

                ViewBag.Numberofwfh = emplist.Count(x => _context.EmpApplywfhs
                .Any(y => y.UserId == x.EmployeeId
                 && y.Iswfh == 2
                 && y.Startdate.Value.Date <= DateTime.Now.Date
                 && y.EndDate.Value.Date >= DateTime.Now.Date));
                //tasklist
                List<TasksListDashDto> response = await _context.EmployeeTasks
     .Where(x => x.Vendorid == adminlogin.Vendorid)
     .OrderByDescending(x => x.Id)
     .Select(x => new TasksListDashDto
     {
         EmployeeId = x.EmployeeId,
         EmployeeName = _context.EmployeeRegistrations
             .Where(em => em.EmployeeId == x.EmployeeId)
             .Select(em => em.FirstName)
             .FirstOrDefault(),
         TaskName = x.Task,
         Taskstatus = _context.TaskStatuses
             .Where(a => a.Id == x.Status)
             .Select(status => status.StatusName)
             .FirstOrDefault(),
     }).Take(5).ToListAsync();
                ViewBag.TasksList = response;
                //replytasklist
                List<string> employeeIds = emplist.Select(emp => emp.EmployeeId).ToList();

                var replytasklist = await (from task in _context.EmpTasksLists
                                           join employee in _context.EmployeeRegistrations
                                               on task.EmployeeId equals employee.EmployeeId
                                           join status in _context.TaskStatuses
                                               on task.Taskstatus equals status.Id
                                           join employeeTask in _context.EmployeeTasks
                                               on task.Subtaskid equals employeeTask.Id into taskDetails
                                           from taskDetail in taskDetails.DefaultIfEmpty()
                                           join subTask in _context.EmployeeTasksLists
                                               on task.Taskid equals subTask.Id into subTaskDetails
                                           from subTaskDetail in subTaskDetails.DefaultIfEmpty()
                                           where employeeIds.Contains(task.EmployeeId)
                                           select new TasksReplyListDashDto
                                           {
                                               id = task.Id,
                                               TaskName = taskDetail != null ? taskDetail.Task : "N/A",
                                               SubTaskName = subTaskDetail != null ? subTaskDetail.Taskname : "N/A",
                                               Replydate = task.Replydate.HasValue
                                                   ? task.Replydate.Value.ToString("dd MMM yy")
                                                   : "N/A",
                                               Taskstatus = status.StatusName,
                                           }).OrderByDescending(task => task.id).Take(4).ToListAsync();

                ViewBag.ReplyTasksList = replytasklist;



                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }


        }

        [HttpGet]
        public IActionResult Banner()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {


                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, BannerMaster banerm)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select a file.";
                return View("Banner");
            }

            // Access the file properties
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadsImage", fileName);

            // Save the file to a folder
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            //save in db

            banerm.Imagepath = filePath;
            banerm.BannerImage = fileName;
            var response = await _ICrmrpo.Banner(banerm);
            //end
            ViewBag.Message = "File uploaded successfully.";
            return View("Banner");
        }

        //======Invoice Section========//
        [HttpGet]
        public IActionResult Invoicelist()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {


                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        //-----Quation
        [HttpGet, Route("Home/Quation")]
        public async Task<IActionResult> Quation(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


                ViewBag.ProductDetails = await _context.ProductMasters.Where(x => x.IsDeleted == false).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ProductName
                }).ToListAsync();
                ViewBag.id = 0;
                ViewBag.CompanyName = "";
                ViewBag.CustomerName = "";
                ViewBag.Email = "";
                ViewBag.SalesPersonName = "";
                ViewBag.ProductId = "";
                ViewBag.Subject = "";
                ViewBag.Amount = "";
                ViewBag.Mobile = "";
                ViewBag.Heading = "Add Quation";
                ViewBag.btnText = "SAVE";

                if (id != 0)
                {
                    var data = await _context.Quations.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.CompanyName = data.CompanyName;
                        ViewBag.CustomerName = data.CustomerName;
                        ViewBag.Email = data.Email;
                        ViewBag.SalesPersonName = data.SalesPersonName;
                        ViewBag.ProductId = data.ProductId;
                        ViewBag.Subject = data.Subject;
                        ViewBag.Amount = data.Amount;
                        ViewBag.Mobile = data.Mobile;
                        ViewBag.Heading = "Update Quation";
                        ViewBag.btnText = "Update";
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
        public async Task<IActionResult> Quation(QuationDto model)
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


                if (model == null)
                {
                    ModelState.Clear();
                    return View();
                }
                if (model.Id != 0)
                {
                    var response = await _ICrmrpo.Iupdate(model);
                    if (response != null)
                    {
                        TempData["Message"] = "Data Update Successfully.";
                        return RedirectToAction("Quation", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record not found for update.");
                        return View(model);
                    }
                }
                else
                {
                    var response = await _ICrmrpo.Quation(model);
                    TempData["Message"] = "Data Added Successfully.";
                    return RedirectToAction("Quation", "Home");
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuationList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var response = await _ICrmrpo.QuationList();

                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public async Task<IActionResult> DeleteQuation(int id)
        {
            try
            {
                var data = _context.Quations.Find(id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    _context.SaveChanges();
                }
                return RedirectToAction("QuationList");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the DeleteQuationList:" + ex.Message);
            }
        }

        [HttpGet, Route("Home/Department")]
        public IActionResult Department(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                List<DepartmentMaster> response = _context.DepartmentMasters.Where(x => x.AdminLoginId == adminlogin.Id).OrderByDescending(d => d.Id).ToList();

                ViewBag.id = "";
                ViewBag.DepartmentName = "";
                ViewBag.Heading = "Add Department";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = _context.DepartmentMasters.Where(x => x.Id == id && x.AdminLoginId == Userid).FirstOrDefault();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.DepartmentName = data.DepartmentName;
                        ViewBag.Heading = "Update Department";
                        ViewBag.btnText = "Update";
                    }
                }
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Department(DepartmentMaster model)
        {

            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


            if (model == null)
            {
                ModelState.Clear();
                return View();
            }
            if (model.Id != 0)
            {
                var Department = await _ICrmrpo.updateDepartment(model);
                if (Department != null)
                {
                    TempData["Message"] = "updok";
                    return RedirectToAction("Department", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Record not found for update.");
                    return View(model);
                }
            }
            else
            {
                DepartmentMaster master = new DepartmentMaster
                {
                    DepartmentName = model.DepartmentName,
                    AdminLoginId = adminlogin.Id,
                };
                _context.DepartmentMasters.Add(master);
                _context.SaveChanges();
                TempData["Message"] = "ok";
                return RedirectToAction("Department", "Home");
            }
        }
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var data = _context.DepartmentMasters.Find(id);
                if (data != null)
                {
                    _context.DepartmentMasters.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("Department");
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"An error occurred while deleting the Department:" + ex.Message;
                throw new Exception("An error occurred while deleting the DeleteQuationList:" + ex.Message);
            }
        }
        [HttpGet]
        public IActionResult WorkLocation()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {


                return View();

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> WorkLocation(WorkLocation1 model)
        {
            if (model != null)
            {
                WorkLocation1 master = new WorkLocation1
                {
                    StateId = model.StateId,
                    CityId = model.CityId,
                    Createdate = DateTime.Now.Date,
                    Isactive = true,
                    Commissoninpercentage = model.Commissoninpercentage,
                };
                _context.WorkLocations1.Add(master);
                _context.SaveChanges();
                return RedirectToAction("WorkLocationlist");
            }
            else
            {
                ModelState.Clear();
                return View("WorkLocationlist");
            }
            return View("WorkLocationlist");
        }

        [HttpGet]
        public IActionResult WorkLocationlist()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var result = from wl in _context.WorkLocations1
                             join ct in _context.Cities on wl.CityId equals ct.Id into cityGroup
                             from city in cityGroup.DefaultIfEmpty()
                             join st in _context.States on wl.StateId equals st.Id into stateGroup
                             from state in stateGroup.DefaultIfEmpty()
                             select new WorkLocationDTO
                             {
                                 Id = wl.Id,
                                 State = state.SName,
                                 City = city.City1,
                                 Commissoninpercentage = Convert.ToInt32(wl.Commissoninpercentage)
                             };



                return View(result);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public async Task<IActionResult> DeleteWorkLocation(int id)
        {
            try
            {
                var data = _context.WorkLocations.Find(id);
                if (data != null)
                {
                    _context.WorkLocations.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("WorkLocationlist");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        [HttpGet, Route("Home/Designation")]
        public IActionResult Designation(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                List<DesignationMaster> response = _context.DesignationMasters.OrderByDescending(d => d.Id).Where(x => x.AdminLoginId == adminlogin.Id).ToList();



                ViewBag.id = "";
                ViewBag.DesignationName = "";
                ViewBag.Heading = "Add Designation";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = _context.DesignationMasters.Where(x => x.Id == id && x.AdminLoginId == Userid).FirstOrDefault();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.DesignationName = data.DesignationName;
                        ViewBag.Heading = "Update Designation";
                        ViewBag.btnText = "Update";
                    }
                }
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Designation(DesignationMaster model)
        {

            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


            if (model == null)
            {
                ModelState.Clear();
                return View();
            }
            if (model.Id != 0)
            {
                var Department = await _ICrmrpo.updateDesignation(model);
                if (Department != null)
                {
                    TempData["Message"] = "updok";
                    return RedirectToAction("Designation", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Record not found for update.");
                    return View(model);
                }
            }
            else
            {
                DesignationMaster master = new DesignationMaster
                {
                    DesignationName = model.DesignationName,
                    AdminLoginId = adminlogin.Id,
                };
                _context.DesignationMasters.Add(master);
                _context.SaveChanges();
                TempData["Message"] = "ok";
                return RedirectToAction("Designation", "Home");
            }
        }
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            try
            {
                var data = _context.DesignationMasters.Find(id);
                if (data != null)
                {
                    _context.DesignationMasters.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("Designation");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        [HttpGet]
        public JsonResult Product(int? id)
        {
            var data = (from pm in _context.ProductMasters
                        join gm in _context.GstMasters on Convert.ToInt32(pm.Gst) equals gm.Id
                        //join cs in _context.CustomerRegistrations on pm.Id equals Convert.ToInt32(cs.ProductDetails)
                        where pm.Id == id && pm.IsDeleted == false
                        select new Customer
                        {
                            Scgst = gm.Scgst,
                            Cgst = gm.Cgst,
                            Igst = gm.Igst,
                            Price = pm.Price,
                            HsnSacCode = pm.HsnSacCode,
                            //State = cs.State,
                        }).FirstOrDefault();

            var result = new
            {
                Data = data,
            };

            return new JsonResult(result);
        }

        public JsonResult EditWorkLocation(int id)
        {
            var loc = new WorkLocation1();
            var data = _ICrmrpo.GetWorkLocationById(id);
            loc.Id = data.Id;
            loc.CityId = data.CityId;
            loc.StateId = data.StateId;
            loc.Commissoninpercentage = data.Commissoninpercentage;

            var result = new
            {
                loc = loc,
            };
            return new JsonResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditWorkLocation(WorkLocation1 model)
        {
            try
            {
                var Location = await _ICrmrpo.updateWorkLocation(model);
                if (Location != null)
                {
                    TempData["ErrorMessage"] = "Work Location update Successfully.";
                    return RedirectToAction("WorkLocationlist", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Work Location not update.";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        public IActionResult ProfessionalTDStax(int id)
        {
            try
            {
                var addedby = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = addedby;

                var professionaltaxes = (from pt in _context.Professionaltaxes
                                         join mfy in _context.MFinancialYears on pt.Finyear equals (int?)mfy.FyearCode into mfyGroup
                                         from mfy in mfyGroup.DefaultIfEmpty()
                                         select new ProfessionaltaxDto
                                         {
                                             Id = pt.Id,
                                             Minamount = pt.Minamount,
                                             Maxamount = pt.Maxamount,
                                             Amountpercentage = pt.Amountpercentage,
                                             Iactive = pt.Iactive,
                                             Finyear = mfy.FyearName,
                                             CreateDate = pt.CreateDate
                                         })
                                         .OrderByDescending(x => x.Id)
                                         .ToList();

                ViewBag.FinancialYear = _context.MFinancialYears
                    .Where(x => x.FyearIsdelete == "N")
                    .Select(p => new SelectListItem
                    {
                        Value = p.FyearCode.ToString(),
                        Text = p.FyearName
                    })
                    .ToList();

                ViewBag.id = 0;
                ViewBag.Minamount = "";
                ViewBag.Maxamount = "";
                ViewBag.Amountpercentage = "";
                ViewBag.IsActive = "";
                ViewBag.Finy = "";
                ViewBag.heading = "Add Professional Tax";
                ViewBag.btnText = "SAVE";

                if (id != 0)
                {
                    var data = _context.Professionaltaxes.Find(id);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Minamount = data.Minamount;
                        ViewBag.Maxamount = data.Maxamount;
                        ViewBag.Amountpercentage = data.Amountpercentage;
                        ViewBag.IsActive = data.Iactive;
                        ViewBag.Finy = data.Finyear;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Professional Tax";
                    }
                }

                return View(professionaltaxes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProfessionalTDStax(ProfessionaltaxDto model)
        {
            try
            {
                bool check = await _ICrmrpo.AddAndUpdateProfessionaltax(model);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("ProfessionalTDStax");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("ProfessionalTDStax");
                    }
                }
                else
                {
                    return RedirectToAction("ProfessionalTDStax");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> DeleteProfessionaltax(int id)
        {
            try
            {
                var dlt = _context.Professionaltaxes.Find(id);
                if (dlt != null)
                {
                    _context.Professionaltaxes.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("ProfessionalTDStax");

            }
            catch (Exception)
            {

                throw;
            }
        }
        //[HttpPost]
        //public IActionResult GetLocationsByCustomer(string customerId)
        //{
        //    if (!string.IsNullOrEmpty(customerId))
        //    {
        //        var locations = _context.CustomerRegistrations
        //            .AsEnumerable()
        //            .Select(c => new
        //            {
        //                c.Id,
        //                WorkLocations = c.WorkLocation?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        //                                            .Select(loc => _context.Cities.FirstOrDefault(w => w.Id == Convert.ToInt32(loc)))
        //                                            .Where(w => w != null)
        //            })
        //            .FirstOrDefault(x => x.Id == Convert.ToInt32(customerId));

        //        if (locations != null && locations.WorkLocations != null)
        //        {
        //            var locationList = locations.WorkLocations.Select(x => new SelectListItem
        //            {
        //                Value = x.Id.ToString(),
        //                Text = x.City1
        //            }).ToList();

        //            return Json(locationList);
        //        }
        //    }

        //    return Json(new List<SelectListItem>());
        //}


        [HttpGet]
        public List<State> BindStateDetails(int CountryId)
        {

            List<State> stateDetail = new List<State>();
            try
            {
                stateDetail = _ICrmrpo.BindState();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return stateDetail;
        }
        [HttpGet]
        public List<City> BindCityDetails(int stateId)
        {
            List<City> cityDetail = new List<City>();
            try
            {
                cityDetail = _ICrmrpo.BindCity(stateId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return cityDetail;
        }
        [Route("Home/addcontribution")]
        [HttpGet]
        public IActionResult addcontribution()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {


                ViewBag.CustomerName = _context.CustomerRegistrations
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                ViewBag.Message = TempData["ErrorMessage"];
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> addcontribution(Additonalcontribution model)
        {
            try
            {
                if (model != null)
                {
                    Additonalcontribution ac = new Additonalcontribution
                    {
                        ContributionName = model.ContributionName,
                        CustomerId = model.CustomerId,
                        WorkLocationId = model.WorkLocationId,
                        CreatedDate = DateTime.Now.Date,
                        Isactive = true,
                    };
                    _context.Additonalcontributions.Add(ac);
                    _context.SaveChanges();
                    TempData["ErrorMessage"] = "Additonalcontribution  Add";
                    return RedirectToAction("addcontribution");
                }
                else
                {
                    TempData["ErrorMessage"] = "data not save";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }

        [HttpGet, Route("Home/Gstmaster")]
        public IActionResult Gstmaster(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                List<GstMaster> response = _context.GstMasters.ToList();
                ViewBag.id = 0;
                ViewBag.GstPercentagen = "";
                ViewBag.Scgst = "";
                ViewBag.Cgst = "";
                ViewBag.Igst = "";
                ViewBag.Heading = "Add Gst";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = _context.GstMasters.Where(x => x.Id == id).FirstOrDefault();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.GstPercentagen = data.GstPercentagen;
                        ViewBag.Scgst = data.Scgst;
                        ViewBag.Cgst = data.Cgst;
                        ViewBag.Igst = data.Igst;
                        ViewBag.Heading = "Update Gst";
                        ViewBag.btnText = "Update";
                    }
                }
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Gstmaster(GstMaster model)
        {

            int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = await _context.AdminLogins.Where(x => x.Id == UserId).FirstOrDefaultAsync();


            if (model == null)
            {
                ModelState.AddModelError("", "Model cannot be null.");
                return View(model);
            }
            if (!decimal.TryParse(model.GstPercentagen, out decimal gstPercentage))
            {
                ModelState.AddModelError("", "Invalid GST Percentage value.");
                return View(model);
            }

            decimal halfGst = gstPercentage / 2;

            if (model.Id != 0)
            {
                var gst = await _context.GstMasters.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (gst != null)
                {
                    gst.GstPercentagen = gstPercentage.ToString();
                    gst.Scgst = halfGst.ToString();
                    gst.Cgst = halfGst.ToString();
                    gst.Igst = gstPercentage.ToString();

                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Data updated successfully.";
                    return RedirectToAction("Gstmaster", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Record not found for update.");
                    return View(model);
                }
            }
            else
            {
                GstMaster master = new GstMaster
                {
                    GstPercentagen = gstPercentage.ToString(),
                    Scgst = halfGst.ToString(),
                    Cgst = halfGst.ToString(),
                    Igst = gstPercentage.ToString(),
                };

                await _context.GstMasters.AddAsync(master);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Data added successfully.";
                return RedirectToAction("Gstmaster", "Home");
            }
        }
        public async Task<IActionResult> DeleteGstmaster(int id)
        {
            try
            {
                var data = _context.GstMasters.Find(id);
                if (data != null)
                {
                    _context.GstMasters.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("Gstmasterlist");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        [HttpGet, Route("Home/Categorymaster")]
        public IActionResult Categorymaster(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                List<Models.Crm.Category> response = _context.Categories.ToList();


                ViewBag.id = "";
                ViewBag.CategoryName = "";
                ViewBag.Heading = "Add Category";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = _context.Categories.Where(x => x.Id == id).FirstOrDefault();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.CategoryName = data.CategoryName;
                        ViewBag.Heading = "Update Category";
                        ViewBag.btnText = "Update";
                    }
                }
                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Categorymaster(Models.Crm.Category model)
        {

            int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = await _context.AdminLogins.Where(x => x.Id == UserId).FirstOrDefaultAsync();


            if (model == null)
            {
                ModelState.AddModelError("", "Model cannot be null.");
                return View(model);
            }

            if (model.Id != 0)
            {
                var ca = await _context.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (ca != null)
                {
                    ca.CategoryName = model.CategoryName;
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "updok";
                    return RedirectToAction("Categorymaster", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Record not found for update.");
                    return View(model);
                }
            }
            else
            {
                Models.Crm.Category master = new Models.Crm.Category
                {
                    CategoryName = model.CategoryName,
                };

                await _context.Categories.AddAsync(master);
                await _context.SaveChangesAsync();
                TempData["Message"] = "ok";
                return RedirectToAction("Categorymaster", "Home");
            }
        }
        public async Task<IActionResult> DeleteCategorymaster(int id)
        {
            try
            {
                var data = _context.Categories.Find(id);
                if (data != null)
                {
                    _context.Categories.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("Categorymaster");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        [HttpGet]
        public async Task<IActionResult> AppFaq(int id)
        {
            try
            {

                int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == UserId).FirstOrDefaultAsync();

                List<AppFaq> faq = _context.AppFaqs.OrderBy(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Tittle = "";
                ViewBag.Subtittle = "";
                ViewBag.heading = "Add Faq";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.AppFaqs.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Tittle;
                        ViewBag.Subtittle = data.Subtittle;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Faq";

                    }
                }

                return View(faq);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AppFaq(AppFaq model)
        {
            try
            {

                int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == UserId).FirstOrDefaultAsync();


                bool check = await _ICrmrpo.Addfaq(model);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("AppFaq");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("AppFaq");
                    }
                }
                else
                {
                    return RedirectToAction("AppFaq");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> DeleteAppfaq(int id)
        {
            try
            {
                var dlt = await _context.AppFaqs.FindAsync(id);
                _context.Remove(dlt);
                _context.SaveChanges();
                TempData["Message"] = "dltok";
                return RedirectToAction("AppFaq");
            }
            catch (Exception)
            {
                TempData["msg"] = "Delete failed: An error occurred.";
                return RedirectToAction("AppFaq");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Blogs(int id)
        {
            try
            {

                int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == UserId).FirstOrDefaultAsync();

                BlogDto model = new BlogDto();
                model.Blogs = _context.Blogs.Where(b => b.IsPublished == true).OrderByDescending(b => b.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.BlogTitle = "";
                ViewBag.Content = "";
                ViewBag.BlogImage = "";
                ViewBag.heading = "Add Blog";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.Blogs.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.BlogTitle = data.Title;
                        ViewBag.Content = data.Content;
                        ViewBag.BlogImage = data.BlogImage;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Blog";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Blogs(BlogDto model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                bool check = await _ICrmrpo.AddAndUpdateBlog(model, AddedBy);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("Blogs");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("Blogs");
                    }
                }
                else
                {
                    return RedirectToAction("Blogs");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteBlogs(int id)
        {
            try
            {
                var dlt = _context.Blogs.Find(id);
                if (dlt != null)
                {
                    _context.Blogs.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("Blogs");

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> OurExpirtise(int id)
        {
            try
            {

                int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == UserId).FirstOrDefaultAsync();

                ExperiseDTO model = new ExperiseDTO();
                model.OurExperties = _context.OurExpertises.OrderByDescending(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.ExpertiseName = "";
                ViewBag.Description = "";
                ViewBag.ExperiseImage = "";
                ViewBag.heading = "Add Our Expertise";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.OurExpertises.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.ExpertiseName = data.ExpertiseName;
                        ViewBag.Description = data.Description;
                        ViewBag.ExperiseImage = data.ExperiseImage;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Our Expertise";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> OurExpirtise(ExperiseDTO model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                bool check = await _ICrmrpo.AddAndUpdateOurExpertise(model);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("OurExpirtise");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("OurExpirtise");
                    }
                }
                else
                {
                    return RedirectToAction("OurExpirtise");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteOurExpirtise(int id)
        {
            try
            {
                var dlt = _context.OurExpertises.Find(id);
                if (dlt != null)
                {
                    _context.OurExpertises.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("OurExpirtise");

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> OurStory(int id)
        {
            try
            {
                OurStoryDTO model = new OurStoryDTO();
                model.OurStoryList = _context.OurStories.OrderByDescending(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Tittle = "";
                ViewBag.Content = "";
                ViewBag.IsActive = "";
                ViewBag.Image = "";
                ViewBag.heading = "Add Our Story";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.OurStories.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Title;
                        ViewBag.Content = data.Content;
                        ViewBag.Image = data.Image;
                        ViewBag.IsActive = data.IsActive;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Our Story";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> OurStory(OurStoryDTO model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                bool check = await _ICrmrpo.AddAndUpdateOurStory(model, AddedBy);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("OurStory");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("OurStory");
                    }
                }
                else
                {
                    return RedirectToAction("OurStory");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteOurStory(int id)
        {
            try
            {
                var dlt = _context.OurStories.Find(id);
                if (dlt != null)
                {
                    _context.OurStories.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("OurStory");

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> RequestDemo(int id)
        {
            try
            {
                RequestDemoDto model = new RequestDemoDto();
                model.RequestDemoList = _context.RequestDemos.OrderByDescending(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Tittle = "";
                ViewBag.Content = "";
                ViewBag.IsActive = "";
                ViewBag.Image = "";
                ViewBag.heading = "Add Request Demo";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.RequestDemos.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Title;
                        ViewBag.Content = data.Content;
                        ViewBag.Image = data.Image;
                        ViewBag.IsActive = data.IsActive;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Request Demo";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> RequestDemo(RequestDemoDto model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                bool check = await _ICrmrpo.AddAndUpdateRequestDemo(model, AddedBy);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("RequestDemo");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("RequestDemo");
                    }
                }
                else
                {
                    return RedirectToAction("RequestDemo");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteRequestDemo(int id)
        {
            try
            {
                var dlt = _context.RequestDemos.Find(id);
                if (dlt != null)
                {
                    _context.RequestDemos.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("RequestDemo");

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> OurCoreValues(int id)
        {
            try
            {
                OurCoreValuesDto model = new OurCoreValuesDto();
                model.OurCoreValueList = _context.OurCoreValues.OrderByDescending(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Tittle = "";
                ViewBag.Content = "";
                ViewBag.IsActive = "";
                ViewBag.Image = "";
                ViewBag.heading = "Add OurCore Value";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.OurCoreValues.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Title;
                        ViewBag.Content = data.Content;
                        ViewBag.Image = data.Image;
                        ViewBag.IsActive = data.IsActive;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update OurCore Value";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> OurCoreValues(OurCoreValuesDto model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                bool check = await _ICrmrpo.AddAndUpdateOurCoreValues(model, AddedBy);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("OurCoreValues");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("OurCoreValues");
                    }
                }
                else
                {
                    return RedirectToAction("OurCoreValues");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteOurCoreValues(int id)
        {
            try
            {
                var dlt = _context.OurCoreValues.Find(id);
                if (dlt != null)
                {
                    _context.OurCoreValues.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("OurCoreValues");

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Featurebenifits(int id)
        {
            try
            {
                FeaturebenifitsDto model = new FeaturebenifitsDto();
                model.FeaturebenifitList = _context.Featurebenifits.OrderByDescending(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Tittle = "";
                ViewBag.Content = "";
                ViewBag.IsActive = "";
                ViewBag.Image = "";
                ViewBag.heading = "Add Features & Benefits";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.Featurebenifits.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Title;
                        ViewBag.Content = data.Content;
                        ViewBag.Image = data.Image;
                        ViewBag.IsActive = data.IsActive;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Features & Benefits";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Featurebenifits(FeaturebenifitsDto model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                bool check = await _ICrmrpo.AddAndUpdateFeaturebenifits(model, AddedBy);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("Featurebenifits");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("Featurebenifits");
                    }
                }
                else
                {
                    return RedirectToAction("Featurebenifits");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteFeaturebenifits(int id)
        {
            try
            {
                var dlt = _context.Featurebenifits.Find(id);
                if (dlt != null)
                {
                    _context.Featurebenifits.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("Featurebenifits");

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OurTutorial(int id)
        {
            try
            {
                TutorialDTO model = new TutorialDTO();
                model.OurTutorials = _context.OurTutorials.OrderByDescending(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Tittle = "";
                ViewBag.Description = "";
                ViewBag.VedioURL = "";
                ViewBag.IsActive = "";
                ViewBag.heading = "Add Our Tutorial";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.OurTutorials.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Title;
                        ViewBag.Description = data.Description;
                        ViewBag.VedioURL = data.VedioUrl;
                        ViewBag.IsActive = data.IsActive;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Our Tutorial";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> OurTutorial(TutorialDTO model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                bool check = await _ICrmrpo.AddAndUpdateOurTutorial(model, AddedBy);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("OurTutorial");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("OurTutorial");
                    }
                }
                else
                {
                    return RedirectToAction("OurStory");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteOurTutorial(int id)
        {
            try
            {
                var dlt = _context.OurTutorials.Find(id);
                if (dlt != null)
                {
                    _context.OurTutorials.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("OurTutorial");

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> CaseStudies(int id)
        {
            try
            {
                CaseStudiesDTO model = new CaseStudiesDTO();
                model.CaseStudyList = _context.CaseStudies.OrderByDescending(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.Tittle = "";
                ViewBag.Description = "";
                ViewBag.IsActive = "";
                ViewBag.Image = "";
                ViewBag.heading = "Add Case Studies";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.CaseStudies.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Tittle = data.Title;
                        ViewBag.Description = data.Description;
                        ViewBag.Image = data.Image;
                        ViewBag.IsActive = data.IsActive;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Case Studies";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> CaseStudies(CaseStudiesDTO model)
        {
            try
            {
                bool check = await _ICrmrpo.AddAndUpdateCaseStudies(model);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("CaseStudies");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("CaseStudies");
                    }
                }
                else
                {
                    return RedirectToAction("CaseStudies");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteCaseStudies(int id)
        {
            try
            {
                var dlt = _context.CaseStudies.Find(id);
                if (dlt != null)
                {
                    _context.CaseStudies.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("CaseStudies");

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> PricingPlan(int id)
        {
            try
            {
                //List<PricingPlan> model = _context.PricingPlans.OrderByDescending(x => x.Id).ToList();
                PricingPlanDTO model = new PricingPlanDTO();
                model.PricingPlansList = _context.PricingPlans.OrderByDescending(x => x.Id).ToList();
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.PlanName = "";
                ViewBag.Price = "";
                ViewBag.tittle = "";
                ViewBag.Support = "";
                ViewBag.AnnulPrice = "";
                ViewBag.AnnulPriceInPercentage = "";
                ViewBag.Description = "";
                ViewBag.IsActive = "";
                ViewBag.Image = "";
                ViewBag.AnnulPriceInPercentage = "";
                ViewBag.AnnulPrice = "";
                ViewBag.heading = "Add Pricing Plan";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.PricingPlans.Find(iId);

                    var existingfeatures = await _context.PricingPlanFeatures
                        .Where(s => s.PricingPlanId == data.Id)
                        .ToListAsync();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.PlanName = data.PlanName;
                        ViewBag.Price = data.Price;
                        ViewBag.tittle = data.Title;
                        ViewBag.Support = data.Support;
                        ViewBag.AnnulPrice = data.AnnulPrice;
                        ViewBag.AnnulPriceInPercentage = data.AnnulPriceInPercentage;
                        ViewBag.IsActive = data.IsActive;
                        ViewBag.Image = data.Image;
                        ViewBag.AnnulPriceInPercentage = data.AnnulPriceInPercentage;
                        ViewBag.AnnulPrice = data.AnnulPrice;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Pricing Plan";

                        model.PlanFeatures = existingfeatures.Select(s => new PlanFeature
                        {
                            Feature = s.Feature
                        }).ToList();
                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddPricingPlans(PricingPlanDTO model)
        {
            try
            {
                bool check = await _ICrmrpo.AddAndUpdatePricingPlan(model);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("PricingPlan");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("PricingPlan");
                    }
                }
                else
                {
                    return RedirectToAction("PricingPlan");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeletePricingPlan(int id)
        {
            try
            {
                var dlt = _context.PricingPlans.Find(id);
                if (dlt != null)
                {
                    _context.PricingPlans.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("PricingPlan");

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> OtherServices(int id)
        {
            try
            {
                List<OtherService> model = _context.OtherServices.OrderByDescending(x => x.Id).ToList();

                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.ServiceName = "";
                ViewBag.Description = "";
                ViewBag.IsActive = "";
                ViewBag.heading = "Add Other Services";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.OtherServices.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.ServiceName = data.ServiceName;
                        ViewBag.Description = data.Description;
                        ViewBag.IsActive = data.IsActive;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Other Services";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> OtherServices(OtherService model)
        {
            try
            {
                bool check = await _ICrmrpo.AddAndUpdateOtherService(model);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("OtherServices");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("OtherServices");
                    }
                }
                else
                {
                    return RedirectToAction("OtherServices");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteOtherServices(int id)
        {
            try
            {
                var dlt = _context.OtherServices.Find(id);
                if (dlt != null)
                {
                    _context.OtherServices.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("OtherServices");

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            try
            {
                List<ContactU> model = _context.ContactUs.OrderByDescending(x => x.Id).ToList();
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> MissionVisions(int id)
        {
            try
            {
                MissionVisionDTO model = new MissionVisionDTO();
                model.MissionVisions = _context.MissionVisions.OrderByDescending(x => x.Id).ToList();
                ;
                int iId = (int)(id == null ? 0 : id);
                ViewBag.id = 0;
                ViewBag.MissionVisionName = "";
                ViewBag.Description = "";
                ViewBag.IsActive = "";
                ViewBag.Image = "";
                ViewBag.heading = "Add Mission & Vision";
                ViewBag.btnText = "SAVE";
                if (iId != null && iId != 0)
                {
                    var data = _context.MissionVisions.Find(iId);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.MissionVisionName = data.MissionVisionName;
                        ViewBag.Description = data.Description;
                        ViewBag.Image = data.Image;
                        ViewBag.IsActive = data.IsActive;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Mission & Vision";

                    }
                }

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> MissionVisions(MissionVisionDTO model)
        {
            try
            {
                bool check = await _ICrmrpo.AddAndUpdateMissionVisions(model);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("MissionVisions");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("MissionVisions");
                    }
                }
                else
                {
                    return RedirectToAction("MissionVisions");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> DeleteMissionVisions(int id)
        {
            try
            {
                var dlt = _context.MissionVisions.Find(id);
                if (dlt != null)
                {
                    _context.MissionVisions.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("MissionVisions");

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDemoRequests()
        {
            try
            {
                List<DemoRequest> model = _context.DemoRequests.OrderByDescending(x => x.Id).ToList();
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetHelpCenters()
        {
            try
            {
                List<HelpCenter> model = _context.HelpCenters.OrderByDescending(x => x.Id).ToList();
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetPaymentStatusData()
        {
            using (var context = new admin_NDCrMContext())
            {
                int currentYear = DateTime.Now.Year;

                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins
                    .Where(x => x.Id == userId)
                    .FirstOrDefaultAsync();

                if (adminLogin == null)
                {
                    return BadRequest("Invalid user.");
                }

                var data = await (from invoice in context.CustomerInvoices
                                  join mode in context.Paymentmodes
                                  on invoice.Paymentstatus equals mode.Id
                                  where invoice.CreatedDate.HasValue
                                        && invoice.CreatedDate.Value.Year == currentYear
                                        && invoice.VendorId == adminLogin.Vendorid
                                  group new { invoice, mode } by new
                                  {
                                      Month = invoice.CreatedDate.Value.Month,
                                      invoice.Paymentstatus,
                                      mode.PaymentType,
                                      Invoice = invoice.InvoiceNumber,
                                      PaidAmount = invoice.PaidAmount,
                                      UnPaidAmount = invoice.DueAmount,
                                      overdueAmount = invoice.Taxamount,
                                  } into g
                                  select new
                                  {
                                      Month = g.Key.Month,
                                      PaymentStatus = g.Key.Paymentstatus,
                                      PaymentMode = g.Key.PaymentType,
                                      Count = g.Select(x => x.invoice.InvoiceNumber).Distinct().Count(),
                                      PaidAmount = g.Select(x => x.invoice.PaidAmount).Distinct().Sum(),
                                      UnPaidAmount = g.Select(x => x.invoice.DueAmount).Distinct().Sum(),
                                      overdueAmount = g.Select(x => x.invoice.Taxamount).Sum()
                                  }).ToListAsync();

                var groupedData = data
                    .GroupBy(x => new { x.Month, x.PaymentMode })
                    .Select(g => new
                    {
                        Month = g.Key.Month,
                        PaymentMode = g.Key.PaymentMode,
                        Count = g.Sum(x => x.Count),
                        PaidAmount = g.Sum(x => x.PaidAmount),
                        UnPaidAmount = g.Sum(x => x.UnPaidAmount),
                        overdueAmount = g.Sum(x => x.overdueAmount)
                    }).ToList();

                var monthNames = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

                var monthlyData = new List<object>();
                for (int i = 1; i <= 12; i++)
                {
                    var monthlyPaymentData = groupedData.Where(x => x.Month == i).ToList();
                    var paymentModes = monthlyPaymentData.ToDictionary(x => x.PaymentMode, x => x.Count);

                    var monthName = monthNames[i - 1];

                    decimal paidTotalAmount = (decimal)monthlyPaymentData.Sum(x => x.PaidAmount);
                    decimal unpaidTotalAmount = (decimal)monthlyPaymentData.Sum(x => x.UnPaidAmount);
                    decimal overdueTotalAmount = (decimal)monthlyPaymentData.Sum(x => x.overdueAmount);

                    monthlyData.Add(new
                    {
                        Month = monthName,
                        Paid = paymentModes.GetValueOrDefault("Paid", 0),
                        Unpaid = paymentModes.GetValueOrDefault("Unpaid", 0),
                        Partial = paymentModes.GetValueOrDefault("Partial", 0),
                        Canceled = paymentModes.GetValueOrDefault("Canceled", 0),
                        PaidAmt = paidTotalAmount,
                        UnPaidAmt = unpaidTotalAmount,
                        overdueAmount = overdueTotalAmount
                    });
                }

                return Ok(monthlyData);
            }
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceTaxmaster(int id)
        {
            try
            {
                var model = new InvoiceChargesmasterDto();


                var result = from charges in _context.InvoiceChargesmasters
                             join vendor in _context.VendorRegistrations
                             on charges.Vendorid equals vendor.Id
                             select new InvoiceChargesmasterDto
                             {
                                 Id = charges.Id,
                                 Vendorid = vendor.CompanyName,
                             };
                model.taxlist = result.ToList();

                ViewBag.vendorid = _context.VendorRegistrations
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.CompanyName,
                    })
                    .ToList();

                // Initialize ViewBag values
                ViewBag.id = 0;
                ViewBag.Vendor = "";
                ViewBag.IsActive = "";
                ViewBag.heading = "Add Tax";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = await _context.InvoiceChargesmasters.FindAsync(id);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Vendor = data.Vendorid;
                        ViewBag.IsActive = data.Isactive;
                        ViewBag.btnText = "UPDATE";
                        ViewBag.heading = "Update Tax";
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> InvoiceTaxmaster(InvoiceChargesmaster model)
        {
            try
            {
                bool check = await _ICrmrpo.AddAndUpdateInvoiceChargesmaster(model);
                if (check)
                {
                    if (model.Id == 0)
                    {
                        TempData["msg"] = "ok";
                        return RedirectToAction("InvoiceTaxmaster");
                    }
                    else
                    {
                        TempData["msg"] = "updok";
                        return RedirectToAction("InvoiceTaxmaster");
                    }
                }
                else
                {
                    return RedirectToAction("InvoiceTaxmaster");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteInvoiceChargesmaster(int id)
        {
            try
            {
                var dlt = _context.InvoiceChargesmasters.Find(id);
                if (dlt != null)
                {
                    _context.InvoiceChargesmasters.Remove(dlt);
                    _context.SaveChanges();
                }
                TempData["msg"] = "dltok";
                return RedirectToAction("InvoiceTaxmaster");

            }
            catch (Exception)
            {

                throw;
            }
        }
        public static string getMonthName(int monthValue)
        {
            string monthName;

            switch (monthValue)
            {
                case 1:
                    monthName = "January";
                    break;
                case 2:
                    monthName = "February";
                    break;
                case 3:
                    monthName = "March";
                    break;
                case 4:
                    monthName = "April";
                    break;
                case 5:
                    monthName = "May";
                    break;
                case 6:
                    monthName = "June";
                    break;
                case 7:
                    monthName = "July";
                    break;
                case 8:
                    monthName = "August";
                    break;
                case 9:
                    monthName = "September";
                    break;
                case 10:
                    monthName = "October";
                    break;
                case 11:
                    monthName = "November";
                    break;
                case 12:
                    monthName = "December";
                    break;
                default:
                    monthName = "Invalid Month";
                    break;
            }

            return monthName;
        }
    }
}
