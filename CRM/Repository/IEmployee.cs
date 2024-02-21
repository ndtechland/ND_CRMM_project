using CRM.Models.APIDTO;

namespace CRM.Repository
{
    public interface IEmployee
    {
        public Task<EmployeeBasicInfo> GetEmployeeById(string Employeeid);

    }
}
