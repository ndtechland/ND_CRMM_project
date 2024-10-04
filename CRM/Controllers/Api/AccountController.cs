using CRM.IUtilities;
using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Repository;
using CRM.Utilities;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.TeamFoundation.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRM.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IApiAccount _apiAccount;
        private readonly IJwtToken _jwtToken;

        private readonly admin_NDCrMContext _context;   
        public AccountController(IJwtToken jwtToken,IApiAccount apiAccount, admin_NDCrMContext context)
        {
            this._jwtToken = jwtToken;
            this._apiAccount = apiAccount;
            this._context = context;
        }
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var response = new Response<LoginProfile>();
            try
            {
                bool check = await _apiAccount.Login(model);
                if (check)
                {
                    var loginProfile = _context.EmployeeRegistrations
                        .Where(x => x.EmployeeId == model.Employee_ID)
                        .Select(x => new LoginProfile
                        {
                            userid = x.Id,
                            Employee_Name = x.MiddleName == null ? x.FirstName + " " + x.LastName : x.FirstName + " " + x.MiddleName + " " + x.LastName,
                            Employee_ID = x.EmployeeId
                        })
                        .FirstOrDefault();

                    if (loginProfile != null)
                    {
                        var token = _jwtToken.GenerateAccessToken(model);
                        var refreshToken = _jwtToken.GenerateRefreshToken(model);

                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Login Successful.";
                        response.Data = loginProfile;

                        return Ok(new { response, token, refreshToken });
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
                    response.Message = "Employee ID and Password do not exist.";
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Route("RefreshToken")]
        [HttpPost]
        public IActionResult RefreshToken([FromBody] refreshTokenModel refreshToken)
        {
            var response = new Response<LoginProfile>();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("8Zz5tw0Ionm3XPZZfN0NOml3z9FMfmpgXwovR9fp6ryDIoGRM8EPHAB6iHsc0fb"); 

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero 
                };

                var principal = tokenHandler.ValidateToken(refreshToken.refreshToken, tokenValidationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var employeeId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                    if (employeeId != null)
                    {
                        var token = _jwtToken.GenerateAccessToken(new LoginDTO { Employee_ID = employeeId });

                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Tokens refreshed successfully.";

                        return Ok(new { token = token });
                    }
                }

                response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Message = "Invalid refresh token.";
                return Unauthorized(response);
            }
            catch (SecurityTokenException)
            {
                response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = "Invalid or expired refresh token.";
                return Forbid(response.Message);
            }
        }

    }
}
