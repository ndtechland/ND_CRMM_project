﻿using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using CRM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Components.RenderTree;
using System.IO;
using Org.BouncyCastle.Asn1.Mozilla;
using SelectPdf;
using static CRM.Controllers.Employee;
using System.Globalization;
using System.Data;
using MimeKit.Encodings;
using Fingers10.ExcelExport.ActionResults;
using System.Net.Mail;
using MimeKit;
using Microsoft.AspNetCore.Hosting;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Diagnostics.Metrics;
using NuGet.Versioning;
using DocumentFormat.OpenXml.Drawing.Charts;
using DataTable = System.Data.DataTable;
using DocumentFormat.OpenXml.Office.CustomUI;
using Humanizer;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using DocumentFormat.OpenXml.InkML;
using CRM.IUtilities;
using Microsoft.VisualStudio.Services.Commerce;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.VisualStudio.Services.WebPlatform;
using Microsoft.Azure.Pipelines.WebApi;
using CRM.Models.APIDTO;
using Org.BouncyCastle.Asn1.Pkcs;
using CRM.Utilities;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using NLog;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Crmf;

using DinkToPdf;
using DinkToPdf.Contracts;
using StackExchange.Profiling.Internal;
using RestSharp;
using DocumentFormat.OpenXml.Office.Word;
using jsreport.AspNetCore;
using jsreport.Types;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using Org.BouncyCastle.Ocsp;
using Umbraco.Core;
using System.Text.RegularExpressions;
using System.Linq;
using CRM.Models.Jobcontext;

namespace CRM.Controllers
{
    public class Employee : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _IEmailService;
        private readonly IConverter _converter;

