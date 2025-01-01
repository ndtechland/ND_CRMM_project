using CRM.Models.Crm;

namespace CRM.Repository
{
    public class AdministratorImplementation:IAdministrator
    {
        private readonly admin_NDCrMContext _context;
        public AdministratorImplementation(admin_NDCrMContext context)
        {
            _context= context;
        }
    }
}
