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
        [Route("GetEmployeeById")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeById(string Employeeid)
        {
            var response = new Response<EmployeeBasicInfo>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {

                    EmployeeBasicInfo isEmployeeExists = await _apiemp.GetEmployeeById(Employeeid);
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
                    return Ok(response);
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Route("PersonalDetail")]
        [HttpPost]
        public async Task<IActionResult> PersonalDetail(EmpPersonalDetail model)
        {
            var response = new Response<EmployeePersonalDetail>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    EmployeePersonalDetail apiModel = await _apiemp.PersonalDetail(model);

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
        [Route("BankDetail")]
        [HttpPost]
        public async Task<IActionResult> BankDetail(bankdetail model)
        {
            var response = new Response<EmployeeBankDetail>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    EmployeeBankDetail apiModel = await _apiemp.Bankdetail(model);

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

    }
}
