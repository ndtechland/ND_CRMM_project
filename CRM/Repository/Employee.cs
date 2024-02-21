using CRM.Controllers;
using CRM.Models.APIDTO;
using CRM.Models.Crm;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;

namespace CRM.Repository
{
    public class Employee : IEmployee
    {
        private readonly admin_NDCrMContext _context;
        public Employee(admin_NDCrMContext context)
        {
            this._context = context;
        }
        public async Task<EmployeeBasicInfo> GetEmployeeById(string Employeeid)
        {
            try
            {
                if (Employeeid != null)
                {
                    var empid = _context.EmployeeRegistrations.Where(x => x.EmployeeId == Employeeid).Select(x => new EmployeeBasicInfo
                    {
                          FirstName = x.FirstName,
                          MiddleName = x.MiddleName,
                          LastName = x.LastName,
                          DateOfJoining = x.DateOfJoining,
                          WorkEmail = x.WorkEmail,
                          GenderName = _context.GenderMasters.Where(g =>g.Id == x.GenderId).Select(g => g.GenderName).First(),
                          WorkLocationName = _context.Cities.Where(g => g.Id == Convert.ToInt16(x.WorkLocationId)).Select(g => g.City1).First(),
                          DesignationName = _context.DesignationMasters.Where(g => g.Id == Convert.ToInt16(x.DesignationId)).Select(g => g.DesignationName).First(),
                          DepartmentName = _context.DepartmentMasters.Where(g => g.Id == Convert.ToInt16(x.DepartmentId)).Select(g => g.DepartmentName).First().Trim(),
                        CustomerName = _context.CustomerRegistrations.Where(g => g.Id == x.CustomerId).Select(g => g.CompanyName).First(),
                          EmployeeId = x.EmployeeId,
                           //RoleId = x.RoleId,
                          IsDeleted = x.IsDeleted,
                    }).FirstOrDefault();
                    return empid;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }
        }
    }
}
