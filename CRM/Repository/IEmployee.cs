using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.DTO;
using NuGet.Common;

namespace CRM.Repository
{
    public interface IEmployee
    {
        public Task<EmployeeBasicInfo> GetEmployeeById(string userid);
        public Task<ApprovedPresnolInfo> PersonalDetail(EmpPersonalDetail model,string userid);
        public Task<Approvedbankdetail> Bankdetail(bankdetail model,string userid);
        public Task<List<City>> getcity(int stateid);
        public Task<List<State>> Getstate();
        public Task<bankdetail> GetBankdetail(string userid);
        public Task<salarydetails> Getsalarydetails(string userid);
        public Task<leavedto> LeaveType(string userid);
        public Task<bool> ApplyLeave(ApplyLeave model, string userid);
        public Task<List<EmpattendanceDto>> GetAllEmpsalaryslip(string userid);
        public Task<EmpattendanceDto> Getsalarydetails(string userid, int month, int year);
        public Task<CompanyLoctionDto> GetCompanyLoction(string userid);
        public Task<EmployeeCheckIn> Empcheckin(EmpCheckIn model, string userid);

    }
}
