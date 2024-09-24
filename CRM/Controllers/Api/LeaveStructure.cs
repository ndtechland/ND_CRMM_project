using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using CRM.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveStructure : ControllerBase
    {
        private readonly IEmployee _apiemp;
        private readonly admin_NDCrMContext _context;
        public LeaveStructure(IEmployee apiemp, admin_NDCrMContext context)
        {
            this._apiemp = apiemp;
            this._context = context;
        }

        [HttpGet("GetLeaveType")]
        public async Task<ActionResult<leavedto>> GetLeaveType()
        {
            var response = new Utilities.Response<leavedto>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    var data = await _apiemp.LeaveType(userid);
                    if (data != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Leave Type and Leave";
                        response.Data = data;
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
            catch (Exception)
            {

                throw;
            }
        }

        [Route("EmployeeApplyLeave")]
        [HttpPost]
        public async Task<IActionResult> EmployeeApplyLeave([FromForm] ApplyLeave model)
        {
            var response = new Utilities.Response<bool>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userid = User.Claims.FirstOrDefault().Value;
                    var data = await _context.ApplyLeaveNews.Where(x => x.UserId == userid).ToListAsync();
                    if (data != null)
                    {
                        if (data.Any(x => x.StartDate == model.StartDate) || data.Any(x => x.EndDate == model.EndDate))
                        {
                            response.Succeeded = false;
                            response.StatusCode = StatusCodes.Status501NotImplemented;
                            response.Message = "Leave Already Applyed...!";
                            return Ok(response);
                        }
                    }
                    bool Check = await _apiemp.ApplyLeave(model, userid);
                    if (Check)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Message = "Leave Apply Successful...!";
                        response.Data = Check;
                        return Ok(response);
                    }
                    else
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        response.Message = "Leave Not Applied...!";
                        response.Data = Check;
                        return BadRequest(response);
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

                throw new Exception("Error Message : " + ex);
            }
        }
    }
}
