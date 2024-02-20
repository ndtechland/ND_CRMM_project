﻿using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Repository;
using CRM.Utilities;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.Common;

namespace CRM.Controllers.Api
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IApiAccount _apiAccount;
        private readonly admin_NDCrMContext _context;   
        public AccountController(IApiAccount apiAccount, admin_NDCrMContext context)
        {
            this._apiAccount = apiAccount;
            this._context = context;
        }
        [Route("Account/Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var response = new Response<LoginProfile>();
            try
            {
                bool check = await _apiAccount.Login(model);
                if (check)
                {
                    var loginProfile = _context.EmployeeRegistrations.Select(x => new LoginProfile
                    {
                        Employee_Name = x.MiddleName == null ? x.FirstName + " " + x.LastName : x.FirstName + " " + x.MiddleName + " " + x.LastName,
                        Employee_ID = x.EmployeeId

                    }).FirstOrDefault();
                    if (loginProfile != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Login Successfully Here.";
                        response.Data = loginProfile;
                        return Ok(response);
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        response.Message = "Data not found.";
                        return Ok(response);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
