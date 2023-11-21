using CRM.Models.Crm;
using Microsoft.EntityFrameworkCore;

namespace CRM.Models.DTO
{
    public class admin_NDCrM :DbContext
    {
        public admin_NDCrM()
        {
        }

        public admin_NDCrM(DbContextOptions<admin_NDCrM> options)
            : base(options)
        {
        }
        public virtual DbSet<EmpRegistration> EmpRegistrations { get; set; } = null!;
        public virtual DbSet<EmployeeBasicinfo> EmployeeBasicinfos { get; set; } = null!;


    }
}
