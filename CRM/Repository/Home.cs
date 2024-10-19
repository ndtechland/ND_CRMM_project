using CRM.Models.APIDTO;
using CRM.Models.Crm;
using Microsoft.EntityFrameworkCore;

namespace CRM.Repository
{
    public class Home:IHome
    {
        private readonly admin_NDCrMContext _context;
        public Home(admin_NDCrMContext context)
        {
            _context = context;
        }
        public async Task<List<Blog>> GetBlogs()
        {
            try
            {
                var result = _context.Blogs.Where(b => b.IsPublished == true).ToList();
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<aboutCompanyDto> Getaboutcompany(string userid)
        {
            try
            {
                if (userid != null)
                {
                    var empid = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).Select(x => new aboutCompanyDto
                    {
                        Companylink = _context.Aboutcompanies.Where(g => g.Id == x.Vendorid).Select(g => g.Companylink).First(),
                    }).FirstOrDefaultAsync();
                    return empid;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
    }
}
