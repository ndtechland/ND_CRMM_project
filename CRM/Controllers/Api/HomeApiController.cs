using CRM.Models.Crm;
using CRM.Repository;
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
                if(blogs!=null)
                {
                    return Ok(new { Status = 200, Message = "Blogs retrieved successfully.",data=blogs });
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
    }
}
