using CRM.Models.APIDTO;

namespace CRM.Repository
{
    public interface IApiAccount
    {
      Task<bool>  Login(LoginDTO model);
    }
}
