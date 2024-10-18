using CRM.Controllers;
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
        Task<int> LoginAsync(AdminLogin model);
        public Task<int> Product(ProductMaster model);
        public Task<List<ProductMaster>> ProductList();
        public Task<int> Customer(Customer model,int vendorid);
        public Task<List<CustomerListDto>> CustomerList(int vendorid);
        public Task<int> EmpRegistration(EmpMultiform model, string Mode, string Emp_Reg_ID,int userId);
        public Task<List<StateMaster>> GetAllState();
        public Task<int> Banner(BannerMaster model);
        // EmployeeList
        public Task<List<EmployeeImportExcel>> EmployeeList();

        //public ProductMaster GetproductById(int id);
        public Task<int> updateproduct(ProductMaster model);
        public Task<int> Iupdate(EmployeePersonalDetail model);
        public EmployeePersonalDetail GetempPersonalDetailById(int id);
        public Task<int> updateEmployee(EmployeeList model);

        public Task<int> Quation(QuationDto model);
        public Task<List<QuationDto>> QuationList();
        public Task<int> Iupdate(QuationDto model);
        public DataTable ForgetPassword(AdminLogin model);

        public Task<List<salarydetail>> salarydetail(int Userid);

        public Task<List<GenerateSalary>> GenerateSalary(int Month, int year, int Userid,string EmployeeId);
        public Task<int> Employer(EmployeerModelEPF model,int AdminLoginId);
        public Task<List<EmployeerEpf>> EmployerList(string Deduction_Cycle, int AdminLoginId);
        public Task<Invoice> GenerateInvoice(int ID);
      
        public DataTable GetEmployDetailById(string EmpId, int Userid);
        //for excel
         public byte[] EmployeeListForExcel();
        
         public Task<List<ECS>> ESCExcel(int ? Userid);

        public WorkLocation1 GetWorkLocationById(int id);
        public Task<int> updateWorkLocation(WorkLocation1 model);
        public Task<int> updateDesignation(DesignationMaster model);
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
        public Task<VendorRegResultDTO> Vendorreg(VendorDto model);
        public Task<int> updateVendorreg(VendorDto model);
        public Task<List<VendorDto>> VendorList();
        public Task<VendorRegistrationDto> GetVendorProfile(string? id);
        public Task<int> UpdateVendorProfile(VendorRegistrationDto model, int id);
        public Task<List<EmployeeApprovedPresnolInfo>> ApprovedPresnolInfoList(int Userid);
        public Task<bool> AddApprovedPresnolInfo(EmployeePresnolInfoList model);
        public Task<List<ApprovedbankdetailList>> ApprovedbankdetailList(int Userid);
        public Task<bool> AddApprovedbankdetail(ApprovedbankdetailList model);
        public Task<Offerletter> GetOfferletterbyid(int? id);
        public Task<int> AddOfferletterdetail(Offerletters model, int Userid);
        public Task<int> updateOfferletterdetail(Offerletters model);
        public Task<List<empOfferletter>> OfferletterdetailList(int Userid);
        Task<int> AddVendorProduct(VendorProductMaster model, int VendorId);
        Task<List<VendorProductDTO>> GetVendorProductList(int vendorid);
        public Task<List<LeavemasterDto>> getLeavemaster(int Userid);
        Task<bool> AddVendorCategory(VendorCategoryMaster model, int VendorId);
        Task<List<VendorCategoryMaster>> GetVendorCategoryListByVendorId(int VendorId);
        public Task<EmpExperienceletter> GetExperienceletterbyid(int? id);
        public Task<int> updateExperienceletterdetail(EmpExperienceletter model);
        public Task<int> AddExperienceletterdetail(EmpExperienceletter model, int Userid);
        Task<bool> CustomerInvoice(List<ProductDetail> model ,string InvoiceNo, int vendorid);
        Task<List<CustomerInvoiceDTO>> GetCustometInvoiceList(int vendorid);
        Task<CustomerInvoiceDTO> CustomerProductInvoice(string InvoiceNumber);
        Task<bool> AddVendorBankDeatils(VendorBankDetail model, int VendorId);
        Task<List<VendorBankDetail>> GetVendorBankDetail(int VendorId);
        Task<bool> AddOfficeEvents(OfficeEvent model, int VendorId);
        public Task<List<ApprovedLeaveApplyList>> GetLeaveapplydetailList(int? Userid);
        Task<bool> AddEmployeeEpf(EmployeeEpfPayrollInfo model, int VendorId);
        Task<bool> AddEmployeeEsic(EmployeeEsicPayrollInfo model, int VendorId);
    }
}