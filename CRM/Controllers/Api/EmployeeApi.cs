using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using CRM.Utilities;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [Route("PersonalDetail")]
        [HttpPost]
        public async Task<IActionResult> PersonalDetail([FromForm] EmpPersonalDetail model)
        {
            var response = new Response<EmployeePersonalDetail>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;

                    EmployeePersonalDetail apiModel = await _apiemp.PersonalDetail(model, userid);

                    if (apiModel != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Data saved successfully.";
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
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = $"An error occurred: {ex.Message}";
                return Ok(response);
            }
        }

        [Route("BankDetail")]
        [HttpPost]
        public async Task<IActionResult> BankDetail([FromForm] bankdetail model)
        {
            var response = new Response<EmployeeBankDetail>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    EmployeeBankDetail apiModel = await _apiemp.Bankdetail(model, userid);
                    if (apiModel != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Data saved successfully.";
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
        [Route("Updateprofilepicture")]
        [HttpPost]
        public async Task<IActionResult> Updateprofilepicture([FromForm] profilepicture model)
        {
            var response = new Response<profilepicture>();
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
        [Route("Getprofilepicture")]
        [HttpGet]
        public async Task<IActionResult> Getprofilepicture()
        {
            var response = new Response<profilepicture>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userid = User.Claims.FirstOrDefault().Value;
                    profilepicture isEmployeeExists = await _apiemp.Getprofilepicture(userid);
                    if (isEmployeeExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Profile Here.";
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
    }
}
