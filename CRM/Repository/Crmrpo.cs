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
using Humanizer;
using Org.BouncyCastle.Asn1.X509;


namespace CRM.Repository
{
    public class Crmrpo : ICrmrpo
    {
        public IConfiguration Configuration { get; }
        private admin_NDCrMContext _context;
        private readonly IEmailService _IEmailService;
        //public virtual DbSet<EmployeeImportExcel> EmpMultiforms { get; set; } = null!;
        public Crmrpo(admin_NDCrMContext context, IConfiguration configuration, IEmailService IEmailService)
        {
            _context = context;
            Configuration = configuration;
            _IEmailService = IEmailService;
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
            parameter.Add(new SqlParameter("@stateId", model.StateId));

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec CustomerRegistration @Company_Name, @Work_Location,@Mobile_number,@Alternate_number,@Email,@GST_Number,@Billing_Address,@Product_Details,@Start_date,@Renew_Date,@State,@stateId", parameter.ToArray()));

            return result;
        }
        public async Task<List<Customer>> CustomerList()
        {
            List<Customer> cs = new List<Customer>();
            try
            {
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("Customerlist", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var cse = new Customer()
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        CompanyName = rdr["Company_Name"] == DBNull.Value ? null : Convert.ToString(rdr["Company_Name"]),
                        WorkLocation = rdr["Work_Location"] == DBNull.Value ? null : new string[] { Convert.ToString(rdr["Work_Location"]) },
                        MobileNumber = rdr["Mobile_number"] == DBNull.Value ? "0" : Convert.ToString(rdr["Mobile_number"]),
                        AlternateNumber = rdr["Alternate_number"] == DBNull.Value ? "0" : Convert.ToString(rdr["Alternate_number"]),
                        Email = rdr["Email"] == DBNull.Value ? "0" : Convert.ToString(rdr["Email"]),
                        GstNumber = rdr["GST_Number"] == DBNull.Value ? "0" : Convert.ToString(rdr["GST_Number"]),
                        BillingAddress = rdr["Billing_Address"] == DBNull.Value ? "0" : Convert.ToString(rdr["Billing_Address"]),
                        ProductDetails = rdr["Product_Details"] == DBNull.Value ? "0" : Convert.ToString(rdr["Product_Details"]),
                        StartDate = rdr["Start_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rdr["Start_date"]),
                        RenewDate = rdr["Renew_Date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rdr["Renew_Date"]),
                        State = rdr["State"] == DBNull.Value ? null : Convert.ToString(rdr["State"]),
                        StateName = rdr["statename"] == DBNull.Value ? null : Convert.ToString(rdr["statename"]),

                    };

                    cs.Add(cse);
                }
                return cs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cs = null;
            }
            //List<Customer> cs = new List<Customer>();
            //var result = await _context.CustomerRegistrations.FromSqlRaw<Customer>("Customerlist").ToListAsync();
            //return result;
        }
        public async Task<int> EmpRegistration(EmpMultiform model, string Mode, string Emp_Reg_ID)
        {
            try
            {
                ///
                model.EmployeeId = Emp_Reg_ID;
                SqlConnection con = new SqlConnection(Configuration.GetConnectionString("db1"));
                SqlCommand cmd = new SqlCommand("EmployeeRegistrationtest", con);
                cmd.Parameters.AddWithValue("@mode", Mode);
                cmd.Parameters.AddWithValue("@Emp_RegID", Emp_Reg_ID);
                cmd.Parameters.AddWithValue("@Employee_ID", model.EmployeeId);
                cmd.Parameters.AddWithValue("@Customer_Id", model.CustomerID);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@MiddleName", model.MiddleName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@DateOfJoining", model.DateOfJoining);
                cmd.Parameters.AddWithValue("@WorkEmail", model.WorkEmail);
                cmd.Parameters.AddWithValue("@GenderID", model.GenderID);
                cmd.Parameters.AddWithValue("@WorkLocationID", model.WorkLocationID);
                cmd.Parameters.AddWithValue("@DesignationID", model.DesignationID);
                cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                cmd.Parameters.AddWithValue("@stateId", model.stateId);

                //-- Salary Details
                cmd.Parameters.AddWithValue("@AnnualCTC", model.AnnualCTC);
                cmd.Parameters.AddWithValue("@Basic", model.Basic);
                cmd.Parameters.AddWithValue("@HouseRentAllowance", model.HouseRentAllowance);
                //cmd.Parameters.AddWithValue("@TravellingAllowance", model.TravellingAllowance);
                cmd.Parameters.AddWithValue("@ESIC", model.ESIC);
                cmd.Parameters.AddWithValue("@EPF", model.EPF);
                cmd.Parameters.AddWithValue("@MonthlyGrossPay", model.MonthlyGrossPay);
                cmd.Parameters.AddWithValue("@MonthlyCTC", model.MonthlyCTC);
                cmd.Parameters.AddWithValue("@Professionaltax", model.Professionaltax);
                cmd.Parameters.AddWithValue("@servicecharge", model.Servicecharge);
                cmd.Parameters.AddWithValue("@SpecialAllowance", model.SpecialAllowance);
                cmd.Parameters.AddWithValue("@gross", model.Gross);
                cmd.Parameters.AddWithValue("@amount", model.Amount);
                cmd.Parameters.AddWithValue("@tdspercentage", model.Tdspercentage);
                // Personal detail
                cmd.Parameters.AddWithValue("@Personal_Email_Address", model.PersonalEmailAddress);
                cmd.Parameters.AddWithValue("@Mobile_Number", model.MobileNumber);
                cmd.Parameters.AddWithValue("@Date_Of_Birth", model.DateOfBirth);
                cmd.Parameters.AddWithValue("@Father_Name", model.FatherName);
                cmd.Parameters.AddWithValue("@PAN", model.PAN);
                cmd.Parameters.AddWithValue("@Address_Line_1", model.AddressLine1);
                cmd.Parameters.AddWithValue("@Address_Line_2", model.AddressLine2);
                cmd.Parameters.AddWithValue("@City", model.City);
                cmd.Parameters.AddWithValue("@State_ID", model.StateID);
                cmd.Parameters.AddWithValue("@Pincode", model.Pincode);

                // Bank detail
                cmd.Parameters.AddWithValue("@Account_Holder_Name", model.AccountHolderName);
                cmd.Parameters.AddWithValue("@Bank_Name", model.BankName);
                cmd.Parameters.AddWithValue("@Account_Number", model.AccountNumber);
                cmd.Parameters.AddWithValue("@Re_Enter_Account_Number", model.ReEnterAccountNumber);
                cmd.Parameters.AddWithValue("@IFSC", model.IFSC);
                cmd.Parameters.AddWithValue("@EPF_Number", model.EPF_Number);
                cmd.Parameters.AddWithValue("@Deduction_Cycle", model.Deduction_Cycle);
                cmd.Parameters.AddWithValue("@Employee_Contribution_Rate", model.Employee_Contribution_Rate);
                cmd.Parameters.AddWithValue("@Account_Type_ID", model.AccountTypeID);
                cmd.Parameters.AddWithValue("@nominee", model.nominee);

                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);


                if(Mode == "INS")
                {
                    EmployeeRole employeeRole = new()
                    {
                        EmployeeRegistrationId = model.EmployeeId,
                        EmployeeRole1 = "2",
                        Description = "Employee"

                    };
                    _context.EmployeeRoles.Add(employeeRole);
                    _context.SaveChanges();

                    EmployeeLogin employeeLogin = new()
                    {
                        EmployeeId = model.EmployeeId,
                        Password = "" + model.FirstName + "" + model.DateOfBirth.Date.Year + ""
                    };
                    _context.EmployeeLogins.Add(employeeLogin);
                    _context.SaveChanges();
                    string password = "" + model.FirstName + "" + model.DateOfBirth.Date.Year + "";
                    _IEmailService.SendEmailCred(model, password);
                }

                return 1;
            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    Console.WriteLine("Error Number: {0}", error.Number);
                    Console.WriteLine("Error Message: {0}", error.Message);
                    Console.WriteLine("Procedure: {0}", error.Procedure);
                    Console.WriteLine("Line Number: {0}", error.LineNumber);
                    Console.WriteLine("Source: {0}", error.Source);
                    Console.WriteLine("Server: {0}", error.Server);
                }

