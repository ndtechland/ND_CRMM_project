using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using CRM.Utilities;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeApi : ControllerBase
    {
        private readonly IEmployee _apiemp;
        private readonly admin_NDCrMContext _context;
        public EmployeeApi(IEmployee apiemp, admin_NDCrMContext context)
        {
            this._apiemp = apiemp;
            this._context = context;
        }
        [Route("GetEmployeeById")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeById(string Employeeid)
        {
            try
            {
                var response = new Response<EmployeeRegistration>();
                bool isEmployeeExists = await _apiemp.GetEmployeeById(Employeeid);
                if (isEmployeeExists)
                {
                    var empDetails = _context.EmployeeRegistrations
                        .Where(x => x.EmployeeId == Employeeid)
                        .FirstOrDefault();

                    if (empDetails != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Details Here.";
                        response.Data = empDetails;
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
                    response.Message = "Employee not found.";
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
