﻿using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CRM.Repository
{
    public interface ICrmrpo
    {
        public DataTable Login(AdminLogin model);
        public Task<int> Product(ProductMaster model);
        public Task<List<ProductMaster>> ProductList();
        public Task<int> Customer(Customer model);
        public Task<List<CustomerRegistration>> CustomerList();
        public Task<int> EmpRegistration(EmpMultiform model, string Mode, string Emp_Reg_ID);
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
        public Task<int> Employer(Employeer_EPF model);
        public Task<List<EmployeerEpf>> EmployerList();
        public Task<List<Invoice>> GenerateInvoice(string customerId, int Month, int year, string WorkLocation);
      
        public DataTable GetEmployDetailById(string EmpId);
        //for excel
         public byte[] EmployeeListForExcel();


    }
}