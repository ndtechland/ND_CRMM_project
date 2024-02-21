using CRM.Models.APIDTO;
using CRM.Models.Crm;

namespace CRM.Repository
{
    public interface IEmployee
    {
        public Task<EmployeeBasicInfo> GetEmployeeById(string Employeeid);
        public Task<EmployeePersonalDetail> PersonalDetail(EmpPersonalDetail model);
        public Task<EmployeeBankDetail> Bankdetail(bankdetail model);

    }
}
