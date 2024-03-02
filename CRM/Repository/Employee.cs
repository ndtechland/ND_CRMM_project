using CRM.Controllers;
using CRM.IUtilities;
using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Utilities;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Services.Account;
using MimeKit.Encodings;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using Syncfusion.Pdf.Graphics;
using System;
using System.Globalization;

namespace CRM.Repository
{
    public class Employee : IEmployee
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly admin_NDCrMContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Employee(admin_NDCrMContext context, IWebHostEnvironment hostingEnvironment)
        {
            this._context = context;
            _webHostEnvironment = hostingEnvironment;
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
                        FullName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                        DateOfJoining = String.Format("{0:dd-MM-yyyy}", x.DateOfJoining),
                        WorkEmail = x.WorkEmail,
                        GenderName = _context.GenderMasters.Where(g => g.Id == x.GenderId).Select(g => g.GenderName).First(),
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
        public async Task<EmployeePersonalDetail> PersonalDetail(EmpPersonalDetail model, string userid)
        {
            try
            {
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                var emppersonal = await _context.EmployeePersonalDetails.Where(x => x.EmpRegId == userid && x.IsDeleted == false).FirstOrDefaultAsync();
                if (emppersonal != null )
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
                    emppersonal.Aadharcard = model.AadharNo;
                    string panImagePath = fileOperation.SaveBase64Image("img1", model.Panbase64, allowedExtensions);
                    emppersonal.Panimg = panImagePath;
                    if (model.Aadharbase64 != null)
                    {
                        for (int i = 0; i < model.Aadharbase64.Count; i++)
                        {
                            string aadharImagePath = fileOperation.SaveBase64Image("img1",  model.Aadharbase64[i], allowedExtensions);

                            if (i == 0)
                            {
                                emppersonal.AadharOne = aadharImagePath;
                            }
                            else if (i == 1)
                            {
                                emppersonal.AadharTwo = aadharImagePath;
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    return emppersonal;
                }
                else
                {
                    var emp = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).FirstOrDefaultAsync();
                    if (emp != null)
                    {
                        EmployeePersonalDetail empP = new EmployeePersonalDetail();
                        {
                            empP.PersonalEmailAddress = model.PersonalEmailAddress;
                            empP.MobileNumber = model.MobileNumber;
                            empP.DateOfBirth = model.DateOfBirth;
                            empP.Age = model.Age;
                            empP.FatherName = model.FatherName;
                            empP.Pan = model.Pan;
                            empP.AddressLine1 = model.AddressLine1;
                            empP.AddressLine2 = model.AddressLine2;
                            empP.City = model.City;
                            empP.StateId = model.StateId;
                            empP.Pincode = model.Pincode;
                            empP.EmpRegId = userid;
                            empP.Aadharcard = model.AadharNo;
                            if (model.Aadharbase64 != null && model.Aadharbase64.Count > 0)
                            {
                                for (int i = 0; i < model.Aadharbase64.Count; i++)
                                {
                                    string aadharImagePath = fileOperation.SaveBase64Image("img1", model.Aadharbase64[i], allowedExtensions);

                                    if (i == 0)
                                    {
                                        empP.AadharOne = aadharImagePath;
                                    }
                                    else if (i == 1)
                                    {
                                        empP.AadharTwo = aadharImagePath;
                                    }
                                }
                            }
                            if (model.Panbase64 != null)
                            {
                                string panImagePath = fileOperation.SaveBase64Image("img1", model.Panbase64, allowedExtensions);
                                empP.Panimg = panImagePath;
                            }
                        }

                        _context.EmployeePersonalDetails.Add(empP);
                        await _context.SaveChangesAsync();

                        return empP;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PersonalDetail: {ex.Message}");
                return null;
            }
        }

        public async Task<EmployeeBankDetail> Bankdetail(bankdetail model, string userid)
        {
            try
            {
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                var empbank = await _context.EmployeeBankDetails.Where(x => x.EmpId == userid && x.IsDeleted == false).FirstOrDefaultAsync();
                if (empbank != null)
                {
                    empbank.AccountHolderName = model.AccountHolderName;
                    empbank.BankName = model.BankName;
                    empbank.AccountNumber = model.AccountNumber;
                    empbank.ReEnterAccountNumber = model.ReEnterAccountNumber;
                    empbank.Ifsc = model.Ifsc;
                    empbank.AccountTypeId = Convert.ToInt32(model.AccountTypeId);
                    empbank.EpfNumber = model.EpfNumber;
                    empbank.DeductionCycle = model.DeductionCycle;
                    empbank.EmployeeContributionRate = model.EmployeeContributionRate;
                    empbank.Nominee = model.Nominee;
                    string ChequeImagePath = fileOperation.SaveBase64Image("ChequeImage", model.Chequebase64, allowedExtensions);
                    empbank.Chequeimage = ChequeImagePath;
                    empbank.IsDeleted = false;
                    await _context.SaveChangesAsync();
                    return empbank;
                }
                else
                {
                    var emp = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).FirstOrDefaultAsync();
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
                            AccountTypeId = Convert.ToInt32(model.AccountTypeId),
                            EpfNumber = model.EpfNumber,
                            DeductionCycle = model.DeductionCycle,
                            EmployeeContributionRate = model.EmployeeContributionRate,
                            Nominee = model.Nominee,
                            IsDeleted = false,
                        };
                        string ChequeImagePath = fileOperation.SaveBase64Image("ChequeImage", model.Chequebase64, allowedExtensions);
                        empB.Chequeimage = ChequeImagePath;
                        _context.EmployeeBankDetails.Add(empB);
                        await _context.SaveChangesAsync();

                        return empB;
                    }
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
                if (userid != null)
                {
                    var result = await _context.EmployeePersonalDetails.Where(x => x.EmpRegId == userid && x.IsDeleted == false).Select(x => new EmpPersonalDetail
                    {
                        PersonalEmailAddress = x.PersonalEmailAddress,
                        MobileNumber = x.MobileNumber,
                        DateOfBirth = x.DateOfBirth,
                        Age = x.Age,
                        FatherName = x.FatherName,
                        Pan = x.Pan,
                        AddressLine1 = x.AddressLine1,
                        AddressLine2 = x.AddressLine2,
                        City = x.City,
                        StateId = x.StateId,
                        Pincode = x.Pincode,
                        AadharNo = x.Aadharcard,
                        AadharOne = "/img1/" + x.AadharOne,
                        AadharTwo = "/img1/" + x.AadharTwo,
                        Panimg = "/img1/" + x.Panimg,
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
        public async Task<bankdetail> GetBankdetail(string userid)
        {
            try
            {
                if (userid != null)
                {
                    var result = await _context.EmployeeBankDetails.Where(x => x.EmpId == userid && x.IsDeleted == false).Select(x => new bankdetail
                    {
                        AccountHolderName = x.AccountHolderName,
                        BankName = x.BankName,
                        AccountNumber = x.AccountNumber,
                        ReEnterAccountNumber = x.ReEnterAccountNumber,
                        Ifsc = x.Ifsc,
                        AccountTypeId = _context.AccountTypeMasters.Where(g => g.Id == x.AccountTypeId).Select(g => g.AccountType).First().Trim(),
                        EpfNumber = x.EpfNumber,
                        DeductionCycle = x.DeductionCycle,
                        EmployeeContributionRate = x.EmployeeContributionRate,
                        Nominee = x.Nominee,
                        Chequeimage = "/ChequeImage/" + x.Chequeimage,
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
        public async Task<EmployeeRegistration> Updateprofilepicture(profilepicture model, string userid)
        {
            try
            {
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                var emp = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).FirstOrDefaultAsync();

                if (emp != null)
                {   

                    if (model.Empprofilebase64 != null)
                    {
                        string EmpprofileImagePath = fileOperation.SaveBase64Image("EmpProfile", model.Empprofilebase64, allowedExtensions);
                        emp.EmpProfile = EmpprofileImagePath;
                    }


                    await _context.SaveChangesAsync();
                    return emp;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PersonalDetail: {ex.Message}");
                return null;
            }
        }
        public async Task<profilepicture> Getprofilepicture(string userid)
        {
            try
            {
                if (userid != null)
                {
                    var result = await _context.EmployeeRegistrations.Where(x => x.EmployeeId == userid && x.IsDeleted == false).Select(x => new profilepicture
                    {
                        EmpProfile = "/EmpProfile/" + x.EmpProfile,
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
    }
}
    