using CRM.Models.Crm;

namespace CRM.Repository
{
    public interface IHome
    {
        Task<List<Blog>> GetBlogs();
    }
}
