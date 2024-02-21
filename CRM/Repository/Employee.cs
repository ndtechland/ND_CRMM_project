using CRM.Controllers;
using CRM.Models.Crm;
using Microsoft.EntityFrameworkCore;

namespace CRM.Repository
{
    public class Employee : IEmployee
    {
        private readonly admin_NDCrMContext _context;
        public Employee(admin_NDCrMContext context)
        {
            this._context = context;
        }
        public async Task<bool> GetEmployeeById(string Employeeid)
        {
            try
            {
                if (Employeeid != null)
                {
                    var empid = _context.EmployeeRegistrations.Where(x => x.EmployeeId == Employeeid).Select(x => new EmployeeRegistration
                      {
                          FirstName = x.FirstName,
                          MiddleName = x.MiddleName,
                          LastName = x.LastName,
                          DateOfJoining = x.DateOfJoining,
                          WorkEmail = x.WorkEmail,
                          GenderId = x.GenderId,
                           WorkLocationId = x.WorkLocationId,
                          DesignationId = x.DesignationId,
                          DepartmentId = x.DepartmentId,
                          CustomerId = x.CustomerId,
                          EmployeeId = x.EmployeeId,
                           RoleId = x.RoleId,
                          IsDeleted = x.IsDeleted,
                    }).FirstOrDefault();
                    if (empid != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
    }
}
