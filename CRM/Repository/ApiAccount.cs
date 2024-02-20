using CRM.Models.APIDTO;
using CRM.Models.Crm;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.ModelBinding;

namespace CRM.Repository
{
    public class ApiAccount : IApiAccount
    {
        private readonly admin_NDCrMContext _context;
        public ApiAccount(admin_NDCrMContext context)
        {
            this._context = context;
        }
        public async Task<bool> Login(LoginDTO model)
        {
			try
			{
				if(model !=  null)
				{
                    bool check = await _context.EmployeeLogins.AnyAsync(x => x.EmployeeId == model.Employee_ID && x.Password == model.Password);
				    if(check)
                    {
                        return true;
                    }
                    else {
                        return false; 
                    }
                }
                return false;
            }
			catch (Exception ex)
			{

				throw new Exception("Error : " + ex.Message);
			}
        }
    }
}
