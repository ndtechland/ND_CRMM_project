﻿using CRM.Models.APIDTO;
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
        [Route("GetEmployeeById")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeById()
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
                    //var data = _context.EmployeePersonalDetails.FirstOrDefault(x => x.PersonalEmailAddress == model.PersonalEmailAddress && x.AddressLine1 == model.AddressLine1 && x.AddressLine2 == model.AddressLine2 && x.Pan == model.Pan && x.MobileNumber == model.MobileNumber);
                    //if (data.PersonalEmailAddress != null)
                    //{
                    //    response.Message = "Personal Email already exists.";
                    //}
                    //if (data.AddressLine1 != null)
                    //{
                    //    response.Message = "AddressLine1 already exists.";
                    //}
                    //if (data.AddressLine2 != null)
                    //{
                    //    response.Message = "AddressLine2 already exists.";
                    //}
                    //if (data.Pan != null)
                    //{
                    //    response.Message = "Pan already exists.";
                    //}
                    //if (data.MobileNumber != null)
                    //{
                    //    response.Message = "MobileNumber already exists.";
                    //}
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

        [Route("GetPresnolInfo")]
        [HttpGet]
        public async Task<IActionResult> GetPresnolInfo()
        {
            var response = new Response<EmpPersonalDetail>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userid = User.Claims.FirstOrDefault().Value;
                    EmpPersonalDetail isEmployeeExists = await _apiemp.GetPresnolInfo(userid);
                    if (isEmployeeExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Employee Personal Details Here.";
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
    }
}
