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
        public Task<EmployeeCheckIn> Empcheckin(EmpCheckIn model, bool CheckIN);
        public Task<ApprovedPresnolInfo> webPersonalDetail(webPersonalDetail model, string userid);
        public Task<EmployeeCheckIn> Empcheckout(EmpCheckIn model, bool CheckIN);
        public Task<Empattendancedatail> GetEmpattendance(string userid);
        public Task<EmployeeRegistration> Updateprofilepicture(profilepicture model, string userid);
        public Task<Loginactivity> GetEmpLoginactivity(string userid);
        public Task<List<TasksassignDto>> GetEmpTasksassign(string userid);

    }
}
