﻿using CRM.Models.Crm;
using CRM.Models.CRM;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM.Models.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.DependencyResolver;
using System.Reflection;
using System;
using System.Reflection.Metadata;
using System.Drawing;
using Syncfusion.Drawing;
using ClosedXML.Excel;


namespace CRM.Repository
{
    public class Crmrpo : ICrmrpo
    {
        private admin_NDCrMContext _context;

        public Crmrpo(admin_NDCrMContext context)
        {
            _context = context;

        }

        public DataTable Login(AdminLogin model)
        {
            SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
            SqlCommand cmd = new SqlCommand("usp_adminlogin", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@UserName", model.UserName));
            cmd.Parameters.Add(new SqlParameter("@password", model.Password));
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;

        }

        public DataTable ForgetPassword(AdminLogin model)
        {
            SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
            SqlCommand cmd = new SqlCommand("usp_adminloginforgetpassword", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@UserName", model.UserName));
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;

        }
        public async Task<int> Product(ProductMaster model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@ProductName", model.ProductName));
            parameter.Add(new SqlParameter("@Category", model.Category));
            parameter.Add(new SqlParameter("@HSN_SAC_Code", model.HsnSacCode));
            parameter.Add(new SqlParameter("@GST", model.Gst));
            parameter.Add(new SqlParameter("@Price", model.Price));
            parameter.Add(new SqlParameter("@IsDeleted", "0"));


            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec sp_Addproduct @ProductName, @Category,@HSN_SAC_Code,@GST,@Price,@IsDeleted", parameter.ToArray()));

            return result;
        }

        public async Task<List<ProductMaster>> ProductList()
        {
            var result = await _context.ProductMasters
        .FromSqlRaw("EXEC sp_Productlist")
        .ToListAsync();
            return result;
        }
        public async Task<int> Customer(Customer model)
        {

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@Company_Name", model.CompanyName));
            parameter.Add(new SqlParameter("@Work_Location", string.Join(",", model.WorkLocation)));
            parameter.Add(new SqlParameter("@Mobile_number", model.MobileNumber));
            parameter.Add(new SqlParameter("@Alternate_number", model.AlternateNumber));
            parameter.Add(new SqlParameter("@Email", model.Email));
            parameter.Add(new SqlParameter("@GST_Number", model.GstNumber));
            parameter.Add(new SqlParameter("@Billing_Address", model.BillingAddress));
            parameter.Add(new SqlParameter("@Product_Details", model.ProductDetails));
            parameter.Add(new SqlParameter("@Start_date", model.StartDate));
            parameter.Add(new SqlParameter("@Renew_Date", model.RenewDate));
            parameter.Add(new SqlParameter("@State", model.State));

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec CustomerRegistration @Company_Name, @Work_Location,@Mobile_number,@Alternate_number,@Email,@GST_Number,@Billing_Address,@Product_Details,@Start_date,@Renew_Date,@State", parameter.ToArray()));

            return result;
        }
        public async Task<List<CustomerRegistration>> CustomerList()
        {
            var result = await _context.CustomerRegistrations.FromSqlRaw<CustomerRegistration>("Customerlist").ToListAsync();
            return result;
        }
        public async Task<int> EmpRegistration(EmpMultiform model, string Mode, string Emp_Reg_ID)
        {
            try
            {

                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@mode", Mode));
                parameter.Add(new SqlParameter("@Emp_RegID", Emp_Reg_ID));
                parameter.Add(new SqlParameter("@Customer_Id", model.CustomerID));
                parameter.Add(new SqlParameter("@FirstName", model.FirstName));
                parameter.Add(new SqlParameter("@MiddleName", model.MiddleName));
                parameter.Add(new SqlParameter("@LastName", model.LastName));
                parameter.Add(new SqlParameter("@DateOfJoining", model.DateOfJoining));
                parameter.Add(new SqlParameter("@WorkEmail", model.WorkEmail));
                parameter.Add(new SqlParameter("@GenderID", model.GenderID));
                parameter.Add(new SqlParameter("@WorkLocationID", model.WorkLocationID));
                parameter.Add(new SqlParameter("@DesignationID", model.DesignationID));
                parameter.Add(new SqlParameter("@DepartmentID", model.DepartmentID));

                //-- Salary Details
                parameter.Add(new SqlParameter("@AnnualCTC", model.AnnualCTC));
                parameter.Add(new SqlParameter("@Basic", model.Basic));
                parameter.Add(new SqlParameter("@HouseRentAllowance", model.HouseRentAllowance));
                parameter.Add(new SqlParameter("@ConveyanceAllowance", model.ConveyanceAllowance));
                parameter.Add(new SqlParameter("@FixedAllowance", model.FixedAllowance));
                parameter.Add(new SqlParameter("@EPF", model.EPF));
                parameter.Add(new SqlParameter("@MonthlyGrossPay", model.MonthlyGrossPay));
                parameter.Add(new SqlParameter("@MonthlyCTC", model.MonthlyCTC));
                //
                parameter.Add(new SqlParameter("@Personal_Email_Address", model.PersonalEmailAddress));
                parameter.Add(new SqlParameter("@Mobile_Number", model.MobileNumber));
                parameter.Add(new SqlParameter("@Date_Of_Birth", model.DateOfBirth));
                parameter.Add(new SqlParameter("@Father_Name", model.FatherName));
                parameter.Add(new SqlParameter("@PAN", model.PAN));
                parameter.Add(new SqlParameter("@Address_Line_1", model.AddressLine1));
                parameter.Add(new SqlParameter("@Address_Line_2", model.AddressLine2));
                parameter.Add(new SqlParameter("@City", model.City));
                parameter.Add(new SqlParameter("@State_ID", model.StateID));
                parameter.Add(new SqlParameter("@Pincode", model.Pincode));
                //
                parameter.Add(new SqlParameter("@Account_Holder_Name", model.AccountHolderName));
                parameter.Add(new SqlParameter("@Bank_Name", model.BankName));
                parameter.Add(new SqlParameter("@Account_Number", model.AccountNumber));
                parameter.Add(new SqlParameter("@Re_Enter_Account_Number", model.ReEnterAccountNumber));
                parameter.Add(new SqlParameter("@IFSC", model.IFSC));
                parameter.Add(new SqlParameter("@EPF_Number", model.EPF_Number));
                parameter.Add(new SqlParameter("@Deduction_Cycle", model.Deduction_Cycle));
                parameter.Add(new SqlParameter("@Employee_Contribution_Rate", model.Employee_Contribution_Rate));
                parameter.Add(new SqlParameter("@Account_Type_ID", model.AccountTypeID));
                var result = await Task.Run(() => _context.Database.ExecuteSqlRawAsync(@"exec EmployeeRegistration @mode,@Emp_RegID,@Customer_Id,@FirstName,@MiddleName,@LastName,@DateOfJoining,@WorkEmail,@GenderID,@WorkLocationID,@DesignationID,@DepartmentID,@AnnualCTC,@Basic,@HouseRentAllowance,@ConveyanceAllowance,@FixedAllowance,@EPF,@MonthlyGrossPay,@MonthlyCTC,@Personal_Email_Address,@Mobile_Number,@Date_Of_Birth,@Father_Name,@PAN,@Address_Line_1,@Address_Line_2,@City,@State_ID,@Pincode,@Account_Holder_Name,@Bank_Name,@Account_Number,@Re_Enter_Account_Number,@IFSC,@EPF_Number,@Deduction_Cycle,@Employee_Contribution_Rate,@Account_Type_ID", parameter.ToArray()));

                return result;
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }

        public async Task<List<StateMaster>> GetAllState()
        {
            return await _context.StateMasters.ToListAsync();
        }
        public async Task<int> Banner(BannerMaster model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@BannerImage", model.BannerImage));
            parameter.Add(new SqlParameter("@Bannerdescription", model.Bannerdescription));
            parameter.Add(new SqlParameter("@BannerPath", model.Imagepath));
            parameter.Add(new SqlParameter("@AddedBy", model.AddedBy));
            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec Sp_Banner @BannerImage,@Bannerdescription,@BannerPath,@AddedBy", parameter.ToArray()));
            return result;
        }

        public async Task<List<EmployeeImportExcel>> EmployeeList()
        {
            List<EmployeeImportExcel> employeeList = _context.EmpMultiforms.FromSqlRaw<EmployeeImportExcel>("USP_GetEmployeeDetails").ToListAsync().Result;

            return employeeList;
        }

        public ProductMaster GetproductById(int id)
        {
            return _context.ProductMasters.Find(id);
        }
        public async Task<int> updateproduct(ProductMaster model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@id", model.Id));
            parameter.Add(new SqlParameter("@ProductName", model.ProductName));
            parameter.Add(new SqlParameter("@Category", model.Category));
            parameter.Add(new SqlParameter("@HSN_SAC_Code", model.HsnSacCode));
            parameter.Add(new SqlParameter("@GST", model.Gst));
            parameter.Add(new SqlParameter("@Price", model.Price));

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec sp_Updateproduct @id,@ProductName,@Category,@HSN_SAC_Code,@GST,@Price", parameter.ToArray()));

            return result;
        }

        public async Task<int> Iupdate(EmployeePersonalDetail model)
        {
            int result;
            try

            {
                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@ID", model.Id));
                parameter.Add(new SqlParameter("@action", 2));
                parameter.Add(new SqlParameter("@Personal_Email_Address", model.PersonalEmailAddress));
                parameter.Add(new SqlParameter("@Mobile_Number", model.MobileNumber));
                parameter.Add(new SqlParameter("@Date_Of_Birth", model.DateOfBirth));
                parameter.Add(new SqlParameter("@Father_Name", model.FatherName));
                parameter.Add(new SqlParameter("@PAN", model.Pan));
                parameter.Add(new SqlParameter("@Address_Line_1", model.AddressLine1));
                parameter.Add(new SqlParameter("@Address_Line_2", model.AddressLine2));
                parameter.Add(new SqlParameter("@City", model.City));
                parameter.Add(new SqlParameter("@State_ID", model.StateId));
                parameter.Add(new SqlParameter("@Pincode", model.Pincode));

                result = await Task.Run(() => _context.Database
               .ExecuteSqlRawAsync(@"exec sp_Employee_Personal_Details @action,@ID,@Personal_Email_Address,
            @Mobile_Number,@Date_Of_Birth,@Father_Name,@PAN,@Address_Line_1,
              @Address_Line_2,@City,@State_ID,@Pincode", parameter.ToArray()));

            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
            return result;
        }

        public EmployeePersonalDetail GetempPersonalDetailById(int id)
        {
            return _context.EmployeePersonalDetails.Find(id);
        }

        public async Task<int> updateEmployee(EmployeeList model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@Id", model.Id));
            parameter.Add(new SqlParameter("@FirstName", model.FirstName));
            parameter.Add(new SqlParameter("@MiddleName", model.MiddleName));
            parameter.Add(new SqlParameter("@LastName", model.LastName));
            parameter.Add(new SqlParameter("@DateOfJoining", DateTime.Now));
            parameter.Add(new SqlParameter("@WorkEmail", model.WorkEmail));
            parameter.Add(new SqlParameter("@GenderID", model.GenderId));
            parameter.Add(new SqlParameter("@WorkLocationID", model.WorkLocationId));
            parameter.Add(new SqlParameter("@DesignationID", model.DesignationId));
            parameter.Add(new SqlParameter("@DepartmentID", model.DepartmentId));
            parameter.Add(new SqlParameter("@MonthlyCTC", model.MonthlyCTC));

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec sp_updateEmpRegistration @Id,@FirstName, @MiddleName,@LastName,@DateOfJoining,@WorkEmail,@GenderID,@WorkLocationID,@DesignationID,@DepartmentID,@MonthlyCTC", parameter.ToArray()));

            return result;
        }

        public async Task<int> Quation(Quation model)
        {

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@Action", 1));
            parameter.Add(new SqlParameter("@ID", model.Id));
            parameter.Add(new SqlParameter("@Company_Name", model.CompanyName));
            parameter.Add(new SqlParameter("@Customer_Name", model.CustomerName));
            parameter.Add(new SqlParameter("@Email", model.Email));
            parameter.Add(new SqlParameter("@Sales_Person_Name", model.SalesPersonName));
            parameter.Add(new SqlParameter("@Product_ID", model.ProductId));
            parameter.Add(new SqlParameter("@Subject", model.Subject));
            parameter.Add(new SqlParameter("@Amount", model.Amount));
            parameter.Add(new SqlParameter("@Mobile", model.Mobile));
            parameter.Add(new SqlParameter("@IsDeleted", '0'));


            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec SP_Quation @Action,@ID,@Company_Name,@Customer_Name,@Email,@Sales_Person_Name,@Product_ID,@Subject,@Amount,@Mobile", parameter.ToArray()));

            return result;
        }


        public async Task<List<Quation>> QuationList()
        {
            var result = await _context.Quations.FromSqlRaw<Quation>("QuationList").ToListAsync();
            return result;
        }


        public Quation GetempQuationById(int id)
        {
            return _context.Quations.Find(id);
        }

        public async Task<int> Iupdate(Quation model)
        {
            int result;
            try

            {
                var parameter = new List<SqlParameter>();

                parameter.Add(new SqlParameter("@action", 2));
                parameter.Add(new SqlParameter("@ID", model.Id));
                parameter.Add(new SqlParameter("@Company_Name", model.CompanyName));
                parameter.Add(new SqlParameter("@Customer_Name", model.CustomerName));
                parameter.Add(new SqlParameter("@Email", model.Email));
                parameter.Add(new SqlParameter("@Sales_Person_Name", model.SalesPersonName));
                parameter.Add(new SqlParameter("@Product_ID ", model.ProductId));
                parameter.Add(new SqlParameter("@Subject", model.Subject));
                parameter.Add(new SqlParameter("@Amount", model.Amount));
                parameter.Add(new SqlParameter("@Mobile", model.Mobile));
                parameter.Add(new SqlParameter("@IsDeleted", '0'));


                result = await Task.Run(() => _context.Database
               .ExecuteSqlRawAsync(@"exec SP_Quation @action,@ID,@Company_Name,
            @Customer_Name,@Email,@Sales_Person_Name,@Product_ID,@Subject,
              @Amount,@Mobile,@IsDeleted", parameter.ToArray()));

            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
            return result;
        }


        public async Task<List<salarydetail>> salarydetail(string customerId, string WorkLocation)
        {
            List<salarydetail> emp = new List<salarydetail>();
            try
            {
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("sp_SalaryDetail", con);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = Convert.ToInt32(customerId) });
                cmd.Parameters.Add(new SqlParameter("@WorkLocation", SqlDbType.Int) { Value = Convert.ToInt32(WorkLocation) });
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var emps = new salarydetail()
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        FirstName = rdr["FirstName"] == DBNull.Value ? null : Convert.ToString(rdr["FirstName"]),
                        EmployeeId = rdr["EmployeeId"] == DBNull.Value ? null : Convert.ToString(rdr["EmployeeId"]),
                        MonthlyCtc = rdr["MonthlyCtc"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["MonthlyCtc"])
                    };

                    emp.Add(emps);
                }
                return emp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                emp = null;
            }
        }

        public async Task<List<GenerateSalary>> GenerateSalary(string customerId, int Month, int year, string WorkLocation)
        {
            try
            {
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("GetGenerateSalary", con);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = Convert.ToInt32(customerId) });
                cmd.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int) { Value = Convert.ToInt32(Month) });
                cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.Int) { Value = Convert.ToInt32(year) });
                cmd.Parameters.Add(new SqlParameter("@WorkLocation", SqlDbType.Int) { Value = Convert.ToInt32(WorkLocation) });
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                List<GenerateSalary> emp = new List<GenerateSalary>();
                while (rdr.Read())
                {
                    var emps = new GenerateSalary()
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        EmployeeId = Convert.ToString(rdr["Employee_ID"]),
                        EmployeeName = Convert.ToString(rdr["First_Name"]),
                        MonthlyGrossPay = Convert.ToDecimal(rdr["MonthlyGrossPay"]),
                        MonthlyCtc = Convert.ToDecimal(rdr["MonthlyCTC"])
                    };

                    emp.Add(emps);
                }
                return emp;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<int> Employer(Employeer_EPF model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@EPF_Number", model.EPF_Number));
            parameter.Add(new SqlParameter("@Deduction_Cycle", model.Deduction_Cycle));
            parameter.Add(new SqlParameter("@Employer_Contribution_Rate", model.Employer_Contribution_Rate));

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec USP_Employeer_EPF  @EPF_Number, @Deduction_Cycle,@Employer_Contribution_Rate", parameter.ToArray()));

            return result;
        }

