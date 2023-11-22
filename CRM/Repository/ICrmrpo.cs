using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CRM.Repository
{
    public interface ICrmrpo
    {
        public DataTable Login(AdminLogin model);
        public Task<int> Product(ProductMaster model);
        public Task<List<ProductMaster>> ProductList();
        public Task<int> Customer(CustomerRegistration model);
        public Task<List<CustomerRegistration>> CustomerList();
        public Task<int> EmpRegistration(EmployeeRegistration model);
        public Task<int> Banner(BannerMaster model);
        public Task<List<EmpRegistration>> EmployeeList();

    }
}
