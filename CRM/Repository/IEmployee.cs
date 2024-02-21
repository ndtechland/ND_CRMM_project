namespace CRM.Repository
{
    public interface IEmployee
    {
        public Task<bool> GetEmployeeById(string Employeeid);

    }
}
