using CRM.Models.Crm;
using CRM.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace CRM.Repository
{
    public interface ICrmrpo
    {
        public Task<int> Login(AdminLogin model);
        public Task<int> Product(ProductMaster model);
        public Task<List<ProductMaster>> ProductList();
        public Task<int> Customer(CustomerRegistration model);
        public Task<List<CustomerRegistration>> CustomerList();
        public Task<int> EmpRegistration(EmployeeRegistration model);
    }
}
