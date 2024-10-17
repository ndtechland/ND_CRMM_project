using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using CRM.Repository;
using CRM.Utilities;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Security.Claims;
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
        private readonly Dcrypt _dcrypt;
        private readonly IEmailService _IEmailService;
        private readonly Encrypt _encrypt;

        public EmployeeApiController(IEmployee apiemp, admin_NDCrMContext context, Dcrypt dcrypt, IEmailService iEmailService, Encrypt encrypt)
        {
            this._apiemp = apiemp;
            this._context = context;
            this._dcrypt = dcrypt;
            _IEmailService = iEmailService;
            _encrypt = encrypt;
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
        [HttpGet("EmployeeDashboard")]
        public async Task<IActionResult> EmployeeDashboard()
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
                    var offerletter = "/EMPpdfs/" + empoffer.OfferletterFile;
                    var appointmentletter = "/EMPpdfs/" + employee.Appoinmentletter;
                    //EmployeeprofileComplete
                    //var totalFields = typeof(EmployeeRegistration).GetProperties().Length - 4;

                    //double filledFields = 0;
                    //if (employee != null)
                    //{
                    //    foreach (var property in typeof(EmployeeRegistration).GetProperties())
                    //    {
                    //        var value = property.GetValue(employee);
                    //        if (value != null)
                    //        {
                    //            filledFields++;
                    //        }
                    //    }
                    //}

                    //int completionPercentage = (int)((filledFields / totalFields) * 100);
                    //var CompletionPercentage = completionPercentage.ToString() + '%';

                    response.Data = new
                    {
                        TotalAttendance = adjustedDifference,
                        Attendance = "" + attendance + " / " + daysInMonth + "",
                        LeaveLeft = TottalLeaveRight,
                        Leave = leave,
                        offerletter = offerletter,
                        appointmentletter = appointmentletter,
                        //CompletionPercentage = CompletionPercentage
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
        [Route("GetAllEmployeesalaryslip")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployeesalaryslip()
        {
            var response = new Utilities.Response<List<EmpattendanceDto>>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userid = User.Claims.FirstOrDefault().Value;
                    List<EmpattendanceDto> isEmployeeExists = await _apiemp.GetAllEmpsalaryslip(userid);
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
        [Route("GetEmployeesalaryslip")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeesalaryslip(int month, int year)
        {
            var response = new Utilities.Response<EmpattendanceDto>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userid = User.Claims.FirstOrDefault().Value;
                    EmpattendanceDto isEmployeeExists = await _apiemp.Getsalarydetails(userid, month, year);
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
        [AllowAnonymous]
        [HttpPost("EmployeeForgotPassword")]
        public async Task<IActionResult> EmployeeForgotPassword(ForgotPassword model)
        {
            var response = new Utilities.Response<string>();
            try
            {
                if (model != null)
                {
                    var employee = await _context.EmployeeRegistrations.FirstOrDefaultAsync(x => x.WorkEmail == model.Email);
                    if (employee == null)
                    {
                        response.StatusCode = StatusCodes.Status404NotFound;
                        response.Message = "Invalid email address.";
                        return NotFound(response);
                    }

                    var employeePassword = await _context.EmployeeLogins.FirstOrDefaultAsync(x => x.EmployeeId == employee.EmployeeId);
                    if (employeePassword == null)
                    {
                        response.StatusCode = StatusCodes.Status404NotFound;
                        response.Message = "Employee login details not found.";
                        return NotFound(response);
                    }

                    var newPassword = _dcrypt.GenerateRandomPassword();
                    employeePassword.Password = newPassword;

                    _context.Update(employeePassword);
                    await _context.SaveChangesAsync();

                    await _IEmailService.EmpRandomPasswordSendEmailAsync(model, newPassword);

                    response.Succeeded = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Status = "Success";
                    response.Message = "A new password has been generated and sent to your registered email address.";
                    response.Data = employee.WorkEmail;
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Data not Found";
                    return Unauthorized(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while processing your request.";
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost("EmployeeChangePassword")]
        public async Task<IActionResult> EmployeeChangePassword(EmpchangepasswordDto model)
        {
            var response = new Utilities.Response<EmpchangepasswordDto>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.Claims.FirstOrDefault().Value;
                    var employee = await _context.EmployeeRegistrations.FirstOrDefaultAsync(x => x.EmployeeId == userId);
                    var EmployeePassword = await _context.EmployeeLogins.FirstOrDefaultAsync(x => x.EmployeeId == employee.EmployeeId);

                    if (employee != null)
                    {
                        if (EmployeePassword.Password != model.CurrentPassword)
                        {
                            response.StatusCode = StatusCodes.Status400BadRequest;
                            response.Message = "Current password is incorrect.";
                            return BadRequest(response);
                        }
                        if (model.ConfirmPassword != model.NewPassword)
                        {
                            response.StatusCode = StatusCodes.Status400BadRequest;
                            response.Message = "New Password and Confirm Password does not match.";
                            return BadRequest(response);
                        }
                        EmployeePassword.Password = model.NewPassword;
                        _context.Update(employee);
                        await _context.SaveChangesAsync();

                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Password changed successfully.";
                        response.Data = model;

                        return Ok(response);
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status404NotFound;
                        response.Message = "Employee not found.";
                        return NotFound(response);
                    }
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
                response.Message = "An error occurred while processing your request.";
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("GetCompanyLoction")]
        [HttpGet]
        public async Task<IActionResult> GetCompanyLoction()
        {
            var response = new Response<CompanyLoctionDto>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    CompanyLoctionDto isLoctionExists = await _apiemp.GetCompanyLoction(userid);
                    if (isLoctionExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Company Loction Here.";
                        response.Data = isLoctionExists;
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

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            double latDistance = ToRadians(lat2 - lat1);
            double lonDistance = ToRadians(lon2 - lon1);

            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = R * c * 1000;
            return distance;
        }

        private static double ToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }
        [AllowAnonymous]
        [HttpPost, Route("EmployeeCheckIn")]
        public async Task<IActionResult> EmployeeCheckIn([FromBody] EmpCheckIn model)
        {
            var response = new Response<EmployeeCheckIn>();
            try
            {
                bool CheckIN = true;
                var employee = await _context.EmployeeRegistrations.FirstOrDefaultAsync(x => x.Id == model.Userid);

                if (employee == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Employee not found.";
                    return NotFound(response);
                }
                var company = await _context.VendorRegistrations.FirstOrDefaultAsync(x => x.Id == employee.Vendorid);
                if (company == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Company not found.";
                    return NotFound(response);
                }

                double radiusInMeters = (double)Convert.ToDecimal(company.Radious);
                double distance = CalculateDistance(
                    double.Parse(company.Maplat),
                    double.Parse(company.Maplong),
                    double.Parse(model.CurrentLat),
                    double.Parse(model.Currentlong)
                );

                if (distance > radiusInMeters)
                {
                    bool CheckIn = await _context.EmployeeCheckIns.Where(x => x.EmployeeId == employee.EmployeeId && x.Currentdate.Value.Date == DateTime.Now.Date).OrderByDescending(x => x.Id)
                    .AnyAsync();
                    if (CheckIn)
                    {
                        CheckIN = false;
                        var CCModel = await _apiemp.Empcheckin(model, CheckIN);
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Check-Out successful.";
                        response.Data = CCModel;
                        return Ok(response);
                    }
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = $"Employee is not within the {radiusInMeters} meter radius of the company's location.";
                    return BadRequest(response);
                }

                var apiModel = await _apiemp.Empcheckin(model, CheckIN);
                if (apiModel != null)
                {
                    response.Succeeded = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Status = "Success";
                    response.Message = "Check-in successful.";
                    response.Data = apiModel;
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Check-in failed. Please check your input.";
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred: " + ex.Message;
                return StatusCode(response.StatusCode, response);
            }
        }
        //Web
        [HttpPost, Route("WebEmployeePersonalDetail")]
        public async Task<IActionResult> WebEmployeePersonalDetail([FromForm] webPersonalDetail model)
        {
            var response = new Response<ApprovedPresnolRes>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;

                    var apiModel = await _apiemp.webPersonalDetail(model, userid);
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
        [AllowAnonymous]
        [HttpPost, Route("EmployeeCheckOut")]
        public async Task<IActionResult> EmployeeCheckOut([FromBody] EmpCheckIn model)
        {
            var response = new Response<EmployeeCheckIn>();
            try
            {
                bool CheckIN = false;
                var employee = await _context.EmployeeRegistrations.FirstOrDefaultAsync(x => x.Id == model.Userid);
                if (employee == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Employee not found.";
                    return NotFound(response);
                }
                var employeeCheckIns = await _context.EmployeeCheckIns
                    .FirstOrDefaultAsync(x => x.EmployeeId == employee.EmployeeId && x.CheckIn == true);

                if (employeeCheckIns == null)
                {
                    response.StatusCode = StatusCodes.Status409Conflict;
                    response.Message = "You are not currently checked in.";
                    return Conflict(response);
                }

                var company = await _context.VendorRegistrations.FirstOrDefaultAsync(x => x.Id == employee.Vendorid);

                if (company == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Company not found.";
                    return NotFound(response);
                }
                double radiusInMeters = (double)Convert.ToDecimal(company.Radious);
                double distance = CalculateDistance(
                    double.Parse(company.Maplat),
                    double.Parse(company.Maplong),
                    double.Parse(model.CurrentLat),
                    double.Parse(model.Currentlong)
                );

                if (distance > radiusInMeters)
                {
                    response.Succeeded = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = $"Employee is not within the {radiusInMeters} meter radius of the company's location.";
                    return BadRequest(response);
                }

                var apiModel = await _apiemp.Empcheckout(model, CheckIN);
                if (apiModel != null)
                {
                    response.Succeeded = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Status = "Success";
                    response.Message = "Check-out successful.";
                    response.Data = apiModel;
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Check-out failed. Please check your input.";
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred: " + ex.Message;
                return StatusCode(response.StatusCode, response);
            }
        }

        [Route("Empattendancedatail")]
        [HttpGet]
        public async Task<IActionResult> Empattendancedatail()
        {
            var response = new Response<Empattendancedatail>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    Empattendancedatail isExists = await _apiemp.GetEmpattendance(userid);
                    if (isExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Attendancedatail Here.";
                        response.Data = isExists;
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
        //Web
        [Route("EmployeeUpdateprofilepicture")]
        [HttpPost]
        public async Task<IActionResult> EmpUpdateprofilepicture([FromForm] profilepicture model)
        {
            var response = new Utilities.Response<profilepicture>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (model.Empprofile != null && model.Empprofile.Length > 0)
                    {
                        string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                        string EmpprofileImagePath = model.Empprofile.FileName;
                        string exten = EmpprofileImagePath.Split('.')[1];
                        string extention = "." + exten;
                        if (!allowedExtensions.Contains(extention))
                        {
                            response.Succeeded = false;
                            response.StatusCode = StatusCodes.Status404NotFound;
                            response.Status = "not allowed";
                            response.Message = "Only .jpg, .jpeg, .png files are allowed";
                            return BadRequest(response);
                        }
                        if (model.Empprofile.Length > 2 * 1024 * 1024)
                        {
                            response.Succeeded = false;
                            response.StatusCode = StatusCodes.Status404NotFound;
                            response.Status = "not allowed";
                            response.Message = "Image should not be more than 2 MB";
                            return BadRequest(response);
                        }
                    }
                    else
                    {
                        response.Succeeded = false;
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        response.Status = "not allowed";
                        response.Message = "Empprofile is required";
                        return BadRequest(response);
                    }

                    var userid = User.Claims.FirstOrDefault().Value;
                    var apiModel = await _apiemp.Updateprofilepicture(model, userid);
                    profilepicture pp = new()
                    {
                        EmpProfiles = "/EmpProfile/" + apiModel.EmpProfile,
                    };
                    if (apiModel != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Profile Update successfully.";
                        response.Data = pp;
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
        [Route("EmpLoginactivity")]
        [HttpGet]
        public async Task<IActionResult> EmpLoginactivity()
        {
            var response = new Response<Loginactivity>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    Loginactivity isLoginExists = await _apiemp.GetEmpLoginactivity(userid);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee LoginActivity Here.";
                        response.Data = isLoginExists;
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
        [Route("EmpTasksassign")]
        [HttpGet]
        public async Task<IActionResult> EmpTasksassign()
        {
            var response = new Response<List<TasksassignDto>>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    List<TasksassignDto> isLoginExists = await _apiemp.GetEmpTasksassign(userid);
                    if (isLoginExists.Count == 0)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee does not Task .";
                        response.Data = isLoginExists;
                        return Ok(response);
                    }
                    else if (isLoginExists.Count != 0)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Task Here.";
                        response.Data = isLoginExists;
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
        [Route("EmpTasksassignbyid")]
        [HttpGet]
        public async Task<IActionResult> EmpTasksassignbyid(int id)
        {
            var response = new Response<TasksassignnameDto>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    TasksassignnameDto isLoginExists = await _apiemp.GetEmpTasksassignname(userid, id);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Task Here.";
                        response.Data = isLoginExists;
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
        [HttpPost("/api/EmployeeApi/CompletedTasksassign")]
        public async Task<IActionResult> CompletedTasksassign([FromForm] EmpTasksListDto model)
        {
            var response = new Response<EmpTasksList>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    var isLoginExists = await _apiemp.CompletedempTasksassign(model, userid);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Task Completed..";
                        response.Data = isLoginExists;
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
        [HttpPost("/api/EmployeeApi/UnCompletedTasksassign")]
        public async Task<IActionResult> UnCompletedTasksassign([FromForm] EmpSubTasksListDto model)
        {
            var response = new Response<EmpTasksList>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    var isLoginExists = await _apiemp.UnCompletedempTasksassign(model, userid);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Task UnCompleted..";
                        response.Data = isLoginExists;
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
        //Web
        [HttpPost("/api/EmployeeApi/AddTasksassign")]
        public async Task<IActionResult> AddTasksassign([FromForm] TasksListDto model)
        {
            var response = new Response<EmpTasksList>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    var isLoginExists = await _apiemp.Tasksassign(model, userid);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Task Add Successful..";
                        response.Data = isLoginExists;
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
        [HttpPost("/api/EmployeeApi/SubTaskCompleted")]
        public async Task<IActionResult> SubTaskCompleted([FromForm] EmpSubTasksDto model)
        {
            var response = new Response<EmpTasksList>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    var isLoginExists = await _apiemp.SubTaskCompletedempTasksassign(model, userid);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Task Completed..";
                        response.Data = isLoginExists;
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

        // Web
        [Route("WebEmpLoginactivity")]
        [HttpGet]
        public async Task<IActionResult> WebEmpLoginactivity()
        {
            var response = new Response<List<WebLoginactivity>>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    List<WebLoginactivity> isLoginExists = await _apiemp.GetWebEmpLoginactivity(userid);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Login Activity Here.";
                        response.Data = isLoginExists;
                        return Ok(response);
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status404NotFound;
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

        [Route("GetofficeEvents")]
        [HttpGet]
        public async Task<IActionResult> GetofficeEvents()
        {
            var response = new Response<List<officeEventsDto>>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    List<officeEventsDto> isLoginExists = await _apiemp.GetOfficeEvents(userid);
                    if (isLoginExists.Count == 0)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "No upcoming or past events found.";
                        response.Data = isLoginExists;
                        return Ok(response);
                    }
                    else if (isLoginExists.Count != 0)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = $"{isLoginExists.Count} upcoming event(s) scheduled soon.";
                        response.Data = isLoginExists;
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

        [Route("GetEmpattendancedatail")]
        [HttpGet]
        public async Task<IActionResult> GetEmpattendancedatail(DateTime Currentdate)
        {
            var response = new Response<Empattendancedatail>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    Empattendancedatail isExists = await _apiemp.GetFilterattendance(userid, Currentdate);
                    if (isExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Attendancedatail Here.";
                        response.Data = isExists;
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

        // Web
        [Route("EmpMonthlyattendanceDetails")]
        [HttpGet]
        public async Task<IActionResult> EmpMonthlyattendanceDetails()
        {
            var response = new Response<Monthlyattendancedatail>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    Monthlyattendancedatail isLoginExists = await _apiemp.GetMonthAttanceDetails(userid);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Monthly AttanceDetails Here.";
                        response.Data = isLoginExists;
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
        [Route("EmpTotalLeaves")]
        [HttpGet]
        public async Task<IActionResult> EmpTotalLeaves()
        {
            var response = new Response<TotalLeave>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    TotalLeave isLoginExists = await _apiemp.Getleavelist(userid);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee TotalLeaves Here.";
                        response.Data = isLoginExists;
                        return Ok(response);
                    }
                    else if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Leaves has been processed.";
                        response.Data = isLoginExists;
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

        [Route("EmpTotalLeavesbyid")]
        [HttpGet]
        public async Task<IActionResult> EmpTotalLeavesbyid(int id)
        {
            var response = new Response<getTotalLeave>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    getTotalLeave isLoginExists = await _apiemp.GetEmptotalleave(userid, id);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee TotalLeaves Here.";
                        response.Data = isLoginExists;
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

        [Route("EmpattendanceGraph")]
        [HttpGet]
        public async Task<IActionResult> EmpattendanceGraph()
        {
            var response = new Response<List<getattendancegraph>>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    List<getattendancegraph> isLoginExists = await _apiemp.GetEmpGraph(userid);
                    if (isLoginExists.Count == 0)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee does not have Graph detail.";
                        response.Data = isLoginExists;
                        return Ok(response);
                    }
                    else if (isLoginExists.Count != 0)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Graph detail is available.";
                        response.Data = isLoginExists;
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

        [Route("EmpTasklist")]
        [HttpGet]
        public async Task<IActionResult> EmpTasklist()
        {
            var response = new Response<getTasklist>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    var isLoginExists = await _apiemp.gettasklist(userid);
                    if (isLoginExists == null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee does not have task detail.";
                        response.Data = isLoginExists;
                        return Ok(response);
                    }
                    else if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee task detail is available.";
                        response.Data = isLoginExists;
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
        [Route("EmpSubTasklist")]
        [HttpGet]
        public async Task<IActionResult> EmpSubTasklist(int? SubTaskid)
        {
            var response = new Response<List<TasksassignlistDto>>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    List<TasksassignlistDto> isLoginExists = await _apiemp.getSubtasklist(userid, SubTaskid);
                    if (isLoginExists.Count == 0)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee does not have Subtask detail.";
                        response.Data = isLoginExists;
                        return Ok(response);
                    }
                    else if (isLoginExists.Count != 0)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Subtask detail is available.";
                        response.Data = isLoginExists;
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
    }
}