                throw new Exception("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
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
           .ExecuteSqlRawAsync(@"exec SP_Quation @Action,@ID,@Company_Name,@Customer_Name,@Email,@Sales_Person_Name,@Product_ID,@Subject,@Amount,@Mobile,@IsDeleted", parameter.ToArray()));

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
                        MonthlyCtc = rdr["MonthlyCtc"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["MonthlyCtc"]),
                        CustomerID = (long)(rdr["CustomerID"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr["CustomerID"])),
                        FatherName = rdr["FirstName"] == DBNull.Value ? null : Convert.ToString(rdr["FatherName"]),
                        Incentive = rdr["Incentive"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["Incentive"]),
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
        public async Task<int> Employer(EmployeerModelEPF model)
        {
            var existingActiveRecords = _context.EmployeerEpfs.Where(e => e.IsActive && e.DeductionCycle == model.Deduction_Cycle).ToList();

            if (existingActiveRecords.Count > 0)
            {
                foreach (var record in existingActiveRecords)
                {
                    record.IsActive = false;
                }

                _context.SaveChanges();
            }
            if (model.EsicEmployer_Contribution_Rate == null && model.EsicEPF_Number == null)
            {
                var parameter = new List<SqlParameter>
    {
        new SqlParameter("@EPF_Number", model.EPF_Number),
        new SqlParameter("@Deduction_Cycle", model.Deduction_Cycle),
        new SqlParameter("@Employer_Contribution_Rate", model.Employer_Contribution_Rate)
    };

                var result = await Task.Run(() => _context.Database
                    .ExecuteSqlRawAsync(@"exec USP_Employeer_EPF  @EPF_Number, @Deduction_Cycle, @Employer_Contribution_Rate", parameter.ToArray()));

                return result;
            }
            else if (model.Employer_Contribution_Rate == null && model.EPF_Number == null)
            {
                var parameter = new List<SqlParameter>
    {
        new SqlParameter("@EPF_Number", model.EsicEPF_Number),
        new SqlParameter("@Deduction_Cycle", model.Deduction_Cycle),
        new SqlParameter("@Employer_Contribution_Rate", model.EsicEmployer_Contribution_Rate)
    };

                var result = await Task.Run(() => _context.Database
                    .ExecuteSqlRawAsync(@"exec USP_Employeer_EPF  @EPF_Number, @Deduction_Cycle, @Employer_Contribution_Rate", parameter.ToArray()));

                return result;
            }
            return 1;
        }

