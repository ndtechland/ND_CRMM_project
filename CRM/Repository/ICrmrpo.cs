using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using Microsoft.AspNetCore.Mvc;
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
        public Task<int> EmployeeBasicinfo(EmployeePersonalDetail model);
        public Task<List<StateMaster>> GetAllState();
        public Task<int> Banner(BannerMaster model);
        public Task<List<EmployeeList>> EmployeeList();
        public Task<List<EmployeePersonalDetail>> EmployeeBasicinfoList();

        public ProductMaster GetproductById(int id);
        public Task<int> updateproduct(ProductMaster model);
        public Task<int> Iupdate(EmployeePersonalDetail model);
        public EmployeePersonalDetail GetempPersonalDetailById(int id);
        public Task<int> updateEmployee(EmployeeList model);

        public Task<int> Gengeneratesalary(EmployeeSalaryDetail model);

        public Task<int> Quation(Quation model);
        public Task<List<Quation>> QuationList();

        //public Task<int> EmployeeBankDetail(EmployeeBankDetail model);


        public Quation GetempQuationById(int id);
        public Task<int> Iupdate(Quation model);

        public Task<List<salarydetail>> salarydetail();



    }
}
