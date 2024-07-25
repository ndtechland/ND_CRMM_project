using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.DTO;
using NuGet.Common;

namespace CRM.Repository
{
    public interface IEmployee
    {
        public Task<EmployeeBasicInfo> GetEmployeeById(string userid);
        public Task<EmployeePersonalDetail> PersonalDetail(EmpPersonalDetail model,string userid);
        public Task<EmployeeBankDetail> Bankdetail(bankdetail model,string userid);
        public Task<EmpPersonalDetail> GetPresnolInfo(string userid);
        public Task<List<City>> getcity(int stateid);
        public Task<List<State>> Getstate();
        public Task<bankdetail> GetBankdetail(string userid);
        public Task<EmployeeRegistration> Updateprofilepicture(profilepicture model, string userid);
        public Task<profilepicture> Getprofilepicture(string userid);
        public Task<salarydetails> Getsalarydetails(string userid);
    }
}
