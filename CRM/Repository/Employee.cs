using CRM.Controllers;
using CRM.Models.APIDTO;
using CRM.Models.Crm;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
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
        public async Task<EmployeeBasicInfo> GetEmployeeById(string userid)
        {
            try
            {
                if (userid != null)
                {
                    var empid = _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).Select(x => new EmployeeBasicInfo
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
        public async Task<EmployeePersonalDetail> PersonalDetail(EmpPersonalDetail model,string userid)
        {
            try
            {
                var emppersonal = await _context.EmployeePersonalDetails
                    .Where(x => x.EmpRegId == userid)
                    .FirstOrDefaultAsync();

                if (emppersonal != null)
                {
                    emppersonal.PersonalEmailAddress = model.PersonalEmailAddress;
                    emppersonal.MobileNumber = model.MobileNumber;
                    emppersonal.DateOfBirth = model.DateOfBirth;
                    emppersonal.Age = model.Age;
                    emppersonal.FatherName = model.FatherName;
                    emppersonal.Pan = model.Pan;
                    emppersonal.AddressLine1 = model.AddressLine1;
                    emppersonal.AddressLine2 = model.AddressLine2;
                    emppersonal.City = model.City;
                    emppersonal.StateId = model.StateId;
                    emppersonal.Pincode = model.Pincode;
                    await _context.SaveChangesAsync();
                    return emppersonal;
                }
                var emp = await _context.EmployeeRegistrations
                    .Where(x => x.EmployeeId == userid)
                    .FirstOrDefaultAsync();

                if (emp != null)
                {
                    EmployeePersonalDetail empP = new EmployeePersonalDetail
                    {
                        PersonalEmailAddress = model.PersonalEmailAddress,
                        MobileNumber = model.MobileNumber,
                        DateOfBirth = model.DateOfBirth,
                        Age = model.Age,
                        FatherName = model.FatherName,
                        Pan = model.Pan,
                        AddressLine1 = model.AddressLine1,
                        AddressLine2 = model.AddressLine2,
                        City = model.City,
                        StateId = model.StateId,
                        Pincode = model.Pincode,
                        EmpRegId = userid,
                    };

                    _context.EmployeePersonalDetails.Add(empP);
                    await _context.SaveChangesAsync();

                    return empP; 
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public async Task<EmployeeBankDetail> Bankdetail(bankdetail model, string userid)
        {
            try
            {
                var empbank = await _context.EmployeeBankDetails
                    .Where(x => x.EmpId == userid)
                    .FirstOrDefaultAsync();

                if (empbank != null)
                {
                    empbank.AccountHolderName = model.AccountHolderName;
                    empbank.BankName = model.BankName;
                    empbank.AccountNumber = model.AccountNumber;
                    empbank.ReEnterAccountNumber = model.ReEnterAccountNumber;
                    empbank.Ifsc = model.Ifsc;
                    empbank.AccountTypeId = model.AccountTypeId;
                    empbank.EpfNumber = model.EpfNumber;
                    empbank.DeductionCycle = model.DeductionCycle;
                    empbank.EmployeeContributionRate = model.EmployeeContributionRate;
                    empbank.Nominee = model.Nominee;
                    await _context.SaveChangesAsync();
                    return empbank;
                }
                var emp = await _context.EmployeeRegistrations
                    .Where(x => x.EmployeeId == userid)
                    .FirstOrDefaultAsync();

                if (emp != null)
                {
                    EmployeeBankDetail empB = new EmployeeBankDetail
                    {
                        AccountHolderName = model.AccountHolderName,
                        BankName = model.BankName,
                        AccountNumber = model.AccountNumber,
                        ReEnterAccountNumber = model.ReEnterAccountNumber,
                        Ifsc = model.Ifsc,
                        EmpId = userid,
                        AccountTypeId = model.AccountTypeId,
                        EpfNumber = model.EpfNumber,
                        DeductionCycle = model.DeductionCycle,
                        EmployeeContributionRate = model.EmployeeContributionRate,
                        Nominee = model.Nominee,                       
                    };

                    _context.EmployeeBankDetails.Add(empB);
                    await _context.SaveChangesAsync();

                    return empB;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public async Task<EmpPersonalDetail> GetPresnolInfo(string userid)
        {
            try
            {
                if(userid != null)
                {
                    var result = await _context.EmployeePersonalDetails.Where(x =>x.EmpRegId == userid && x.IsDeleted == false).Select( x => new EmpPersonalDetail
                    {
                       PersonalEmailAddress =x.PersonalEmailAddress,
                        MobileNumber =x.MobileNumber,
                        DateOfBirth =x.DateOfBirth,
                        Age =x.Age,
                        FatherName =x.FatherName,
                        Pan =x.Pan,
                        AddressLine1 =x.AddressLine1,
                        AddressLine2 =x.AddressLine2,
                        City =x.City,
                        StateId =x.StateId,
                        Pincode =x.Pincode,
                    }).FirstOrDefaultAsync();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Error :" + ex.Message);
            }
        }
        public async Task<List<City>> getcity(int stateid)
        {
            try
            {
                var cities = await _context.Cities.Where(x => x.StateId == stateid).ToListAsync();

                return cities;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<List<State>> Getstate()
        {
            try
            {
                var state = await _context.States.ToListAsync();
                return state;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
    }
}
