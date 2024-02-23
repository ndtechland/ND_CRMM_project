using CRM.Models.APIDTO;
using CRM.Models.Crm;

namespace CRM.Repository
{
    public interface IEmployee
    {
        public Task<EmployeeBasicInfo> GetEmployeeById(string Employeeid);
        public Task<EmployeePersonalDetail> PersonalDetail(EmpPersonalDetail model,string userid);
        public Task<EmployeeBankDetail> Bankdetail(bankdetail model,string userid);

    }
}
