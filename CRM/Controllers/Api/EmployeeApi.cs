using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using CRM.Utilities;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using City = CRM.Models.Crm.City;

namespace CRM.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeApiController : ControllerBase
    {

        private readonly IEmployee _apiemp;
        private readonly admin_NDCrMContext _context;
        public EmployeeApiController(IEmployee apiemp, admin_NDCrMContext context)
        {
            this._apiemp = apiemp;
            this._context = context;
        }
        [Route("GetEmployeeBasicInfo")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeBasicInfo()
        {
            var response = new Response<EmployeeBasicInfo>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    EmployeeBasicInfo isEmployeeExists = await _apiemp.GetEmployeeById(userid);
                    if (isEmployeeExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Details Here.";
                        response.Data = isEmployeeExists;
                        return Ok(response);
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        response.Message = "Data not found.";
                        return Ok(response);
                    }
                }
                else
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Token is expired.";
                    return BadRequest(response);
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Route("EmployeePersonalDetail")]
        [HttpPost]
        public async Task<IActionResult> EmployeePersonalDetail([FromForm] EmpPersonalDetail model)
        {
            var response = new Response<ApprovedPresnolRes>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;

                    var apiModel = await _apiemp.PersonalDetail(model, userid);
                    var empp = await _context.EmployeeRegistrations
                        .Where(x => x.EmployeeId == userid)
                        .FirstOrDefaultAsync();
                    ApprovedPresnolRes hgh = new ApprovedPresnolRes()
                    {
                        PersonalEmailAddress = apiModel.PersonalEmailAddress,
                        MobileNumber = apiModel.MobileNumber,
                        DateOfBirth = apiModel.DateOfBirth?.ToString("dd-MM-yyyy"),
                        Pan = apiModel.Pan,
                        AddressLine1 = apiModel.AddressLine1,
                        AddressLine2 = apiModel.AddressLine2,
                        City = apiModel.City,
                        StateId = apiModel.StateId,
                        Pincode = apiModel.Pincode,
                        EmployeeId = apiModel.EmployeeId,
                        AadharNo = apiModel.AadharNo,
                        AadharOne = apiModel.AadharOne,
                        Panimg = apiModel.Panimg,
                        AadharTwo = apiModel.AadharTwo,
                        IsApproved = apiModel.IsApproved,
                        UpdateDate = apiModel.UpdateDate,
                        Vendorid = apiModel.Vendorid,
                        FullName = apiModel.FullName,
                        FatherName = apiModel.FatherName,
                        EmpProfile = empp.EmpProfile
                    };
                    if (apiModel != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Personal Detail updated successfully.";
                        response.Data = hgh;
                        return Ok(new { response });
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status404NotFound;
                        response.Message = "Data not found.";
                        return NotFound(response);
                    }
                }
                else
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Token is expired.";
                    return Unauthorized(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("GetBankdetail")]
        [HttpGet]
        public async Task<IActionResult> GetBankdetail()
        {
            var response = new Response<bankdetail>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userid = User.Claims.FirstOrDefault().Value;
                    bankdetail isEmployeeExists = await _apiemp.GetBankdetail(userid);
                    if (isEmployeeExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Bank Details Here.";
                        response.Data = isEmployeeExists;
                        return Ok(response);
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        response.Message = "Data not found.";
                        return Ok(response);
                    }
                }
                else
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Token is expired.";
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {

                throw new Exception("Error :" + ex.Message);
            }
        }
        [Route("EmployeeBankDetail")]
        [HttpPost]
        public async Task<IActionResult> EmployeeBankDetail([FromForm] bankdetail model)
        {
            var response = new Response<Approvedbankdetail>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    Approvedbankdetail apiModel = await _apiemp.Bankdetail(model, userid);
                    if (apiModel != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Bank Detail Update successfully.";
                        response.Data = apiModel;
                        return Ok(response);
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        response.Message = "Data not found.";
                        return Ok(response);
                    }
                }
                else
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Token is expired.";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("getcity")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCity(int stateid)
        {
            var response = new Response<List<City>>();
            try
            {
                List<City> cities = await _apiemp.getcity(stateid);
                response.Data = cities;
                response.Message = "Cities retrieved successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Error: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("Getstate")]
        [AllowAnonymous]
        public async Task<IActionResult> Getstate()
        {
            var response = new Response<List<State>>();
            try
            {
                List<State> st = await _apiemp.Getstate();
                response.Data = st;
                response.Message = "State retrieved successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Error: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("Getsalarydetails")]
        [HttpGet]
        public async Task<IActionResult> Getsalarydetails()
        {
            var response = new Response<salarydetails>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userid = User.Claims.FirstOrDefault().Value;
                    salarydetails isEmployeeExists = await _apiemp.Getsalarydetails(userid);
                    if (isEmployeeExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Salary deatils Here.";
                        response.Data = isEmployeeExists;
                        return Ok(response);
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        response.Message = "Data not found.";
                        return Ok(response);
                    }
                }
                else
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Token is expired.";
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {

                throw new Exception("Error :" + ex.Message);
            }
        }
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var response = new Utilities.Response<dynamic>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userId = User.Claims.FirstOrDefault()?.Value;
                    var employee = _context.EmployeeRegistrations.FirstOrDefault(emp => emp.EmployeeId == userId);
                    //var tottalAttandance = _context.Applieddate
                    //    .Where(ad => ad.EmployeeId == userId && ad.Isactive == true)
                    //    .OrderByDescending(ad => ad.AppliedDate)
                    //    .ToList();
                    // var tottalAttandance1 = tottalAttandance.Where(ad => ad.EmployeeId == userId && ad.Isactive == true && ad.AppliedDate.Value.Month == DateTime.Now.Month && ad.AppliedDate.Value.Year == DateTime.Now.Year);

                    var tottalAttandance = _context.ApplyLeaveNews.Where(x => x.UserId == userId)
                        .OrderByDescending(ad => ad.StartDate).ToList();

                    var tottalAttandance1 = tottalAttandance.Where(ad => ad.UserId == userId && ad.CreatedDate.Month == DateTime.Now.Month && ad.CreatedDate.Year == DateTime.Now.Year).ToList();
                    decimal totalleave = (decimal)0.00;
                    foreach (var ad in tottalAttandance1)
                    {
                        totalleave += ad.CountLeave;
                    }


                    //TotalAttendance
                    DateTime dateOfJoining = employee.DateOfJoining.Value;
                    DateTime currentDate = DateTime.Now;
                    TimeSpan difference = currentDate - dateOfJoining;
                    int totalDaysDifference = (int)difference.TotalDays;
                    int countOfAttendance = tottalAttandance1.Count();
                    int adjustedDifference = totalDaysDifference - countOfAttendance;
                    //Attandance
                    int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                    int attendance = Math.Max(0, daysInMonth - countOfAttendance);
                    //Leave Left
                    var leaveRecords = _context.Leavemasters.Where(le => le.EmpId == userId && (le.LeavetypeId == 1 || le.LeavetypeId == 2 || le.LeavetypeId == 3)).ToList();
                    var ERleave = leaveRecords.FirstOrDefault(le => le.LeavetypeId == 1)?.Value ?? 0;
                    var FLleave = leaveRecords.FirstOrDefault(le => le.LeavetypeId == 2)?.Value ?? 0;
                    var CMFleave = leaveRecords.FirstOrDefault(le => le.LeavetypeId == 3)?.Value ?? 0;
                    var TotalLeaveLeft = ERleave + FLleave + CMFleave;
                    //var TottalLeaveRight = TotalLeaveLeft - tottalAttandance.Count();
                    var TottalLeaveRight = TotalLeaveLeft;// == 0 ? totalleave : TotalLeaveLeft - totalleave;
                                                          //Leave month wise
                                                          //var leave = tottalAttandance.Count();
                    var empoffer = _context.Offerletters.Where(le => le.Id == employee.Offerletterid).FirstOrDefault();

                    var leave = totalleave;
                    var offerletter = empoffer.OfferletterFile;
                    var appointmentletter = employee.Appoinmentletter;
                    //EmployeeprofileComplete
                    var totalFields = typeof(EmployeeRegistration).GetProperties().Length - 4;

                    double filledFields = 0;
                    if (employee != null)
                    {
                        foreach (var property in typeof(EmployeeRegistration).GetProperties())
                        {
                            var value = property.GetValue(employee);
                            if (value != null)
                            {
                                filledFields++;
                            }
                        }
                    }

                    int completionPercentage = (int)((filledFields / totalFields) * 100);
                    var CompletionPercentage = completionPercentage.ToString() + '%';

                    response.Data = new
                    {
                        TotalAttendance = adjustedDifference,
                        Attendance = "" + attendance + " / " + daysInMonth + "",
                        LeaveLeft = TottalLeaveRight,
                        Leave = leave,
                        offerletter = offerletter,
                        appointmentletter = appointmentletter,
                        CompletionPercentage = CompletionPercentage
                    };
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Succeeded = true;
                    response.Status = "Success";
                    response.Message = "Total deatils Here";
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Unauthorized. User not authenticated.";
                    return Unauthorized(response);
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Error: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}