        public async Task<List<EmployeerEpf>> EmployerList()
        {
            var result = await _context.EmployeerEpfs.FromSqlRaw<EmployeerEpf>("EmployerList").ToListAsync();
            return result;
        }

        public async Task<List<Invoice>> GenerateInvoice(string customerId, int Month, int year, string WorkLocation)
        {
            try
            {
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("GenerateInvoice", con);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = Convert.ToInt32(customerId) });
                cmd.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int) { Value = Convert.ToInt32(Month) });
                cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.Int) { Value = Convert.ToInt32(year) });
                cmd.Parameters.Add(new SqlParameter("@WorkLocation", SqlDbType.Int) { Value = Convert.ToInt32(WorkLocation) });
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                List<Invoice> emp = new List<Invoice>();
                while (rdr.Read())
                {
                    var emps = new Invoice()
                    {
                        EmployeeCount = Convert.ToInt32(rdr["EmployeeCount"]),
                        Company_Name = Convert.ToString(rdr["Company_Name"]),
                        Billing_Address = Convert.ToString(rdr["Billing_Address"]),
                        GST_Number = Convert.ToString(rdr["GST_Number"]),
                        HSN_SAC_Code = Convert.ToString(rdr["HSN_SAC_Code"]),
                        Cgst = Convert.ToString(rdr["Cgst"]),
                        Scgst = Convert.ToString(rdr["Scgst"]),
                        Igst = Convert.ToString(rdr["Igst"]),
                        State = Convert.ToString(rdr["State"]),
                    };

                    emp.Add(emps);
                }
                return emp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetEmployDetailById(string EmpId)
        {

            SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
            SqlCommand cmd = new SqlCommand("USP_GetEmployeeDetailById", con);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;

        }
        //for excel
        public byte[] EmployeeListForExcel()
        {
            List<EmployeeImportExcel> employeeList = _context.EmpMultiforms.FromSqlRaw<EmployeeImportExcel>("USP_GetEmployeeDetails").ToListAsync().Result;

            using (var workbook = new XLWorkbook())
            {

               var worksheet = workbook.Worksheets.Add("EmployeeList");
                var currentwork = 1;
                worksheet.Cell(currentwork, 1).Value = "Sr.No.";
                worksheet.Cell(currentwork, 1).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 2).Value = "First Name";
                //worksheet.Cell(currentwork, 2).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 3).Value = "Middle Name";
                //worksheet.Cell(currentwork, 3).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 2).Value = "Employee Name";
                worksheet.Cell(currentwork, 2).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 3).Value = "Employee ID";
                worksheet.Cell(currentwork, 3).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 4).Value = "Date Of Joining";
                worksheet.Cell(currentwork, 4).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 5).Value = "Work Email";
                worksheet.Cell(currentwork, 5).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 6).Value = "Gender";
                worksheet.Cell(currentwork, 6).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 7).Value = "Work Location";
                worksheet.Cell(currentwork, 7).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 8).Value = "Designation";
                worksheet.Cell(currentwork, 8).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 9).Value = "Department";
                worksheet.Cell(currentwork, 9).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 10).Value = "Company Name";
                worksheet.Cell(currentwork, 10).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 11).Value = "Personal Email Address";
                worksheet.Cell(currentwork, 11).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 12).Value = "Mobile Number";
                worksheet.Cell(currentwork, 12).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 13).Value = "Date Of Birth";
                worksheet.Cell(currentwork, 13).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 14).Value = "Age";
                worksheet.Cell(currentwork, 14).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 15).Value = "Father Name";
                worksheet.Cell(currentwork, 15).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 16).Value = "PAN";
                worksheet.Cell(currentwork, 16).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 19).Value = "Address Line 1";
                //worksheet.Cell(currentwork, 19).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 20).Value = "Address Line 2";
                //worksheet.Cell(currentwork, 20).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 21).Value = "City";
                //worksheet.Cell(currentwork, 21).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 22).Value = "State";
                //worksheet.Cell(currentwork, 22).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 23).Value = "Pin Code";
                //worksheet.Cell(currentwork, 23).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 24).Value = "Account Holder Name";
                //worksheet.Cell(currentwork, 24).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 17).Value = "Bank Name";
                worksheet.Cell(currentwork, 17).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 18).Value = "Account Number";
                worksheet.Cell(currentwork, 18).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 27).Value = "Re-enter Account Number";
                //worksheet.Cell(currentwork, 27).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 19).Value = "IFSC";
                worksheet.Cell(currentwork, 19).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 20).Value = "EPF Number";
                worksheet.Cell(currentwork, 20).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 21).Value = "Employee Contribution Rate";
                worksheet.Cell(currentwork, 21).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 22).Value = "Deduction Cycle";
                worksheet.Cell(currentwork, 22).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 23).Value = "Account Type";
                worksheet.Cell(currentwork, 23).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 24).Value = "Annual CTC";
                worksheet.Cell(currentwork, 24).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 34).Value = "Basic";
                //worksheet.Cell(currentwork, 34).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 35).Value = "HouseRent Allowance";
                //worksheet.Cell(currentwork, 35).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 36).Value = "Conveyance Allowance";
                //worksheet.Cell(currentwork, 36).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 37).Value = "Fixed Allowance";
                //worksheet.Cell(currentwork, 37).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 25).Value = "EPF";
                worksheet.Cell(currentwork, 25).Style.Fill.BackgroundColor = XLColor.Yellow;
                //worksheet.Cell(currentwork, 39).Value = "Monthly CTC";
                //worksheet.Cell(currentwork, 39).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 26).Value = "Monthly Gross Pay";
                worksheet.Cell(currentwork, 26).Style.Fill.BackgroundColor = XLColor.Yellow;
                currentwork++;

                var index = 1;
                foreach (var item in employeeList)
                {

                    worksheet.Cell(currentwork, 1).Value = index++;
                    //worksheet.Cell(currentwork, 2).Value = item.FirstName;
                    //worksheet.Cell(currentwork, 3).Value = item.MiddleName;
                    worksheet.Cell(currentwork, 2).Value = item.MiddleName == null ? "" + item.FirstName + " " + "" + ' ' + "" + "" + item.LastName + "" : "" + item.FirstName + "" + "" + ' ' + "" + "" + item.MiddleName + "" + "" + ' ' + "" + "" + item.LastName + "";
                    worksheet.Cell(currentwork, 3).Value = item.EmployeeId;
                    worksheet.Cell(currentwork, 4).Value = item.DateOfJoining;
                    worksheet.Cell(currentwork, 5).Value = item.WorkEmail;
                    worksheet.Cell(currentwork, 6).Value = item.Gender;
                    worksheet.Cell(currentwork, 7).Value = item.WorkLocation;
                    worksheet.Cell(currentwork, 8).Value = item.DesignationName;
                    worksheet.Cell(currentwork, 9).Value = item.DepartmentName;
                    worksheet.Cell(currentwork, 10).Value = item.CustomerName;
                    worksheet.Cell(currentwork, 11).Value = item.PersonalEmailAddress;
                    worksheet.Cell(currentwork, 12).Value = item.Mobile;
                    worksheet.Cell(currentwork, 13).Value = item.DateOfBirth;
                    worksheet.Cell(currentwork, 14).Value = item.Age;
                    worksheet.Cell(currentwork, 15).Value = item.FatherName;
                    worksheet.Cell(currentwork, 16).Value = item.PAN;
                    //worksheet.Cell(currentwork, 19).Value = item.AddressLine1;
                    //worksheet.Cell(currentwork, 20).Value = item.AddressLine2;
                    //worksheet.Cell(currentwork, 21).Value = item.City;
                    //worksheet.Cell(currentwork, 22).Value = item.State;
                    //worksheet.Cell(currentwork, 23).Value = item.Pincode;
                    //worksheet.Cell(currentwork, 24).Value = item.AccountHolderName;
                    worksheet.Cell(currentwork, 17).Value = item.BankName;
                    worksheet.Cell(currentwork, 18).Value = item.AccountNumber;
                    //worksheet.Cell(currentwork, 27).Value = item.ReEnterAccountNumber;
                    worksheet.Cell(currentwork, 19).Value = item.IFSC;
                    worksheet.Cell(currentwork, 20).Value = item.EPF_Number;
                    worksheet.Cell(currentwork, 21).Value = item.Employee_Contribution_Rate;
                    worksheet.Cell(currentwork, 22).Value = item.Deduction_Cycle;
                    worksheet.Cell(currentwork, 23).Value = item.AccountType;
                    worksheet.Cell(currentwork, 24).Value = item.AnnualCTC;
                    //worksheet.Cell(currentwork, 34).Value = item.Basic;
                    //worksheet.Cell(currentwork, 35).Value = item.HouseRentAllowance;
                    //worksheet.Cell(currentwork, 36).Value = item.ConveyanceAllowance;
                    //worksheet.Cell(currentwork, 37).Value = item.FixedAllowance;
                    worksheet.Cell(currentwork, 25).Value = item.EPF;
                    //worksheet.Cell(currentwork, 39).Value = item.MonthlyCTC;
                    worksheet.Cell(currentwork, 26).Value = item.MonthlyGrossPay;
                    currentwork++;
                }
                using (var stram = new MemoryStream())
                {
                    workbook.SaveAs(stram);
                    return stram.ToArray();
                }
            }

        }
    }

}