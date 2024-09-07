using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Esf;
using System.Data;

namespace CRM.Repository
{
    public interface ICrmrpo
    {
        //public DataTable Login(AdminLogin model);
        Task<int> LoginAsync(AdminLogin model);
        public Task<int> Product(ProductMaster model);
        public Task<List<ProductMaster>> ProductList();
        public Task<int> Customer(Customer model);
        public Task<List<Customer>> CustomerList(string userIdString);
        public Task<int> EmpRegistration(EmpMultiform model, string Mode, string Emp_Reg_ID,string userId);
        public Task<List<StateMaster>> GetAllState();
        public Task<int> Banner(BannerMaster model);
        // EmployeeList
        public Task<List<EmployeeImportExcel>> EmployeeList();

        public ProductMaster GetproductById(int id);
        public Task<int> updateproduct(ProductMaster model);
        public Task<int> Iupdate(EmployeePersonalDetail model);
        public EmployeePersonalDetail GetempPersonalDetailById(int id);
        public Task<int> updateEmployee(EmployeeList model);

        public Task<int> Quation(Quation model);
        public Task<List<Quation>> QuationList();
        public Quation GetempQuationById(int id);
        public Task<int> Iupdate(Quation model);
        public DataTable ForgetPassword(AdminLogin model);

        public Task<List<salarydetail>> salarydetail(string customerId, string WorkLocation);

        public Task<List<GenerateSalary>> GenerateSalary(string customerId, int Month, int year, string WorkLocation);
        public Task<int> Employer(EmployeerModelEPF model);
        public Task<List<EmployeerEpf>> EmployerList(string Deduction_Cycle);
        public Task<List<Invoice>> GenerateInvoice(string customerId, int Month, int year, string WorkLocation);
      
        public DataTable GetEmployDetailById(string EmpId);
        //for excel
         public byte[] EmployeeListForExcel();
        
         public Task<List<ECS>> ESCExcel(string customerId,string WorkLocation);

        public WorkLocation1 GetWorkLocationById(int id);
        public Task<int> updateWorkLocation(WorkLocation1 model);
        public DesignationMaster GetDesignationById(int id);
        public Task<int> updateDesignation(DesignationMaster model);
        public DepartmentMaster GetDepartmentById(int id);
        public Task<int> updateDepartment(DepartmentMaster model);
        public Customer GetCustomerById(int id);
        public EmployeeSalaryDetail GetempSalaryDetailtById(string EmployeeId);
        public Task<int> updateSalaryDetail(EmployeeSalaryDetail model);
        public Task<int> updateCustomerReg(Customer model);
        public Task<List<GenerateSalaryReportDTO>> GenerateSalaryReport(string customerId, int Month, int year, string WorkLocation);
        public Task<List<EPFReportDTO>> EPFReport(string customerId, int Month, int year, string WorkLocation);
        public Task<List<EPFReportDTO>> ESIReport(string customerId, int Month, int year, string WorkLocation);
        public EmployeerEpf GetEmployer(int id);
        public Task<int> updateEmployer(EmployeerEpf model);
        public EmployeerTd tdsDetails(int CustomerId);
        public byte[] ImportToExcelAttendance(List<salarydetail> data);
        public List<State> BindState();
        public List<City> BindCity(int stateId);
        public Task<List<monthlysalaryExcel>> monthlysalaryReport(string customerId, int Month, int year, string WorkLocation);
        public Task<CustomerRegistration> GetCustomerProfile(string? id);
        public Task<int> UpdateCustomerProfile(CustomerRegistration model, string AddedBy);
        public Task<int> UpdateChangepassword(ChangePassworddto model, string AddedBy, int id);
        public Task<List<EmployeeImportExcel>> CustomerEmployeeList(int id);
        public VendorDto GetVendorById(int id);
        public Task<int> Vendorreg(VendorDto model);
        public Task<int> updateVendorreg(VendorDto model);
        public Task<List<VendorDto>> VendorList();
        public Task<VendorRegistration> GetVendorProfile(string? id);
        public Task<int> UpdateVendorProfile(VendorRegistration model, int id);

    }
}