        public async Task<List<EmployeerEpf>> EmployerList(string Deduction_Cycle)
        {
            var result = await _context.EmployeerEpfs
                .FromSqlInterpolated($"EXEC EmployerList {Deduction_Cycle}")
                .ToListAsync();

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
                cmd.Parameters.Add(new SqlParameter("@WorkLocation", SqlDbType.NVarChar) { Value = WorkLocation });
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
                        GenerateSalary = Convert.ToDecimal(rdr["GenerateSalary"]),
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
                worksheet.Cell(currentwork, 20).Style.NumberFormat.NumberFormatId = 49;
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
                worksheet.Cell(currentwork, 25).Style.NumberFormat.NumberFormatId = 1;
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
        public WorkLocation1 GetWorkLocationById(int id)
        {
            return _context.WorkLocations1.Find(id);
        }
        public async Task<int> updateWorkLocation(WorkLocation1 model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@id", model.Id));
            parameter.Add(new SqlParameter("@CityID", model.CityId));
            parameter.Add(new SqlParameter("@stateId", model.StateId));
            parameter.Add(new SqlParameter("@Commissoninpercentage", model.Commissoninpercentage));


            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec sp_updateWorkLocation @id,@CityID,@stateId,@Commissoninpercentage", parameter.ToArray()));

            return result;
        }

        public DesignationMaster GetDesignationById(int id)
        {
            return _context.DesignationMasters.Find(id);
        }
        public async Task<int> updateDesignation(DesignationMaster model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@id", model.Id));
            parameter.Add(new SqlParameter("@DesignationName", model.DesignationName));

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec sp_updateDesignation @id,@DesignationName", parameter.ToArray()));

            return result;
        }
        public DepartmentMaster GetDepartmentById(int id)
        {
            return _context.DepartmentMasters.Find(id);
        }
        public async Task<int> updateDepartment(DepartmentMaster model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@id", model.Id));
            parameter.Add(new SqlParameter("@DepartmentName", model.DepartmentName));

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec sp_updateDepartment @id,@DepartmentName", parameter.ToArray()));

            return result;
        }
        public Customer GetCustomerById(int id)
        {
            Customer cs = new Customer();
            try
            {
                DateTime startDate;
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("sp_GetCustomerById", con);
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Convert.ToInt32(id) });
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    cs = new Customer()
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        CompanyName = rdr["Company_Name"] == DBNull.Value ? null : Convert.ToString(rdr["Company_Name"]),
                        WorkLocation = rdr["Work_Location"] == DBNull.Value ? new string[0] : ((string)rdr["Work_Location"]).Split(','),
                        MobileNumber = rdr["Mobile_number"] == DBNull.Value ? null : Convert.ToString(rdr["Mobile_number"]),
                        AlternateNumber = (rdr["Alternate_number"] == DBNull.Value ? null : Convert.ToString(rdr["Alternate_number"])),
                        Email = rdr["Email"] == DBNull.Value ? null : Convert.ToString(rdr["Email"]),
                        GstNumber = rdr["GST_Number"] == DBNull.Value ? null : Convert.ToString(rdr["GST_Number"]),
                        BillingAddress = rdr["Billing_Address"] == DBNull.Value ? null : Convert.ToString(rdr["Billing_Address"]),
                        ProductDetails = rdr["ProductDetails"] == DBNull.Value ? null : Convert.ToString(rdr["ProductDetails"]),
                        StartDate = (DateTime)(rdr["Start_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["Start_date"])),
                        RenewDate = (DateTime)(rdr["Renew_Date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["Renew_Date"])),
                        State = rdr["State"] == DBNull.Value ? null : Convert.ToString(rdr["State"]),
                        StateId = rdr["stateId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["stateId"]),
                    };
                }
                return cs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeSalaryDetail GetempSalaryDetailtById(string EmployeeId)
        {
            return _context.EmployeeSalaryDetails.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
        }
        public async Task<int> updateSalaryDetail(EmployeeSalaryDetail model)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@EmployeeID", model.EmployeeId));
            parameters.Add(new SqlParameter("@AnnualCTC", model.AnnualCtc));
            parameters.Add(new SqlParameter("@Basic", model.Basic));
            parameters.Add(new SqlParameter("@HouseRentAllowance", model.HouseRentAllowance));
            parameters.Add(new SqlParameter("@TravellingAllowance", model.TravellingAllowance));
            parameters.Add(new SqlParameter("@ESIC", model.Esic));
            parameters.Add(new SqlParameter("@EPF", model.Epf));
            parameters.Add(new SqlParameter("@MonthlyGrossPay", model.MonthlyGrossPay));
            parameters.Add(new SqlParameter("@MonthlyCTC", model.MonthlyCtc));
            parameters.Add(new SqlParameter("@Professionaltax", model.Professionaltax));
            parameters.Add(new SqlParameter("@Incentive", model.Incentive));
            var result = await _context.Database.ExecuteSqlRawAsync(@"exec sp_SalaryDetails @EmployeeID,@AnnualCTC,@Basic,@HouseRentAllowance,@TravellingAllowance,@ESIC,@EPF,@MonthlyGrossPay,@MonthlyCTC,@Professionaltax,@Incentive", parameters.ToArray());
            return result;
        }
        public async Task<int> updateCustomerReg(Customer model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@id", model.Id));
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
            parameter.Add(new SqlParameter("@stateId", model.StateId));
            var result = await _context.Database.ExecuteSqlRawAsync(@"exec sp_updateCustomer_Reg @id,@Company_Name, @Work_Location,@Mobile_number,@Alternate_number,@Email,@GST_Number,@Billing_Address,@Product_Details,@Start_date,@Renew_Date,@State,@stateId", parameter.ToArray());
            return result;
        }

        public async Task<List<ECS>> ESCExcel(string customerId, string WorkLocation)
        {
            List<ECS> emp = new List<ECS>();
            try
            {
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("sp_ECSSalary", con);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = Convert.ToInt32(customerId) });
                cmd.Parameters.Add(new SqlParameter("@WorkLocation", SqlDbType.Int) { Value = Convert.ToInt32(WorkLocation) });
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var emps = new ECS()
                    {
                        FirstName = rdr["FirstName"] == DBNull.Value ? null : Convert.ToString(rdr["FirstName"]),
                        EmployeeId = rdr["EmployeeId"] == DBNull.Value ? null : Convert.ToString(rdr["EmployeeId"]),
                        AccountNumber = (int)(rdr["AccountNumber"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["AccountNumber"])),
                        Ifsc = rdr["Ifsc"] == DBNull.Value ? null : Convert.ToString(rdr["Ifsc"]),
                        netpayment = rdr["netpayment"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["netpayment"]),
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

        public async Task<List<GenerateSalaryReportDTO>> GenerateSalaryReport(string customerId, int Month, int year, string WorkLocation)
        {
            try
            {
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("GetGenerateSalary_Report", con);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = Convert.ToInt32(customerId) });
                cmd.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int) { Value = Convert.ToInt32(Month) });
                cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.Int) { Value = Convert.ToInt32(year) });
                cmd.Parameters.Add(new SqlParameter("@WorkLocation", SqlDbType.Int) { Value = Convert.ToInt32(WorkLocation) });
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                List<GenerateSalaryReportDTO> emp = new List<GenerateSalaryReportDTO>();
                while (rdr.Read())
                {
                    var emps = new GenerateSalaryReportDTO()
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        EmployeeId = Convert.ToString(rdr["Employee_ID"]),
                        EmployeeName = Convert.ToString(rdr["First_Name"]),
                        MonthlyGrossPay = Convert.ToDecimal(rdr["MonthlyGrossPay"]),
                        MonthlyCtc = Convert.ToDecimal(rdr["MonthlyCTC"]),
                        GenerateSalary = Convert.ToDecimal(rdr["GenerateSalary"]),
                        EPF = Convert.ToDecimal(rdr["EPF"]),
                        ESIC = Convert.ToDecimal(rdr["ESIC"])
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

        public async Task<List<EPFReportDTO>> EPFReport(string customerId, int Month, int year, string WorkLocation)
        {
            try
            {
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("GetEPF_Report", con);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = Convert.ToInt32(customerId) });
                cmd.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int) { Value = Convert.ToInt32(Month) });
                cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.Int) { Value = Convert.ToInt32(year) });
                cmd.Parameters.Add(new SqlParameter("@WorkLocation", SqlDbType.Int) { Value = Convert.ToInt32(WorkLocation) });
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                List<EPFReportDTO> emp = new List<EPFReportDTO>();
                while (rdr.Read())
                {
                    var emps = new EPFReportDTO()
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        EmployeeId = Convert.ToString(rdr["Employee_ID"]),
                        EmployeeName = Convert.ToString(rdr["EmployeeName"]),
                        MonthlyCtc = Convert.ToDecimal(rdr["MonthlyCTC"]),
                        PAN = Convert.ToString(rdr["PAN"])
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

        public async Task<List<EPFReportDTO>> ESIReport(string customerId, int Month, int year, string WorkLocation)
        {
            try
            {
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("GetEPF_Report", con);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = Convert.ToInt32(customerId) });
                cmd.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int) { Value = Convert.ToInt32(Month) });
                cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.Int) { Value = Convert.ToInt32(year) });
                cmd.Parameters.Add(new SqlParameter("@WorkLocation", SqlDbType.Int) { Value = Convert.ToInt32(WorkLocation) });
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                List<EPFReportDTO> emp = new List<EPFReportDTO>();
                while (rdr.Read())
                {
                    var emps = new EPFReportDTO()
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        EmployeeId = Convert.ToString(rdr["Employee_ID"]),
                        EmployeeName = Convert.ToString(rdr["EmployeeName"]),
                        MonthlyCtc = Convert.ToDecimal(rdr["MonthlyCTC"]),
                        PAN = Convert.ToString(rdr["PAN"])
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

        public EmployeerEpf GetEmployer(int id)
        {
            return _context.EmployeerEpfs.Find(id);
        }

        public async Task<int> updateEmployer(EmployeerEpf model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@id", model.Id));
            parameter.Add(new SqlParameter("@EPF_Number", model.EpfNumber));
            parameter.Add(new SqlParameter("@Deduction_Cycle", model.DeductionCycle));
            parameter.Add(new SqlParameter("@Employer_Contribution_Rate", model.EmployerContributionRate));


            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec sp_updateEmployer @id,@EPF_Number,@Deduction_Cycle,@Employer_Contribution_Rate", parameter.ToArray()));

            return result;
        }
        public EmployeerTd tdsDetails(int CustomerId)
        {
            return _context.EmployeerTds.Where(x => x.CustomerId == CustomerId).FirstOrDefault();
        }

        public byte[] ImportToExcelAttendance(List<salarydetail> data)
        {
            
           // List<EmployeeImportExcel> employeeList = _context.EmpMultiforms.FromSqlRaw<EmployeeImportExcel>("USP_GetEmployeeDetails").ToListAsync().Result;

            using (var workbook = new XLWorkbook())
            {

                var worksheet = workbook.Worksheets.Add("EmployeeList");
                var currentwork = 1;
                worksheet.Cell(currentwork, 1).Value = "Sr.No.";
                worksheet.Cell(currentwork, 1).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 2).Value = "Employee Name";
                worksheet.Cell(currentwork, 2).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 3).Value = "Father Name";
                worksheet.Cell(currentwork, 3).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 4).Value = "Employee Id";
                worksheet.Cell(currentwork, 4).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 5).Value = "Monthly CTC";
                worksheet.Cell(currentwork, 5).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Cell(currentwork, 6).Value = "Attendance";
                worksheet.Cell(currentwork, 6).Style.Fill.BackgroundColor = XLColor.Yellow;

                currentwork++;

                var index = 1;
                foreach (var item in data)
                {
                    worksheet.Cell(currentwork, 1).Value = index++;
                    worksheet.Cell(currentwork, 2).Value = item.FirstName;
                    worksheet.Cell(currentwork, 3).Value = item.FatherName;
                    worksheet.Cell(currentwork, 4).Value = item.EmployeeId;
                    worksheet.Cell(currentwork, 5).Value = item.MonthlyCtc;
                    currentwork++;
                }
                using (var stram = new MemoryStream())
                {
                    workbook.SaveAs(stram);
                    return stram.ToArray();
                }
            }

        }

        public List<State> BindState()
        {
            List<State> lstState = new List<State>();
            try
            {
                lstState = _context.States.ToList();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return lstState;
        }

        public List<City> BindCity(int stateId)
        {
            List<City> lstCity = new List<City>();
            try
            {
                lstCity = _context.Cities.Where(a => a.StateId == stateId).ToList();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return lstCity;
       }     
        public async Task<List<monthlysalaryExcel>> monthlysalaryReport(string customerId, int Month, int year, string WorkLocation)
        {
            List<monthlysalaryExcel> emp = new List<monthlysalaryExcel>();
            try
            {
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("sp_monthlySalaryreport", con);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = Convert.ToInt32(customerId) });
                cmd.Parameters.Add(new SqlParameter("@WorkLocation", SqlDbType.Int) { Value = Convert.ToInt32(WorkLocation) });
                cmd.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int) { Value = Convert.ToInt32(Month) });
                cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.Int) { Value = Convert.ToInt32(year) });
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var emps = new monthlysalaryExcel()
                    {
                        FirstName = rdr["FirstName"] == DBNull.Value ? null : Convert.ToString(rdr["FirstName"]),
                        CustomerName = rdr["CustomerName"] == DBNull.Value ? null : Convert.ToString(rdr["CustomerName"]),
                        MiddleName = rdr["MiddleName"] == DBNull.Value ? null : Convert.ToString(rdr["MiddleName"]),
                        LastName = rdr["LastName"] == DBNull.Value ? null : Convert.ToString(rdr["LastName"]),
                        DateOfJoining = rdr["DateOfJoining"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rdr["DateOfJoining"]),
                        WorkEmail = rdr["WorkEmail"] == DBNull.Value ? null : Convert.ToString(rdr["WorkEmail"]),
                        Gender = rdr["Gender"] == DBNull.Value ? null : Convert.ToString(rdr["Gender"]),
                        WorkLocation = rdr["WorkLocation"] == DBNull.Value ? null : Convert.ToString(rdr["WorkLocation"]),
                        DesignationName = rdr["DesignationName"] == DBNull.Value ? null : Convert.ToString(rdr["DesignationName"]),
                        DepartmentName = rdr["DepartmentName"] == DBNull.Value ? null : Convert.ToString(rdr["DepartmentName"]),
                        Emp_Reg_ID = rdr["Emp_Reg_ID"] == DBNull.Value ? null : Convert.ToString(rdr["Emp_Reg_ID"]),
                        AnnualCTC = rdr["AnnualCTC"] == DBNull.Value ? 0.0m : Convert.ToDecimal(rdr["AnnualCTC"]),
                        Basic = rdr["Basic"] == DBNull.Value ? 0.0m : Convert.ToDecimal(rdr["Basic"]),
                        HouseRentAllowance = rdr["HouseRentAllowance"] == DBNull.Value ? 0.0m : Convert.ToDecimal(rdr["HouseRentAllowance"]),
                        TravellingAllowance = rdr["TravellingAllowance"] == DBNull.Value ? 0.0m : Convert.ToDecimal(rdr["TravellingAllowance"]),
                        ESIC = rdr["ESIC"] == DBNull.Value ? 0.0m : Convert.ToDecimal(rdr["ESIC"]),
                        EPF = rdr["EPF"] == DBNull.Value ? 0.0m : Convert.ToDecimal(rdr["EPF"]),
                        MonthlyGrossPay = rdr["MonthlyGrossPay"] == DBNull.Value ? 0.0m : Convert.ToDecimal(rdr["MonthlyGrossPay"]),
                        MonthlyCTC = rdr["MonthlyCTC"] == DBNull.Value ? 0.0m : Convert.ToDecimal(rdr["MonthlyCTC"]),
                        SpecialAllowance = rdr["SpecialAllowance"] == DBNull.Value ? 0.0m : Convert.ToDecimal(rdr["SpecialAllowance"]),
                        gross = rdr["gross"] == DBNull.Value ? 0.0m : Convert.ToDecimal(rdr["gross"]),
                        PersonalEmailAddress = rdr["PersonalEmailAddress"] == DBNull.Value ? null : Convert.ToString(rdr["PersonalEmailAddress"]),
                        Mobile = rdr["Mobile"] == DBNull.Value ? null : Convert.ToString(rdr["Mobile"]),
                        DateOfBirth = rdr["DateOfBirth"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rdr["DateOfBirth"]),
                        Age = rdr["Age"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["Age"]),
                        FatherName = rdr["FatherName"] == DBNull.Value ? null : Convert.ToString(rdr["FatherName"]),
                        PAN = rdr["PAN"] == DBNull.Value ? null : Convert.ToString(rdr["PAN"]),
                        AddressLine1 = rdr["AddressLine1"] == DBNull.Value ? null : Convert.ToString(rdr["AddressLine1"]),
                        AddressLine2 = rdr["AddressLine2"] == DBNull.Value ? null : Convert.ToString(rdr["AddressLine2"]),
                        City = rdr["City"] == DBNull.Value ? null : Convert.ToString(rdr["City"]),
                        State = rdr["State"] == DBNull.Value ? null : Convert.ToString(rdr["State"]),
                        Pincode = rdr["Pincode"] == DBNull.Value ? null : Convert.ToString(rdr["Pincode"]),
                        AccountHolderName = rdr["AccountHolderName"] == DBNull.Value ? null : Convert.ToString(rdr["AccountHolderName"]),
                        BankName = rdr["BankName"] == DBNull.Value ? null : Convert.ToString(rdr["BankName"]),
                        AccountNumber = rdr["AccountNumber"] == DBNull.Value ? null : Convert.ToString(rdr["AccountNumber"]),
                        ReEnterAccountNumber = rdr["ReEnterAccountNumber"] == DBNull.Value ? null : Convert.ToString(rdr["ReEnterAccountNumber"]),
                        IFSC = rdr["Ifsc"] == DBNull.Value ? null : Convert.ToString(rdr["Ifsc"]),
                        AccountType = rdr["AccountType"] == DBNull.Value ? null : Convert.ToString(rdr["AccountType"]),
                        EPF_Number = rdr["EPF_Number"] == DBNull.Value ? null : Convert.ToString(rdr["EPF_Number"]),
                        Deduction_Cycle = rdr["Deduction_Cycle"] == DBNull.Value ? null : Convert.ToString(rdr["Deduction_Cycle"]),
                        Employee_Contribution_Rate = rdr["Employee_Contribution_Rate"] == DBNull.Value ? null : Convert.ToString(rdr["Employee_Contribution_Rate"]),
                        nominee = rdr["nominee"] == DBNull.Value ? null : Convert.ToString(rdr["nominee"]),
                        
                        //netpayment = rdr["netpayment"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["netpayment"]),
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
    }

}