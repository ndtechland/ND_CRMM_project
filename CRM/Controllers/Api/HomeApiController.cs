using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Repository;
using CRM.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeApiController : ControllerBase
    {
        private readonly admin_NDCrMContext _context;
        private readonly IHome _home;
        public HomeApiController(admin_NDCrMContext context, IHome home)
        {
            _context = context;
            _home = home;
        }
        [HttpGet]
        [Route("Blogs")]
        public async Task<IActionResult> Blogs()
        {
            try
            {
                List<Blog> blogs = await _home.GetBlogs();
                if (blogs != null)
                {
                    return Ok(new { Status = 200, Message = "Blogs retrieved successfully.", data = blogs });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any blog available." });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("Getaboutcompany")]
        public async Task<IActionResult> Getaboutcompany()
        {
            var response = new Response<aboutCompanyDto>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    var isLoginExists = await _home.Getaboutcompany(userid);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Aboutcompany retrieved successfully.";
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