        public Employee(ICrmrpo _ICrmrpo, admin_NDCrMContext _context, IConfiguration configuration, IEmailService _IEmailService, IConverter _IConverter)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
            this._IEmailService = _IEmailService;
            _configuration = configuration;
            _converter = _IConverter;
        }
        [HttpGet, Route("Employee/EmployeeRegistration")]
        public async Task<IActionResult> EmployeeRegistration(string id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.userId = adminlogin.Vendorid;
                var checkvendor = await _context.VendorRegistrations.Where(x => x.Id == adminlogin.Vendorid).FirstOrDefaultAsync();

                ViewBag.WorkLocation = _context.Cities
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.City1
                })
                 .ToList();
                ViewBag.Cities = _context.Cities
               .Select(w => new SelectListItem
               {
                   Value = w.Id.ToString(),
                   Text = w.City1
               })
                .ToList();
                ViewBag.stes = _context.States
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.SName
                })
                 .ToList();
                ViewBag.Department = _context.DepartmentMasters.Where(x => x.AdminLoginId == Userid).Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.DepartmentName

                }).ToList();
                //Designation dropdown 
                ViewBag.Designation = _context.DesignationMasters.Where(x => x.AdminLoginId == Userid).Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.DesignationName
                }).ToList();
                //States dropdown
                ViewBag.States = _context.States.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.SName
                })
                 .ToList();
                //CustomerName dropdown
                ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                //Offerletter dropdown
                ViewBag.Offerletter = _context.Offerletters.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();
                ViewBag.officeshift = _context.Officeshifts.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Starttime} - {x.Endtime} - {x.ShiftTypeid}"
                }).ToList();
                if (checkvendor.SelectCompany == true)
                {
                    ViewBag.CustomerCompanyName = _context.CustomerRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = $"{x.CompanyName}"
                    }).ToList();
                }
                ViewBag.CheckSelectCompany = checkvendor.SelectCompany;
                ViewBag.Emp_Reg_Code = "";
                ViewBag.First_Name = "";
                ViewBag.Middle_Name = "";
                ViewBag.Last_Name = "";
                ViewBag.Date_Of_Joining = "";
                ViewBag.Work_Email = "";
                ViewBag.Gender_ID = "";
                ViewBag.Work_Location_ID = "";
                ViewBag.Designation_ID = "";
                ViewBag.Department_ID = "";
                ViewBag.CustomerID = "";
                ViewBag.Basic = "";
                ViewBag.AnnualCTC = "";
                ViewBag.HouseRentAllowance = "";
                ViewBag.TravellingAllowance = "";
                ViewBag.ESIC = "";
                ViewBag.EPF = "";
                ViewBag.MonthlyGrossPay = "";
                ViewBag.MonthlyCTC = "";
                ViewBag.Personal_Email_Address = "";
                ViewBag.Mobile_Number = "";
                ViewBag.Date_Of_Birth = "";
                ViewBag.Age = "";
                ViewBag.Father_Name = "";
                ViewBag.PAN = "";
                ViewBag.Address_Line_1 = "";
                ViewBag.Address_Line_2 = "";
                ViewBag.City = "";
                ViewBag.State_ID = "";
                ViewBag.Pincode = "";
                ViewBag.Account_Holder_Name = "";
                ViewBag.Bank_Name = "";
                ViewBag.Account_Number = "";
                ViewBag.Re_Enter_Account_Number = "";
                ViewBag.IFSC = "";
                ViewBag.Account_Type_ID = "";
                ViewBag.EPF_Number = "";
                ViewBag.Deduction_Cycle = "";
                ViewBag.Employee_Contribution_Rate = "";
                ViewBag.Professionaltax = "";
                ViewBag.nominee = "";
                ViewBag.servicecharge = "";
                ViewBag.specialallowance = "";
                ViewBag.gross = "";
                ViewBag.Amount = "";
                ViewBag.Tdspercentage = "";
                ViewBag.Offerletters = "";
                ViewBag.shifttype = "";
                ViewBag.statesy = "";
                ViewBag.Conveyanceallowance = "";
                ViewBag.MedicalAllowance = "";
                ViewBag.VariablePay = "";
                ViewBag.EmployerContribution = "";
                ViewBag.tdsvalue = "";
                ViewBag.Basicpercentage = "";
                ViewBag.HRApercentage = "";
                ViewBag.Conveyancepercentage = "";
                ViewBag.Medicalpercentage = "";
                ViewBag.Variablepercentage = "";
                ViewBag.EmployerContributionpercentage = "";
                ViewBag.EPfpercentage = "";
                ViewBag.Esipercentage = "";
                ViewBag.CustomerCompany = "";
                ViewBag.btnText = "SAVE";

                if (id != null)
                {
                    var empid = await _context.EmployeeRegistrations
                                               .Where(x => x.EmployeeId == id && x.Vendorid == adminlogin.Vendorid)
                                               .FirstOrDefaultAsync();

                    if (empid != null)
                    {
                        DataTable dtEmployeeRecord = _ICrmrpo.GetEmployDetailById(id, Userid);

                        if (dtEmployeeRecord != null && dtEmployeeRecord.Rows.Count > 0)
                        {
                            DataRow row = dtEmployeeRecord.Rows[0] as DataRow;
                            if (row != null)
                            {

                                ViewBag.First_Name = row["First_Name"].ToString();
                                ViewBag.Middle_Name = row["Middle_Name"].ToString();
                                ViewBag.Last_Name = row["Last_Name"].ToString();
                                ViewBag.Date_Of_Joining = ((DateTime)row["Date_Of_Joining"]).ToString("yyyy-MM-dd");
                                ViewBag.Work_Email = row["Work_Email"].ToString();
                                ViewBag.Gender_ID = row["Gender_ID"].ToString();
                                ViewBag.Work_Location_ID = row["Work_Location_ID"].ToString();
                                ViewBag.Designation_ID = row["Designation_ID"].ToString();
                                ViewBag.Department_ID = row["Department_ID"].ToString();
                                ViewBag.CustomerID = row["Vendorid"].ToString();
                                ViewBag.Basic = row["Basic"].ToString();
                                ViewBag.AnnualCTC = row["AnnualCTC"].ToString();
                                ViewBag.HouseRentAllowance = row["HouseRentAllowance"].ToString();
                                ViewBag.TravellingAllowance = row["TravellingAllowance"].ToString();
                                ViewBag.ESIC = row["ESIC"].ToString();
                                ViewBag.EPF = row["EPF"].ToString();
                                ViewBag.MonthlyGrossPay = row["MonthlyGrossPay"].ToString();
                                ViewBag.MonthlyCTC = row["MonthlyCTC"].ToString();
                                ViewBag.Personal_Email_Address = row["Personal_Email_Address"].ToString();
                                ViewBag.Mobile_Number = row["Mobile_Number"].ToString();
                                ViewBag.Date_Of_Birth = ((DateTime)row["Date_Of_Birth"]).ToString("yyyy-MM-dd");
                                ViewBag.Age = row["Age"].ToString();
                                ViewBag.Father_Name = row["Father_Name"].ToString();
                                ViewBag.PAN = row["PAN"].ToString();
                                ViewBag.Address_Line_1 = row["Address_Line_1"].ToString();
                                ViewBag.Address_Line_2 = row["Address_Line_2"].ToString();
                                ViewBag.City = row["City"].ToString();
                                ViewBag.State_ID = row["State_ID"].ToString();
                                ViewBag.Pincode = row["Pincode"].ToString();
                                ViewBag.Account_Holder_Name = row["Account_Holder_Name"].ToString();
                                ViewBag.Bank_Name = row["Bank_Name"].ToString();
                                ViewBag.Account_Number = row["Account_Number"].ToString();
                                ViewBag.Re_Enter_Account_Number = row["Re_Enter_Account_Number"].ToString();
                                ViewBag.IFSC = row["IFSC"].ToString();
                                ViewBag.Account_Type_ID = row["Account_Type_ID"].ToString();
                                ViewBag.EPF_Number = row["EPF_Number"].ToString();
                                ViewBag.Deduction_Cycle = row["Deduction_Cycle"].ToString();
                                ViewBag.Employee_Contribution_Rate = row["Employee_Contribution_Rate"].ToString();
                                ViewBag.Professionaltax = row["Professionaltax"].ToString();
                                ViewBag.nominee = row["nominee"].ToString();
                                ViewBag.servicecharge = row["servicecharge"].ToString();
                                ViewBag.specialallowance = row["SpecialAllowance"].ToString();
                                ViewBag.gross = row["gross"].ToString();
                                ViewBag.Amount = 0;//row["Amount"].ToString();
                                ViewBag.Tdspercentage = 0;//row["tdspercentage"].ToString();
                                ViewBag.Offerletters = row["offerletterid"].ToString();
                                ViewBag.statesy = row["stateId"].ToString();
                                ViewBag.shifttype = row["officeshiftTypeid"].ToString();
                                ViewBag.Conveyanceallowance = row["conveyanceallowance"].ToString();
                                ViewBag.FixedAllowance = row["FixedAllowance"].ToString();
                                ViewBag.MedicalAllowance = row["Medical"].ToString();
                                ViewBag.VariablePay = row["VariablePay"].ToString();
                                ViewBag.EmployerContribution = row["EmployerContribution"].ToString();
                                ViewBag.tdsvalue = row["tdsvalue"].ToString();
                                ViewBag.Basicpercentage = row["Basicpercentage"].ToString();
                                ViewBag.HRApercentage = row["HRApercentage"].ToString();
                                ViewBag.Conveyancepercentage = row["Conveyancepercentage"].ToString();
                                ViewBag.Medicalpercentage = row["Medicalpercentage"].ToString();
                                ViewBag.Variablepercentage = row["Variablepercentage"].ToString();
                                ViewBag.EmployerContributionpercentage = row["EmployerContributionpercentage"].ToString();
                                ViewBag.EPfpercentage = row["EPfpercentage"].ToString();
                                ViewBag.Esipercentage = row["Esipercentage"].ToString();
                                ViewBag.CustomerCompany = row["CustomerCompanyid"].ToString();
                                ViewBag.Emp_Reg_Code = id;
                                ViewBag.btnText = "UPDATE";

                            }

                        }
                    }
                    else
                    {
                        TempData["Message"] = "Invalid Employee Id";
                        return RedirectToAction("PreviewEmployeelist");
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
        public async Task<IActionResult> EmployeeRegistration(EmpMultiform model)
        {
            try
            {
                if (model == null)
                {
                    TempData["Message"] = "Invalid data received. Please try again.";
                    return View();
                }

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                string Mode = "INS";
                string Empid = "";

                if (!string.IsNullOrEmpty(model.Emp_Reg_ID))
                {
                    Mode = "UPD";
                    Empid = model.Emp_Reg_ID;
                }
                else
                {
                    Empid = GenerateEmployeeId();
                    model.EmployeeId = Empid;

                    var existingEmployee = _context.EmployeeRegistrations.FirstOrDefault(x => x.WorkEmail == model.WorkEmail);
                    if (existingEmployee != null)
                    {
                        TempData["Message"] = "WorkEmail already exists";
                        return View();
                    }
                }

                if (Userid != null)
                {
                    if (Mode == "INS")
                    {
                        TempData["Message"] = "ok";
                        var response = await _ICrmrpo.EmpRegistration(model, Mode, Empid, Userid);
                        ModelState.Clear();
                        return RedirectToAction("EmployeeRegistration");
                    }
                    else if (Mode == "UPD")
                    {
                        TempData["Message"] = "updok";
                        var response = await _ICrmrpo.EmpRegistration(model, Mode, Empid, Userid);
                        ModelState.Clear();
                        return RedirectToAction("EmployeeRegistration");
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
                return View();
            }
        }

        //[HttpGet, Route("Employee/Employeelist")]
        //public async Task<IActionResult> Employeelist()
        //{
        //    try
        //    {

        //        List<EmployeeImportExcel> response = new List<EmployeeImportExcel>();
        //        if (HttpContext.Session.GetString("UserName") != null)
        //        {
        //            string userIdString = HttpContext.Session.GetString("UserId");
        //            var adminlogin = await _context.AdminLogins.Where(x => x.Id == Convert.ToInt16(userIdString)).FirstOrDefaultAsync();
        //            ViewBag.officeshift = _context.Officeshifts.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
        //            {
        //                Value = x.Id.ToString(),
        //                Text = $"{x.Starttime} - {x.Endtime} - {x.ShiftTypeid}"
        //            }).ToList();
        //            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int id))
        //            {
        //                if (id == 1)
        //                {
        //                    response = await _ICrmrpo.EmployeeList();
        //                }
        //                else
        //                {
        //                    response = await _ICrmrpo.CustomerEmployeeList(id);
        //                    foreach (var item in response)
        //                    {
        //                        ViewBag.shiftlist = item.ShiftTypeid;
        //                        ViewBag.Isactive = item.Isactive;
        //                    }
        //                }

        //                ViewBag.UserName = HttpContext.Session.GetString("UserName");
        //                return View(response);
        //            }
        //            response = await _ICrmrpo.EmployeeList();
        //            ViewBag.UserName = HttpContext.Session.GetString("UserName");
        //            return View(response);
        //        }

        //        return RedirectToAction("Login", "Admin");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error: " + ex.Message);
        //    }
        //}
        [HttpGet, Route("Employee/getEmployeedata")]
        public async Task<IActionResult> Employeelist(string empid)
        {
            try
            {
                List<EmployeeImportExcel> response = new List<EmployeeImportExcel>();
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    string userIdString = HttpContext.Session.GetString("UserId");
                    var adminlogin = await _context.AdminLogins
                        .Where(x => x.Id == Convert.ToInt16(userIdString))
                        .FirstOrDefaultAsync();

                    ViewBag.officeshift = _context.Officeshifts
                        .Where(x => x.Vendorid == adminlogin.Vendorid)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = $"{x.Starttime} - {x.Endtime} - {x.ShiftTypeid}"
                        })
                        .ToList();

                    if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int id))
                    {
                        if (id == 1)
                        {
                            response = await _ICrmrpo.EmployeeList();
                        }
                        else
                        {
                            response = await _ICrmrpo.CustomerEmployeeList((int)adminlogin.Vendorid);
                        }
                        if (!string.IsNullOrEmpty(empid))
                        {
                            response = response.Where(e => e.EmpRegId == empid).ToList();
                        }

                        foreach (var item in response)
                        {
                            ViewBag.shiftlist = item.ShiftTypeid;
                            ViewBag.Isactive = item.Isactive;
                        }

                        ViewBag.UserName = HttpContext.Session.GetString("UserName");
                        return View(response);
                    }
                }

                return RedirectToAction("Login", "Admin");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
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

        public async Task<IActionResult> DeleteEmployee(string Emp_Reg_ID)
        {
            try
            {
                var data = _context.EmployeeRegistrations.SingleOrDefault(e => e.EmployeeId == Emp_Reg_ID);
                var data1 = _context.EmployeePersonalDetails.SingleOrDefault(e => e.EmpRegId == Emp_Reg_ID);
                var data2 = _context.EmployeeSalaryDetails.SingleOrDefault(e => e.EmployeeId == Emp_Reg_ID);
                var data3 = _context.EmployeeBankDetails.SingleOrDefault(e => e.EmpId == Emp_Reg_ID);

                if (data != null && data1 != null && data2 != null && data3 != null)
                {
                    data.IsDeleted = true;
                    data1.IsDeleted = true;
                    data2.IsDeleted = true;
                    data3.IsDeleted = true;
                    _context.SaveChanges();
                }
                TempData["Message"] = "dltok";
                return RedirectToAction("PreviewEmployeelist");
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
                if (data != null)
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
        public JsonResult Edit(int id)
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
            var department = _context.DepartmentMasters.ToList();
            var MonthlyCTC = _context.EmployeeSalaryDetails.Where(x => x.EmployeeId == data.EmployeeId).FirstOrDefault();
            var result = new
            {
                Data = data,
                Gender = gender,
                Worklocation = worklocation,
                Designation = designation,
                Department = department,
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
        [HttpGet]
        [Route("Employee/salarydetail")]
        public async Task<IActionResult> salarydetail(int month = 0, int year = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var attt = await _context.Attendancedays
     .Where(x => x.Vendorid == adminlogin.Vendorid)
     .FirstOrDefaultAsync();

                ViewBag.Nodays = attt == null || string.IsNullOrEmpty(attt.Nodays)
                    ? 0
                    : int.Parse(attt.Nodays);

                string AddedBy = HttpContext.Session.GetString("UserName");
                TempData["UserName"] = AddedBy;
                if (!string.IsNullOrEmpty(AddedBy))
                {
                    HttpContext.Session.SetString("UserName", AddedBy);
                }

                var response = await _ICrmrpo.salarydetail(Userid);
                decimal total = 0.00M;
                foreach (var item in response)
                {
                    total += (decimal)item.Grosspay;
                }

                ViewBag.TotalAmmount = (decimal)total;
                ViewBag.CustomerName = _context.CustomerRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }

        [HttpPost]
        public async Task<JsonResult> Empattendance(List<Empattendance> customers, int month)
        {
            bool isActive = false;

            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var attendanceDays = await _context.Attendancedays.FirstOrDefaultAsync(x => x.Vendorid == adminlogin.Vendorid);
                decimal noOfDays = 0;
                noOfDays = Convert.ToDecimal(attendanceDays.Nodays);
                foreach (Empattendance empattendance in customers)
                {
                    var months = await _context.Empattendances.Where(x => x.Month == empattendance.Month && x.Year == empattendance.Year && x.EmployeeId == empattendance.EmployeeId).ToListAsync();
                    if (months.Count > 0)
                    {
                        isActive = true;
                    }
                }
                if (!isActive)
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {

                            foreach (var item in customers)
                            {
                                var ctc = await _context.EmployeeSalaryDetails
                                    .Where(x => x.EmployeeId == item.EmployeeId)
                                    .FirstOrDefaultAsync();
                                if (ctc.Incentive != null && ctc.TravellingAllowance != null)
                                {
                                    if (ctc.Incentive != null && ctc.Incentive >= 0 && ctc.TravellingAllowance != null && ctc.TravellingAllowance >= 0)
                                    {
                                        ctc.Incentive = 0;
                                        ctc.TravellingAllowance = 0;
                                        await _context.SaveChangesAsync();
                                    }
                                }

                                decimal totalsalary = Math.Round((decimal)(ctc.AnnualCtc ?? 0) / 12, 2);
                                decimal totalBasicsalary = Math.Round((((decimal)(ctc.Basic) / noOfDays) * (decimal)item.Attendance) / 12, 2);
                                decimal totalhra = Math.Round((((decimal)(ctc.HouseRentAllowance) / noOfDays) * (decimal)item.Attendance) / 12, 2);
                                decimal totalSpecialAllowance = Math.Round((((decimal)(ctc.SpecialAllowance ?? 0) / noOfDays) * (decimal)(item.Attendance ?? 0)) / 12, 2);
                                decimal totalConveyanceallowance = Math.Round((((decimal)(ctc.Conveyanceallowance ?? 0) / noOfDays) * (decimal)(item.Attendance ?? 0)) / 12, 2);
                                decimal totalMedicalAllowance = Math.Round((((decimal)(ctc.Medical ?? 0) / noOfDays) * (decimal)(item.Attendance ?? 0)) / 12, 2);
                                decimal totalVariablePay = Math.Round((((decimal)(ctc.VariablePay ?? 0) / noOfDays) * (decimal)(item.Attendance ?? 0)) / 12, 2);
                                decimal totalProfessionaltax = Math.Round((((decimal)(ctc.Professionaltax ?? 0) / noOfDays) * (decimal)(item.Attendance ?? 0)) / 12, 2);
                                decimal totalTds = Math.Round((((decimal)(ctc.Tdsvalue ?? 0) / noOfDays) * (decimal)(item.Attendance ?? 0)) / 12, 2);

                                decimal checknum = noOfDays - (decimal)item.Attendance;
                                if (item.Id != 0)
                                {
                                    Empattendance emp = new Empattendance
                                    {
                                        EmployeeId = item.EmployeeId,
                                        Month = item.Month,
                                        Year = item.Year,
                                        Attendance = item.Attendance,
                                        Entry = DateTime.Now,
                                        Incentive = item.Incentive,
                                        TravellingAllowance = item.TravellingAllowance,
                                        GenerateSalary = item.GenerateSalary /*- totalMedicalAllowance*/,
                                        Lop = item.Lop /*(checknum > 0) ? Math.Round(((totalsalary / noOfDays) * checknum), 2) : 0*/,
                                        EmpEpfvalue = item.EmpEpfvalue,
                                        EmpEsivalue = item.EmpEsivalue,
                                        Basicsalary = totalBasicsalary,
                                        Hra = totalhra,
                                        SpecialAllowance = totalSpecialAllowance,
                                        Conveyanceallowance = totalConveyanceallowance,
                                        MedicalAllowance = totalMedicalAllowance,
                                        VariablePay = totalVariablePay,
                                        Professionaltax = totalProfessionaltax,
                                        Tds = totalTds,
                                    };

                                    _context.Empattendances.Add(emp);
                                    await _context.SaveChangesAsync();
                                }
                            }

                            transaction.Commit();

                            foreach (var item in customers)
                            {
                                SendPDF(item.Id, (int)item.Month, (int)item.Year);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                            return Json(new { success = false, message = "Error occurred while saving data." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                return Json(new { success = false, message = "Error occurred while checking existing data." });
            }
            return Json(new { success = true, message = "Data saved successfully.", Data = isActive });
        }
        [HttpGet, Route("Employee/GenerateSalary")]
        public async Task<IActionResult> GenerateSalary()
        {
            try
            {

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();

                GenerateSalary salary = new GenerateSalary();
                salary.GeneratedSalaries = await _ICrmrpo.GenerateSalary(Userid);
                if (salary.GeneratedSalaries.Count > 0)
                {
                    foreach (var item in salary.GeneratedSalaries)
                    {
                        item.SalarySlipName = "" + getMonthName(item.Month, true) + "_" + item.Year + " ";
                    }

                    return View(salary);
                }
                else
                {
                    //TempData["Message"] = "No data found for the selected month and year.";
                    return View();
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
        [Route("Employee/SalarySlipInPDF")]
        public async Task<IActionResult> SalarySlipInPDF(int id, int month)
        {
            try
            {
                string AddedBy = HttpContext.Session.Id;


                var result = await (from emp in _context.EmployeeRegistrations
                                    join empsalary in _context.EmployeeSalaryDetails on emp.EmployeeId equals empsalary.EmployeeId into empsalaryGroup
                                    from empsalary in empsalaryGroup.DefaultIfEmpty()
                                    join empbank in _context.EmployeeBankDetails on emp.EmployeeId equals empbank.EmpId into empbankGroup
                                    from empbank in empbankGroup.DefaultIfEmpty()
                                    join empatt in _context.Empattendances on emp.EmployeeId equals empatt.EmployeeId into empattGroup
                                    from empatt in empattGroup.DefaultIfEmpty()
                                    join worklocation in _context.Cities on emp.WorkLocationId equals worklocation.Id.ToString() into worklocationGroup
                                    from worklocation in worklocationGroup.DefaultIfEmpty()
                                    join designation in _context.DesignationMasters on emp.DesignationId equals designation.Id.ToString() into designationGroup
                                    from designation in designationGroup.DefaultIfEmpty()
                                        // join tds in _context.Professionaltaxes on emp.Vendorid equals tds.CustomerId into tdsGroup
                                        // from tds in tdsGroup.DefaultIfEmpty()
                                    join vrs in _context.VendorRegistrations on emp.Vendorid equals (int?)vrs.Id into vrsGroup
                                    from vrs in vrsGroup.DefaultIfEmpty()
                                    where emp.Id == id && empatt.Month == month
                                    select new SalarySlipDetails
                                    {
                                        Id = emp.Id,
                                        Employee_ID = emp.EmployeeId,
                                        First_Name = emp.FirstName,
                                        Address_Line_1 = worklocation.City1,
                                        Designation_Name = designation.DesignationName,
                                        Bank_Name = empbank.BankName,
                                        Account_Number = empbank.AccountNumber,
                                        EPF_Number = empbank.EpfNumber,
                                        Month = getMonthName((int)empatt.Month, false),
                                        Year = empatt.Year,
                                        Lop = empatt.Lop,
                                        Professionaltax = empsalary.Professionaltax,
                                        TravellingAllowance = empatt.TravellingAllowance,
                                        Esic = empatt.EmpEsivalue,
                                        Epf = empatt.EmpEpfvalue,
                                        Basic = empatt.Basicsalary,
                                        HouseRentAllowance = empatt.Hra,
                                        SpecialAllowance = empatt.SpecialAllowance,
                                        Conveyanceallowance = empatt.Conveyanceallowance,
                                        MA = empatt.MedicalAllowance,
                                        VariablePay = empatt.VariablePay,
                                        //Amount = tds.Amount,
                                        CompanyName = vrs.CompanyName,
                                        CompanyImage = vrs.CompanyImage,
                                        Companysignature = vrs.VendorSingature,
                                    }).FirstOrDefaultAsync();

                if (result != null)
                {
                    return View(result);
                }
                else
                {
                    return View();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public async Task<IActionResult> Employer(int id = 0)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                    EmployeerModelEPF model = new EmployeerModelEPF();
                    model.EmployeerEpflist = await _ICrmrpo.EmployerList(Userid);
                    ViewBag.DeductionCycle = "";
                    ViewBag.EmployerContributionRate = "";
                    ViewBag.EpfNumber = "";
                    ViewBag.BtnText = "SAVE";
                    if (id > 0)
                    {
                        var data = _ICrmrpo.GetEmployer(id);
                        if (data != null)
                        {
                            ViewBag.id = data.Id;
                            ViewBag.DeductionCycle = data.DeductionCycle;
                            ViewBag.EmployerContributionRate = data.EmployerContributionRate;
                            ViewBag.EpfNumber = data.EpfNumber;
                            ViewBag.BtnText = "UPDATE";
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Employer not found.";
                            return View();
                        }
                    }
                    if (model.EmployeerEpflist.Count() > 0)
                    {
                        return View(model);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No details found.";
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Employer(EmployeerModelEPF model)
        {
            try
            {

                int adminloginid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));


                var response = await _ICrmrpo.Employer(model, adminloginid);
                ModelState.Clear();
                return RedirectToAction("Employer");

            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        public IActionResult DocPDF(int id, int month)
        {
            try
            {
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                HtmlToPdf converter = new HtmlToPdf();
                string SlipURL = $"{schema}://{host}/Employee/SalarySlipInPDF?id={id}&&month={month}";
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }

                var result = (from emp in _context.EmployeeRegistrations
                              join empt in _context.Empattendances
                             on emp.EmployeeId equals empt.EmployeeId
                              where emp.Id == id && empt.Month == month
                              select new SalarySlipDetails
                              {
                                  Id = emp.Id,
                                  Employee_ID = emp.EmployeeId,
                                  First_Name = emp.FirstName,
                                  Email_Id = emp.WorkEmail,
                                  Month = getMonthName((int)month, true),
                                  Year = empt.Year,
                                  vendorid = emp.Vendorid,
                                  CompanyName = _context.VendorRegistrations.Where(x => x.Id == emp.Vendorid).Select(x => x.CompanyName).FirstOrDefault()

                              }).FirstOrDefault();
                string uniqueFileName = $"{result.Employee_ID}_{result.First_Name}_{result.Month}{result.Year}.pdf";
                string filePath = Path.Combine(wwwRootPath, uniqueFileName);
                doc.Save(filePath);
                byte[] pdf = System.IO.File.ReadAllBytes(filePath);
                doc.Close();

                if (result != null)
                {
                    string fileName = uniqueFileName;
                    string emailSubject = $"Salary Slip for {result.Month} {result.Year}";
                    string emailBody = $"Dear {result.First_Name},\n\n"
                 + $"Please find attached your salary slip for the month of {result.Month}, {result.Year}. "
                 + "If you have any questions or require any clarification, feel free to reach out.\n\n"
                 + "Thank you for your hard work and dedication. We truly appreciate your contribution to the team.\n\n"
                 + $"Best regards,\n{result.CompanyName}";


                    // string emailBody = $"Dear {result.First_Name} ({result.Employee_ID}), your salary slip for this {result.Month} has been generated and is attached to this email. Please review the details and contact HR if you have any questions.";
                    _IEmailService.SendEmailAsync(result.Email_Id, emailSubject, emailBody, pdf, fileName, "application/pdf", (int)result.vendorid);

                    return Json(new { success = true, message = "Salary Slip has been sent successfully.", fileName = fileName, pdf = Convert.ToBase64String(pdf) });

                }

                return Json(new { success = false, message = "Error: Employee not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        public void SendPDF(int id, int month, int Year)
        {
            try
            {
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                HtmlToPdf converter = new HtmlToPdf();
                string SlipURL = $"{schema}://{host}/Employee/SalarySlipInPDF?id={id}&&month={month}";
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }

                var result = (from emp in _context.EmployeeRegistrations
                              join empt in _context.Empattendances
                             on emp.EmployeeId equals empt.EmployeeId
                              where emp.Id == id && empt.Month == month
                              select new SalarySlipDetails
                              {
                                  Id = emp.Id,
                                  Employee_ID = emp.EmployeeId,
                                  First_Name = emp.FirstName,
                                  Email_Id = emp.WorkEmail,
                                  Month = getMonthName((int)month, true),
                                  Year = empt.Year,
                                  vendorid = emp.Vendorid,
                                  CompanyName = _context.VendorRegistrations.Where(x => x.Id == emp.Vendorid).Select(x => x.CompanyName).FirstOrDefault()
                              }).FirstOrDefault();
                string uniqueFileName = $"{result.Employee_ID}_{result.First_Name}_{result.Month}{result.Year}.pdf";
                string filePath = Path.Combine(wwwRootPath, uniqueFileName);
                doc.Save(filePath);
                byte[] pdf = System.IO.File.ReadAllBytes(filePath);
                doc.Close();
                string savedFileName = uniqueFileName;

                if (result == null)
                {
                    throw new Exception("Employee not found.");
                }
                var empAttendance = _context.Empattendances.FirstOrDefault(e => e.EmployeeId == result.Employee_ID && e.Month == month && e.Year == Year);
                if (empAttendance != null)
                {
                    empAttendance.SalarySlip = savedFileName;
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Attendance record not found for the employee.");
                }
                string Email_Subject = uniqueFileName;
                string Email_body = $"Dear {result.First_Name},\n\n"
                 + $"Please find attached your salary slip for the month of {result.Month}, {result.Year}. "
                 + "If you have any questions or require any clarification, feel free to reach out.\n\n"
                 + "Thank you for your hard work and dedication. We truly appreciate your contribution to the team.\n\n"
                 + $"Best regards,\n{result.CompanyName}";
                _IEmailService.SendEmailAsync(result.Email_Id, Email_Subject, Email_body, pdf, uniqueFileName, "application/pdf", (int)result.vendorid);

                Console.WriteLine($"Salary slip for Employee ID {result.Employee_ID} has been saved and the Empattendance table has been updated.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getMonthName(int monthValue, bool isShortName = false)
        {
            string monthName;

            switch (monthValue)
            {
                case 1:
                    monthName = isShortName ? "Jan" : "January";
                    break;
                case 2:
                    monthName = isShortName ? "Feb" : "February";
                    break;
                case 3:
                    monthName = isShortName ? "Mar" : "March";
                    break;
                case 4:
                    monthName = isShortName ? "Apr" : "April";
                    break;
                case 5:
                    monthName = isShortName ? "May" : "May";
                    break;
                case 6:
                    monthName = isShortName ? "Jun" : "June";
                    break;
                case 7:
                    monthName = isShortName ? "Jul" : "July";
                    break;
                case 8:
                    monthName = isShortName ? "Aug" : "August";
                    break;
                case 9:
                    monthName = isShortName ? "Sep" : "September";
                    break;
                case 10:
                    monthName = isShortName ? "Oct" : "October";
                    break;
                case 11:
                    monthName = isShortName ? "Nov" : "November";
                    break;
                case 12:
                    monthName = isShortName ? "Dec" : "December";
                    break;
                default:
                    monthName = "Invalid Month";
                    break;
            }

            return monthName;
        }

        //-----ImportToExcelEmployeeList
        public async Task<IActionResult> ExportToExcelEmployeeList()
        {
            try
            {
                List<EmployeeImportExcel> response = new List<EmployeeImportExcel>();
                string userIdString = HttpContext.Session.GetString("UserId");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");

                if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int id))
                {
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id == id).FirstOrDefaultAsync(); // Await the async call

                    if (id == 1)
                    {
                        response = await _ICrmrpo.EmployeeList();
                        var response1 = _ICrmrpo.EmployeeListForExcel(response);
                        return File(response1, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeList.xlsx");
                    }
                    else
                    {
                        response = await _ICrmrpo.CustomerEmployeeList(id);
                        foreach (var item in response)
                        {
                            ViewBag.shiftlist = item.ShiftTypeid;
                        }
                        var response1 = _ICrmrpo.EmployeeListForExcel(response);
                        return File(response1, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeList.xlsx");
                    }
                }
                return View(response);
            }
            catch (Exception Ex)
            {

                throw;
            }
        }

        public JsonResult EditSalaryDetails(string EmployeeId)
        {
            var empSalaryDetail = new EmployeeSalaryDetail();
            var data = _ICrmrpo.GetempSalaryDetailtById(EmployeeId);
            empSalaryDetail.EmployeeId = data.EmployeeId;
            empSalaryDetail.AnnualCtc = data.AnnualCtc;
            empSalaryDetail.Esic = data.Esic / 12;
            empSalaryDetail.TravellingAllowance = data.TravellingAllowance;
            empSalaryDetail.Professionaltax = data.Professionaltax / 12;
            empSalaryDetail.Basic = data.Basic / 12;
            empSalaryDetail.HouseRentAllowance = data.HouseRentAllowance / 12;
            empSalaryDetail.Epf = data.Epf / 12;
            empSalaryDetail.MonthlyCtc = data.MonthlyCtc;
            empSalaryDetail.MonthlyGrossPay = data.MonthlyGrossPay / 12;
            empSalaryDetail.Incentive = data.Incentive;
            empSalaryDetail.TravellingAllowance = data.TravellingAllowance / 12;
            var result = new
            {
                empSalaryDetail = empSalaryDetail,

            };
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditSalaryDetails(EmployeeSalaryDetail model)
        {
            try
            {
                var Salary = await _ICrmrpo.updateSalaryDetail(model);
                if (Salary != null)
                {
                    ViewBag.Message = "salarydetail update Successfully.";
                    return RedirectToAction("salarydetail", "Employee");
                }
                else
                {
                    ModelState.Clear();
                    return RedirectToAction("salarydetail", "Employee");
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        [HttpGet]
        [Route("/Employee/ESCDownloadExcel")]
        public async Task<IActionResult> ESCDownloadExcel()
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var employeeList = await _ICrmrpo.ESCExcel(Userid).ConfigureAwait(false);
                if (employeeList.Count != 0)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("ESC");
                        var currentRow = 1;

                        worksheet.Cell(currentRow, 1).Value = "Sr.No.";
                        worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 2).Value = "Employee ID";
                        worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 3).Value = "Employee Name";
                        worksheet.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 4).Value = "Account Number";
                        worksheet.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 5).Value = "IFSC";
                        worksheet.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 6).Value = "netpayment";
                        worksheet.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.LightGray;

                        currentRow++;

                        var index = 1;
                        foreach (var item in employeeList)
                        {
                            worksheet.Cell(currentRow, 1).Value = index++;
                            worksheet.Cell(currentRow, 2).Value = item.EmployeeId;
                            worksheet.Cell(currentRow, 3).Value = item.FirstName;
                            worksheet.Cell(currentRow, 4).Value = item.AccountNumber;
                            worksheet.Cell(currentRow, 5).Value = item.Ifsc;
                            worksheet.Cell(currentRow, 6).Value = item.netpayment;
                            currentRow++;
                        }


                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            var fileBytes = stream.ToArray();
                            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Ecsreport.xlsx");
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "No salary details found.";
                    return RedirectToAction("salarydetail", "Employee");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        [Route("/Employee/GenerateSalaryReport")]
        public IActionResult GenerateSalaryReport()
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                var checkvendor = _context.VendorRegistrations.Where(x => x.Id == adminlogin.Vendorid).FirstOrDefault();
                ViewBag.CheckSelectCompany = checkvendor.SelectCompany;
                if (checkvendor.SelectCompany == true)
                {
                    ViewBag.CustomerName = _context.CustomerRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                }


                return View();

            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSalaryReport(int customerId = 0, int Month = 0, int year = 0, int WorkLocation = 0)
        {
            try
            {
                ViewBag.custid = customerId;
                ViewBag.locid = WorkLocation;
                ViewBag.monthid = Month;
                ViewBag.yearid = year;
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                var checkvendor = _context.VendorRegistrations.Where(x => x.Id == adminlogin.Vendorid).FirstOrDefault();
                ViewBag.CheckSelectCompany = checkvendor.SelectCompany;
                if (checkvendor.SelectCompany == true)
                {
                    ViewBag.CustomerName = _context.CustomerRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                }
                if (customerId != 0 || Month != 0 || year != 0 || WorkLocation != 0)
                {
                    GenerateSalaryReportDTO salary = new GenerateSalaryReportDTO();
                    salary.GenerateSalaryReports = await _ICrmrpo.GenerateSalaryReport(customerId, Month, year, WorkLocation, (int)adminlogin.Vendorid);
                    decimal total = 0.00M;
                    foreach (var item in salary.GenerateSalaryReports)
                    {
                        total += (decimal)item.GenerateSalary;
                    }
                    ViewBag.TotalAmmount = total;
                    if (salary.GenerateSalaryReports.Count > 0)
                    {
                        return View(salary);
                    }
                    else
                    {
                        TempData["Message"] = "No data found";
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("GenerateSalaryReport");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }


        }

        public IActionResult EPFReport()
        {
            try
            {


                ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
                return View();

            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> EPFReport(string customerId, int Month, int year, string WorkLocation)

        {
            try
            {
                ViewBag.custid = customerId;
                ViewBag.locid = WorkLocation;
                ViewBag.monthid = Month;
                ViewBag.yearid = year;
                if (customerId != null && Month != null && year != null && WorkLocation != null)
                {
                    ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                    EPFReportDTO salary = new EPFReportDTO();



                    salary.EPFReports = await _ICrmrpo.EPFReport(customerId, Month, year, WorkLocation);
                    if (salary.EPFReports.Count > 0)

                    {
                        return View(salary);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No data found";
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("EPFReport");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }


        }

        public IActionResult ESIReport()
        {
            try
            {


                ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName
                }).ToList();
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
                return View();

            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ESIReport(string customerId, int Month, int year, string WorkLocation)

        {
            try
            {
                ViewBag.custid = customerId;
                ViewBag.locid = WorkLocation;
                ViewBag.monthid = Month;
                ViewBag.yearid = year;
                if (customerId != null && Month != null && year != null && WorkLocation != null)
                {
                    ViewBag.CustomerName = _context.CustomerRegistrations.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyName
                    }).ToList();
                    EPFReportDTO salary = new EPFReportDTO();



                    salary.EPFReports = await _ICrmrpo.ESIReport(customerId, Month, year, WorkLocation);
                    if (salary.EPFReports.Count > 0)

                    {
                        return View(salary);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No data found";
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("EPFReport");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }


        }

        private string GenerateEmployeeId()
        {
            var data = _context.EmployeeRegistrations
                              .Where(x => !string.IsNullOrEmpty(x.EmployeeId))
                              .OrderByDescending(x => x.Id)
                              .FirstOrDefault();

            string EmpID = string.Empty;
            int numericValue = 1001;

            var CompanyDetail = _context.VendorRegistrations.FirstOrDefault(x => x.Id == (int)data.Vendorid);
            if (CompanyDetail == null) return EmpID; // Ensure CompanyDetail is not null

            // Clean up company name by removing unwanted substrings
            string companyName = CompanyDetail.CompanyName;
            string[] unwantedWords = { "pvt ltd", "private limited", "ltd", "inc", "corporation", "corp" };

            foreach (var word in unwantedWords)
            {
                companyName = companyName.Replace(word, "", StringComparison.OrdinalIgnoreCase).Trim();
            }

            string result = string.Empty;
            var words = companyName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 2)
            {
                if (words[0].Equals(words[1], StringComparison.OrdinalIgnoreCase))
                {
                    result = char.ToUpper(words[0][0]).ToString() + char.ToUpper(words[0][1]).ToString();
                }
                else
                {
                    result = char.ToUpper(words[0][0]).ToString() + char.ToUpper(words[0][1]).ToString() + char.ToUpper(words[1][0]);
                }
            }
            else if (words.Length > 2)
            {
                foreach (var word in words)
                {
                    result += char.ToUpper(word[0]);
                }
            }
            else
            {
                result = companyName.Length >= 3 ? companyName.Substring(0, 3).ToUpper() : companyName.ToUpper();
            }

            string firstChars = result;
            string currentMonthYear = DateTime.Now.ToString("MMyyyy");

            if (data != null && !string.IsNullOrEmpty(data.EmployeeId))
            {
                string[] parts = data.EmployeeId.Split('-');

                if (parts.Length > 1)
                {
                    string previousAbbr = parts[0].Substring(0, parts[0].Length - 6);  // Extract company abbreviation

                    if (previousAbbr == firstChars)
                    {
                        // Fetch the max numeric value for this company from all records
                        var lastRecordForCompany = _context.EmployeeRegistrations
                            .Where(x => x.EmployeeId.StartsWith(firstChars))
                            .OrderByDescending(x => x.Id)
                            .FirstOrDefault();

                        if (lastRecordForCompany != null)
                        {
                            string[] lastParts = lastRecordForCompany.EmployeeId.Split('-');
                            if (lastParts.Length > 1 && int.TryParse(lastParts.Last(), out numericValue))
                            {
                                numericValue++; // Keep incrementing for the same company
                            }
                        }
                    }
                    else
                    {
                        numericValue = 1001; // Reset if the company name changes
                    }
                }
            }

            EmpID = $"{firstChars}{currentMonthYear}-{numericValue:D4}";
            return EmpID;
        }


        //private string GenerateEmployeeId()
        //{
        //    var data = _context.EmployeeRegistrations
        //                      .OrderByDescending(x => x.Id)
        //                      .FirstOrDefault();
        //    string EmpID = string.Empty;
        //    int numericValue = 1001;

        //    var CompanyDetail = _context.VendorRegistrations.Where(x => x.Id == data.Vendorid).FirstOrDefault();

        //    // Clean up the company name by removing unwanted substrings
        //    string companyName = CompanyDetail.CompanyName;
        //    string[] unwantedWords = new[] { "pvt ltd", "private limited", "ltd", "inc", "corporation", "corp" };

        //    // Remove unwanted words (case-insensitive)
        //    foreach (var word in unwantedWords)
        //    {
        //        companyName = companyName.Replace(word, "", StringComparison.OrdinalIgnoreCase).Trim();
        //    }

        //    string result = string.Empty;

        //    // Split company name by space to handle multi-word names
        //    var words = companyName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //    if (words.Length == 2)
        //    {
        //        // For two-word names, check if the words are the same
        //        if (words[0].Equals(words[1], StringComparison.OrdinalIgnoreCase))
        //        {
        //            result = char.ToUpper(words[0][0]).ToString() + char.ToUpper(words[0][1]).ToString();
        //        }
        //        else
        //        {
        //            result = char.ToUpper(words[0][0]).ToString() + char.ToUpper(words[0][1]).ToString() + char.ToUpper(words[1][0]);
        //        }
        //    }
        //    else if (words.Length > 2)
        //    {
        //        // For more than two words, take the first letter of each word
        //        foreach (var word in words)
        //        {
        //            result += char.ToUpper(word[0]);
        //        }
        //    }
        //    else
        //    {
        //        result = companyName.Length >= 3 ?
        //                 companyName.Substring(0, 3).ToUpper() :
        //                 companyName.ToUpper();
        //    }

        //    string firstChars = result;

        //    // Get the current month and year in the "MMyyyy" format
        //    string currentMonthYear = DateTime.Now.ToString("MMyyyy");
        //    string currentYear = DateTime.Now.ToString("yyyy");

        //    // Check if a previous employee ID exists and increment the numeric value
        //    if (data != null && !string.IsNullOrEmpty(data.EmployeeId))
        //    {
        //        string[] parts = data.EmployeeId.Split('-');

        //        if (parts.Length > 1)
        //        {
        //            string previousYear = parts[0].Substring(parts[0].Length - 4, 4);
        //            string previousAbbr = parts[0].Substring(0, parts[0].Length - 6);

        //            if (previousYear == currentYear && previousAbbr == firstChars)
        //            {
        //                // Increment numeric value if the year and abbreviation are the same
        //                if (int.TryParse(parts.Last(), out numericValue))
        //                {
        //                    numericValue++;
        //                }
        //            }
        //            else
        //            {
        //                // Reset numeric value if year or abbreviation changes
        //                numericValue = 1001;
        //            }
        //        }
        //    }

        //    // Format the final employee ID with leading zeros
        //    EmpID = $"{firstChars}{currentMonthYear}-{numericValue:D4}";

        //    return EmpID;
        //}

        public async Task<IActionResult> DeleteEmployer(int id)
        {
            try
            {
                var data = _context.EmployeerEpfs.Find(id);
                if (data != null)
                {
                    _context.EmployeerEpfs.Remove(data);
                    _context.SaveChanges();
                }
                return RedirectToAction("Employer");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public JsonResult Epfesilist()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminLogin = _context.AdminLogins.FirstOrDefault(x => x.Id == userId);

            if (adminLogin == null)
            {
                return new JsonResult(new { error = "User not found." });
            }

            var employeerTd = new EmployeerEpf();
            var data = _context.EmployeerEpfs
                               .Where(e => e.AdminLoginId == userId)
                               .Select(e => new
                               {
                                   e.DeductionCycle,
                                   e.EpfNumber,
                                   e.EmployerContributionRate
                               }).ToList();

            var esicData = _context.EmployeeEsicPayrollInfos
                                   .FirstOrDefault(e => e.Vendorid == adminLogin.Vendorid);

            decimal? esicAmount = esicData?.EsicAmount ?? 0;

            var result = new
            {
                data = data,
                esicAmount = esicAmount
            };

            return new JsonResult(result);
        }

        public IActionResult ImportToExcelAttendance()
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var data = _ICrmrpo.salarydetail(Userid).Result;
                var response = _ICrmrpo.ImportToExcelAttendance(data);
                return File(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeSalaryDetails.xlsx");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }

        [HttpPost]
        public ActionResult ImportProductionExcel(IFormFile upload)
        {

            if (ModelState.IsValid)
            {
                if (upload != null && upload.Length > 0)
                {
                    Stream stream = upload.OpenReadStream();

                    IXLWorkbook workbook = null;

                    try
                    {
                        // Load workbook using ClosedXML
                        workbook = new XLWorkbook(stream);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("File", "Unable to Upload file!");
                        return View();
                    }

                    IXLWorksheet worksheet = workbook.Worksheets.First();

                    DataTable dt = new DataTable();

                    try
                    {
                        foreach (IXLCell cell in worksheet.Row(1).Cells())
                        {
                            string columnName = cell.Value.ToString();

                            // Add the column to the DataTable
                            dt.Columns.Add(columnName);
                        }


                        for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                        {
                            DataRow newRow = dt.Rows.Add();
                            for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                            {
                                newRow[col - 1] = worksheet.Cell(row, col).Value.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("File", "Error processing Excel file!");
                        return View();
                    }

                    DataSet result = new DataSet();
                    result.Tables.Add(dt);
                    var rcount = 0;

                    var attendance = new List<salarydetail>();

                    foreach (DataRow row in dt.Rows)
                    {
                        var attend = new salarydetail();
                        rcount++;

                        var ccount = 0;
                        foreach (DataColumn col in dt.Columns)
                        {
                            try
                            {
                                ccount++;
                                Console.WriteLine(row[col]);
                                if (col.Caption == "Sr.No.")
                                {


                                    attend.Id = Convert.ToInt32(row[col].ToString());
                                }
                                else if (col.Caption == "Employee Name")
                                {
                                    string financialYearValue = row[col].ToString().Trim();

                                    // Check if the financial year value exists in the list of FinYear names
                                    attend.FirstName = row[col].ToString();


                                }
                                else if (col.Caption == "Father Name")
                                {
                                    attend.FatherName = row[col].ToString();
                                }
                                else if (col.Caption == "Employee Id")
                                {
                                    attend.EmployeeId = row[col].ToString();
                                }
                                else if (col.Caption == "Gross Pay")
                                {
                                    attend.Grosspay = Convert.ToInt32(row[col].ToString());
                                }
                                //else if (col.Caption == "MonthlyPay")
                                //{
                                //	attend.Grosspay = Convert.ToInt32(row[col].ToString());
                                //}
                                //else if (col.Caption == "Employee Epf")
                                //{
                                //	attend.Grosspay = Convert.ToInt32(row[col].ToString());
                                //}
                                //else if (col.Caption == "Employee Esi")
                                //{
                                //	attend.Grosspay = Convert.ToInt32(row[col].ToString());
                                //}
                                else if (col.Caption == "Attendance")
                                {
                                    attend.Attendance = Convert.ToInt32(row[col].ToString());
                                }


                            }
                            catch (Exception ex)
                            {
                                Console.Write($" row number: {rcount}:{dt.Columns[ccount - 1]}");
                                //  throw ex;
                                continue;
                            }



                        }


                        attendance.Add(attend);
                    }
                    return Json(attendance);


                }


            }
            else
            {
                ModelState.AddModelError("File", "Please Upload Your file");
            }

            return null;
        }
        [HttpGet, Route("/Employee/MonthlysalaryReport")]
        public async Task<IActionResult> MonthlysalaryReport(string customerId, int Month, int year, string WorkLocation)
        {
            try
            {
                var response = await _ICrmrpo.monthlysalaryReport(customerId, Month, year, WorkLocation);
                if (response.Count != 0)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("monthlyReport");
                        var currentRow = 1;

                        worksheet.Cell(currentRow, 1).Value = "Sr.No.";
                        worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 2).Value = "First Name";
                        worksheet.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 3).Value = "Middle Name";
                        worksheet.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 4).Value = "Company Name";
                        worksheet.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 5).Value = "Date Of Joining";
                        worksheet.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 6).Value = "Work Email";
                        worksheet.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 7).Value = "Gender";
                        worksheet.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 8).Value = "Work Location";
                        worksheet.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 9).Value = "Designation";
                        worksheet.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 10).Value = "Department";
                        worksheet.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 11).Value = "Emp_Reg_ID";
                        worksheet.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 12).Value = "Annual CTC";
                        worksheet.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 13).Value = "Basic";
                        worksheet.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 14).Value = "HouseRent Allowance";
                        worksheet.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 15).Value = "Travelling Allowance";
                        worksheet.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 16).Value = "ESIC";
                        worksheet.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 17).Value = "EPF";
                        worksheet.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 18).Value = "Monthly Gross Pay";
                        worksheet.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 19).Value = "Monthly CTC";
                        worksheet.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 20).Value = "Special Allowance";
                        worksheet.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 21).Value = "Gross";
                        worksheet.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 22).Value = "Personal Email Address";
                        worksheet.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 23).Value = "Mobile";
                        worksheet.Cell(currentRow, 24).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 24).Value = "Date Of Birth";
                        worksheet.Cell(currentRow, 25).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 25).Value = "Age";
                        worksheet.Cell(currentRow, 26).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 26).Value = "Father Name";
                        worksheet.Cell(currentRow, 27).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 27).Value = "PAN";
                        worksheet.Cell(currentRow, 28).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 28).Value = "Address Line 1";
                        worksheet.Cell(currentRow, 29).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 29).Value = "Address Line 2";
                        worksheet.Cell(currentRow, 30).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 30).Value = "City";
                        worksheet.Cell(currentRow, 31).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 31).Value = "State";
                        worksheet.Cell(currentRow, 32).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 32).Value = "PinCode";
                        worksheet.Cell(currentRow, 33).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 33).Value = "Account Holder Name";
                        worksheet.Cell(currentRow, 34).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 34).Value = "Bank Name";
                        worksheet.Cell(currentRow, 35).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 35).Value = "Account Number";
                        worksheet.Cell(currentRow, 36).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 36).Value = "Re-enter Account Number";
                        worksheet.Cell(currentRow, 37).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 37).Value = "IFSC";
                        worksheet.Cell(currentRow, 38).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 38).Value = "Account Type";
                        worksheet.Cell(currentRow, 39).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 39).Value = "EPF Number";
                        worksheet.Cell(currentRow, 40).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 40).Value = "Deduction Cycle";
                        worksheet.Cell(currentRow, 41).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 41).Value = "Employee Contribution Rate";
                        worksheet.Cell(currentRow, 42).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(currentRow, 42).Value = "Nominee";


                        currentRow++;

                        var index = 1;
                        foreach (var item in response)
                        {
                            worksheet.Cell(currentRow, 1).Value = index++;
                            worksheet.Cell(currentRow, 2).Value = item.FirstName;
                            worksheet.Cell(currentRow, 3).Value = item.MiddleName == null ? "" + item.FirstName + " " + "" + ' ' + "" + "" + item.LastName + "" : "" + item.FirstName + "" + "" + ' ' + "" + "" + item.MiddleName + "" + "" + ' ' + "" + "" + item.LastName + "";
                            worksheet.Cell(currentRow, 4).Value = item.CustomerName;
                            worksheet.Cell(currentRow, 5).Value = item.DateOfJoining;
                            worksheet.Cell(currentRow, 6).Value = item.WorkEmail;
                            worksheet.Cell(currentRow, 7).Value = item.Gender;
                            worksheet.Cell(currentRow, 8).Value = item.WorkLocation;
                            worksheet.Cell(currentRow, 9).Value = item.DesignationName;
                            worksheet.Cell(currentRow, 10).Value = item.DepartmentName;
                            worksheet.Cell(currentRow, 11).Value = item.Emp_Reg_ID;
                            worksheet.Cell(currentRow, 12).Value = item.AnnualCTC;
                            worksheet.Cell(currentRow, 13).Value = item.Basic;
                            worksheet.Cell(currentRow, 14).Value = item.HouseRentAllowance;
                            worksheet.Cell(currentRow, 15).Value = item.TravellingAllowance;
                            worksheet.Cell(currentRow, 16).Value = item.ESIC;
                            worksheet.Cell(currentRow, 17).Value = item.EPF;
                            worksheet.Cell(currentRow, 18).Value = item.MonthlyGrossPay;
                            worksheet.Cell(currentRow, 19).Value = item.MonthlyCTC;
                            worksheet.Cell(currentRow, 20).Value = item.SpecialAllowance;
                            worksheet.Cell(currentRow, 21).Value = item.gross;
                            worksheet.Cell(currentRow, 22).Value = item.PersonalEmailAddress;
                            worksheet.Cell(currentRow, 23).Value = item.Mobile;
                            worksheet.Cell(currentRow, 24).Value = item.DateOfBirth;
                            worksheet.Cell(currentRow, 25).Value = item.Age;
                            worksheet.Cell(currentRow, 26).Value = item.FatherName;
                            worksheet.Cell(currentRow, 27).Value = item.PAN;
                            worksheet.Cell(currentRow, 28).Value = item.AddressLine1;
                            worksheet.Cell(currentRow, 29).Value = item.AddressLine2;
                            worksheet.Cell(currentRow, 30).Value = item.City;
                            worksheet.Cell(currentRow, 31).Value = item.State;
                            worksheet.Cell(currentRow, 32).Value = item.Pincode;
                            worksheet.Cell(currentRow, 33).Value = item.AccountHolderName;
                            worksheet.Cell(currentRow, 34).Value = item.BankName;
                            worksheet.Cell(currentRow, 35).Value = item.AccountNumber;
                            worksheet.Cell(currentRow, 36).Value = item.ReEnterAccountNumber;
                            worksheet.Cell(currentRow, 37).Value = item.IFSC;
                            worksheet.Cell(currentRow, 38).Value = item.AccountType;
                            worksheet.Cell(currentRow, 39).Value = item.EPF_Number;
                            worksheet.Cell(currentRow, 40).Value = item.Deduction_Cycle;
                            worksheet.Cell(currentRow, 41).Value = item.Employee_Contribution_Rate;
                            worksheet.Cell(currentRow, 42).Value = item.nominee;

                            currentRow++;
                        }


                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            var fileBytes = stream.ToArray();
                            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "monthlyReport.xlsx");
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "No salary details found.";
                    return RedirectToAction("GenerateSalary", "Employee");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet, Route("/Employee/EmployeeOfferletter")]
        public async Task<IActionResult> EmployeeOfferletter(int? Id = 0)
        {
            try
            {
                var offerletter = await _context.Offerletters.Where(x => x.Id == Id && x.IsDeleted == false).FirstOrDefaultAsync();
                var vendorinfo = _context.VendorRegistrations.Where(v => v.Id == offerletter.Vendorid).FirstOrDefault();
                if (offerletter == null)
                {
                    return BadRequest("Offer letter not found");
                }
                ViewBag.Protocol = Request.Scheme;
                ViewBag.Host = Request.Host.Value;
                var result = new getempOfferletter
                {
                    Id = offerletter.Id,
                    Name = offerletter.Name,
                    MonthlyCtc = offerletter.MonthlyCtc,
                    AnnualCtc = offerletter.AnnualCtc,
                    CandidateAddress = offerletter.CandidateAddress,
                    CandidatePincode = offerletter.CandidatePincode,
                    HrName = offerletter.HrName,
                    HrJobTitle = offerletter.HrJobTitle,
                    HrSignature = offerletter.HrSignature,
                    Currentdate = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    StateName = _context.States.Where(g => g.Id == offerletter.StateId).Select(g => g.SName).FirstOrDefault(),
                    CityName = _context.Cities.Where(g => g.Id == offerletter.CityId).Select(g => g.City1).FirstOrDefault(),
                    DateOfJoining = offerletter.DateOfJoining?.ToString("dd/MM/yyyy"),
                    DepartmentName = _context.DepartmentMasters.Where(g => g.Id == Convert.ToInt16(offerletter.DepartmentId)).Select(g => g.DepartmentName).FirstOrDefault()?.Trim(),
                    DesignationName = _context.DesignationMasters.Where(g => g.Id == Convert.ToInt16(offerletter.DesignationId)).Select(g => g.DesignationName).FirstOrDefault()?.Trim(),
                    Validdate = offerletter.Validdate?.ToString("dd/MM/yyyy"),
                    CompanyImage = _context.VendorRegistrations.Where(g => g.Id == offerletter.Vendorid).Select(g => g.CompanyImage).FirstOrDefault(),
                    CompanyName = _context.VendorRegistrations.Where(g => g.Id == offerletter.Vendorid).Select(g => g.CompanyName).FirstOrDefault(),
                    OfficeLocation = _context.VendorRegistrations.Where(g => g.Id == offerletter.Vendorid).Select(g => g.Location).FirstOrDefault(),
                    OfficeState = _context.States.Where(g => g.Id == vendorinfo.StateId).Select(g => g.SName).FirstOrDefault(),
                    OfficeCity = _context.Cities.Where(g => g.Id == vendorinfo.CityId).Select(g => g.City1).FirstOrDefault(),
                };
                return View(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        public async Task<IActionResult> OfferletterDocPDF(int? Id = 0)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                if (adminlogin == null)
                {
                    return BadRequest("Admin login not found");
                }
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                string SlipURL = $"{schema}://{host}/Employee/EmployeeOfferletter?Id={Id}";
                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }
                var result = (from off in _context.Offerletters
                              where off.Id == Id && off.Vendorid == adminlogin.Vendorid
                              select new getempOfferletter
                              {
                                  Id = off.Id,
                                  Name = off.Name,
                                  CandidateEmail = off.CandidateEmail,
                                  CompanyName = _context.VendorRegistrations.Where(x => x.Id == adminlogin.Vendorid).Select(x => x.CompanyName).FirstOrDefault()
                              }).FirstOrDefault();
                string uniqueFileName = $"Offerletter_{Id}.pdf";
                string filePath = Path.Combine(wwwRootPath, uniqueFileName);
                doc.Save(filePath);
                byte[] pdf = System.IO.File.ReadAllBytes(filePath);



                if (result == null)
                {
                    return BadRequest("Employee not found.");
                }

                var empoff = _context.Offerletters.FirstOrDefault(e => e.Id == result.Id);
                if (empoff != null)
                {
                    empoff.OfferletterFile = uniqueFileName;
                    _context.SaveChanges();
                    string emailSubject = $"Offerletter for {result.Name}";
                    string emailBody = $"Dear {result.Name},We are pleased to offer you a position at our company. Please find your attached offer letter for detailed information regarding your employment. If you have any questions or require further assistance, feel free to reach out.Best regards,{result.CompanyName}";
                    await _IEmailService.SendEmailAsync(result.CandidateEmail, emailSubject, emailBody, pdf, uniqueFileName, "application/pdf", (int)adminlogin.Vendorid);
                    return Json(new { success = true, message = "Offer letter  has been Sent successfully.", fileName = uniqueFileName });
                }
                else
                {
                    return Json(new { BadRequest = 404, message = "Data not found for the employee." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        [HttpGet, Route("/Employee/Appointmentletter")]
        public async Task<IActionResult> Appointmentletter(int? Id = 0)
        {
            try
            {
                // Check if the ID is valid
                if (Id == null || Id == 0)
                {
                    return BadRequest("Invalid employee ID");
                }

                // Query the data with type matching for the joins
                var result = await (from emp in _context.EmployeeRegistrations
                                    join des in _context.DesignationMasters
                                    on Convert.ToInt16(emp.DesignationId) equals des.Id
                                    join vendor in _context.VendorRegistrations on emp.Vendorid equals vendor.Id
                                    join state in _context.States on vendor.StateId equals state.Id
                                    join dpt in _context.DepartmentMasters on Convert.ToInt16(emp.DepartmentId) equals dpt.Id
                                    join salary in _context.EmployeeSalaryDetails on emp.EmployeeId equals salary.EmployeeId into salaryJoin
                                    from salaryDetails in salaryJoin.DefaultIfEmpty()
                                    join salaryepf in _context.EmployeeEpfPayrollInfos on emp.Vendorid equals salaryepf.Vendorid into salaryep
                                    from salaryepfs in salaryep.DefaultIfEmpty()
                                    join salaryesic in _context.EmployeeEsicPayrollInfos on emp.Vendorid equals salaryesic.Vendorid into salaryes
                                    from salaryesics in salaryes.DefaultIfEmpty()
                                    where emp.Id == Id && emp.IsDeleted == false
                                    select new EmpAppointmentletter
                                    {
                                        Designation = des.DesignationName.Trim(),
                                        EmployeeName = $"{emp.FirstName} {(emp.MiddleName ?? "")} {emp.LastName}",
                                        Empcode = emp.EmployeeId,
                                        CompanyName = vendor.CompanyName,
                                        JobLocation = vendor.Location,
                                        CompanyAddress = vendor.Location,
                                        CompanyEmail = vendor.Email,
                                        CompanyState = state.SName,
                                        HRAYearly = salaryDetails.HouseRentAllowance,
                                        HRA = Math.Round((decimal)(salaryDetails.HouseRentAllowance / 12), 2),
                                        CompanyImage = vendor.CompanyImage,
                                        DateOfJoining = emp.DateOfJoining.Value.ToString("dd/MM/yyyy"),
                                        BasicSalaryYearly = salaryDetails.Basic,
                                        BasicSalary = Math.Round((decimal)(salaryDetails.Basic / 12), 2),
                                        MedicalYearly = salaryDetails.Medical,
                                        Medical = Math.Round((decimal)(salaryDetails.Medical / 12), 2),
                                        ConveyanceYearly = salaryDetails.Conveyanceallowance,
                                        Conveyance = Math.Round((decimal)(salaryDetails.Conveyanceallowance / 12), 2),
                                        PFYearly = salaryDetails.Epf,
                                        PF = Math.Round((decimal)(salaryDetails.Epf / 12), 2),
                                        ESICYearly = salaryDetails.Esic,
                                        ESIC = Math.Round((decimal)(salaryDetails.Esic / 12), 2),
                                        DepartmentName = dpt.DepartmentName.Trim(),
                                        TotalMonthlySalary = Math.Round((decimal)(salaryDetails.HouseRentAllowance + salaryDetails.Basic + salaryDetails.Medical + salaryDetails.Conveyanceallowance + salaryDetails.Epf + salaryDetails.Esic) / 12, 2),
                                        TotalYearlySalary = Math.Round((decimal)(salaryDetails.Basic + salaryDetails.HouseRentAllowance + salaryDetails.Medical + salaryDetails.Conveyanceallowance + salaryDetails.Epf + salaryDetails.Esic), 2),
                                        HRAper = Math.Round((decimal)(salaryDetails.Hrapercentage), 2),
                                        BasicSalaryper = Math.Round((decimal)(salaryDetails.Basicpercentage), 2),
                                        Medicalper = Math.Round((decimal)(salaryDetails.Medicalpercentage ), 2),
                                        Conveyanceper = Math.Round((decimal)(salaryDetails.Conveyancepercentage), 2),
                                        PFper = Math.Round((decimal)salaryepfs.Epfpercentage, 2),
                                        ESICper = Math.Round((decimal)salaryesics.Esicpercentage, 2),
                                    }).FirstOrDefaultAsync();



                if (result == null)
                {
                    return BadRequest("Appointment letter details not found");
                }

                return View(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        public async Task<IActionResult> AppointmentletterDocPDF(int? Id = 0)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var emp = await _context.EmployeeRegistrations.Where(x => x.Id == Id && adminlogin.Vendorid == x.Vendorid).FirstOrDefaultAsync();
                if (adminlogin == null)
                {
                    return BadRequest("Admin login not found");
                }
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                string SlipURL = $"{schema}://{host}/Employee/Appointmentletter?Id={Id}";
                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }
                string uniqueFileName = $"Appointmentletter_{emp.EmployeeId}.pdf";
                string filePath = Path.Combine(wwwRootPath, uniqueFileName);
                doc.Save(filePath);
                byte[] pdf = System.IO.File.ReadAllBytes(filePath);

                var result = await _context.EmployeePersonalDetails.Where(x => x.EmpRegId == emp.EmployeeId).FirstOrDefaultAsync();
                var companyname = await _context.VendorRegistrations.Where(x => x.Id == adminlogin.Vendorid).Select(x => x.CompanyName).FirstOrDefaultAsync();
                if (result == null)
                {
                    return BadRequest("Employee not found.");
                }

                var empoff = _context.EmployeeRegistrations.FirstOrDefault(e => e.EmployeeId == result.EmpRegId);
                if (empoff != null)
                {
                    empoff.Appoinmentletter = uniqueFileName;
                    _context.SaveChanges();
                    string emailSubject = $"Appointmentletter for {emp.FirstName}";
                    string emailBody = $"Dear {emp.FirstName},Congratulations! We are excited to offer you a position with our organization. Please find your attached Appointment Letter, which contains all the relevant details about your employment. Should you have any questions or require further clarification, please do not hesitate to reach out to us.We look forward to having you on our team!Best regards,{companyname}";
                    await _IEmailService.SendEmailAsync(result.PersonalEmailAddress, emailSubject, emailBody, pdf, uniqueFileName, "application/pdf", (int)adminlogin.Vendorid);

                    return Json(new { success = true, message = "Appointment letter has been sent successfully.", fileName = uniqueFileName });

                }
                else
                {
                    return Json(new { BadRequest = 404, message = "Data not found for the employee." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet, Route("/Employee/AddOfferletterdetail")]
        public async Task<IActionResult> AddOfferletterdetail(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                ViewBag.Department = await _context.DepartmentMasters.Where(d => d.AdminLoginId == Userid).Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.DepartmentName
                }).ToListAsync();

                ViewBag.Designation = await _context.DesignationMasters.Where(x => x.AdminLoginId == Userid).Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.DesignationName
                }).ToListAsync();

                ViewBag.States = await _context.States.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.SName
                }).ToListAsync();

                ViewBag.Cities = await _context.Cities.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.City1
                }).ToListAsync();

                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var hrdetail = await _context.Hrsignatures.Where(x => x.Vendorid == adminlogin.Vendorid).FirstOrDefaultAsync();
                if (hrdetail != null)
                {
                    ViewBag.HrJobTitle = hrdetail.HrJobTitle;
                    ViewBag.HrSignature = hrdetail.HrSignature1;
                    ViewBag.HrName = hrdetail.HrName;
                }
                else
                {
                    ViewBag.HrJobTitle = "";
                    ViewBag.HrSignature = "";
                    ViewBag.HrName = "";
                }
                ViewBag.userId = adminlogin.Vendorid;
                ViewBag.Heading = "Add Offerletter Detail";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = await _ICrmrpo.GetOfferletterbyid(id);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.Name = data.Name;
                        ViewBag.CandidatePincode = data.CandidatePincode;
                        ViewBag.CandidateAddress = data.CandidateAddress;
                        ViewBag.DepartmentId = data.DepartmentId;
                        ViewBag.DesignationId = data.DesignationId;
                        ViewBag.MonthlyCtc = data.MonthlyCtc;
                        ViewBag.AnnualCtc = data.AnnualCtc;
                        ViewBag.StateId = data.StateId;
                        ViewBag.CityId = data.CityId;
                        ViewBag.DateOfJoining = data.DateOfJoining?.ToString("yyyy-MM-dd");
                        ViewBag.Validdate = data.Validdate?.ToString("yyyy-MM-dd");
                        ViewBag.HrJobTitle = data.HrJobTitle;
                        ViewBag.HrSignature = data.HrSignature;
                        ViewBag.HrName = data.HrName;
                        ViewBag.CandidateEmail = data.CandidateEmail;

                        ViewBag.Heading = "Update Offerletter Detail";
                        ViewBag.btnText = "UPDATE";
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
        public async Task<IActionResult> AddOfferletterdetail(Offerletters model)
        {
            try
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();

                if (model.Id > 0)
                {
                    var data = await _ICrmrpo.updateOfferletterdetail(model);
                    if (data > 0)
                    {
                        TempData["Message"] = "updok";
                        return RedirectToAction("AddOfferletterdetail", "Employee");
                    }
                    else
                    {
                        TempData["Message"] = "Update Failed.";
                        return View(model);
                    }
                }
                else
                {
                    var response = await _ICrmrpo.AddOfferletterdetail(model, Userid);
                    if (response > 0)
                    {
                        TempData["Message"] = "ok";
                        return RedirectToAction("AddOfferletterdetail", "Employee");
                    }
                    else
                    {
                        TempData["Message"] = "Failed.";
                        ModelState.Clear();
                        return View(model);
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> OfferletterList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {


                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var response = await _ICrmrpo.OfferletterdetailList(Userid);

                return View(response);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }
        public async Task<IActionResult> DeleteOfferletter(int id)
        {
            try
            {
                var data = _context.Offerletters.Find(id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    _context.Offerletters.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("OfferletterList");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        [HttpGet]
        public IActionResult DeletHrSignature(string FilePath, int id)
        {
            bool success = false;

            if (FilePath != "")
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CompanyImage");
                string folderfilepathPath = folderPath + "//" + FilePath;
                if (Directory.Exists(folderPath))
                {
                    if (System.IO.File.Exists(folderfilepathPath))
                    {
                        System.IO.File.Delete(folderfilepathPath);
                        success = true;
                    }
                    var img = _context.Offerletters.FirstOrDefault(s => s.HrSignature == FilePath && s.Id == id);
                    if (img != null)
                    {
                        img.HrSignature = null;
                        _context.SaveChangesAsync();
                    }

                }
            }
            return Json(success);
        }

        [HttpGet, Route("/Employee/Appointmentletterlist")]
        public async Task<IActionResult> Appointmentletterlist()
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {


                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                    var response = await _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false).Select(x => new Appointmentdetail
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        DateOfJoining = x.DateOfJoining.GetValueOrDefault(),
                        MiddleName = x.MiddleName,
                        WorkEmail = x.WorkEmail,
                        Emp_Reg_ID = x.EmployeeId,
                        Appoinmentletter = x.Appoinmentletter
                    }).OrderByDescending(x => x.Id).ToListAsync();

                    return View(response);
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        [HttpPost]
        public JsonResult UpdateShiftType(int OfficeshiftTypeid, string EmployeeId)
        {
            var emp = _context.EmployeeRegistrations.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
            emp.OfficeshiftTypeid = OfficeshiftTypeid;
            _context.SaveChanges();
            return Json(new { success = true, message = "Shift type updated successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> EmpLeavemaster(int id = 0)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    ViewBag.UserName = HttpContext.Session.GetString("UserName");
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                    ViewBag.EmployeeId = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false).Select(D => new SelectListItem
                    {
                        Value = D.EmployeeId.ToString(),
                        Text = $"{D.EmployeeId} {' '} ({D.FirstName})"

                    }).ToList();
                    ViewBag.leavetype = _context.LeaveTypes.Where(x => x.Vendorid == adminlogin.Vendorid).Select(D => new SelectListItem
                    {
                        Value = D.Id.ToString(),
                        Text = $"{D.Leavetype1} {' '} ({D.Leavevalue})"

                    }).ToList();
                    ViewBag.id = 0;
                    ViewBag.LeavetypeId = "";
                    ViewBag.LeaveStartDate = "";
                    ViewBag.EmpId = "";
                    ViewBag.Status = "";
                    ViewBag.heading = "Add Leavemaster :";
                    ViewBag.btnText = "SAVE";
                    if (id != null && id != 0)
                    {
                        var data = _context.Leavemasters.Find(id);
                        if (data != null)
                        {

                            ViewBag.id = data.Id;
                            ViewBag.LeavetypeId = data.LeavetypeId;
                            ViewBag.Value = data.Value;
                            ViewBag.EmpId = data.EmpId;
                            ViewBag.Status = data.IsActive;
                            ViewBag.LeaveStartDate = data.LeaveStartDate.ToString("yyyy-MM-dd");
                            ViewBag.btnText = "UPDATE";
                            ViewBag.heading = "Update Leavemaster :";

                        }
                    }
                    LeavemasterDto response = new LeavemasterDto();
                    response.lmd = await _ICrmrpo.getLeavemaster(Userid, adminlogin.Vendorid);
                    return View(response);
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Route("/Employee/EmpLeavemaster")]
        public async Task<IActionResult> EmpLeavemaster(LeavemasterDto model)
        {

            if (HttpContext.Session.GetString("UserName") != null)
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var existingleave = await _context.Leavemasters
               .Where(b => b.EmpId == model.EmpId && b.LeavetypeId == Convert.ToInt16(model.LeavetypeId))
                 .FirstOrDefaultAsync();
                if (model.id == 0)
                {
                    if (existingleave != null)
                    {
                        TempData["Message"] = "Already exists.";
                        return RedirectToAction("EmpLeavemaster");
                    }
                }
                var options = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false)
                 .Select(D => new SelectListItem
                 {
                     Value = D.EmployeeId.ToString(),
                     Text = $"{D.EmployeeId} {' '} ({D.FirstName})"
                 }).ToList();

                ViewBag.EmployeeId = options;
                ViewBag.leavetype = _context.LeaveTypes.Select(D => new SelectListItem
                {
                    Value = D.Id.ToString(),
                    Text = $"{D.Leavetype1} {' '} ({D.Leavevalue})"

                }).ToList();
                int AddedByid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                if (model.id > 0)
                {
                    var existingEntity = _context.Leavemasters.Find(model.id);
                    if (existingEntity != null)
                    {

                        existingEntity.LeavetypeId = Convert.ToInt16(model.LeavetypeId);
                        existingEntity.Value = model.Value;
                        existingEntity.EmpId = model.EmpId;
                        existingEntity.IsActive = true;
                        existingEntity.Createddate = DateTime.Now.Date;
                        existingEntity.LeaveStartDate = DateTime.Now.Date;
                        existingEntity.LeaveUpdateDate = DateTime.Now.Date;
                        existingEntity.Vendorid = adminlogin.Vendorid;
                        _context.SaveChanges();
                        TempData["Message"] = "updok";
                        return RedirectToAction("EmpLeavemaster");
                    }

                }
                else
                {

                    var newRecord = new Models.Crm.Leavemaster
                    {
                        LeavetypeId = Convert.ToInt16(model.LeavetypeId),
                        EmpId = model.EmpId,
                        Value = model.Value,
                        LeaveStartDate = (DateTime)model.LeaveStartDate,
                        IsActive = true,
                        Createddate = DateTime.Now.Date,
                        LeaveUpdateDate = DateTime.Now.Date,
                        Vendorid = adminlogin.Vendorid

                    };


                    _context.Leavemasters.Add(newRecord);
                    _context.SaveChanges();
                    TempData["Message"] = "ok";
                    return RedirectToAction("EmpLeavemaster");
                }

                return RedirectToAction("EmpLeavemaster");
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public async Task<IActionResult> DeleteEmpLeavemaster(int id)
        {
            var LeavemasterDelete = _context.Leavemasters.Find(id);
            if (LeavemasterDelete != null)
            {
                _context.Leavemasters.Remove(LeavemasterDelete);
                _context.SaveChanges();
                TempData["Message"] = "dltok";
                return RedirectToAction("EmpLeavemaster");
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet, Route("/Employee/EmployeeExperienceletter")]
        public async Task<IActionResult> EmployeeExperienceletter(int? Id = 0)
        {
            try
            {
                var Experienceletter = await _context.EmpExperienceletters.Where(x => x.Id == Id).FirstOrDefaultAsync();
                var empExperienceletter = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == Experienceletter.EmployeeId).FirstOrDefaultAsync();
                var vendorinfo = _context.VendorRegistrations.Where(v => v.Id == empExperienceletter.Vendorid).FirstOrDefault();
                if (Experienceletter == null)
                {
                    return BadRequest("Experience letter not found");
                }
                var result = new Experienceletters
                {
                    EmployeeName = empExperienceletter.FirstName + " " + empExperienceletter.MiddleName + " " + empExperienceletter.LastName,
                    EmployeeCode = Experienceletter.EmployeeId,
                    HrName = Experienceletter.HrName,
                    HrDesignation = Experienceletter.HrDesignation,
                    Designation = _context.DesignationMasters.Where(g => g.Id == Convert.ToInt16(empExperienceletter.DesignationId)).Select(g => g.DesignationName).FirstOrDefault()?.Trim(),
                    StartDate = Experienceletter.StartDate?.ToString("dd/MM/yyyy"),
                    EndDate = Experienceletter.EndDate?.ToString("dd/MM/yyyy"),
                    CompanyName = vendorinfo.CompanyName,
                    CompanyAddress = vendorinfo.Location,
                    CompanyEmail = vendorinfo.Email,
                    CompanyPhoneNumber = vendorinfo.MobileNumber,
                    CompanyImage = vendorinfo.CompanyImage,
                };
                return View(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        public async Task<IActionResult> ExperienceletterDocPDF(int? Id = 0)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                if (adminlogin == null)
                {
                    return BadRequest("Admin login not found");
                }
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                string SlipURL = $"{schema}://{host}/Employee/EmployeeExperienceletter?Id={Id}";
                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc = converter.ConvertUrl(SlipURL);
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }
                var result = (from er in _context.EmployeeRegistrations
                              join emp in _context.EmployeePersonalDetails
                              on er.EmployeeId equals emp.EmpRegId
                              join emppp in _context.EmpExperienceletters
                              on emp.EmpRegId equals emppp.EmployeeId
                              where emppp.Id == Id && er.Vendorid == adminlogin.Vendorid
                              select new
                              {
                                  Id = emppp.Id,
                                  EmployeeId = er.EmployeeId,
                                  Name = er.FirstName,
                                  WorkEmail = emp.PersonalEmailAddress,
                                  companyname = _context.VendorRegistrations.Where(x => x.Id == adminlogin.Vendorid).Select(x => x.CompanyName).FirstOrDefault(),
                                  tenure = (emppp.StartDate - emppp.EndDate).Value.Days,
                              }).FirstOrDefault();




                string uniqueFileName = $"Experienceletter_{result.EmployeeId}.pdf";
                string filePath = Path.Combine(wwwRootPath, uniqueFileName);
                doc.Save(filePath);
                byte[] pdf = System.IO.File.ReadAllBytes(filePath);



                if (result == null)
                {
                    return BadRequest("Employee not found.");
                }

                var empoff = _context.EmpExperienceletters.FirstOrDefault(e => e.Id == result.Id);
                if (result != null)
                {
                    empoff.ExperienceletterFile = uniqueFileName;
                    _context.SaveChanges();
                    string emailSubject = $"Experience Letter for {result.Name}";
                    string emailBody = $"Dear {result.Name},We are pleased to provide you with your Experience Letter for your tenure at {result.tenure}. Please find the attached document for your reference.If you have any questions or require further assistance, feel free to contact us.Best regards,{result.companyname}";
                    await _IEmailService.SendEmailAsync(result.WorkEmail, emailSubject, emailBody, pdf, uniqueFileName, "application/pdf", (int)adminlogin.Vendorid);

                    return Json(new { success = true, message = "Experience letter has been sent successfully.", fileName = uniqueFileName });

                }
                else
                {
                    return Json(new { BadRequest = 404, message = "Data not found for the employee." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        [HttpGet, Route("/Employee/AddExperienceletteretail")]
        public async Task<IActionResult> AddExperienceletteretail(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


                ViewBag.EmployeeId = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false).Select(D => new SelectListItem
                {
                    Value = D.EmployeeId.ToString(),
                    Text = $"{D.EmployeeId} {' '} ({D.FirstName})"

                }).ToList();

                ViewBag.StartDate = "";
                ViewBag.EndDate = "";
                ViewBag.HrJobTitle = "";
                ViewBag.HrName = "";
                ViewBag.EmpId = "";
                ViewBag.userId = adminlogin.Vendorid;
                ViewBag.Heading = "Add Experienceletter Detail";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = await _ICrmrpo.GetExperienceletterbyid(id);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.StartDate = data.StartDate?.ToString("yyyy-MM-dd");
                        ViewBag.EndDate = data.EndDate?.ToString("yyyy-MM-dd");
                        ViewBag.HrJobTitle = data.HrDesignation;
                        ViewBag.HrName = data.HrName;
                        ViewBag.EmpId = data.EmployeeId;
                        ViewBag.Heading = "Update Experienceletter Detail";
                        ViewBag.btnText = "UPDATE";
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
        public async Task<IActionResult> AddExperienceletteretail(EmpExperienceletter model)
        {
            try
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();

                if (model.Id > 0)
                {
                    bool data = await _ICrmrpo.updateExperienceletterdetail(model);
                    if (data)
                    {
                        TempData["Message"] = "updok";
                        return RedirectToAction("AddExperienceletteretail", "Employee");
                    }
                    else
                    {
                        TempData["Message"] = "Update Failed.";
                        return View(model);
                    }
                }
                else
                {
                    var response = await _ICrmrpo.AddExperienceletterdetail(model, Userid);
                    if (response > 0)
                    {
                        TempData["Message"] = "ok";
                        return RedirectToAction("AddExperienceletteretail", "Employee");
                    }
                    else
                    {
                        TempData["Message"] = "Failed.";
                        ModelState.Clear();
                        return View(model);
                    }
                }

            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ExperienceletterList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {


                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

                if (adminLogin == null)
                {
                    return RedirectToAction("Login", "Admin");
                }

                var response = await (from exp in _context.EmpExperienceletters
                                      join emp in _context.EmployeeRegistrations
                                      on exp.EmployeeId equals emp.EmployeeId
                                      join cdm in _context.DesignationMasters
                                     on Convert.ToInt32(emp.DesignationId) equals cdm.Id
                                      where exp.Vendorid == adminLogin.Vendorid
                                      select new Experienceletters
                                      {
                                          Id = exp.Id,
                                          EmployeeName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName,
                                          EmployeeCode = exp.EmployeeId,
                                          Designation = cdm.DesignationName,
                                          StartDate = exp.StartDate.Value.ToString("dd/MM/yyyy"),
                                          EndDate = exp.EndDate.Value.ToString("dd/MM/yyyy"),
                                          HrName = exp.HrName,
                                          WorkEmail = emp.WorkEmail,
                                          ExperienceletterFile = exp.ExperienceletterFile,
                                          HrDesignation = exp.HrDesignation,
                                      }).OrderByDescending(x => x.Id).ToListAsync();

                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public async Task<IActionResult> DeleteExperienceletter(int id)
        {
            try
            {
                var data = _context.EmpExperienceletters.Find(id);
                if (data != null)
                {
                    _context.EmpExperienceletters.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("ExperienceletterList");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        [HttpGet, Route("/Employee/AddRelievingletterdetail")]
        public async Task<IActionResult> AddRelievingletterdetail(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();


                ViewBag.EmployeeId = _context.EmployeeRegistrations.Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false).Select(D => new SelectListItem
                {
                    Value = D.EmployeeId.ToString(),
                    Text = $"{D.EmployeeId} {' '} ({D.FirstName})"

                }).ToList();

                ViewBag.ResignationDate = "";
                ViewBag.LastDateofEmployment = "";
                ViewBag.EmpId = "";
                ViewBag.userId = adminlogin.Vendorid;
                ViewBag.Heading = "Add Relieving Detail";
                ViewBag.btnText = "SAVE";
                if (id != 0)
                {
                    var data = await _ICrmrpo.GetRelievingletterbyid(id);
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.ResignationDate = data.ResignationDate?.ToString("yyyy-MM-dd");
                        ViewBag.LastDateofEmployment = data.LastDateofEmployment?.ToString("yyyy-MM-dd");
                        ViewBag.EmpId = data.EmployeeId;
                        ViewBag.Heading = "Update Relieving Detail";
                        ViewBag.btnText = "UPDATE";
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
        public async Task<IActionResult> AddRelievingletterdetail(EmpRelievingletter model)
        {
            try
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();

                if (model.Id > 0)
                {
                    bool data = await _ICrmrpo.updateRelievingletterdetail(model);
                    if (data)
                    {
                        TempData["Message"] = "updok";
                        return RedirectToAction("AddRelievingletterdetail", "Employee");
                    }
                    else
                    {
                        TempData["Message"] = "Update Failed.";
                        return View(model);
                    }
                }
                else
                {
                    var response = await _ICrmrpo.AddRelievingletterdetail(model, Userid);
                    if (response > 0)
                    {
                        TempData["Message"] = "ok";
                        return RedirectToAction("AddRelievingletterdetail", "Employee");
                    }
                    else
                    {
                        TempData["Message"] = "Failed.";
                        ModelState.Clear();
                        return View(model);
                    }
                }

            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> RelievingletterList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {


                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

                if (adminLogin == null)
                {
                    return RedirectToAction("Login", "Admin");
                }

                var response = await (from exp in _context.EmpRelievingletters
                                      join emp in _context.EmployeeRegistrations
                                      on exp.EmployeeId equals emp.EmployeeId
                                      join dm in _context.DesignationMasters
                                      on Convert.ToInt32(emp.DesignationId) equals dm.Id
                                      where exp.Vendorid == adminLogin.Vendorid
                                      select new Relievingletters
                                      {
                                          Id = exp.Id,
                                          EmployeeName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName,
                                          EmployeeCode = exp.EmployeeId,
                                          Designation = dm.DesignationName,
                                          ResignationDate = exp.ResignationDate.Value.ToString("dd/MM/yyyy"),
                                          LastDateofEmployment = exp.LastDateofEmployment.Value.ToString("dd/MM/yyyy"),
                                          WorkEmail = emp.WorkEmail,
                                      }).OrderByDescending(x => x.Id).ToListAsync();

                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public async Task<IActionResult> DeleteRelievingletter(int id)
        {
            try
            {
                var data = _context.EmpRelievingletters.Find(id);
                if (data != null)
                {
                    _context.EmpRelievingletters.Remove(data);
                    _context.SaveChanges();
                    TempData["Message"] = "dltok";
                }
                return RedirectToAction("RelievingletterList");
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        [HttpGet, Route("/Employee/EmployeeRelievingletter")]
        public async Task<IActionResult> EmployeeRelievingletter(int? Id = 0, bool Ismail = false)
        {
            try
            {
                var Relievingletter = await _context.EmpRelievingletters.Where(x => x.Id == Id).FirstOrDefaultAsync();
                var empRelievingletter = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == Relievingletter.EmployeeId).FirstOrDefaultAsync();
                var vendorinfo = _context.VendorRegistrations.Where(v => v.Id == empRelievingletter.Vendorid).FirstOrDefault();
                if (Relievingletter == null)
                {
                    return BadRequest("Relieving letter not found");
                }
                ViewBag.Protocol = Request.Scheme;
                ViewBag.Host = Request.Host.Value;

                var result = new Relievingletters
                {
                    EmployeeName = empRelievingletter.FirstName + " " + empRelievingletter.MiddleName + " " + empRelievingletter.LastName,
                    EmployeeCode = Relievingletter.EmployeeId,
                    Designation = _context.DesignationMasters.Where(g => g.Id == Convert.ToInt16(empRelievingletter.DesignationId)).Select(g => g.DesignationName).FirstOrDefault()?.Trim(),
                    ResignationDate = Relievingletter.ResignationDate?.ToString("dd/MM/yyyy"),
                    LastDateofEmployment = Relievingletter.LastDateofEmployment?.ToString("dd/MM/yyyy"),
                    CompanyName = vendorinfo.CompanyName,
                    CompanyAddress = vendorinfo.Location,
                    CompanyEmail = vendorinfo.Email,
                    CompanyPhoneNumber = vendorinfo.MobileNumber,
                    CompanyImage = vendorinfo.CompanyImage,
                    Ismail = Ismail
                };
                return View(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        public async Task<IActionResult> RelievingletterDocPDF(int? Id = 0, bool Ismail = false)
        {
            try
            {
                // Retrieve UserId from session and validate admin login
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == Userid);
                if (adminlogin == null)
                {
                    return BadRequest("Admin login not found");
                }

                // Construct Slip URL
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                string SlipURL = $"{schema}://{host}/Employee/EmployeeRelievingletter?Id={Id}&Ismail={Ismail}";

                // Fetch HTML content from SlipURL
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(SlipURL);
                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest("Failed to retrieve content from the URL.");
                    }

                    var htmlContent = await response.Content.ReadAsStringAsync();

                    // Generate PDF from HTML content
                    byte[] pdf = GeneratePdf(htmlContent);
                    //return File(pdf, "application/pdf", "kk.pdf");
                    // Define directory for saving PDF
                    string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                    if (!Directory.Exists(wwwRootPath))
                    {
                        Directory.CreateDirectory(wwwRootPath);
                    }

                    // Fetch employee details for the relieving letter
                    var result = (from er in _context.EmployeeRegistrations
                                  join emp in _context.EmployeePersonalDetails on er.EmployeeId equals emp.EmpRegId
                                  join emppp in _context.EmpRelievingletters on emp.EmpRegId equals emppp.EmployeeId
                                  where emppp.Id == Id && er.Vendorid == adminlogin.Vendorid
                                  select new
                                  {
                                      Id = emppp.Id,
                                      EmployeeId = er.EmployeeId,
                                      Name = er.FirstName,
                                      WorkEmail = emp.PersonalEmailAddress,
                                      companyname = _context.VendorRegistrations
                                          .Where(x => x.Id == adminlogin.Vendorid)
                                          .Select(x => x.CompanyName)
                                          .FirstOrDefault(),
                                  }).FirstOrDefault();

                    if (result == null)
                    {
                        return BadRequest("Employee not found.");
                    }

                    // Create a unique filename for the PDF
                    string uniqueFileName = $"Relievingletter_{result.EmployeeId}.pdf";
                    string filePath = Path.Combine(wwwRootPath, uniqueFileName);

                    // Save the PDF file
                    await System.IO.File.WriteAllBytesAsync(filePath, pdf);

                    // Update employee record with the generated file name
                    var empoff = _context.EmpRelievingletters.FirstOrDefault(e => e.Id == result.Id);
                    if (empoff != null)
                    {
                        empoff.RelievingletterFile = uniqueFileName;
                        await _context.SaveChangesAsync();

                        // Send email with the PDF attachment
                        string emailSubject = $"Relieving Letter for {result.Name}";
                        string emailBody = $"Dear {result.Name},\n\nWe appreciate your contributions during your time with {result.companyname}. Please find your attached Relieving Letter.";

                        await _IEmailService.SendEmailAsync(
                            result.WorkEmail,
                            emailSubject,
                            emailBody,
                            pdf,
                            uniqueFileName,
                            "application/pdf",
                            (int)adminlogin.Vendorid
                        );

                        // Return success message with the file name
                        return Json(new { success = true, message = "Relieving letter has been sent successfully.", fileName = uniqueFileName });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Data not found for the employee." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private byte[] GeneratePdf(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = DinkToPdf.Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 15, Bottom = 15, Left = 15, Right = 15 },
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" },
                FooterSettings = new FooterSettings
                {
                    FontName = "Arial, Helvetica, sans-serif",
                    //FontName = "Arial",
                    FontSize = 8,
                    Center = @"                
              N D Techland Private Limited
              Head Office: C-53, Sector-2, Noida-201301, U.P. | Branches: Noida | Delhi | Patna | Bangalore
              Web:http://www.ndtechland.com www.ndtechland.com | Email :info@ndtechland.com
                  
             "
                },
                LoadSettings = { BlockLocalFileAccess = false }
            };

            var htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }
        public async Task<JsonResult> getEmpattendancedays(int month, int year)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

                if (adminLogin == null)
                {
                    return new JsonResult(new { success = false, message = "Admin login not found" });
                }

                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                if (year > currentYear || (year == currentYear && month > currentMonth))
                {
                    return new JsonResult(new { success = false, message = "Attendance data for the selected month is not available." });
                }

                var attendanceDays = await _context.Attendancedays.FirstOrDefaultAsync(x => x.Vendorid == adminLogin.Vendorid);
                var Esidata = await _context.EmployeeEsicPayrollInfos.FirstOrDefaultAsync(x => x.Vendorid == adminLogin.Vendorid);

                if (attendanceDays == null)
                {
                    return new JsonResult(new { success = false, message = "Attendance days not found or invalid" });
                }
                decimal noOfDays = 0;
                noOfDays = Convert.ToDecimal(attendanceDays.Nodays);
                var employees = await _context.EmployeeRegistrations
                                              .Where(x => x.Vendorid == adminLogin.Vendorid && x.IsDeleted == false).OrderByDescending(x=>x.Id)
                                              .ToListAsync();

                if (!employees.Any())
                {
                    return new JsonResult(new { success = false, message = "No employees found for the vendor" });
                }

                var employeeIds = employees.Select(e => e.EmployeeId).ToList();
                var ctcData = (await _context.EmployeeSalaryDetails
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToListAsync())
               .GroupBy(x => x.EmployeeId)
               .ToDictionary(g => g.Key, g => g.First().AnnualCtc / 12 ?? 0);


                var employeeSalaryDetails = await _context.EmployeeSalaryDetails
               .Where(x => employeeIds.Contains(x.EmployeeId))
                   .ToListAsync(); // Fetch data once

                var EmployeeEpfData = employeeSalaryDetails
                    .GroupBy(x => x.EmployeeId)
                    .ToDictionary(g => g.Key, g => g.First().Epfpercentage ?? 0);

                var EmployeeEsiData = employeeSalaryDetails
                    .GroupBy(x => x.EmployeeId)
                    .ToDictionary(g => g.Key, g => g.First().Esipercentage ?? 0);

                var EmployeebasicData = employeeSalaryDetails
                    .GroupBy(x => x.EmployeeId)
                    .ToDictionary(g => g.Key, g => g.First().Basic / 12);

                var EmployeeMonthlyCtc = employeeSalaryDetails
                    .GroupBy(x => x.EmployeeId)
                    .ToDictionary(g => g.Key, g => g.First().MonthlyCtc ?? 0);


                var leaveCounts = await _context.ApplyLeaveNews
                                             .Where(leave => employeeIds.Contains(leave.UserId) &&
                                                             leave.Isapprove == 2 &&
                                                             leave.StartDate.Year == year &&
                                                             leave.StartDate.Month == month &&
                                                             leave.EndDate.Year == year &&
                                                             leave.EndDate.Month == month)
                                             .GroupBy(leave => leave.UserId)
                                             .Select(g => new
                                             {
                                                 UserId = g.Key,
                                                 TotalLeave = g.Sum(leave => leave.CountLeave),
                                                 PaidCountLeave = g.Sum(leave => leave.PaidCountLeave),
                                             }).ToListAsync();

                var shiftTimes = await _context.Officeshifts
                                                .Where(x => x.Vendorid == adminLogin.Vendorid)
                                                .ToListAsync();

                decimal totalMonthlyPay = 0.00M;
                var result = new List<object>();

                foreach (var employeesdata in employees)
                {
                    var leaveData = leaveCounts.FirstOrDefault(l => l.UserId == employeesdata.EmployeeId);
                    decimal totalLeave = leaveData?.TotalLeave ?? 0;
                    decimal PaidtotalLeave = leaveData?.PaidCountLeave ?? 0;

                    var EmpTotalWorkingHours = _context.EmployeeCheckInRecords
                        .Where(att => att.EmpId == employeesdata.EmployeeId &&
                                      att.CheckIntime.HasValue &&
                                      att.CheckOuttime.HasValue &&
                                      att.CheckIntime.Value.Year == year &&
                                      att.CheckIntime.Value.Month == month).ToList();

                    if (!EmpTotalWorkingHours.Any())
                    {
                        result.Add(new
                        {
                            EmployeeId = employeesdata.EmployeeId,
                            LeaveRemaining = 0,
                            MonthlyPay = 0,
                            SalaryDeductionDays = 0,
                            NumberOfLateMarks = 0,
                            EmployeeEpf = 0,
                            EmployeeEsi = 0,
                            Lop = 0

                        });
                    }
                    else
                    {
                        var shiftTime = shiftTimes.FirstOrDefault(st => st.Id == employeesdata.OfficeshiftTypeid);

                        decimal shiftworkingHours = 0;
                        decimal NumberOfLateMarks = 0;
                        decimal salaryDeductionDays = 0;
                        decimal TotalsalaryDeduction =0;
                        decimal EmployeeEpf = 0;
                        decimal EmployeeEsi = 0;

                        TimeSpan lateThreshold = TimeSpan.FromMinutes(30);
                        TimeSpan halfDayThreshold = TimeSpan.FromHours(2);

                        if (shiftTime != null)
                        {
                            DateTime startTime = DateTime.Parse(shiftTime.Starttime);
                            DateTime endTime = DateTime.Parse(shiftTime.Endtime);
                            TimeSpan shiftDuration = endTime - startTime;
                            shiftworkingHours = Math.Max((decimal)shiftDuration.TotalHours, 0);
                        }

                        decimal lateMarkCount = 0;
                        decimal totalpresentdays = 0;
                        bool ischeck = false;
                        foreach (var item in EmpTotalWorkingHours)
                        {
                            decimal TotalWorkingHours = (decimal)Math.Round((item.CheckOuttime.Value - item.CheckIntime.Value).TotalHours, 2);
                            decimal totalLateHours = shiftworkingHours - TotalWorkingHours;

                            if (shiftworkingHours > TotalWorkingHours)
                            {
                                lateMarkCount += (int)(totalLateHours / 0.5m);
                                if (totalLateHours >= 0.5m)
                                {
                                    if (lateMarkCount == 3 && ischeck == false)
                                    {
                                        NumberOfLateMarks += 1;
                                        ischeck = true;
                                    }
                                    else if (lateMarkCount > 3)
                                    {
                                        NumberOfLateMarks += (1 / 3.0m);
                                    }
                                }
                            }
                            totalpresentdays = EmpTotalWorkingHours.Count();

                        }

                        List<DateTime> sundays = new List<DateTime>();
                        DateTime startOfMonth = new DateTime(year, month, 1);
                        DateTime firstSunday = startOfMonth;
                        while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                        {
                            firstSunday = firstSunday.AddDays(1);
                        }
                        DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                        for (DateTime date = firstSunday; date <= lastDayOfMonth; date = date.AddDays(7))
                        {
                            sundays.Add(date);
                        }
                        foreach (var sunday in sundays)
                        {
                            Console.WriteLine(sunday.ToString("yyyy-MM-dd"));
                        }

                        decimal sundayCount = sundays.Count;
                        decimal noofleave = noOfDays - (totalpresentdays + sundayCount + PaidtotalLeave);

                        // TotalsalaryDeduction = Math.Round((decimal)(totalpresentdays + sundayCount + PaidtotalLeave - totalLeave - NumberOfLateMarks), 2);
                        decimal  salaryDeduction = Math.Round((decimal)(totalpresentdays + sundayCount) + PaidtotalLeave - totalLeave - NumberOfLateMarks, 2);

                        TotalsalaryDeduction = salaryDeduction + noofleave;
                        decimal monthlyPay = 0;
                        decimal empbasic = 0;
                        decimal totalSalary = 0;
                        decimal Lop = 0;
                        if (TotalsalaryDeduction > 0 && ctcData.TryGetValue(employeesdata.EmployeeId, out var monthlyCtc))
                        {
                            monthlyPay = decimal.Round((monthlyCtc / noOfDays) * TotalsalaryDeduction, 2);
                            Lop = decimal.Round(monthlyCtc - monthlyPay, 2);
                            if (monthlyPay > 0)
                            {
                                EmployeeEpf = 0;
                                EmployeeEsi = 0;
                                empbasic = 0;

                                if (EmployeebasicData.TryGetValue(employeesdata.EmployeeId, out var basic))
                                {
                                    empbasic = decimal.Round((basic / noOfDays) * TotalsalaryDeduction, 2);
                                }
                                if (EmployeeMonthlyCtc.TryGetValue(employeesdata.EmployeeId, out var MonthlyCtc))
                                {
                                    totalSalary = decimal.Round((decimal)MonthlyCtc, 2);
                                }
                                if (totalSalary < Esidata.EsicAmount)
                                {
                                    if (EmployeeEsiData.TryGetValue(employeesdata.EmployeeId, out var esiPercentage))
                                    {
                                        EmployeeEsi = decimal.Round((empbasic * esiPercentage) / 100, 2);
                                        monthlyPay -= EmployeeEsi;
                                    }
                                    if (EmployeeEpfData.TryGetValue(employeesdata.EmployeeId, out var epfPercentage))
                                    {
                                        EmployeeEpf = decimal.Round((empbasic * epfPercentage) / 100, 2);
                                        monthlyPay -= EmployeeEpf;
                                    }
                                }
                                else
                                {

                                    if (EmployeeEpfData.TryGetValue(employeesdata.EmployeeId, out var epfPercentage))
                                    {
                                        EmployeeEpf = decimal.Round((empbasic * epfPercentage) / 100, 2);
                                        monthlyPay -= EmployeeEpf;
                                    }
                                    EmployeeEsi = 0;
                                    //monthlyPay -= Lop;
                                }
                            }
                        }

                        totalMonthlyPay += monthlyPay;

                        result.Add(new
                        {
                            EmployeeId = employeesdata.EmployeeId,
                            LeaveRemaining = TotalsalaryDeduction,
                            MonthlyPay = monthlyPay,
                            SalaryDeductionDays = salaryDeductionDays,
                            NumberOfLateMarks = NumberOfLateMarks,
                            EmployeeEpf = EmployeeEpf,
                            EmployeeEsi = EmployeeEsi,
                            Lop = Lop,
                        });
                    }
                }

                return new JsonResult(new { Employees = result, TotalMonthlyPay = totalMonthlyPay });
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public JsonResult EmpEpfesilist()
        {
            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminLogin = _context.AdminLogins
                                          .FirstOrDefault(x => x.Id == Userid);

            var esicdata = _context.EmployeeEsicPayrollInfos.Where(e => e.Vendorid == adminLogin.Vendorid).FirstOrDefault();
            var epfdata = _context.EmployeeEpfPayrollInfos.Where(e => e.Vendorid == adminLogin.Vendorid).FirstOrDefault();

            decimal? esicPercentage = esicdata?.Esicpercentage;
            decimal? epfPercentage = epfdata?.Epfpercentage;
            decimal? esicAmount = esicdata?.EsicAmount;

            var result = new
            {
                EsicPercentage = esicPercentage,
                EpfPercentage = epfPercentage,
                EsicAmount = esicAmount
            };

            return new JsonResult(result);
        }
        [HttpPost]
        public JsonResult UpdateEmployeeIsactive(int id, bool isActive)
        {
            var emp = _context.EmployeeRegistrations.FirstOrDefault(x => x.Id == id);
            if (emp != null)
            {
                emp.Isactive = isActive;
                _context.SaveChanges();
                return Json(new
                {
                    success = true,
                    message = isActive ? "Employee Activated successfully." : "Employee Deactivated successfully."
                });
            }
            return Json(new { success = false, message = "Employee not found." });
        }
        public JsonResult GetEmployeetax(decimal GrossPay)
        {
            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminLogin = _context.AdminLogins.FirstOrDefault(x => x.Id == Userid);

            if (adminLogin == null)
            {
                return new JsonResult(new { success = false, message = "User not found." });
            }

            var Vendor = _context.VendorRegistrations.FirstOrDefault(e => e.Id == adminLogin.Vendorid);

            if (Vendor == null)
            {
                return new JsonResult(new { success = false, message = "Vendor not found." });
            }

            var professionaltaxdata = _context.Professionaltaxes
                .Where(pt => pt.Minamount <= GrossPay && pt.Maxamount >= GrossPay)
                .ToList();

            if (!professionaltaxdata.Any())
            {
                return new JsonResult(new { success = false, message = "No professional tax data found for the provided GrossPay." });
            }

            bool check = (bool)Vendor.Isprofessionaltax;
            decimal taxpercentage = 0;
            decimal taxAmount = 0;
            var pt = professionaltaxdata.FirstOrDefault();
            if (pt != null)
            {
                taxpercentage = pt.Amountpercentage ?? 0;
                taxAmount = (GrossPay * taxpercentage) / 100;
            }

            var result = new
            {
                checkIsprofessionaltax = check,
                taxpercentage = taxpercentage,
                taxAmount = taxAmount
            };

            return new JsonResult(new { success = true, result });
        }
        public JsonResult GetOfferLetterDetails(int offerLetterId)
        {
            var offerLetter = _context.Offerletters
                .Where(o => o.Id == offerLetterId)
                .Select(o => new
                {
                    FullName = o.Name,
                    DepartmentID = o.DepartmentId,
                    DesignationID = o.DesignationId,
                    CityID = o.CityId,
                    StateID = o.StateId,
                    AddressLine1 = o.CandidateAddress,
                    AddressLine2 = o.CandidateAddress,
                    PersonalEmailAddress = o.CandidateEmail,
                    AnnualCTC = o.AnnualCtc,
                    Pincode = o.CandidatePincode,
                }).FirstOrDefault();

            if (offerLetter == null)
            {
                return Json(null);
            }

            var nameParts = (offerLetter.FullName ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string firstName = nameParts.Length > 0 ? nameParts[0] : "";
            string middleName = nameParts.Length > 2 ? nameParts[1] : "";
            string lastName = nameParts.Length > 1 ? nameParts[^1] : "";

            return Json(new
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                DepartmentID = offerLetter.DepartmentID,
                DesignationID = offerLetter.DesignationID,
                CityID = offerLetter.CityID,
                StateID = offerLetter.StateID,
                AddressLine1 = offerLetter.AddressLine1,
                AddressLine2 = offerLetter.AddressLine2,
                PersonalEmailAddress = offerLetter.PersonalEmailAddress,
                AnnualCTC = offerLetter.AnnualCTC,
                Pincode = offerLetter.Pincode,
            });
        }
        [HttpGet, Route("Employee/PreviewEmployeelist")]
        public async Task<IActionResult> PreviewEmployeelist()
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") == null)
                {
                    return RedirectToAction("Login", "Admin");
                }

                string userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return RedirectToAction("Login", "Admin");
                }
                var adminlogin = _context.AdminLogins.Where(x => x.Id == userId).FirstOrDefault();

                List<Priewempdata> response = await _ICrmrpo.PreviewEmployeeList((int)adminlogin.Vendorid);

                ViewBag.Isactive = response.FirstOrDefault()?.Isactive;
                ViewBag.UserName = HttpContext.Session.GetString("UserName");

                return View(response);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        [HttpPost, Route("Employee/UpdateIsIncrement")]
        public JsonResult UpdateIsIncrement(string Emp_Reg_ID)
        {
            var privioussalarydetail = _context.EmployeeSalaryDetails
                .Where(x => x.EmployeeId == Emp_Reg_ID)
                .FirstOrDefault();

            if (privioussalarydetail == null)
            {
                return Json(null);
            }
            OldEmployeeSalaryDetail osd = new OldEmployeeSalaryDetail
            {
                EmployeeId = privioussalarydetail.EmployeeId,
                EmployeeName = privioussalarydetail.EmployeeName,
                Basic = privioussalarydetail.Basic,
                HouseRentAllowance = privioussalarydetail.HouseRentAllowance,
                TravellingAllowance = privioussalarydetail.TravellingAllowance,
                Esic = privioussalarydetail.Esic,
                Epf = privioussalarydetail.Epf,
                IsDeleted = privioussalarydetail.IsDeleted,
                MonthlyGrossPay = privioussalarydetail.MonthlyGrossPay,
                MonthlyCtc = privioussalarydetail.MonthlyCtc,
                EmpId = privioussalarydetail.EmpId,
                AnnualCtc = privioussalarydetail.AnnualCtc,
                Professionaltax = privioussalarydetail.Professionaltax,
                Incentive = privioussalarydetail.Incentive,
                Servicecharge = privioussalarydetail.Servicecharge,
                SpecialAllowance = privioussalarydetail.SpecialAllowance,
                Gross = privioussalarydetail.Gross,
                Amount = privioussalarydetail.Amount,
                Tdspercentage = privioussalarydetail.Tdspercentage,
                Conveyanceallowance = privioussalarydetail.Conveyanceallowance,
                FixedAllowance = privioussalarydetail.FixedAllowance,
                Medical = privioussalarydetail.Medical,
                Composite = privioussalarydetail.Composite,
                VariablePay = privioussalarydetail.VariablePay,
                EmployerContribution = privioussalarydetail.EmployerContribution,
                Tdsvalue = privioussalarydetail.Tdsvalue,
                Basicpercentage = privioussalarydetail.Basicpercentage,
                Hrapercentage = privioussalarydetail.Hrapercentage,
                Conveyancepercentage = privioussalarydetail.Conveyancepercentage,
                Medicalpercentage = privioussalarydetail.Medicalpercentage,
                Variablepercentage = privioussalarydetail.Variablepercentage,
                EmployerContributionpercentage = privioussalarydetail.EmployerContributionpercentage,
                Epfpercentage = privioussalarydetail.Epfpercentage,
                Esipercentage = privioussalarydetail.Esipercentage,
                IncrementPercentage = privioussalarydetail.IncrementPercentage,
                Createddate = DateTime.Now.Date,
                IsIncrement = true
            };

            _context.OldEmployeeSalaryDetails.Add(osd);

            if (privioussalarydetail.EmployeeId != null)
            {
                privioussalarydetail.IsIncrement = true;
                privioussalarydetail.Basic = 0;
                privioussalarydetail.HouseRentAllowance = 0;
                privioussalarydetail.TravellingAllowance = 0;
                privioussalarydetail.Esic = 0;
                privioussalarydetail.Epf = 0;
                privioussalarydetail.MonthlyGrossPay = 0;
                privioussalarydetail.MonthlyCtc = 0;
                privioussalarydetail.EmpId = 0;
                privioussalarydetail.AnnualCtc = 0;
                privioussalarydetail.Professionaltax = 0;
                privioussalarydetail.Incentive = 0;
                privioussalarydetail.Servicecharge = 0;
                privioussalarydetail.SpecialAllowance = 0;
                privioussalarydetail.Gross = 0;
                privioussalarydetail.Amount = 0;
                privioussalarydetail.Tdspercentage = 0;
                privioussalarydetail.Conveyanceallowance = 0;
                privioussalarydetail.FixedAllowance = 0;
                privioussalarydetail.Medical = 0;
                privioussalarydetail.Composite = 0;
                privioussalarydetail.VariablePay = 0;
                privioussalarydetail.EmployerContribution = 0;
                privioussalarydetail.Tdsvalue = 0;
                privioussalarydetail.Basicpercentage = 0;
                privioussalarydetail.Hrapercentage = 0;
                privioussalarydetail.Conveyancepercentage = 0;
                privioussalarydetail.Medicalpercentage = 0;
                privioussalarydetail.Variablepercentage = 0;
                privioussalarydetail.EmployerContributionpercentage = 0;
                privioussalarydetail.Epfpercentage = 0;
                privioussalarydetail.Esipercentage = 0;
                privioussalarydetail.IncrementPercentage = 0;
                _context.EmployeeSalaryDetails.Update(privioussalarydetail);

            }
            _context.SaveChanges();

            return Json(new { success = true });
        }

        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                bool checktomulticustomer = _context.VendorRegistrations.Any(x => x.Id == adminlogin.Vendorid && x.SelectCompany == true);
                // Step 1: Create a DataTable and add columns
                DataTable dt = new DataTable();


                // Define all columns (original names)
                var columns = new List<string>
        {
            "FirstName", "MiddleName", "LastName",
            "DateOfJoining", "WorkEmail", "GenderID", "WorkLocationID", "DesignationID", "DepartmentID",
            "stateId", "offerletterid", "officeshiftTypeid", "AnnualCTC", "Basic", "HouseRentAllowance",
            "TravellingAllowance", "ESIC", "EPF", "MonthlyGrossPay", "MonthlyCTC",
            "SpecialAllowance", "tdspercentage", "conveyanceallowance",
            "Medical", "VariablePay", "EmployerContribution", "tdsvalue", "Basicpercentage", "HRApercentage",
            "Conveyancepercentage", "Medicalpercentage", "Variablepercentage", "EmployerContributionpercentage",
            "EPfpercentage", "Esipercentage", "Personal_Email_Address", "Mobile_Number", "Date_Of_Birth",
            "Father_Name", "PAN", "Address_Line_1", "Address_Line_2", "City", "StateID", "Pincode",
            "Account_Holder_Name", "Bank_Name", "Account_Number", "Re_Enter_Account_Number", "IFSC", "EPF_Number",
             "Employee_Contribution_Rate", "Account_Type_ID", "nominee" ,"EmployeeId"
        };
                if (checktomulticustomer)
                {
                    columns.Add("Customer Name");
                    columns.Add("Customer Location");
                }


                foreach (var column in columns)
                {
                    dt.Columns.Add(column);
                }

                // Step 2: Add sample data
                for (int i = 1; i <= 2; i++) // Add 10 rows of sample data
                {
                    DataRow row = dt.NewRow();
                    foreach (var column in columns)
                    {
                        row[column] = $"{column}_SampleData_{i}";
                    }
                    dt.Rows.Add(row);
                }

                // Step 3: Define custom headers
                Dictionary<string, string> customHeaders = new Dictionary<string, string>
        {
            { "FirstName", "First Name" },
            { "MiddleName", "Middle Name" },
            { "LastName", "Last Name" },
            { "DateOfJoining", "Date of Joining" },
            { "WorkEmail", "Work Email" },
            { "GenderID", "Gender" },
            { "WorkLocationID", "Work Location" },
            { "DesignationID", "Designation" },
            { "DepartmentID", "Department" },
            { "stateId", "Company State ID" },
            { "offerletterid", "Offer Letter ID" },
            { "officeshiftTypeid", "Office Shift Type ID" },
            { "AnnualCTC", "Annual CTC" },
            { "Basic", "Basic Salary" },
            { "HouseRentAllowance", "House Rent Allowance" },
            { "TravellingAllowance", "Travelling Allowance" },
            { "ESIC", "ESIC Contribution" },
            { "EPF", "EPF Contribution" },
            { "MonthlyGrossPay", "Monthly Gross Pay" },
            { "MonthlyCTC", "Monthly CTC" },
            { "SpecialAllowance", "Special Allowance" },
            { "tdspercentage", "TDS Percentage" },
            { "conveyanceallowance", "Conveyance Allowance" },
            { "Medical", "Medical Allowance" },
            { "VariablePay", "Variable Pay" },
            { "EmployerContribution", "Employer Contribution" },
            { "tdsvalue", "TDS Value" },
            { "Basicpercentage", "Basic Percentage" },
            { "HRApercentage", "HRA Percentage" },
            { "Conveyancepercentage", "Conveyance Percentage" },
            { "Medicalpercentage", "Medical Percentage" },
            { "Variablepercentage", "Variable Percentage" },
            { "EmployerContributionpercentage", "Employer Contribution Percentage" },
            { "EPfpercentage", "EPF Percentage" },
            { "Esipercentage", "ESI Percentage" },
            { "Personal_Email_Address", "Personal Email Address" },
            { "Mobile_Number", "Mobile Number" },
            { "Date_Of_Birth", "Date of Birth" },
            { "Father_Name", "Father's Name" },
            { "PAN", "PAN Number" },
            { "Address_Line_1", "Address Line 1" },
            { "Address_Line_2", "Address Line 2" },
            { "City", "City" },
            { "StateID", "State ID" },
            { "Pincode", "Pincode" },
            { "Account_Holder_Name", "Account Holder Name" },
            { "Bank_Name", "Bank Name" },
            { "Account_Number", "Account Number" },
            { "Re_Enter_Account_Number", "Re-enter Account Number" },
            { "IFSC", "IFSC Code" },
            { "EPF_Number", "EPF Number" },
            { "Employee_Contribution_Rate", "Employee Contribution Rate" },
            { "Account_Type_ID", "Account Type ID" },
            { "nominee", "Nominee Name" },
            { "EmployeeId", "Employee Id" }
        };
                if (checktomulticustomer)
                {
                    customHeaders.Add("CustomerCode", "Customer Name");
                    customHeaders.Add("CustomerName", "Customer Location");
                }

                // Step 4: Create an Excel workbook
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Employee Data");

                    // Step 5: Add headers with custom names
                    int colIndex = 1;
                    foreach (var column in columns)
                    {
                        string header = customHeaders.ContainsKey(column) ? customHeaders[column] : column;
                        worksheet.Cell(1, colIndex).Value = header; // Use custom header or fallback to original name
                        worksheet.Cell(1, colIndex).Style.Font.Bold = true;
                        worksheet.Cell(1, colIndex).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(1, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        colIndex++;
                    }

                    // Step 6: Add rows
                    for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                    {
                        colIndex = 1;
                        foreach (var column in columns)
                        {
                            worksheet.Cell(rowIndex + 2, colIndex).Value = dt.Rows[rowIndex][column];
                            colIndex++;
                        }
                    }

                    // Step 7: Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    // Step 8: Export the Excel file
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeDetails.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Server Error: " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            try
            {
                var model = new EmpMultiform();
                if (file == null || file.Length == 0)
                {
                    throw new Exception("No file uploaded or the file is empty.");
                }

                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    throw new Exception("Invalid file format. Please upload an Excel file (.xlsx or .xls).");
                }


                DataTable dataTable = new DataTable();

                // Read Excel file
                using (var stream = file.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Read the first worksheet
                    var rows = worksheet.RowsUsed();

                    // Read headers from the first row and create DataTable columns
                    var headers = rows.First().Cells().Select(cell => cell.Value.ToString()).ToArray();
                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    // Populate DataTable rows
                    foreach (var row in rows.Skip(1)) // Skip header row
                    {
                        var dataRow = dataTable.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dataRow[i] = row.Cell(i + 1).Value; // Excel is 1-based index
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                string Mode = "INS";
                string Empid = "";
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                bool checktomulticustomer = _context.VendorRegistrations.Any(x => x.Id == adminlogin.Vendorid && x.SelectCompany == true);


                //if (!string.IsNullOrEmpty(model.Emp_Reg_ID))
                //{
                //    Mode = "UPD";
                //    Empid = model.Emp_Reg_ID;
                //}
                //else
                //{
                //    Empid = GenerateEmployeeId();
                //    model.EmployeeId = Empid;

                //    var existingEmployee = _context.EmployeeRegistrations.FirstOrDefault(x => x.WorkEmail == model.WorkEmail);
                //    if (existingEmployee != null)
                //    {
                //        TempData["Message"] = "WorkEmail already exists";
                //        return View();
                //    }
                //}
                var count = 0;
                List<ExcelErrorModel> excelErrorModels = new List<ExcelErrorModel>();
                foreach (DataRow row in dataTable.Rows)
                {
                    //290125
                    count++;
                    var mobno = row[36]?.ToString() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(row[0]?.ToString()))
                    {
                        excelErrorModels.Add(new ExcelErrorModel
                        {
                            ErrorType = "First Name",
                            AffectedRow = count,
                            Description = $"First Name can not be empty"
                        });
                    }
                    if (!mobno.All(char.IsDigit))
                    {
                        excelErrorModels.Add(new ExcelErrorModel
                        {
                            ErrorType = "Mobile Number",
                            AffectedRow = count,
                            Description = $"Mobile Number {mobno} contains invalid characters. Only digits are allowed."
                        });
                    }
                    else if (_context.EmployeePersonalDetails.Any(e => e.MobileNumber == mobno))
                    {
                        excelErrorModels.Add(new ExcelErrorModel
                        {
                            ErrorType = "Mobile Number",
                            AffectedRow = count,
                            Description = $"Mobile Number {mobno} already exists."
                        });
                    }
                    else if (mobno.ToString().Length != 10)
                    {
                        excelErrorModels.Add(new ExcelErrorModel
                        {
                            ErrorType = "Mobile Number",
                            AffectedRow = count,
                            Description = $"Mobile no. {mobno} must be exactly 10 digits."
                        });
                    }
                    // Email Validation
                    var workemail = row[4]?.ToString() ?? string.Empty;

                    if (!Regex.IsMatch(workemail, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase))
                    {
                        excelErrorModels.Add(new ExcelErrorModel
                        {
                            ErrorType = "Work Email Address",
                            AffectedRow = count,
                            Description = $"Work email address {workemail} is invalid. Please provide a valid email."
                        });
                    }
                    var validGenders = new List<string> { "male", "female", "other" };
                    // Validate Gender
                    if (!validGenders.Contains(row[5]?.ToString().ToLower()))
                    {
                        excelErrorModels.Add(new ExcelErrorModel
                        {
                            ErrorType = "Gender",
                            AffectedRow = count,
                            Description = $"Kindly check the employee Gender: {row[5]?.ToString()}."
                        });
                    }
                    // Validate for pincode
                    if ((bool)row[44]?.ToString().Any(c => !char.IsDigit(c)))
                    {
                        excelErrorModels.Add(new ExcelErrorModel
                        {
                            ErrorType = "Pincode",
                            AffectedRow = count,
                            Description = $"Pincode {row[44]?.ToString()} contains invalid characters. Only digits are allowed."
                        });
                    }
                    else if (row[44]?.ToString().Length != 6)
                    {
                        excelErrorModels.Add(new ExcelErrorModel
                        {
                            ErrorType = "Pincode",
                            AffectedRow = count,
                            Description = $"Pincode {row[44]?.ToString()} must be exactly 6 digits."
                        });
                    }

                }

                if (excelErrorModels.Any())
                {
                    TempData["HasErrors"] = true;
                    TempData["ExcelErrors"] = Newtonsoft.Json.JsonConvert.SerializeObject(excelErrorModels);
                    return RedirectToAction("EmployeeRegistration");
                }
                //end 290125
                foreach (DataRow row in dataTable.Rows)
                {

                    //  Empid = GenerateEmployeeId();
                    //string gender = row[38]?.ToString();
                    //model.EmployeeId = Empid;

                    Empid = GenerateEmployeeId();
                    model.EmployeeId = Empid;

                    model.Vendorid = adminlogin.Vendorid != 0 ? adminlogin.Vendorid : 0;
                    model.CustomerCompanyid = 27;
                    // Bind data using column indexes (replace hardcoded column numbers as necessary)
                    model.FirstName = row[0]?.ToString();
                    model.MiddleName = row[1]?.ToString();
                    model.LastName = row[2]?.ToString();
                    model.DateOfJoining = row[3] != DBNull.Value ? Convert.ToDateTime(row[3]) : DateTime.MinValue;
                    model.WorkEmail = row[4]?.ToString();
                    model.GenderID = row[5] != DBNull.Value ? Convert.ToInt32(_context.GenderMasters.Where(x => x.GenderName.ToLower() == row[5].ToString().ToLower()).FirstOrDefault().Id) : (int?)null;
                    //model.GenderID = row[5] != DBNull.Value ? Convert.ToInt32(row[5]) : (int?)null;
                    model.WorkLocationID = row[6] != DBNull.Value ? Convert.ToInt32(_context.Cities.Where(x => x.City1.ToLower() == row[6].ToString().ToLower()).FirstOrDefault().Id) : 0;
                    //model.WorkLocationID = row[6] != DBNull.Value ? Convert.ToInt32(row[6]) : 0;
                    model.DesignationID = row[7] != DBNull.Value ? Convert.ToInt32(_context.DesignationMasters.Where(x => x.DesignationName.ToLower() == row[7].ToString().ToLower()).FirstOrDefault().Id) : 0;
                    //model.DesignationID = row[7] != DBNull.Value ? Convert.ToInt32(row[7]) : 0;
                    model.DepartmentID = row[8] != DBNull.Value ? Convert.ToInt32(_context.DepartmentMasters.Where(x => x.DepartmentName.ToLower() == row[8].ToString().ToLower()).FirstOrDefault().Id) : 0;
                    //model.DepartmentID = row[8] != DBNull.Value ? Convert.ToInt32(row[8]) : 0;
                    model.stateId = row[9] != DBNull.Value ? Convert.ToInt32(_context.StateMasters.Where(x => x.StateName.ToLower() == row[9].ToString().ToLower()).FirstOrDefault().Id) : 0;
                    //model.stateId = row[9] != DBNull.Value ? Convert.ToInt32(row[9]) : 0;
                    model.offerletterid = row[10] != DBNull.Value ? Convert.ToInt32(_context.Offerletters.Where(x => x.Name.ToLower() == row[10].ToString().ToLower()).FirstOrDefault().Id) : 0;
                    //model.offerletterid = row[10] != DBNull.Value ? Convert.ToInt32(row[10]) : 0;

                    model.officeshiftTypeid = row[11] != DBNull.Value ? Convert.ToInt32(row[11]) : 0;
                    //model.officeshiftTypeid = row[11] != DBNull.Value ? Convert.ToInt32(row[11]) : 0;

                    // Salary Details
                    model.AnnualCTC = row[12] != DBNull.Value ? Convert.ToDecimal(row[12]) : 0;
                    model.Basic = row[13] != DBNull.Value ? Convert.ToDecimal(row[13]) : (decimal?)null;
                    model.HouseRentAllowance = row[14] != DBNull.Value ? Convert.ToDecimal(row[14]) : (decimal?)null;
                    model.TravellingAllowance = row[15] != DBNull.Value ? Convert.ToDecimal(row[15]) : (decimal?)null;
                    model.ESIC = row[16] != DBNull.Value ? Convert.ToDecimal(row[16]) : (decimal?)null;
                    model.EPF = row[17] != DBNull.Value ? Convert.ToDecimal(row[17]) : (decimal?)null;
                    model.MonthlyGrossPay = row[18] != DBNull.Value ? Convert.ToDecimal(row[18]) : (decimal?)null;
                    model.MonthlyCTC = row[19] != DBNull.Value ? Convert.ToDecimal(row[19]) : 0;
                    model.SpecialAllowance = row[20] != DBNull.Value ? Convert.ToDecimal(row[20]) : (decimal?)null;
                    model.Tdspercentage = row[21] != DBNull.Value ? Convert.ToDecimal(row[21]) : (decimal?)null;
                    model.Conveyanceallowance = row[22] != DBNull.Value ? Convert.ToDecimal(row[22]) : (decimal?)null;
                    model.Medical = row[23] != DBNull.Value ? Convert.ToDecimal(row[23]) : (decimal?)null;
                    model.VariablePay = row[24] != DBNull.Value ? Convert.ToDecimal(row[24]) : (decimal?)null;
                    model.EmployerContribution = row[25] != DBNull.Value ? Convert.ToDecimal(row[25]) : (decimal?)null;
                    model.Tdsvalue = row[26] != DBNull.Value ? Convert.ToDecimal(row[26]) : (decimal?)null;
                    model.Basicpercentage = row[27] != DBNull.Value ? Convert.ToDecimal(row[27]) : (decimal?)null;
                    model.Hrapercentage = row[28] != DBNull.Value ? Convert.ToDecimal(row[28]) : (decimal?)null;
                    model.Conveyancepercentage = row[29] != DBNull.Value ? Convert.ToDecimal(row[29]) : (decimal?)null;
                    model.Medicalpercentage = row[30] != DBNull.Value ? Convert.ToDecimal(row[30]) : (decimal?)null;
                    model.Variablepercentage = row[31] != DBNull.Value ? Convert.ToDecimal(row[31]) : (decimal?)null;
                    model.EmployerContributionpercentage = row[32] != DBNull.Value ? Convert.ToDecimal(row[32]) : (decimal?)null;
                    model.Epfpercentage = row[33] != DBNull.Value ? Convert.ToDecimal(row[33]) : (decimal?)null;
                    model.Esipercentage = row[34] != DBNull.Value ? Convert.ToDecimal(row[34]) : (decimal?)null;

                    // Personal Info
                    model.PersonalEmailAddress = row[35]?.ToString();
                    model.MobileNumber = row[36] != DBNull.Value ? (long.TryParse(row[36]?.ToString(), out long result) ? result : 0) : 0;
                    model.DateOfBirth = row[37] != DBNull.Value ? Convert.ToDateTime(row[37]) : DateTime.MinValue;
                    model.FatherName = row[38]?.ToString();
                    model.PAN = row[39]?.ToString();
                    model.AddressLine1 = row[40]?.ToString();
                    model.AddressLine2 = row[41]?.ToString();
                    model.City = row[42] != DBNull.Value ? Convert.ToString(_context.Cities.Where(x => x.City1.ToLower() == row[42].ToString().ToLower()).FirstOrDefault().Id) : "0";
                    //model.City = row[42]?.ToString();
                    model.StateID = row[43] != DBNull.Value ? Convert.ToString(_context.StateMasters.Where(x => x.StateName.ToLower() == row[43].ToString().ToLower()).FirstOrDefault().Id) : "0";
                    //model.StateID = row[43]?.ToString();
                    model.Pincode = row[44]?.ToString();

                    // Bank Details
                    model.AccountHolderName = row[45]?.ToString();
                    model.BankName = row[46]?.ToString();
                    model.AccountNumber = row[47]?.ToString();
                    model.ReEnterAccountNumber = row[48]?.ToString();
                    model.IFSC = row[49]?.ToString();
                    model.EPF_Number = row[50]?.ToString();
                    model.Employee_Contribution_Rate = row[51]?.ToString();
                    model.AccountTypeID = row[52] != DBNull.Value ? Convert.ToInt32(row[52]) : 0;
                    model.nominee = row[53]?.ToString();

                    model.EmployeeId = row[54]?.ToString();
                    Empid = model.EmployeeId;


                    if (checktomulticustomer)
                    {
                        model.CustomerCompanyid = row[54] != DBNull.Value ? Convert.ToInt32(row[52]) : 0;
                        //model. = row[55]?.ToString();
                    }

                    if (model != null)
                    {
                        var response = await _ICrmrpo.EmpRegistration(model, Mode, Empid, Userid);
                    }
                }
                TempData["Message"] = "WorkEmail already exists";
                return Json(new { Success = true, Message = "Data imported successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error: " + ex.Message });
            }
        }

        public async Task<JsonResult> EmpMonthlyadjustmentAttendance(int month, int year, string employeeId, decimal noOfDays = 0)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

            if (adminLogin == null)
            {
                return new JsonResult(new { success = false, message = "Admin login not found" });
            }

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            if (year > currentYear || (year == currentYear && month > currentMonth))
            {
                return new JsonResult(new { success = false, message = "Attendance data for the selected month is not available." });
            }

            var attendanceDays = await _context.Attendancedays.FirstOrDefaultAsync(x => x.Vendorid == adminLogin.Vendorid);
            var Esidata = await _context.EmployeeEsicPayrollInfos.FirstOrDefaultAsync(x => x.Vendorid == adminLogin.Vendorid);

            if (attendanceDays == null)
            {
                return new JsonResult(new { success = false, message = "Attendance days not found or invalid" });
            }
            decimal monthlyPay = 0;
            decimal TotalnoOfDays = 0;
            TotalnoOfDays = Convert.ToDecimal(attendanceDays.Nodays);
            if (noOfDays > TotalnoOfDays)
            {
                noOfDays = 0;
                return new JsonResult(new { success = false, message = "Number is greater than " + TotalnoOfDays });

            }
            else
            {
                noOfDays = (noOfDays == 0) ? 0 : noOfDays;
            }

            var employee = await _context.EmployeeRegistrations
                                         .FirstOrDefaultAsync(x => x.Vendorid == adminLogin.Vendorid
                                                                 && x.IsDeleted == false
                                                                 && x.EmployeeId == employeeId);

            if (employee == null)
            {
                return new JsonResult(new { success = false, message = "Employee not found for the vendor" });
            }

            var ctcData = await _context.EmployeeSalaryDetails
                                        .Where(x => x.EmployeeId == employeeId)
                                        .Select(x => x.AnnualCtc / 12 ?? 0)
                                        .FirstOrDefaultAsync();

            var EmployeeEpfData = await _context.EmployeeSalaryDetails
                                                .Where(x => x.EmployeeId == employeeId)
                                                .Select(x => x.Epfpercentage ?? 0)
                                                .FirstOrDefaultAsync();

            var EmployeeEsiData = await _context.EmployeeSalaryDetails
                                                .Where(x => x.EmployeeId == employeeId)
                                                .Select(x => x.Esipercentage ?? 0)
                                                .FirstOrDefaultAsync();
            var EmployeebasicData = await _context.EmployeeSalaryDetails
                                   .Where(x => x.EmployeeId == employeeId)
                                   .Select(x => (x.Basic) / 12)
                                   .FirstOrDefaultAsync();
            var EmployeeMonthlyCtc = await _context.EmployeeSalaryDetails
                                   .Where(x => x.EmployeeId == employeeId)
                                   .Select(x => x.MonthlyCtc)
                                   .FirstOrDefaultAsync();


            var shiftTime = await _context.Officeshifts
                                          .Where(x => x.Vendorid == adminLogin.Vendorid && x.Id == employee.OfficeshiftTypeid)
                                          .FirstOrDefaultAsync();

            decimal shiftworkingHours = 0;
            decimal NumberOfLateMarks = 0;
            decimal salaryDeductionDays = 0;
            decimal TotalsalaryDeduction = 0;
            decimal EmployeeEpf = 0;
            decimal EmployeeEsi = 0;
            decimal Lop = 0;
            if (shiftTime != null)
            {
                DateTime startTime = DateTime.Parse(shiftTime.Starttime);
                DateTime endTime = DateTime.Parse(shiftTime.Endtime);
                TimeSpan shiftDuration = endTime - startTime;
                shiftworkingHours = Math.Max((decimal)shiftDuration.TotalHours, 0);
            }

            var EmpTotalWorkingHours = await _context.EmployeeCheckInRecords
                .Where(att => att.EmpId == employeeId &&
                              att.CheckIntime.HasValue &&
                              att.CheckOuttime.HasValue &&
                              att.CheckIntime.Value.Year == year &&
                              att.CheckIntime.Value.Month == month)
                .ToListAsync();

            int lateMarkCount = 0;
            bool ischeck = false;
            foreach (var item in EmpTotalWorkingHours)
            {
                decimal TotalWorkingHours = (decimal)Math.Round((item.CheckOuttime.Value - item.CheckIntime.Value).TotalHours, 2);
                decimal totalLateHours = shiftworkingHours - TotalWorkingHours;

                if (shiftworkingHours > TotalWorkingHours)
                {
                    lateMarkCount += (int)(totalLateHours / 0.5m);
                    if (totalLateHours >= 0.5m)
                    {
                        if (lateMarkCount == 3 && ischeck == false)
                        {
                            NumberOfLateMarks += 1;
                            ischeck = true;
                        }
                        else if (lateMarkCount > 3)
                        {
                            NumberOfLateMarks += (1 / 3.0m);
                        }
                    }
                }
            }

            TotalsalaryDeduction = (noOfDays == 0) ? 0 : noOfDays;

            if (TotalsalaryDeduction > 0)
            {
                monthlyPay = decimal.Round((ctcData / TotalnoOfDays) * TotalsalaryDeduction, 2);
                decimal empbasic = decimal.Round((EmployeebasicData / TotalnoOfDays) * TotalsalaryDeduction, 2);
                decimal totalsalary = decimal.Round(((decimal)EmployeeMonthlyCtc), 2);
                Lop = decimal.Round(ctcData - monthlyPay, 2);
                if (monthlyPay > 0)
                {
                    if (totalsalary < Esidata.EsicAmount)
                    {
                        EmployeeEsi = decimal.Round((empbasic * EmployeeEsiData) / 100, 2);
                        monthlyPay -= EmployeeEsi;

                        EmployeeEpf = decimal.Round((empbasic * EmployeeEpfData) / 100, 2);
                        monthlyPay -= EmployeeEpf;
                    }
                    else
                    {
                        EmployeeEpf = decimal.Round((empbasic * EmployeeEpfData) / 100, 2);
                        monthlyPay -= EmployeeEpf;
                        EmployeeEsi = 0;
                    }
                }
            }

            return new JsonResult(new
            {
                EmployeeId = employee.EmployeeId,
                LeaveRemaining = TotalsalaryDeduction,
                MonthlyPay = monthlyPay,
                SalaryDeductionDays = salaryDeductionDays,
                NumberOfLateMarks = NumberOfLateMarks,
                EmployeeEpf = EmployeeEpf,
                EmployeeEsi = EmployeeEsi,
                Lop = Lop,
            });
        }
        public JsonResult GetLocationsByCustomer(int customerId)
        {
            var worklocationIds = _context.WorkLocations1
                .Where(x => x.Customerid == customerId)
                .Select(x => x.Id)
                .ToList();

            var worklocationnamelist = _context.AddWorkLocations
                .Where(x => worklocationIds.Contains((int)x.WorkLocationid))
                .Select(x => new { Value = x.WorkLocationid, Text = x.WorkLocationName })
                .ToList();

            return Json(worklocationnamelist);
        }

        public IActionResult SalaryslipDownloadPDF(int id, int month)
        {
            try
            {
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                HtmlToPdf converter = new HtmlToPdf();
                string SlipURL = $"{schema}://{host}/Employee/SalarySlipInPDF?id={id}&&month={month}";
                PdfDocument doc = converter.ConvertUrl(SlipURL);

                //string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EMPpdfs");
                //if (!Directory.Exists(wwwRootPath))
                //{
                //    Directory.CreateDirectory(wwwRootPath);
                //}

                var result = (from emp in _context.EmployeeRegistrations
                              join empt in _context.Empattendances
                              on emp.EmployeeId equals empt.EmployeeId
                              where emp.Id == id && empt.Month == month
                              select new SalarySlipDetails
                              {
                                  Id = emp.Id,
                                  Employee_ID = emp.EmployeeId,
                                  First_Name = emp.FirstName,
                                  Email_Id = emp.WorkEmail,
                                  Month = getMonthName((int)month, true),
                                  Year = empt.Year,
                                  vendorid = emp.Vendorid,
                                  CompanyName = _context.VendorRegistrations.Where(x => x.Id == emp.Vendorid).Select(x => x.CompanyName).FirstOrDefault()
                              }).FirstOrDefault();

                if (result == null)
                {
                    return Json(new { success = false, message = "Error: Employee not found." });
                }

                string uniqueFileName = $"{result.Employee_ID}_{result.First_Name}_{result.Month}{result.Year}.pdf";
                string filePath = Path.Combine(uniqueFileName);
                doc.Save(filePath);

                byte[] pdfBytes = System.IO.File.ReadAllBytes(filePath);
                doc.Close();

                return File(pdfBytes, "application/pdf", uniqueFileName);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        public async Task<IActionResult> MonthlypayExportSalaryReport(int customerId = 0, int Month = 0, int year = 0, int WorkLocation = 0)
        {
            try
            {
                ViewBag.custid = customerId;
                ViewBag.locid = WorkLocation;
                ViewBag.monthid = Month;
                ViewBag.yearid = year;
                GenerateSalaryReportDTO salary = new GenerateSalaryReportDTO();

                if (customerId != null && Month != null && year != null && WorkLocation != null)
                {
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();

                    salary.GenerateSalaryReports = await _ICrmrpo.GenerateSalaryReport(customerId, Month, year, WorkLocation, (int)adminlogin.Vendorid);

                }
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Employee Break Report");

                    int currentwork = 1;
                    worksheet.Cell(currentwork, 1).Value = "Sr.No.";
                    worksheet.Cell(currentwork, 1).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 2).Value = "EmpId";
                    worksheet.Cell(currentwork, 2).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 3).Value = "EmpName";
                    worksheet.Cell(currentwork, 3).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 4).Value = "MonthlyCTC";
                    worksheet.Cell(currentwork, 4).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 5).Value = "Basic Salary";
                    worksheet.Cell(currentwork, 5).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 6).Value = "HRA";
                    worksheet.Cell(currentwork, 6).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 7).Value = "EPF";
                    worksheet.Cell(currentwork, 7).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 8).Value = "ESIC";
                    worksheet.Cell(currentwork, 8).Style.Fill.BackgroundColor = XLColor.LightGray;


                    worksheet.Cell(currentwork, 9).Value = "SA";
                    worksheet.Cell(currentwork, 9).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 10).Value = "CA";
                    worksheet.Cell(currentwork, 10).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 11).Value = "MA";
                    worksheet.Cell(currentwork, 11).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 12).Value = "Variable Pay";
                    worksheet.Cell(currentwork, 12).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 13).Value = "TA";
                    worksheet.Cell(currentwork, 13).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 14).Value = "Incentive";
                    worksheet.Cell(currentwork, 14).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 15).Value = "TDS";
                    worksheet.Cell(currentwork, 15).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 16).Value = "Professional tax";
                    worksheet.Cell(currentwork, 16).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 17).Value = "Lop";
                    worksheet.Cell(currentwork, 17).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 18).Value = "Generated Salary";
                    worksheet.Cell(currentwork, 18).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentwork, 19).Value = "Attendance";
                    worksheet.Cell(currentwork, 19).Style.Fill.BackgroundColor = XLColor.LightGray;

                    currentwork++;

                    var row = 1;
                    foreach (var record in salary.GenerateSalaryReports)
                    {
                        worksheet.Cell(row, 1).Value = row - 1;  // Sr. No.
                        worksheet.Cell(row, 2).Value = record.EmployeeId;
                        worksheet.Cell(row, 3).Value = record.EmployeeName;
                        worksheet.Cell(row, 4).Value = record.MonthlyCtc;
                        worksheet.Cell(row, 5).Value = record.EPF;
                        worksheet.Cell(row, 6).Value = record.ESIC;
                        worksheet.Cell(row, 7).Value = record.Basicsalary;
                        worksheet.Cell(row, 8).Value = record.Hra;
                        worksheet.Cell(row, 9).Value = record.SpecialAllowance;
                        worksheet.Cell(row, 10).Value = record.Conveyanceallowance;
                        worksheet.Cell(row, 11).Value = record.MedicalAllowance;
                        worksheet.Cell(row, 12).Value = record.VariablePay;
                        worksheet.Cell(row, 13).Value = record.TravellingAllowance;
                        worksheet.Cell(row, 14).Value = record.Incentive;
                        worksheet.Cell(row, 15).Value = record.Tds;
                        worksheet.Cell(row, 16).Value = record.Professionaltax;
                        worksheet.Cell(row, 17).Value = record.Lop;
                        worksheet.Cell(row, 18).Value = record.GenerateSalary;
                        worksheet.Cell(row, 19).Value = record.Attendance;

                        currentwork++;
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileContent = stream.ToArray();
                        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee_Salary_Report.xlsx");
                    }

                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet, Route("Employee/EmployeeSelfassessmentlist")]
        public async Task<IActionResult> EmployeeSelfassessmentlist()
        {
            try
            {
                string userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return RedirectToAction("Login", "Admin");
                }
                var adminlogin = _context.AdminLogins.Where(x => x.Id == userId).FirstOrDefault();

                List<SelfassesstmentempdataDto> response = await _ICrmrpo.SelfassesstmentdataEmployeeList((int)adminlogin.Vendorid);
                if (response != null)
                {
                    return View(response);

                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<IActionResult> UpdateSelfassessmentActiveStatus(int Id)
        {
            var detail = await _context.Selfassesstmentdata.FirstOrDefaultAsync(x => x.Id == Id);

            if (detail == null)
            {
                TempData["msg"] = "data not found!";
                return RedirectToAction("EmployeeSelfassessmentlist");
            }
            detail.IsActive = !detail.IsActive;
            await _context.SaveChangesAsync();
            return RedirectToAction("EmployeeSelfassessmentlist");
        }
        public async Task<IActionResult> getSelfassessmentdata(string empid)
        {
            var detail = await _context.Selfassesstmentdata.FirstOrDefaultAsync(x => x.EmpId == empid);
            var admindata = await _context.Selfassesstmentadmins.ToListAsync();
            var vendordata = await _context.SelfassesstmentVendors.ToListAsync();

            if (detail == null)
            {
                return NotFound("Employee self-assessment data not found.");
            }

            var emp = await _context.EmployeeRegistrations.FirstOrDefaultAsync(x => x.EmployeeId == empid);
            if (emp == null)
            {
                return NotFound("Employee not found.");
            }

            
            List<Selfassesstmentdetails> assessmentData;

            if (!vendordata.Any())
            {
                assessmentData = admindata.Select(s => new Selfassesstmentdetails
                {
                    Id = s.Id,
                    Tittle = s.Tittle,
                    SubTittle = s.SubTittle,
                    Pointname = s.Pointname
                }).ToList();
            }
            else
            {
                assessmentData = await _context.SelfassesstmentVendors
                    .Where(s => s.VendorId == emp.Vendorid)
                    .Select(s => new Selfassesstmentdetails
                    {
                        Id = s.Id,
                        Tittle = s.Tittle,
                        SubTittle = s.SubTittle,
                        Pointname = s.Pointname
                    }).ToListAsync();

            }

            var fdgf = new SelfassesstmentempdataDto();

            if (!string.IsNullOrEmpty(detail.AssesstmentAns))
            {
                try
                {
                    fdgf.AssesstmentAns = JsonConvert.DeserializeObject<AllQuestiondata>(detail.AssesstmentAns);
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    return BadRequest("Error parsing assessment data: " + ex.Message);
                }
            }
            ///1
            foreach (var item in fdgf.AssesstmentAns.QuestionOne.Question1)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item.Id);
                if (matchingItem != null)
                {
                    item.Tittle = matchingItem.Tittle;
                    item.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item2 in fdgf.AssesstmentAns.QuestionOne.Question2)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item2.Id);
                if (matchingItem != null)
                {
                    item2.Tittle = matchingItem.Tittle;
                    item2.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item3 in fdgf.AssesstmentAns.QuestionOne.Question3)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item3.Id);
                if (matchingItem != null)
                {
                    item3.Tittle = matchingItem.Tittle;
                    item3.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item4 in fdgf.AssesstmentAns.QuestionOne.Question4)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item4.Id);
                if (matchingItem != null)
                {
                    item4.Tittle = matchingItem.Tittle;
                    item4.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item5 in fdgf.AssesstmentAns.QuestionOne.Question5)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item5.Id);
                if (matchingItem != null)
                {
                    item5.Tittle = matchingItem.Tittle;
                    item5.SubTittle = matchingItem.SubTittle;
                }
            }
            ///2
            var item6 = fdgf.AssesstmentAns.QuestionTwo.Question1;
            {
                item6.Tittle = assessmentData.Where(x => x.Id == item6.Id).FirstOrDefault().Tittle;
                item6.SubTittle = assessmentData.Where(x => x.Id == item6.Id).FirstOrDefault().SubTittle;
            }
            var item7 = fdgf.AssesstmentAns.QuestionTwo.Question2;
            {
                item7.Tittle = assessmentData.Where(x => x.Id == item7.Id).FirstOrDefault().Tittle;
                item7.SubTittle = assessmentData.Where(x => x.Id == item7.Id).FirstOrDefault().SubTittle;
            }
            var item8 = fdgf.AssesstmentAns.QuestionTwo.Question3;
            {
                item8.Tittle = assessmentData.Where(x => x.Id == item8.Id).FirstOrDefault().Tittle;
                item8.SubTittle = assessmentData.Where(x => x.Id == item8.Id).FirstOrDefault().SubTittle;
            }
            var item9 = fdgf.AssesstmentAns.QuestionTwo.Question4;
            {
                item9.Tittle = assessmentData.Where(x => x.Id == item9.Id).FirstOrDefault().Tittle;
                item9.SubTittle = assessmentData.Where(x => x.Id == item9.Id).FirstOrDefault().SubTittle;
            }
            var item10 = fdgf.AssesstmentAns.QuestionTwo.Question5;
            {
                item10.Tittle = assessmentData.Where(x => x.Id == item10.Id).FirstOrDefault().Tittle;
                item10.SubTittle = assessmentData.Where(x => x.Id == item10.Id).FirstOrDefault().SubTittle;
            }
            ///3
            foreach (var item11 in fdgf.AssesstmentAns.QuestionThird.Question1)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item11.Id);
                if (matchingItem != null)
                {
                    item11.Tittle = matchingItem.Tittle;
                    item11.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item12 in fdgf.AssesstmentAns.QuestionThird.Question2)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item12.Id);
                if (matchingItem != null)
                {
                    item12.Tittle = matchingItem.Tittle;
                    item12.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item13 in fdgf.AssesstmentAns.QuestionThird.Question3)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item13.Id);
                if (matchingItem != null)
                {
                    item13.Tittle = matchingItem.Tittle;
                    item13.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item14 in fdgf.AssesstmentAns.QuestionThird.Question4)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item14.Id);
                if (matchingItem != null)
                {
                    item14.Tittle = matchingItem.Tittle;
                    item14.SubTittle = matchingItem.SubTittle;
                }
            }
            ////4
            foreach (var item15 in fdgf.AssesstmentAns.QuestionFour.Question1)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item15.Id);
                if (matchingItem != null)
                {
                    item15.Tittle = matchingItem.Tittle;
                    item15.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item16 in fdgf.AssesstmentAns.QuestionFour.Question2)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item16.Id);
                if (matchingItem != null)
                {
                    item16.Tittle = matchingItem.Tittle;
                    item16.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item17 in fdgf.AssesstmentAns.QuestionFour.Question3)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item17.Id);
                if (matchingItem != null)
                {
                    item17.Tittle = matchingItem.Tittle;
                    item17.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item18 in fdgf.AssesstmentAns.QuestionFour.Question4)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item18.Id);
                if (matchingItem != null)
                {
                    item18.Tittle = matchingItem.Tittle;
                    item18.SubTittle = matchingItem.SubTittle;
                }
            }
            ////5
            foreach (var item19 in fdgf.AssesstmentAns.QuestionFive.Question1)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item19.Id);
                if (matchingItem != null)
                {
                    item19.Tittle = matchingItem.Tittle;
                    item19.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item20 in fdgf.AssesstmentAns.QuestionFive.Question2)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item20.Id);
                if (matchingItem != null)
                {
                    item20.Tittle = matchingItem.Tittle;
                    item20.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item21 in fdgf.AssesstmentAns.QuestionFive.Question3)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item21.Id);
                if (matchingItem != null)
                {
                    item21.Tittle = matchingItem.Tittle;
                    item21.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item22 in fdgf.AssesstmentAns.QuestionFive.Question4)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item22.Id);
                if (matchingItem != null)
                {
                    item22.Tittle = matchingItem.Tittle;
                    item22.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item23 in fdgf.AssesstmentAns.QuestionFive.Question5)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item23.Id);
                if (matchingItem != null)
                {
                    item23.Tittle = matchingItem.Tittle;
                    item23.SubTittle = matchingItem.SubTittle;
                }
            }
            ///6
            foreach (var item24 in fdgf.AssesstmentAns.QuestionSix.Question1)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item24.Id);
                if (matchingItem != null)
                {
                    item24.Tittle = matchingItem.Tittle;
                    item24.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item25 in fdgf.AssesstmentAns.QuestionSix.Question2)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item25.Id);
                if (matchingItem != null)
                {
                    item25.Tittle = matchingItem.Tittle;
                    item25.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item26 in fdgf.AssesstmentAns.QuestionSix.Question3)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item26.Id);
                if (matchingItem != null)
                {
                    item26.Tittle = matchingItem.Tittle;
                    item26.SubTittle = matchingItem.SubTittle;
                }
            }
            ///7
            foreach (var item27 in fdgf.AssesstmentAns.QuestionSeven.Question1)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item27.Id);
                if (matchingItem != null)
                {
                    item27.Tittle = matchingItem.Tittle;
                    item27.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item28 in fdgf.AssesstmentAns.QuestionSeven.Question2)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item28.Id);
                if (matchingItem != null)
                {
                    item28.Tittle = matchingItem.Tittle;
                    item28.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item29 in fdgf.AssesstmentAns.QuestionSeven.Question3)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item29.Id);
                if (matchingItem != null)
                {
                    item29.Tittle = matchingItem.Tittle;
                    item29.SubTittle = matchingItem.SubTittle;
                }
            }
            foreach (var item30 in fdgf.AssesstmentAns.QuestionSeven.Question4)
            {
                var matchingItem = assessmentData.FirstOrDefault(x => x.Id == item30.Id);
                if (matchingItem != null)
                {
                    item30.Tittle = matchingItem.Tittle;
                    item30.SubTittle = matchingItem.SubTittle;
                }
            }
           
            return View(fdgf);
        }




    }
}







