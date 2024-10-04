using CRM.Models.APIDTO;

namespace CRM.IUtilities
{
    public interface IJwtToken
    {
        string GenerateAccessToken(LoginDTO model);
        string GenerateRefreshToken(LoginDTO model);
    }

}
