using CRM.Models.APIDTO;

namespace CRM.IUtilities
{
    public interface IJwtToken
    {
        string token(LoginDTO model);
    }
}
