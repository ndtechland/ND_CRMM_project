using CRM.Models.Crm;

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
                var result = _context.Blogs.Where(b=>b.IsPublished==true).ToList();
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
