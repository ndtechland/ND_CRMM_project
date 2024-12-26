using CRM.Models.Crm;
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
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Text;
using CRM.IUtilities;
using Microsoft.AspNetCore.Hosting;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using CRM.Models.APIDTO;
using Dapper;
using CRM.Controllers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.TeamFoundation.Common;


namespace CRM.Repository
{
    public class Crmrpo : ICrmrpo
    {
        public IConfiguration Configuration { get; }
        private admin_NDCrMContext _context;
        private readonly IEmailService _IEmailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        //public virtual DbSet<EmployeeImportExcel> EmpMultiforms { get; set; } = null!;
        public Crmrpo(admin_NDCrMContext context, IConfiguration configuration, IEmailService IEmailService, IWebHostEnvironment hostingEnvironment, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            Configuration = configuration;
            _IEmailService = IEmailService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<int> LoginAsync(AdminLogin model)
        {
            using (SqlConnection con = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("usp_adminlogin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UserName", model.UserName));
                    cmd.Parameters.Add(new SqlParameter("@Password", model.Password));

                    await con.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return Convert.ToInt32(reader["ID"]);
                        }
                        else
                        {
                            return -1; // or handle accordingly
                        }
                    }
                }
            }
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
        public async Task<int> Customer(Customer model, int vendorid)
        {
            try
            {
                using (var connection = new SqlConnection(Configuration.GetConnectionString("db1"))) // Make sure to define _connectionString
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Company_Name", model.CompanyName);
                    parameters.Add("@Location", model.Location);
                    parameters.Add("@CityId", string.Join(",", model.CityId));
                    parameters.Add("@Mobile_number", model.MobileNumber);
                    parameters.Add("@Alternate_number", model.AlternateNumber);
                    parameters.Add("@Email", model.Email);
                    parameters.Add("@GST_Number", model.GstNumber);
                    parameters.Add("@Billing_Address", model.BillingAddress);
                    //parameters.Add("@Product_Details", model.ProductDetails);
                    //parameters.Add("@Start_date", model.StartDate);
                    //parameters.Add("@Renew_Date", model.RenewDate);
                    parameters.Add("@BillingStateId", model.BillingStateId);
                    parameters.Add("@stateId", model.StateId);
                    parameters.Add("@BillingCityId", model.BillingCityId);
                    parameters.Add("@Vendorid", vendorid);
                    //parameters.Add("@Renewprice", model.Renewprice);
                    //parameters.Add("@NoOfRenewMonth", model.NoOfRenewMonth);
                    //parameters.Add("@productprice", model.productprice);
                    //parameters.Add("@SCGST", model.Scgst);
                    //parameters.Add("@CGST", model.Cgst);
                    //parameters.Add("@IGST", model.Igst);
                    parameters.Add("@IsSameAddress", model.IsSameAddress == null ? false : model.IsSameAddress);
                    parameters.Add("@CustomerId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@FirstName", model.FirstName, DbType.String);
                    parameters.Add("@LastName", model.LastName, DbType.String);
                    await connection.ExecuteAsync("CustomerRegistration", parameters, commandType: CommandType.StoredProcedure);

                    int newCustomerId = parameters.Get<int>("@CustomerId");
                    return newCustomerId;


                }
            }
            catch (Exception)
            {
                throw; 
            }
        }

        public async Task<List<CustomerListDto>> CustomerList(int VendorId)
        {
            try
            {
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await con.OpenAsync();

                    var customers = await con.QueryAsync<CustomerListDto>(
                        "Customerlist",
                        new { id = VendorId },
                        commandType: CommandType.StoredProcedure
                    );

                    return customers.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching customer list", ex);
            }
        }
        public async Task<int> EmpRegistration(EmpMultiform model, string Mode, string Emp_Reg_ID, int userId)
        {
            try
            {
                ///
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == userId).FirstOrDefaultAsync();
                model.EmployeeId = Emp_Reg_ID;
                SqlConnection con = new SqlConnection(Configuration.GetConnectionString("db1"));
                SqlCommand cmd = new SqlCommand("EmployeeRegistrationtest", con);
                cmd.Parameters.AddWithValue("@mode", Mode);
                cmd.Parameters.AddWithValue("@Emp_RegID", Emp_Reg_ID);
                cmd.Parameters.AddWithValue("@Employee_ID", model.EmployeeId);
                cmd.Parameters.AddWithValue("@Customer_Id", adminlogin.Vendorid);
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
                cmd.Parameters.AddWithValue("@offerletterid", model.offerletterid);
                cmd.Parameters.AddWithValue("@officeshiftTypeid", model.officeshiftTypeid);

                //-- Salary Details
                cmd.Parameters.AddWithValue("@AnnualCTC", model.AnnualCTC);
                cmd.Parameters.AddWithValue("@Basic", model.Basic);
                cmd.Parameters.AddWithValue("@HouseRentAllowance", model.HouseRentAllowance);
                cmd.Parameters.AddWithValue("@TravellingAllowance", model.TravellingAllowance);
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
                cmd.Parameters.AddWithValue("@conveyanceallowance", model.Conveyanceallowance);
                cmd.Parameters.AddWithValue("@Medical", model.Medical);
                cmd.Parameters.AddWithValue("@VariablePay", model.VariablePay);
                cmd.Parameters.AddWithValue("@EmployerContribution", model.EmployerContribution);
                cmd.Parameters.AddWithValue("@tdsvalue", model.Tdsvalue);
                cmd.Parameters.AddWithValue("@Basicpercentage", model.Basicpercentage);
                cmd.Parameters.AddWithValue("@HRApercentage", model.Hrapercentage);
                cmd.Parameters.AddWithValue("@Conveyancepercentage", model.Conveyancepercentage);
                cmd.Parameters.AddWithValue("@Medicalpercentage", model.Medicalpercentage);
                cmd.Parameters.AddWithValue("@Variablepercentage", model.Variablepercentage);
                cmd.Parameters.AddWithValue("@EmployerContributionpercentage", model.EmployerContributionpercentage);
                cmd.Parameters.AddWithValue("@EPfpercentage", model.Epfpercentage);
                cmd.Parameters.AddWithValue("@Esipercentage", model.Esipercentage);

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


                if (Mode == "INS")
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
                    _IEmailService.SendEmailCred(model, password, adminlogin.Vendorid);
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

        //public async Task<List<EmployeeImportExcel>> EmployeeList()
        //{
        //    List<EmployeeImportExcel> employeeList = _context.EmployeeImportExcels.FromSqlRaw<EmployeeImportExcel>("USP_GetEmployeeDetails").ToListAsync().Result;

        //    return employeeList;
        //}
        public async Task<List<EmployeeImportExcel>> EmployeeList()
        {
            List<EmployeeImportExcel> employeeList = await _context.EmployeeImportExcels
                .FromSqlRaw("EXEC USP_GetEmployeeDetails")
                .ToListAsync();

            return employeeList;
        }

        //public ProductMaster GetproductById(int id)
        //{
        //    return _context.ProductMasters.Find(id);
        //}
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

        public async Task<int> Quation(QuationDto model)
        {

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@Action", 1));
            parameter.Add(new SqlParameter("@ID", model.Id));
            parameter.Add(new SqlParameter("@Company_Name", model.CompanyName));
            parameter.Add(new SqlParameter("@Customer_Name", model.CustomerName));
            parameter.Add(new SqlParameter("@Email", model.Email));
            parameter.Add(new SqlParameter("@Sales_Person_Name", model.SalesPersonName));
            parameter.Add(new SqlParameter("@Product_ID", string.Join(",", model.ProductId)));
            parameter.Add(new SqlParameter("@Subject", model.Subject));
            parameter.Add(new SqlParameter("@Amount", string.Join(",", model.Amount)));
            parameter.Add(new SqlParameter("@Mobile", model.Mobile));
            parameter.Add(new SqlParameter("@IsDeleted", '0'));


            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec SP_Quation @Action,@ID,@Company_Name,@Customer_Name,@Email,@Sales_Person_Name,@Product_ID,@Subject,@Amount,@Mobile,@IsDeleted", parameter.ToArray()));

            return result;
        }


        public async Task<List<QuationDto>> QuationList()
        {
            List<QuationDto> cs = new List<QuationDto>();
            try
            {
                using (SqlConnection con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("QuationList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var cse = new QuationDto()
                            {
                                Id = Convert.ToInt32(rdr["ID"]),
                                CompanyName = rdr["Company_Name"] == DBNull.Value ? null : Convert.ToString(rdr["Company_Name"]),
                                CustomerName = rdr["Customer_Name"] == DBNull.Value ? null : Convert.ToString(rdr["Customer_Name"]),
                                Email = rdr["Email"] == DBNull.Value ? null : Convert.ToString(rdr["Email"]),
                                SalesPersonName = rdr["Sales_Person_Name"] == DBNull.Value ? null : Convert.ToString(rdr["Sales_Person_Name"]),
                                ProductId = rdr["Product_ID"] == DBNull.Value ? Array.Empty<string>() : Convert.ToString(rdr["Product_ID"]).Split(','),
                                Subject = rdr["Subject"] == DBNull.Value ? null : Convert.ToString(rdr["Subject"]),
                                Amount = rdr["Amount"] == DBNull.Value ? Array.Empty<string>() : Convert.ToString(rdr["Amount"]).Split(',').Select(a => a.Trim()).ToArray(),
                                Mobile = rdr["Mobile"] == DBNull.Value ? null : Convert.ToString(rdr["Mobile"]),
                                IsDeleted = rdr["IsDeleted"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(rdr["IsDeleted"])
                            };
                            cs.Add(cse);
                        }
                    }
                }
                return cs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Iupdate(QuationDto model)
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
                parameter.Add(new SqlParameter("@Product_ID ", string.Join(",", model.ProductId)));
                parameter.Add(new SqlParameter("@Subject", model.Subject));
                parameter.Add(new SqlParameter("@Amount", string.Join(",", model.Amount)));
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

        public async Task<List<salarydetail>> salarydetail(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<salarydetail>(
                        "sp_SalaryDetail",
                        new { id = userId },
                        commandType: CommandType.StoredProcedure);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<GenerateSalary>> GenerateSalary(int userId)
        {
            try
            {
                var adminLogin = await _context.AdminLogins
                    .Where(x => x.Id == userId)
                    .FirstOrDefaultAsync();

                if (adminLogin == null)
                    throw new Exception("Admin login not found.");

                var vendorId = adminLogin.Vendorid;

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    var parameters = new { id = vendorId };
                    var query = "GetGenerateSalary";

                    var result = await connection.QueryAsync<GenerateSalary>(
                        query,
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while generating the salary.", ex);
            }
        }
        public async Task<int> Employer(EmployeerModelEPF model, int AdminLoginId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@Id", model.Id),
        new SqlParameter("@Deduction_Cycle", model.Deduction_Cycle ?? (object)DBNull.Value),
        new SqlParameter("@AdminLoginId", AdminLoginId)
    };

            if (model.Deduction_Cycle == "ESIC")
            {
                parameters.Add(new SqlParameter("@EPF_Number", model.EsicEPF_Number ?? (object)DBNull.Value));
                parameters.Add(new SqlParameter("@Employer_Contribution_Rate", model.EsicEmployer_Contribution_Rate ?? (object)DBNull.Value));
            }
            else if (model.Deduction_Cycle == "EPF")
            {
                parameters.Add(new SqlParameter("@EPF_Number", model.EPF_Number ?? (object)DBNull.Value));
                parameters.Add(new SqlParameter("@Employer_Contribution_Rate", model.Employer_Contribution_Rate ?? (object)DBNull.Value));
            }
            else
            {
                throw new InvalidOperationException("Invalid Deduction Cycle specified.");
            }

            var result = await _context.Database.ExecuteSqlRawAsync(
                "exec USP_Employeer_EPF @Id, @EPF_Number, @Deduction_Cycle, @Employer_Contribution_Rate, @AdminLoginId",
                parameters.ToArray()
            );

            return result;
        }

        public async Task<List<EmployeerEpf>> EmployerList(int AdminLoginId)
        {
            var result = await _context.EmployeerEpfs
                .FromSqlInterpolated($"EXEC EmployerList {AdminLoginId}")
                .ToListAsync();

            return result;
        }
        public async Task<Invoice> GenerateInvoice(int ID)
        {
            try
            {
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await con.OpenAsync();
                    var invoice = await con.QuerySingleOrDefaultAsync<Invoice>(
                        "GenerateInvoice",
                        new { ID },
                        commandType: CommandType.StoredProcedure
                    );
                    return invoice;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable GetEmployDetailById(string EmpId, int Userid)
        {

            SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
            SqlCommand cmd = new SqlCommand("USP_GetEmployeeDetailById", con);
            cmd.Parameters.AddWithValue("@id", Userid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;

        }
        public byte[] EmployeeListForExcel(List<EmployeeImportExcel> data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("EmployeeList");
                var currentwork = 1;

                // Adding headers with background color
                var headers = new[]
                {
            "Sr.No.", "Employee Name", "Employee ID", "Date Of Joining", "Work Email", "Gender",
            "Work Location", "Designation", "Department", "Company Name", "Personal Email Address",
            "Mobile Number", "Date Of Birth", "Age", "Father Name", "PAN", "Bank Name", "Account Number",
            "IFSC", "EPF Number", "Employee Contribution Rate", "Deduction Cycle", "Account Type",
            "Annual CTC", "EPF", "Monthly Gross Pay", "Shifttype", "ShiftTime"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(currentwork, i + 1).Value = headers[i];
                    worksheet.Cell(currentwork, i + 1).Style.Fill.BackgroundColor = XLColor.Yellow;
                }

                currentwork++;
                int index = 1;

                // Adding data rows
                foreach (var item in data)
                {
                    worksheet.Cell(currentwork, 1).Value = index++;
                    worksheet.Cell(currentwork, 2).Value = $"{item.FirstName} {item.MiddleName} {item.LastName}"; worksheet.Cell(currentwork, 3).Value = item.EmployeeId.ToString() ?? "N/A";
                    worksheet.Cell(currentwork, 4).Value = item.DateOfJoining != DateTime.MinValue
      ? item.DateOfJoining.ToString("yyyy-MM-dd")
      : "N/A";


                    worksheet.Cell(currentwork, 5).Value = !string.IsNullOrEmpty(item.WorkEmail)
    ? item.WorkEmail
    : "N/A";
                    worksheet.Cell(currentwork, 6).Value = !string.IsNullOrEmpty(item.Gender)
     ? item.Gender
     : "N/A";
                    worksheet.Cell(currentwork, 7).Value = !string.IsNullOrEmpty(item.WorkLocation)
       ? item.WorkLocation
       : "N/A";
                    worksheet.Cell(currentwork, 8).Value = item.DesignationName ?? "N/A";
                    worksheet.Cell(currentwork, 9).Value = item.DepartmentName ?? "N/A";
                    worksheet.Cell(currentwork, 10).Value = item.CustomerName ?? "N/A";
                    worksheet.Cell(currentwork, 11).Value = item.PersonalEmailAddress ?? "N/A";
                    worksheet.Cell(currentwork, 12).Value = item.Mobile?.ToString() ?? "N/A";
                    worksheet.Cell(currentwork, 13).Value = item.DateOfBirth.ToString("yyyy-MM-dd") ?? "N/A";
                    worksheet.Cell(currentwork, 14).Value = item.Age > 0 ? item.Age.ToString() : "N/A";
                    worksheet.Cell(currentwork, 15).Value = item.FatherName ?? "N/A";
                    worksheet.Cell(currentwork, 16).Value = item.Pan ?? "N/A";
                    worksheet.Cell(currentwork, 17).Value = item.BankName ?? "N/A";
                    worksheet.Cell(currentwork, 18).Value = item.AccountNumber?.ToString() ?? "N/A";
                    worksheet.Cell(currentwork, 19).Value = item.Ifsc ?? "N/A";
                    worksheet.Cell(currentwork, 20).Value = item.EpfNumber?.ToString() ?? "N/A";
                    worksheet.Cell(currentwork, 21).Value = item.EmployeeContributionRate?.ToString() ?? "N/A";
                    worksheet.Cell(currentwork, 22).Value = item.DeductionCycle ?? "N/A";
                    worksheet.Cell(currentwork, 23).Value = item.AccountType ?? "N/A";
                    worksheet.Cell(currentwork, 24).Value = item.AnnualCtc.ToString() ?? "N/A";
                    worksheet.Cell(currentwork, 25).Value = item.Epf.ToString() ?? "N/A";
                    worksheet.Cell(currentwork, 26).Value = item.MonthlyGrossPay.ToString() ?? "N/A";
                    worksheet.Cell(currentwork, 27).Value = item.Shifttype ?? "N/A";
                    worksheet.Cell(currentwork, 28).Value = item.OfficeshiftTypeid?.ToString() ?? "N/A";

                    currentwork++;
                }

                // Save to memory stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
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

        public async Task<int> updateDesignation(DesignationMaster model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@id", model.Id));
            parameter.Add(new SqlParameter("@DesignationName", model.DesignationName));

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec sp_updateDesignation @id,@DesignationName", parameter.ToArray()));

            return result;
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
            try
            {
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    string storedProcedure = "sp_GetCustomerById";
                    var parameters = new { id };

                    // Synchronous call to retrieve the customer
                    var customer = con.QueryFirstOrDefault<Customer>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

                    return customer;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching customer details", ex);
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
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", model.Id, DbType.Int32);
                parameters.Add("@Company_Name", model.CompanyName, DbType.String);
                parameters.Add("@Location", model.Location, DbType.String);
                parameters.Add("@CityId", model.CityId, DbType.String);
                parameters.Add("@Mobile_number", model.MobileNumber, DbType.String);
                parameters.Add("@Alternate_number", model.AlternateNumber, DbType.String);
                parameters.Add("@Email", model.Email, DbType.String);
                parameters.Add("@GST_Number", model.GstNumber, DbType.String);
                parameters.Add("@Billing_Address", model.BillingAddress, DbType.String);
                //parameters.Add("@Product_Details", model.ProductDetails, DbType.String);
                //parameters.Add("@Start_date", model.StartDate, DbType.DateTime);
                //parameters.Add("@Renew_Date", model.RenewDate, DbType.DateTime);
                parameters.Add("@BillingStateId", model.BillingStateId, DbType.Int32);
                parameters.Add("@BillingCityId", model.BillingCityId, DbType.Int32);
                parameters.Add("@stateId", model.StateId, DbType.Int32);
                //parameters.Add("@Renewprice", model.Renewprice, DbType.Decimal);
                //parameters.Add("@NoOfRenewMonth", model.NoOfRenewMonth, DbType.Int32);
                //parameters.Add("@productprice", model.productprice, DbType.Decimal);
                //parameters.Add("@SCGST", model.Scgst, DbType.Decimal);
                //parameters.Add("@CGST", model.Cgst, DbType.Decimal);
                //parameters.Add("@IGST", model.Igst, DbType.Decimal);
                parameters.Add("@IsSameAddress", model.IsSameAddress == null ? false : model.IsSameAddress, DbType.Boolean);
                parameters.Add("@FirstName", model.FirstName, DbType.String);
                parameters.Add("@LastName", model.LastName, DbType.String);

                var result = await connection.ExecuteAsync(
                    "sp_updateCustomer_Reg",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
        }
        public async Task<List<ECS>> ESCExcel(int? Userid)
        {
            try
            {
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    string storedProcedure = "sp_ECSSalary";
                    var parameters = new { id = Userid };
                    var empList = await con.QueryAsync<ECS>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

                    return empList.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex; // You can log the exception or handle it more gracefully
            }
        }

        //public async Task<List<ECS>> ESCExcel(int? Userid)
        //{
        //    List<ECS> emp = new List<ECS>();
        //    try
        //    {
        //        SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
        //        SqlCommand cmd = new SqlCommand("sp_ECSSalary", con);
        //        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Userid });
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        con.Open();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            var emps = new ECS()
        //            {
        //                FirstName = rdr["FirstName"] == DBNull.Value ? null : Convert.ToString(rdr["FirstName"]),
        //                EmployeeId = rdr["EmployeeId"] == DBNull.Value ? null : Convert.ToString(rdr["EmployeeId"]),
        //                AccountNumber = (int)(rdr["AccountNumber"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["AccountNumber"])),
        //                Ifsc = rdr["Ifsc"] == DBNull.Value ? null : Convert.ToString(rdr["Ifsc"]),
        //                netpayment = rdr["netpayment"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["netpayment"]),
        //            };

        //            emp.Add(emps);

        //        }
        //        return emp;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

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
                worksheet.Cell(currentwork, 5).Value = "Gross Pay";
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
                    worksheet.Cell(currentwork, 5).Value = item.Grosspay;
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
        private string GenerateDynamicUsername(string companyName)
        {
            Random rnd = new Random();
            //string letters = new string(Enumerable.Range(0, 4)
            //    .Select(_ => (char)rnd.Next('a', 'z' + 1))
            //    .ToArray());
            //string numbers = rnd.Next(10, 100).ToString() + rnd.Next(1000, 10000).ToString();
            //return $"{letters}_{numbers}";

            string letters = new string(Enumerable.Range(0, 4)
            .Select(_ => (char)rnd.Next('a', 'z' + 1))
            .ToArray());
            string numbers = $"{rnd.Next(10, 100)}{rnd.Next(1000, 10000)}";
            return $"{letters}_{numbers}";
        }

        private string GenerateDynamicPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            StringBuilder res = new();
            Random rnd = new();
            for (int i = 0; i < 12; i++)
            {
                res.Append(validChars[rnd.Next(validChars.Length)]);
            }
            return res.ToString();
        }

        public async Task<CustomerRegistration> GetCustomerProfile(string? id)
        {
            try
            {
                var query = await (from admin in _context.AdminLogins
                                   join customer in _context.CustomerRegistrations
                                   on admin.Vendorid equals customer.Id
                                   where admin.Id == Convert.ToInt32(id)
                                   select new CustomerRegistration
                                   {
                                       CompanyName = customer.CompanyName,
                                       CityId = customer.CityId,
                                       MobileNumber = customer.MobileNumber,
                                       Email = customer.Email,
                                       GstNumber = customer.GstNumber,
                                       AlternateNumber = customer.AlternateNumber,
                                       BillingAddress = customer.BillingAddress,
                                       //UserName = admin.UserName
                                   }).FirstOrDefaultAsync();
                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> UpdateCustomerProfile(CustomerRegistration model, int? id)
        {
            var customer = await _context.CustomerRegistrations.FirstOrDefaultAsync(x => x.Id == model.Id);
            var adminusername = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == id);
            if (customer == null)
            {
                return 0;
            }
            customer.CompanyName = model.CompanyName;
            customer.Email = model.Email;
            customer.GstNumber = model.GstNumber;
            customer.MobileNumber = model.MobileNumber;
            customer.AlternateNumber = model.AlternateNumber;
            customer.BillingAddress = model.BillingAddress;
            _context.CustomerRegistrations.Update(customer);
            await _context.SaveChangesAsync();
            if (adminusername != null)
            {
                //adminusername.UserName = model.UserName;
                _context.AdminLogins.Update(adminusername);
                await _context.SaveChangesAsync();

            }
            return 1;
        }
        public async Task<int> UpdateChangepassword(ChangePassworddto model, string AddedBy, int id)
        {
            var adminusername = await _context.AdminLogins.FirstOrDefaultAsync(x => x.UserName == AddedBy && x.Id == id);
            if (adminusername == null)
            {
                return 0;
            }

            if (adminusername != null)
            {
                adminusername.Password = model.NewPassword;
                _context.AdminLogins.Update(adminusername);
                await _context.SaveChangesAsync();

            }
            return 1;
        }
        public async Task<List<EmployeeImportExcel>> CustomerEmployeeList(int id)
        {
            List<EmployeeImportExcel> employeeList = await _context.EmployeeImportExcels
                .FromSqlRaw("USP_GetCustomerEmployeeDetails @id", new SqlParameter("@id", id))
                .ToListAsync();

            return employeeList;
        }
        public VendorDto GetVendorById(int id)
        {
            try
            {
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    const string query = "sp_GetVendorById";
                    var vendor = con.Query<VendorDto>(
                        query,
                        new { id },
                        commandType: CommandType.StoredProcedure
                    ).FirstOrDefault();

                    return vendor;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<VendorRegResultDTO> Vendorreg(VendorDto model, string InvoiceNo)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("db1"))) // Use your actual connection string here
            {
                // Generate dynamic username and password
                string dynamicUserName = GenerateDynamicUsername(model.CompanyName);
                string dynamicPassword = GenerateDynamicPassword();

                // Set up parameters for the stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@Company_Name", model.CompanyName);
                parameters.Add("@CityId", string.Join(",", model.CityId));
                parameters.Add("@Mobile_number", model.MobileNumber);
                parameters.Add("@Alternate_number", model.AlternateNumber);
                parameters.Add("@Email", model.Email);
                parameters.Add("@GST_Number", model.GstNumber);
                parameters.Add("@Billing_Address", model.BillingAddress);
                parameters.Add("@Product_Details", model.ProductDetails);
                parameters.Add("@Start_date", model.StartDate);
                parameters.Add("@Renew_Date", model.RenewDate);
                parameters.Add("@BillingStateId", model.BillingStateId);
                parameters.Add("@BillingCityId", model.BillingCityId);
                parameters.Add("@stateId", model.StateId);
                parameters.Add("@Location", model.Location);
                parameters.Add("@productprice", model.Price);
                parameters.Add("@Renewprice", model.Renewprice);
                parameters.Add("@NoOfRenewMonth", model.NoOfRenewMonth);
                parameters.Add("@IsSameAddress", model.IsSameAddress);
                parameters.Add("@Cgst", model.Cgst);
                parameters.Add("@Scgst", model.Scgst);
                parameters.Add("@Igst", model.Igst);
                parameters.Add("@PricingPlanid", model.PricingPlanid);
                parameters.Add("@InvoiceNumber", InvoiceNo);
                parameters.Add("@duedate", model.Duedate);
                parameters.Add("@CustomerId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("VendorRegistration", parameters, commandType: CommandType.StoredProcedure);
                int newCustomerId = parameters.Get<int>("@CustomerId");
                if (newCustomerId > 0)
                {
                    var adminRole = new AdminLogin
                    {
                        UserName = dynamicUserName,
                        Password = dynamicPassword,
                        Role = "Vendor",
                        Emailid = model.Email,
                        Vendorid = newCustomerId
                    };

                    string insertAdminLoginQuery = @"
                INSERT INTO AdminLogin (UserName, Password, Role, Emailid, Vendorid)
                VALUES (@UserName, @Password, @Role, @Emailid, @Vendorid)";

                    await connection.ExecuteAsync(insertAdminLoginQuery, adminRole);
                }

                // Return the result
                return new VendorRegResultDTO
                {
                    NewCustomerId = newCustomerId,
                    UserName = dynamicUserName,
                    Password = dynamicPassword
                };
            }
        }
        public async Task<int> updateVendorreg(VendorDto model)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("db1"))) // Use your actual connection string here
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", model.Id);
                parameters.Add("@Company_Name", model.CompanyName);
                parameters.Add("@CityId", string.Join(",", model.CityId));
                parameters.Add("@Mobile_number", model.MobileNumber);
                parameters.Add("@Alternate_number", model.AlternateNumber);
                parameters.Add("@Email", model.Email);
                parameters.Add("@GST_Number", model.GstNumber);
                parameters.Add("@Billing_Address", model.BillingAddress);
                parameters.Add("@Product_Details", model.ProductDetails);
                parameters.Add("@Start_date", model.StartDate);
                parameters.Add("@Renew_Date", model.RenewDate);
                parameters.Add("@BillingStateId", model.BillingStateId);
                parameters.Add("@BillingCityId", model.BillingCityId);
                parameters.Add("@stateId", model.StateId);
                parameters.Add("@Location", model.Location);
                parameters.Add("@productprice", model.Price);
                parameters.Add("@Renewprice", model.Renewprice);
                parameters.Add("@NoOfRenewMonth", model.NoOfRenewMonth);
                parameters.Add("@IsSameAddress", model.IsSameAddress);
                parameters.Add("@Cgst", model.Cgst);
                parameters.Add("@Scgst", model.Scgst);
                parameters.Add("@Igst", model.Igst);
                parameters.Add("@PricingPlanid", model.PricingPlanid);
                parameters.Add("@duedate", model.Duedate);
                var result = await connection.ExecuteAsync(
                    "sp_updateVendor_Reg",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
        }

        public async Task<List<VendorDto>> VendorList()
        {
            try
            {
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await con.OpenAsync();
                    var vendorList = (await con.QueryAsync<VendorDto>("Vendorlist",
                        commandType: CommandType.StoredProcedure)).ToList();

                    return vendorList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<List<VendorDto>> VendorList()
        //{
        //    List<VendorDto> cs = new List<VendorDto>();
        //    try
        //    {
        //        SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
        //        SqlCommand cmd = new SqlCommand("Vendorlist", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        con.Open();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            var cse = new VendorDto()
        //            {
        //                Id = Convert.ToInt32(rdr["id"]),
        //                CompanyName = rdr["Company_Name"] == DBNull.Value ? null : Convert.ToString(rdr["Company_Name"]),
        //                //WorkLocation = rdr["Work_Location"] == DBNull.Value ? null : new string[] { Convert.ToString(rdr["Work_Location"]) },
        //                OfficeCity = rdr["OfficeCity"] == DBNull.Value ? null : Convert.ToString(rdr["OfficeCity"]),
        //                MobileNumber = rdr["Mobile_number"] == DBNull.Value ? "0" : Convert.ToString(rdr["Mobile_number"]),
        //                AlternateNumber = rdr["Alternate_number"] == DBNull.Value ? "0" : Convert.ToString(rdr["Alternate_number"]),
        //                Email = rdr["Email"] == DBNull.Value ? "0" : Convert.ToString(rdr["Email"]),
        //                GstNumber = rdr["GST_Number"] == DBNull.Value ? "0" : Convert.ToString(rdr["GST_Number"]),
        //                BillingAddress = rdr["Billing_Address"] == DBNull.Value ? "0" : Convert.ToString(rdr["Billing_Address"]),
        //                ProductDetails = rdr["Product_Details"] == DBNull.Value ? "0" : Convert.ToString(rdr["Product_Details"]),
        //                StartDate = rdr["Start_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rdr["Start_date"]),
        //                RenewDate = rdr["Renew_Date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rdr["Renew_Date"]),
        //                OfficeState = rdr["OfficeState"] == DBNull.Value ? null : Convert.ToString(rdr["OfficeState"]),
        //                BillingState = rdr["BillingState"] == DBNull.Value ? null : Convert.ToString(rdr["BillingState"]),
        //                Location = rdr["Billing_Address"] == DBNull.Value ? null : Convert.ToString(rdr["Location"]),
        //                CompanyImage = rdr["Company_Image"] == DBNull.Value ? null : Convert.ToString(rdr["Company_Image"]),
        //                Isactive = rdr["Isactive"] == DBNull.Value ? null : Convert.ToBoolean(rdr["Isactive"]),

        //            };

        //            cs.Add(cse);
        //        }
        //        return cs;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        cs = null;
        //    }
        //}
        public async Task<VendorRegistrationDto> GetVendorProfile(string? id)
        {
            try
            {
                var query = await (from a in _context.AdminLogins
                                   join v in _context.VendorRegistrations
                                   on a.Vendorid equals v.Id
                                   where a.Id == Convert.ToInt32(id)
                                   select new VendorRegistrationDto
                                   {
                                       Id = (int)a.Vendorid,
                                       CompanyName = v.CompanyName,
                                       CityId = v.CityId,
                                       MobileNumber = v.MobileNumber,
                                       Email = v.Email,
                                       GstNumber = v.GstNumber,
                                       AlternateNumber = v.AlternateNumber,
                                       BillingAddress = v.BillingAddress,
                                       Location = v.Location,
                                       UserName = a.UserName,
                                       CompanyImage = v.CompanyImage,
                                       maplat = v.Maplat,
                                       maplong = v.Maplong,
                                       radious = v.Radious,
                                       BillingCityId = v.BillingCityId,
                                       BillingStateId = v.BillingStateId,
                                       OfficeCityId = v.CityId,
                                       OfficeStateId = v.StateId,
                                       Isprofessionaltax = v.Isprofessionaltax,
                                       VendorSingature  =v.VendorSingature
                                   }).FirstOrDefaultAsync();
                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> UpdateVendorProfile(VendorRegistrationDto model, int id)
        {
            FileOperation fileOperation = new FileOperation(_webHostEnvironment);
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

            var adminusername = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == id);
            var customer = await _context.VendorRegistrations.FirstOrDefaultAsync(x => x.Id == adminusername.Vendorid);

            if (customer == null)
            {
                return 0;
            }
            if (model.ImageFile != null)
            {
                var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Only .jpg, .jpeg, and .png files are allowed.");
                }
                string ImagePath = fileOperation.SaveBase64Image("CompanyImage", model.ImageFile, allowedExtensions);
                customer.CompanyImage = ImagePath;
            }
            if (model.VendorSingatureImageFile != null)
            {
                var fileExtension = Path.GetExtension(model.VendorSingatureImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Only .jpg, .jpeg, and .png files are allowed.");
                }
                string ImagePath = fileOperation.SaveBase64Image("CompanyImage", model.VendorSingatureImageFile, allowedExtensions);
                customer.VendorSingature = ImagePath;
            }
            customer.CompanyName = model.CompanyName;
            customer.Email = model.Email;
            customer.GstNumber = model.GstNumber;
            customer.MobileNumber = model.MobileNumber;
            customer.AlternateNumber = model.AlternateNumber;
            customer.BillingAddress = model.BillingAddress;
            customer.Location = model.Location;

            customer.Radious = model.radious;
            customer.Maplat = model.maplat;
            customer.Maplong = model.maplong;
            customer.BillingStateId = model.BillingStateId;
            customer.BillingCityId = model.BillingCityId;
            customer.StateId = model.OfficeStateId;
            customer.CityId = model.OfficeCityId;
            customer.Isprofessionaltax = model.Isprofessionaltax;
            _context.VendorRegistrations.Update(customer);
            await _context.SaveChangesAsync();

            if (adminusername != null)
            {
                adminusername.UserName = model.UserName;
                _context.AdminLogins.Update(adminusername);
                await _context.SaveChangesAsync();
            }

            return 1;
        }

        public async Task<List<EmployeeApprovedPresnolInfo>> ApprovedPresnolInfoList(int Userid)
        {
            try
            {
                List<EmployeeApprovedPresnolInfo> PresnolInfo = new List<EmployeeApprovedPresnolInfo>();
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                PresnolInfo = _context.ApprovedPresnolInfos.Where(x => x.IsApproved == false && x.Vendorid == adminlogin.Vendorid).Select(x => new EmployeeApprovedPresnolInfo
                {
                    id = x.Id,
                    FullName = x.FullName,
                    PersonalEmailAddress = x.PersonalEmailAddress,
                    MobileNumber = x.MobileNumber ?? 0,
                    DateOfBirth = x.DateOfBirth,
                    PAN = x.Pan,
                    AddressLine1 = x.AddressLine1,
                    AddressLine2 = x.AddressLine2,
                    Stateid = _context.States.Where(g => g.Id == Convert.ToInt32(x.StateId)).Select(g => g.SName).First(),
                    cityid = _context.Cities.Where(g => g.Id == Convert.ToInt32(x.City)).Select(g => g.City1).First(),
                    Pincode = x.Pincode,
                    AadharNo = x.AadharNo,
                    AadharOne = x.AadharOne,
                    AadharTwo = x.AadharTwo,
                    Panimg = x.Panimg,
                    EmployeeId = x.EmployeeId,
                    IsApproved = x.IsApproved,
                    UpdateDate = x.UpdateDate,
                    FatherName = x.FatherName,
                }).ToList();
                return PresnolInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> AddApprovedPresnolInfo(EmployeePresnolInfoList model)
        {
            try
            {
                if (model.id > 0)
                {
                    var existingEntity = await _context.ApprovedPresnolInfos.FindAsync(model.id);
                    if (existingEntity != null)
                    {
                        existingEntity.FullName = model.FullName;
                        existingEntity.PersonalEmailAddress = model.PersonalEmailAddress;
                        existingEntity.MobileNumber = model.MobileNumber;
                        existingEntity.DateOfBirth = model.DateOfBirth;
                        existingEntity.Pan = model.PAN;
                        existingEntity.AddressLine1 = model.AddressLine1;
                        existingEntity.AddressLine2 = model.AddressLine2;
                        existingEntity.StateId = Convert.ToString(model.Stateid);
                        existingEntity.City = Convert.ToString(model.cityid);
                        existingEntity.Pincode = model.Pincode;
                        existingEntity.AadharNo = model.AadharNo;
                        existingEntity.FatherName = model.FatherName;
                        if (model.AadharOne != null)
                        {
                            existingEntity.AadharOne = model.AadharOne;
                        }
                        if (model.Panimg != null)
                        {
                            existingEntity.Panimg = model.Panimg;
                        }

                        if (model.AadharTwo! != null)
                        {
                            existingEntity.AadharTwo = model.AadharTwo;
                        }
                        existingEntity.UpdateDate = DateTime.Now;
                        existingEntity.IsApproved = false;
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the contact: " + ex.Message, ex);
            }
        }
        public async Task<List<ApprovedbankdetailList>> ApprovedbankdetailList(int Userid)
        {
            try
            {
                List<ApprovedbankdetailList> PresnolInfo = new List<ApprovedbankdetailList>();
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                PresnolInfo = _context.Approvedbankdetails.Where(x => x.IsApproved == false && x.Vendorid == adminlogin.Vendorid).Select(x => new ApprovedbankdetailList
                {
                    id = x.Id,
                    AccountHolderName = x.AccountHolderName,
                    BankName = x.BankName,
                    AccountNumber = x.AccountNumber,
                    ReEnterAccountNumber = x.ReEnterAccountNumber,
                    Ifsc = x.Ifsc,
                    AccountTypeId = x.AccountTypeId == 1 ? "Current" : x.AccountTypeId == 2 ? "Savings" : "unknown",
                    EpfNumber = x.EpfNumber,
                    Nominee = x.Nominee,
                    Chequeimage = x.Chequeimage,
                    EmployeeId = x.EmployeeId,
                    IsApproved = x.IsApproved,
                    UpdateDate = x.UpdateDate,
                }).ToList();
                return PresnolInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> AddApprovedbankdetail(ApprovedbankdetailList model)
        {
            try
            {
                if (model.id > 0)
                {
                    var existingEntity = await _context.Approvedbankdetails.FindAsync(model.id);
                    if (existingEntity != null)
                    {
                        existingEntity.AccountHolderName = model.AccountHolderName;
                        existingEntity.BankName = model.BankName;
                        existingEntity.AccountNumber = model.AccountNumber;
                        existingEntity.ReEnterAccountNumber = model.ReEnterAccountNumber;
                        existingEntity.Ifsc = model.Ifsc;
                        existingEntity.AccountTypeId = Convert.ToInt32(model.AccountTypeId);
                        existingEntity.EpfNumber = model.EpfNumber;
                        existingEntity.Nominee = model.Nominee;
                        if (model.Chequeimage != null)
                        {
                            existingEntity.Chequeimage = model.Chequeimage;
                        }
                        existingEntity.IsApproved = false;
                        existingEntity.UpdateDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the contact: " + ex.Message, ex);
            }
        }
        public async Task<Offerletter> GetOfferletterbyid(int? id)
        {
            try
            {
                var query = await _context.Offerletters.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddOfferletterdetail(Offerletters model, int Userid)
        {
            try
            {
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png" };
                string ImagePath = "";
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only  .png files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("CompanyImage", model.ImageFile, allowedExtensions);
                }
                Offerletter of = new Offerletter()
                {
                    Name = model.Name,
                    Currentdate = DateTime.Now,
                    Validdate = model.Validdate,
                    MonthlyCtc = model.MonthlyCtc,
                    AnnualCtc = model.AnnualCtc,
                    DesignationId = model.DesignationId,
                    DepartmentId = model.DepartmentId,
                    DateOfJoining = model.DateOfJoining,
                    Vendorid = adminlogin.Vendorid,
                    IsDeleted = false,
                    StateId = model.StateId,
                    CityId = model.CityId,
                    CandidateAddress = model.CandidateAddress,
                    CandidatePincode = model.CandidatePincode,
                    HrJobTitle = model.HrJobTitle,
                    HrName = model.HrName,
                    CandidateEmail = model.CandidateEmail,
                    HrSignature = ImagePath
                };
                _context.Offerletters.Add(of);
                _context.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> updateOfferletterdetail(Offerletters model)
        {
            try
            {
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png" };
                var existingOfferletter = await _context.Offerletters.FindAsync(model.Id);
                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .jpg, .jpeg, and .png files are allowed.");
                    }
                    string ImagePath = fileOperation.SaveBase64Image("CompanyImage", model.ImageFile, allowedExtensions);
                    existingOfferletter.HrSignature = ImagePath;
                }
                if (existingOfferletter != null)
                {
                    existingOfferletter.Name = model.Name;
                    existingOfferletter.Validdate = model.Validdate;
                    existingOfferletter.MonthlyCtc = model.MonthlyCtc;
                    existingOfferletter.AnnualCtc = model.AnnualCtc;
                    existingOfferletter.DesignationId = model.DesignationId;
                    existingOfferletter.DepartmentId = model.DepartmentId;
                    existingOfferletter.DateOfJoining = model.DateOfJoining;
                    existingOfferletter.StateId = model.StateId;
                    existingOfferletter.CityId = model.CityId;
                    existingOfferletter.CandidateAddress = model.CandidateAddress;
                    existingOfferletter.CandidatePincode = model.CandidatePincode;
                    existingOfferletter.HrJobTitle = model.HrJobTitle;
                    existingOfferletter.HrName = model.HrName;
                    existingOfferletter.CandidateEmail = model.CandidateEmail;
                    return await _context.SaveChangesAsync();
                }

                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<empOfferletter>> OfferletterdetailList(int Userid)
        {
            try
            {
                if (Userid != null)
                {
                    var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                    var offerletters = await _context.Offerletters
                        .Where(x => x.Vendorid == adminlogin.Vendorid && x.IsDeleted == false)
                        .Select(x => new empOfferletter
                        {
                            Id = x.Id,
                            Name = x.Name,
                            MonthlyCtc = x.MonthlyCtc,
                            AnnualCtc = x.AnnualCtc,
                            CandidateAddress = x.CandidateAddress,
                            CandidatePincode = x.CandidatePincode,
                            HrSignature = x.HrSignature,
                            HrJobTitle = x.HrJobTitle,
                            HrName = x.HrName,
                            OfferletterFile = x.OfferletterFile,
                            CandidateEmail = x.CandidateEmail,
                            StateName = _context.States.Where(g => g.Id == x.StateId).Select(g => g.SName).FirstOrDefault(),
                            CityName = _context.Cities.Where(g => g.Id == x.CityId).Select(g => g.City1).FirstOrDefault(),
                            DateOfJoining = x.DateOfJoining.GetValueOrDefault(),
                            DepartmentName = _context.DepartmentMasters.Where(g => g.Id == Convert.ToInt16(x.DepartmentId)).Select(g => g.DepartmentName).FirstOrDefault().Trim(),
                            DesignationName = _context.DesignationMasters.Where(g => g.Id == Convert.ToInt16(x.DesignationId)).Select(g => g.DesignationName).FirstOrDefault().Trim(),
                            Validdate = x.Validdate.GetValueOrDefault(),
                        }).ToListAsync();

                    return offerletters;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<LeavemasterDto>> getLeavemaster(int Userid, int? VendorId)
        {
            try
            {
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var result = await (from lm in _context.Leavemasters
                                    join lt in _context.LeaveTypes
                                    on lm.LeavetypeId equals lt.Id
                                    join emp in _context.EmployeeRegistrations
                                   on lm.EmpId equals emp.EmployeeId
                                    where emp.Vendorid == VendorId && lm.Vendorid == VendorId && lt.Vendorid == VendorId
                                    select new LeavemasterDto
                                    {
                                        id = lm.Id,
                                        LeavetypeId = lt.Leavetype1,
                                        Value = lm.Value,
                                        EmpId = lm.EmpId,
                                        Createddate = lm.Createddate,
                                        IsActive = lm.IsActive,
                                        LeaveStartDate = lm.LeaveStartDate,
                                        EmpName = $"{emp.FirstName} {' '} {emp.LastName}",
                                    }).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //Vendor Product
        public async Task<int> AddVendorProduct(VendorProductMaster model, int VendorId)
        {
            if (model.Id == 0)
            {
                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@VendorId", VendorId));
                parameter.Add(new SqlParameter("@ProductName", model.ProductName));
                parameter.Add(new SqlParameter("@CategoryId", model.CategoryId));
                parameter.Add(new SqlParameter("@Gst", model.Gst));
                parameter.Add(new SqlParameter("@ProductPrice", model.ProductPrice));
                parameter.Add(new SqlParameter("@Hsncode", model.Hsncode));
                parameter.Add(new SqlParameter("@IsActive", true));
                parameter.Add(new SqlParameter("@CreatedAt", DateTime.Now));


                var result = await Task.Run(() => _context.Database
               .ExecuteSqlRawAsync(@"exec sp_AddVendorProduct @VendorId,@ProductName,@CategoryId,@GST,@ProductPrice,@HSNCode,@IsActive,@CreatedAt", parameter.ToArray()));

                return result;
            }
            else
            {
                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@id", model.Id));
                parameter.Add(new SqlParameter("@VendorId", VendorId));
                parameter.Add(new SqlParameter("@ProductName", model.ProductName));
                parameter.Add(new SqlParameter("@CategoryId", model.CategoryId));
                parameter.Add(new SqlParameter("@Gst", model.Gst));
                parameter.Add(new SqlParameter("@ProductPrice", model.ProductPrice));
                parameter.Add(new SqlParameter("@Hsncode", model.Hsncode));

                var result = await Task.Run(() => _context.Database
               .ExecuteSqlRawAsync(@"exec sp_UpdateVendorProduct @id,@VendorId,@ProductName,@CategoryId,@GST,@ProductPrice,@HSNCode", parameter.ToArray()));

                return result;
            }

        }

        public async Task<List<VendorProductDTO>> GetVendorProductList(int vendorid)
        {
            var result = await (from p in _context.VendorProductMasters
                                join c in _context.VendorCategoryMasters on p.CategoryId equals c.Id
                                join g in _context.GstMasters on p.Gst equals g.Id
                                where p.VendorId == vendorid && p.IsActive == true
                                select new VendorProductDTO
                                {
                                    Id = p.Id,
                                    ProductName = p.ProductName,
                                    Category = c.CategoryName,
                                    Hsncode = p.Hsncode,
                                    Gst = g.GstPercentagen,
                                    ProductPrice = p.ProductPrice,
                                    IsActive = p.IsActive,
                                    CreatedAt = p.CreatedAt
                                }).OrderByDescending(x => x.Id).ToListAsync();

            return result;
        }

        public async Task<bool> AddVendorCategory(VendorCategoryMaster model, int VendorId)
        {
            try
            {
                if (model.Id == 0)
                {
                    var data = new VendorCategoryMaster()
                    {
                        VendorId = VendorId,
                        CategoryName = model.CategoryName,
                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.VendorCategoryMasters.Find(model.Id);
                    existdata.CategoryName = model.CategoryName;
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<VendorCategoryMaster>> GetVendorCategoryListByVendorId(int VendorId)
        {
            try
            {
                var result = _context.VendorCategoryMasters.Where(c => c.VendorId == VendorId).ToList();
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<EmpExperienceletter> GetExperienceletterbyid(int? id)
        {
            try
            {
                var query = await _context.EmpExperienceletters.Where(x => x.Id == id).FirstOrDefaultAsync();
                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> updateExperienceletterdetail(EmpExperienceletter model)
        {
            try
            {
                var existing = await _context.EmpExperienceletters.FindAsync(model.Id);
                if (existing != null)
                {
                    existing.StartDate = model.StartDate;
                    existing.EndDate = model.EndDate;
                    existing.HrDesignation = model.HrDesignation;
                    existing.HrName = model.HrName;
                    existing.EmployeeId = model.EmployeeId;
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddExperienceletterdetail(EmpExperienceletter model, int Userid)
        {
            try
            {
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                EmpExperienceletter of = new EmpExperienceletter()
                {
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Vendorid = adminlogin.Vendorid,
                    HrDesignation = model.HrDesignation,
                    HrName = model.HrName,
                    EmployeeId = model.EmployeeId,
                };
                _context.EmpExperienceletters.Add(of);
                _context.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> CustomerInvoice(List<ProductDetail> model, string InvoiceNo, int vendorid, DateTime? InvoiceDate = null, DateTime? InvoiceDueDate = null, string InvoiceNotes = null, string InvoiceTerms = null)
        {
            try
            {
                var AlreadyInvoiceNo = string.Empty;

                var allExistingData = await _context.CustomerInvoices
                    .Where(ci => model.Select(m => m.CustomerId).Contains(ci.CustomerId) && ci.InvoiceNumber == InvoiceNo)
                    .ToListAsync();
                var invoicedetail = await _context.CustomerInvoicedetails.FirstOrDefaultAsync(cid => cid.InvoiceNumber == InvoiceNo);

                var groupedData = allExistingData
                    .GroupBy(ci => ci.CustomerId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var product in model)
                {
                    var customerExistingData = groupedData.ContainsKey(product.CustomerId)
                        ? groupedData[product.CustomerId]
                        : new List<CustomerInvoice>();

                    var existingInvoice = customerExistingData
                        .FirstOrDefault(ci => ci.ProductId == product.ProductId && ci.InvoiceNumber == InvoiceNo);

                    if (existingInvoice != null)
                    {
                        AlreadyInvoiceNo = existingInvoice.InvoiceNumber;
                    }

                    decimal totalDueAmount = customerExistingData
                        .Where(ci => ci.DueAmount.HasValue)
                        .Select(ci => ci.DueAmount.Value)
                        .FirstOrDefault();

                    int paymentstatus = customerExistingData
                        .Select(ci => ci.Paymentstatus)
                        .FirstOrDefault() ?? 2; 

                    if (product.Id == 0) 
                    {
                        decimal newDueAmount = 0;

                        if (totalDueAmount != 0)
                        {
                            newDueAmount = (decimal)(totalDueAmount +
                                product.ProductPrice +
                                ((product.ProductPrice * (product.IGST ?? 0m)) / 100) +
                                ((product.ProductPrice * (product.SGST ?? 0m)) / 100) +
                                ((product.ProductPrice * (product.CGST ?? 0m)) / 100));
                        }
                        else
                        {
                            newDueAmount = (decimal)(product.ProductPrice +
                                ((product.ProductPrice * (product.IGST ?? 0m)) / 100) +
                                ((product.ProductPrice * (product.SGST ?? 0m)) / 100) +
                                ((product.ProductPrice * (product.CGST ?? 0m)) / 100));
                        }
                        var newInvoice = new CustomerInvoice()
                        {
                            VendorId = vendorid,
                            CustomerId = product.CustomerId,
                            ProductId = product.ProductId,
                            Description = product.Description,
                            ProductPrice = product.ProductPrice,
                            RenewPrice = product.RenewPrice,
                            NoOfRenewMonth = product.NoOfRenewMonth,
                            Hsncode = product.HsnSacCode,
                            StartDate = product.StartDate,
                            RenewDate = product.RenewDate,
                            Igst = product.IGST,
                            Sgst = product.SGST,
                            Cgst = product.CGST,
                            InvoiceNumber = string.IsNullOrEmpty(AlreadyInvoiceNo) ? InvoiceNo : AlreadyInvoiceNo,
                            CreatedDate = DateTime.Now,
                            Paymentstatus = paymentstatus, 
                            PaidAmount = customerExistingData
                                .FirstOrDefault(ci => ci.PaidAmount.HasValue)?.PaidAmount ?? 0, 
                            DueAmount = newDueAmount,
                        };

                        _context.Add(newInvoice);

                        foreach (var item in customerExistingData)
                        {
                            item.DueAmount = newDueAmount;
                        }
                    }
                    else 
                    {
                        var data = allExistingData.FirstOrDefault(ci => ci.Id == product.Id);
                        if (data != null)
                        {
                            data.ProductPrice = product.ProductPrice;
                            data.RenewPrice = product.RenewPrice;
                            data.NoOfRenewMonth = product.NoOfRenewMonth;
                            data.Description = product.Description;
                            data.StartDate = product.StartDate;
                            data.RenewDate = product.RenewDate;
                            data.Igst = product.IGST;
                            data.Sgst = product.SGST;
                            data.Cgst = product.CGST;
                            _context.Update(data);
                            AlreadyInvoiceNo = data.InvoiceNumber;
                        }
                    }
                    if (invoicedetail != null)
                    {
                        invoicedetail.InvoiceNumber = InvoiceNo;
                        invoicedetail.InvoiceDate = InvoiceDate;
                        invoicedetail.InvoiceDueDate = InvoiceDueDate;
                        invoicedetail.Notes = InvoiceNotes;
                        invoicedetail.Terms = InvoiceTerms;
                        _context.Update(invoicedetail);
                    }
                    else
                    {
                        var newinvoicedetail = new CustomerInvoicedetail()
                        {
                            InvoiceNumber = InvoiceNo,
                            InvoiceDate = InvoiceDate,
                            InvoiceDueDate = InvoiceDueDate,
                            Notes = InvoiceNotes,
                            Terms = InvoiceTerms,
                        };
                        _context.Add(newinvoicedetail);
                    }
                    await _context.SaveChangesAsync();
                }

              return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the invoice.", ex);
            }
        }

        //public async Task<bool> CustomerInvoice(List<ProductDetail> model, string InvoiceNo, int vendorid)
        //{
        //    try
        //    {
        //        var AlreadyInvoiceNo = string.Empty;

        //        var allExistingData = await _context.CustomerInvoices
        //            .Where(ci => model.Select(m => m.CustomerId).Contains(ci.CustomerId) && ci.InvoiceNumber == InvoiceNo )
        //            .ToListAsync();

        //        foreach (var product in model)
        //        {
        //            var existingInvoice = allExistingData
        //                .FirstOrDefault(ci => ci.CustomerId == product.CustomerId &&
        //                                      ci.ProductId == product.ProductId &&
        //                                      ci.InvoiceNumber == InvoiceNo);

        //            var customerExistingData = allExistingData
        //                .Where(ci => ci.CustomerId == product.CustomerId)
        //                .ToList();

        //            if (existingInvoice != null)
        //            {
        //                AlreadyInvoiceNo = existingInvoice.InvoiceNumber;

        //            }
        //            decimal totalDueAmount = customerExistingData
        //                                        .Where(ci => ci.DueAmount.HasValue)
        //                                        .Select(ci => ci.DueAmount.Value)
        //                                        .FirstOrDefault();

        //            decimal dueAmount = (decimal)(totalDueAmount +
        //                                 (product.ProductPrice) + (product.ProductPrice * product.IGST / 100 ?? 0) +
        //                                 (product.ProductPrice * product.SGST / 100 ?? 0) +
        //                                 (product.ProductPrice * product.CGST / 100 ?? 0));

        //            if (product.Id == 0)
        //            {
        //                var newInvoice = new CustomerInvoice()
        //                {
        //                    VendorId = vendorid,
        //                    CustomerId = product.CustomerId,
        //                    ProductId = product.ProductId,
        //                    Description = product.Description,
        //                    ProductPrice = product.ProductPrice,
        //                    RenewPrice = product.RenewPrice,
        //                    NoOfRenewMonth = product.NoOfRenewMonth,
        //                    Hsncode = product.HsnSacCode,
        //                    StartDate = product.StartDate,
        //                    RenewDate = product.RenewDate,
        //                    Igst = product.IGST,
        //                    Sgst = product.SGST,
        //                    Cgst = product.CGST,
        //                    InvoiceNumber = string.IsNullOrEmpty(AlreadyInvoiceNo) ? InvoiceNo : AlreadyInvoiceNo,
        //                    CreatedDate = DateTime.Now,
        //                    Paymentstatus = 2,
        //                    PaidAmount = customerExistingData.FirstOrDefault(ci => ci.PaidAmount.HasValue)?.PaidAmount ?? 0,
        //                    DueAmount = customerExistingData.FirstOrDefault(ci => ci.DueAmount.HasValue)?.DueAmount ?? 0,
        //                    Dueamountdate = product.Dueamountdate,
        //                };

        //                _context.Add(newInvoice);
        //                foreach (var item in customerExistingData)
        //                {
        //                    item.DueAmount = dueAmount;
        //                }
        //                await _context.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                var data = allExistingData.FirstOrDefault(ci => ci.Id == product.Id);
        //                if (data != null)
        //                {
        //                    data.ProductPrice = product.ProductPrice;
        //                    data.RenewPrice = product.RenewPrice;
        //                    data.NoOfRenewMonth = product.NoOfRenewMonth;
        //                    data.Description = product.Description;
        //                    data.StartDate = product.StartDate;
        //                    data.RenewDate = product.RenewDate;
        //                    data.Igst = product.IGST;
        //                    data.Sgst = product.SGST;
        //                    data.Cgst = product.CGST;
        //                    data.Dueamountdate = product.Dueamountdate;

        //                    _context.Update(data);
        //                    foreach (var item in customerExistingData)
        //                    {
        //                        item.DueAmount = dueAmount;
        //                    }
        //                    await _context.SaveChangesAsync();

        //                    AlreadyInvoiceNo = data.InvoiceNumber;
        //                }
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public async Task<List<CustomerInvoiceDTO>> GetCustometInvoiceList(int vendorId)
        {
            try
            {
                var groupedInvoices = await (from ci in _context.CustomerInvoices
                                             join c in _context.CustomerRegistrations on ci.CustomerId equals c.Id
                                             join p in _context.VendorProductMasters on ci.ProductId equals p.Id
                                             join s in _context.States on c.StateId equals s.Id
                                             join ct in _context.Cities on c.CityId equals ct.Id
                                             join sb in _context.States on c.BillingStateId equals sb.Id
                                             join ctb in _context.Cities on c.BillingCityId equals ctb.Id
                                             join pm in _context.Paymentmodes on ci.Paymentstatus equals pm.Id
                                             where c.Vendorid == vendorId
                                             group new { ci, c, p, s, ct, sb, ctb, pm } by ci.InvoiceNumber into grouped
                                             select new CustomerInvoiceDTO
                                             {
                                                 InvoiceId = grouped.First().ci.Id,
                                                 CustomerId = grouped.First().ci.CustomerId,
                                                 CompanyName = grouped.First().c.CompanyName,
                                                 MobileNumber = grouped.First().c.MobileNumber,
                                                 AlternateNumber = grouped.First().c.AlternateNumber,
                                                 Email = grouped.First().c.Email,
                                                 ProductName = grouped.First().p.ProductName,
                                                 OfficeAddress = grouped.First().c.Location,
                                                 OfficeState = grouped.First().s.SName,
                                                 OfficeCity = grouped.First().ct.City1,
                                                 BillingAddress = grouped.First().c.BillingAddress,
                                                 BillingState = grouped.First().sb.SName,
                                                 BillingCity = grouped.First().ctb.City1,
                                                // InvoiceDate = grouped.First().ci.CreatedDate,
                                                 RenewPrice = grouped.First().ci.RenewPrice,
                                                 StartDate = grouped.First().ci.StartDate,
                                                 RenewDate = grouped.First().ci.RenewDate,
                                                 InvoiceNumber = grouped.Key,
                                                 Paymentstatus = grouped.First().pm.PaymentType,
                                                 PaidAmount = grouped.First().ci.PaidAmount ?? 0,
                                                 DueAmount = grouped.First().ci.DueAmount ?? 0,
                                                 IGST = grouped.Sum(g => g.ci.Igst ?? 0),
                                                 SGST = grouped.Sum(g => g.ci.Sgst ?? 0),
                                                 CGST = grouped.Sum(g => g.ci.Cgst ?? 0),
                                                 Paymentid = grouped.First().ci.Paymentstatus,
                                             }).OrderByDescending(ci => ci.InvoiceId).ToListAsync();

                foreach (var invoice in groupedInvoices)
                {
                    invoice.TotalAmount = await CalculateTotalAmountByInvoiceId(invoice.InvoiceNumber);
                }

                return groupedInvoices;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CustomerInvoiceDTO> CustomerProductInvoice(string InvoiceNumber)
        {
            try
            {

                CustomerInvoiceDTO invoiceDTO = new CustomerInvoiceDTO();
                invoiceDTO = (from ci in _context.CustomerInvoices
                              join c in _context.CustomerRegistrations on ci.CustomerId equals c.Id
                              join s in _context.States on c.StateId equals s.Id
                              join ct in _context.Cities on c.CityId equals ct.Id
                              join sb in _context.States on c.BillingStateId equals sb.Id
                              join ctb in _context.Cities on c.BillingCityId equals ctb.Id
                              join v in _context.VendorRegistrations on ci.VendorId equals v.Id
                              join vb in _context.VendorBankDetails on v.Id equals vb.VendorId
                              join vs in _context.States on v.StateId equals vs.Id
                              join vct in _context.Cities on v.CityId equals vct.Id
                              join ctd in _context.CustomerInvoicedetails on ci.InvoiceNumber equals ctd.InvoiceNumber
                              where (ci.InvoiceNumber == InvoiceNumber && ctd.InvoiceNumber == InvoiceNumber)
                              select new CustomerInvoiceDTO
                              {
                                  InvoiceId = ci.Id,
                                  InvoiceNumber = ci.InvoiceNumber,
                                  CompanyName = c.CompanyName,
                                  CompanyLogo = v.CompanyImage,
                                  VendorCompanyName = v.CompanyName,
                                  AccountNumber = vb.AccountNumber,
                                  AccountHolderName = vb.AccountHolderName,
                                  BankName = vb.BankName,
                                  Ifsc = vb.Ifsc,
                                  BranchAddress = vb.BranchAddress,
                                  VendorGstNumber = v.GstNumber,
                                  VendorOfficeAddress = v.Location,
                                  VendorOfficeCity = vct.City1,
                                  VendorOfficeState = vs.SName,
                                  MobileNumber = c.MobileNumber,
                                  Email = c.Email,
                                  OfficeAddress = c.Location,
                                  OfficeState = s.SName,
                                  OfficeCity = ct.City1,
                                  BillingCity = ctb.City1,
                                  BillingAddress = c.BillingAddress,
                                  BillingState = sb.SName,
                                  InvoiceDate = ctd.InvoiceDate.Value.ToString("dd-MM-yyyy"),
                                  IGST = ci.Igst,
                                  CGST = ci.Cgst,
                                  SGST = ci.Sgst,
                                  CustomerGstNumber = c.GstNumber,
                                  NoOfRenewMonth = ci.NoOfRenewMonth,
                                  VendorSingature = v.VendorSingature,
                                  InvoiceDueDate = ctd.InvoiceDueDate.Value.ToString("dd-MM-yyyy"),
                                  Notes = ctd.Notes,
                                  Terms = ctd.Terms
                              }).FirstOrDefault();

                if (invoiceDTO != null)
                {
                    var invoiceItems = (from ci in _context.CustomerInvoices
                                        join p in _context.VendorProductMasters on ci.ProductId equals p.Id
                                        where (ci.InvoiceNumber == InvoiceNumber)
                                        select new ProductDetailList
                                        {
                                            ProductName = p.ProductName,
                                            IGST = ci.Igst,
                                            SGST = ci.Sgst,
                                            CGST = ci.Cgst,
                                            StartDate = ci.StartDate,
                                            RenewDate = ci.RenewDate,
                                            HsnSacCode = ci.Hsncode,
                                            ProductPrice = ci.ProductPrice,
                                            DueAmount = ci.DueAmount,
                                            PaidAmount = ci.PaidAmount,
                                            Description = ci.Description,
                                        }).ToList();

                    // Add the list to the invoice DTO if needed
                    invoiceDTO.ProductDetailLists = invoiceItems;
                }


                return invoiceDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddVendorBankDeatils(VendorBankDetail model, int VendorId)
        {
            try
            {
                if (model.Id == 0)
                {
                    var domainmodel = new VendorBankDetail()
                    {
                        VendorId = VendorId,
                        AccountNumber = model.AccountNumber,
                        AccountHolderName = model.AccountHolderName,
                        BankName = model.BankName,
                        BranchAddress = model.BranchAddress,
                        Ifsc = model.Ifsc
                    };
                    _context.Add(domainmodel);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var data = _context.VendorBankDetails.Where(b => b.Id == model.Id).FirstOrDefault();
                    if (data != null)
                    {
                        data.AccountNumber = model.AccountNumber;
                        data.AccountHolderName = model.AccountHolderName;
                        data.BankName = model.BankName;
                        data.BranchAddress = model.BranchAddress;
                        data.Ifsc = model.Ifsc;
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<VendorBankDetail>> GetVendorBankDetail(int VendorId)
        {
            try
            {
                var result = _context.VendorBankDetails.Where(x => x.VendorId == VendorId).ToList();
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddOfficeEvents(OfficeEvent model, int VendorId)
        {
            try
            {
                if (model.Id == 0)
                {
                    var data = new OfficeEvent()
                    {
                        Vendorid = VendorId,
                        Tittle = model.Tittle,
                        Subtittle = model.Subtittle,
                        Description = model.Description,
                        Date = model.Date
                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.OfficeEvents.Find(model.Id);
                    existdata.Tittle = model.Tittle;
                    existdata.Subtittle = model.Subtittle;
                    existdata.Description = model.Description;
                    existdata.Date = model.Date;
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<ApprovedLeaveApplyList>> GetLeaveapplydetailList(int? userId)
        {
            if (!userId.HasValue)
                throw new ArgumentNullException(nameof(userId), "UserId cannot be null");

            try
            {
                var adminLogin = await _context.AdminLogins.FindAsync(userId);

                var empList = await _context.EmployeeRegistrations
                    .Where(x => x.Vendorid == userId && x.IsDeleted == false)
                    .ToListAsync();

                if (empList == null || empList.Count == 0)
                    throw new Exception("No employees found for the specified user.");

                var leaveDetails = new List<ApprovedLeaveApplyList>();

                foreach (var emp in empList)
                {
                    var leave = await _context.ApplyLeaveNews
                        .Where(p => p.UserId == emp.EmployeeId)
                        .ToListAsync();

                    if (leave == null || leave.Count == 0)
                    {
                        continue;
                    }

                    foreach (var l in leave)
                    {
                        decimal totalFullday = (l.EndDate - l.StartDate).Days - (l.EndDate != l.StartDate ? 1 : 0);
                        totalFullday = Math.Max(totalFullday, 0);
                        leaveDetails.Add(new ApprovedLeaveApplyList
                        {
                            Id = l.Id,
                            UserId = l.UserId,
                            EmployeeName = $"{emp.FirstName} {(emp.MiddleName != null ? emp.MiddleName + " " : "")}{emp.LastName}",
                            EmpMobileNumber = await _context.EmployeePersonalDetails
                                .Where(e => e.EmpRegId == emp.EmployeeId)
                                .Select(e => e.MobileNumber)
                                .FirstOrDefaultAsync() ?? "Unknown",
                            LeaveType = GetLeaveType(l.StartLeaveId, l.EndeaveId, totalFullday, l.CountLeave, (decimal)l.PaidCountLeave),
                            TypeOfLeaveId = await _context.LeaveTypes
                                .Where(s => s.Id == l.TypeOfLeaveId)
                                .Select(s => s.Leavetype1)
                                .FirstOrDefaultAsync() ?? "Unknown",
                            StartDate = l.StartDate,
                            EndDate = l.EndDate,
                            CreatedDate = l.CreatedDate,
                            UnPaidCountLeave = l.CountLeave,
                            Month = l.Month,
                            Reason = l.Reason ?? "No reason provided",
                            Isapprove = l.Isapprove,
                            PaidCountLeave = l.PaidCountLeave,
                        });
                    }
                }
                return leaveDetails
                      .OrderByDescending(l => l.StartDate == default(DateTime) ? DateTime.MinValue : l.StartDate)
                      .ThenByDescending(l => l.EndDate == default(DateTime) ? DateTime.MinValue : l.EndDate)
                      .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving leave details.", ex);
            }
        }

        public async Task<bool> AddEmployeeEpf(EmployeeEpfPayrollInfo model, int VendorId)
        {
            try
            {
                if (model.Id == 0)
                {

                    var domainmodel = new EmployeeEpfPayrollInfo()
                    {
                        Epfpercentage = model.Epfpercentage,
                        Vendorid = VendorId,
                        CreatedDate = DateTime.Now,

                    };
                    await _context.AddAsync(domainmodel);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    var existdata = _context.EmployeeEpfPayrollInfos.Find(model.Id);
                    existdata.Epfpercentage = model.Epfpercentage;

                    await _context.SaveChangesAsync();
                    return true;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AddEmployeeEsic(EmployeeEsicPayrollInfo model, int VendorId)
        {
            try
            {
                if (model.Id == 0)
                {
                    var domainmodel = new EmployeeEsicPayrollInfo()
                    {
                        Esicpercentage = model.Esicpercentage,
                        EsicAmount = model.EsicAmount,
                        Vendorid = VendorId,
                        CreatedDate = DateTime.Now
                    };
                    await _context.AddAsync(domainmodel);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    var existdata = _context.EmployeeEsicPayrollInfos.Find(model.Id);
                    existdata.Esicpercentage = model.Esicpercentage;
                    existdata.EsicAmount = model.EsicAmount;
                    await _context.SaveChangesAsync();
                    return true;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        private static string? GetLeaveType(int startLeaveId, int endLeaveId, decimal totalFullday, decimal countLeave, decimal paidCountLeave)
        {
            int halfDayCount = 0;
            int fullDayCount = (int)totalFullday;

            if (startLeaveId == endLeaveId)
            {
                if (startLeaveId == 1 || startLeaveId == 2)
                {
                    halfDayCount++;
                }
                else if (startLeaveId == 3)
                {
                    fullDayCount++;
                }
            }
            else
            {
                if (startLeaveId == 1 || startLeaveId == 2) halfDayCount++;
                if (startLeaveId == 3) fullDayCount++;

                if (endLeaveId == 1 || endLeaveId == 2) halfDayCount++;
                if (endLeaveId == 3) fullDayCount++;
            }

            List<string> leaveTypes = new List<string>();

            if (halfDayCount > 0)
            {
                leaveTypes.Add($"{halfDayCount} Half Day{(halfDayCount > 1 ? "s" : "")}");
            }
            if (fullDayCount > 0)
            {
                leaveTypes.Add($"{fullDayCount} Full Day{(fullDayCount > 1 ? "s" : "")}");
            }

            decimal totalLeave = countLeave + paidCountLeave;
            leaveTypes.Add($"(Total Leaves: {totalLeave})");

            return string.Join(", ", leaveTypes);
        }
        public async Task<bool> Addfaq(AppFaq model)
        {
            if (model.Id == 0)
            {
                var data = new AppFaq()
                {
                    Tittle = model.Tittle,
                    Subtittle = model.Subtittle,

                };
                _context.Add(data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                var existdata = _context.AppFaqs.Find(model.Id);
                existdata.Tittle = model.Tittle;
                existdata.Subtittle = model.Subtittle;
            }
            _context.SaveChanges();
            return true;
        }
        public async Task<bool> AddAndUpdateBlog(BlogDto model, string AddedBy)
        {
            try
            {

                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                string ImagePath = "";

                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .png, .jpg, and .jpeg files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("image", model.ImageFile, allowedExtensions);
                    model.BlogImage = ImagePath;
                }

                if (model.Id == 0)
                {
                    var data = new Blog()
                    {
                        Title = model.Title,
                        Content = model.Content,
                        BlogImage = model.BlogImage,
                        AddedBy = AddedBy,
                        CreatedAt = DateTime.Now,
                        IsPublished = true

                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.Blogs.Find(model.Id);

                    existdata.Title = model.Title;
                    existdata.Content = model.Content;
                    if (model.BlogImage != null)
                    {
                        existdata.BlogImage = model.BlogImage;
                    }

                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> Addaddcompany(Aboutcompany model, int VendorId)
        {
            try
            {
                if (model.Id == 0)
                {
                    var data = new Aboutcompany()
                    {
                        Vendorid = VendorId,
                        Companylink = model.Companylink,

                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.Aboutcompanies.Find(model.Id);
                    existdata.Companylink = model.Companylink;
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AddEventsScheduler(EventsmeetSchedulerDto model, int VendorId)
        {
            try
            {

                if (model.Id == 0)
                {
                    var data = new EventsmeetScheduler()
                    {
                        Vendorid = VendorId,
                        Tittle = model.Tittle,
                        Description = model.Description,
                        EmployeeId = model.EmployeeId != null ? string.Join(",", model.EmployeeId) : string.Empty,
                        ScheduleDate = model.ScheduleDate,
                        Createddate = DateTime.Now,
                        IsEventsmeet = model.IsEventsmeet,
                        IsActive = model.IsActive,
                        Time = model.Time,
                    };
                    _context.Add(data);
                }
                else
                {
                    var existdata = await _context.EventsmeetSchedulers.FindAsync(model.Id);
                    if (existdata == null)
                    {
                        return false;
                    }
                    existdata.Tittle = model.Tittle;
                    existdata.EmployeeId = model.EmployeeId != null ? string.Join(",", model.EmployeeId) : string.Empty;
                    existdata.Description = model.Description;
                    existdata.ScheduleDate = model.ScheduleDate;
                    existdata.IsEventsmeet = model.IsEventsmeet;
                    existdata.IsActive = model.IsActive;
                    existdata.Time = model.Time;
                    existdata.Vendorid = VendorId;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<EmpTasknameDto>> GetSubTasks(int vendorid)
        {
            try
            {
                var result = await (from taskList in _context.EmployeeTasksLists
                                    join empTask in _context.EmployeeTasks on taskList.Emptaskid equals empTask.Id
                                    join taskStatus in _context.TaskStatuses on taskList.TaskStatus equals taskStatus.Id
                                    join empname in _context.EmployeeRegistrations on taskList.EmployeeId  equals empname.EmployeeId
                                    where (empTask.Vendorid == vendorid)
                                    orderby taskList.Id descending
                                    select new EmpTasknameDto
                                    {
                                        Id = empTask.Id,
                                        Emptask = empTask.Task,
                                        Taskname = taskList.Taskname,
                                        TaskStatusId = taskList.TaskStatus,
                                        TaskStatus = taskStatus.StatusName,
                                        EmployeeId = taskList.EmployeeId,
                                        SubtaskId = taskList.Id,
                                        EmployeeName = empname.FirstName,
                                    }).ToListAsync();
                return result;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddAndUpdateOurExpertise(ExperiseDTO model)
        {
            try
            {

                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                string ImagePath = "";

                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .png, .jpg, and .jpeg files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("image", model.ImageFile, allowedExtensions);
                    model.ExperiseImage = ImagePath;
                }

                if (model.Id == 0)
                {
                    var data = new OurExpertise()
                    {
                        ExpertiseName = model.ExpertiseName,
                        Description = model.Description,
                        ExperiseImage = model.ExperiseImage

                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.OurExpertises.Find(model.Id);

                    existdata.ExpertiseName = model.ExpertiseName;
                    existdata.Description = model.Description;
                    if (model.ExperiseImage != null)
                    {
                        existdata.ExperiseImage = model.ExperiseImage;
                    }

                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddAndUpdateOurStory(OurStoryDTO model, string AddedBy)
        {
            try
            {

                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                string ImagePath = "";

                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .png, .jpg, and .jpeg files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("image", model.ImageFile, allowedExtensions);
                    model.Image = ImagePath;
                }

                if (model.Id == 0)
                {
                    var data = new OurStory()
                    {
                        Title = model.Title,
                        Content = model.Content,
                        Author = AddedBy,
                        Image = model.Image

                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.OurStories.Find(model.Id);

                    existdata.Title = model.Title;
                    existdata.Content = model.Content;
                    existdata.IsActive = model.IsActive;
                    if (model.Image != null)
                    {
                        existdata.Image = model.Image;
                    }

                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AddAndUpdateRequestDemo(RequestDemoDto model, string AddedBy)
        {
            try
            {

                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                string ImagePath = "";

                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .png, .jpg, and .jpeg files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("image", model.ImageFile, allowedExtensions);
                    model.Image = ImagePath;
                }

                if (model.Id == 0)
                {
                    var data = new RequestDemo()
                    {
                        Title = model.Title,
                        Content = model.Content,
                        Author = AddedBy,
                        Image = model.Image,
                        IsActive = true,
                        PublishedDate = DateTime.Now

                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.RequestDemos.Find(model.Id);

                    existdata.Title = model.Title;
                    existdata.Content = model.Content;
                    existdata.IsActive = model.IsActive ?? false;
                    existdata.PublishedDate = DateTime.Now;
                    if (model.Image != null)
                    {
                        existdata.Image = model.Image;
                    }

                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddAndUpdateOurCoreValues(OurCoreValuesDto model, string AddedBy)
        {
            try
            {

                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                string ImagePath = "";

                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .png, .jpg, and .jpeg files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("image", model.ImageFile, allowedExtensions);
                    model.Image = ImagePath;
                }

                if (model.Id == 0)
                {
                    var data = new OurCoreValue()
                    {
                        Title = model.Title,
                        Content = model.Content,
                        Author = AddedBy,
                        Image = model.Image,
                        IsActive = true,
                        PublishedDate = DateTime.Now

                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.OurCoreValues.Find(model.Id);

                    existdata.Title = model.Title;
                    existdata.Content = model.Content;
                    existdata.IsActive = model.IsActive ?? false;
                    existdata.PublishedDate = DateTime.Now;
                    if (model.Image != null)
                    {
                        existdata.Image = model.Image;
                    }

                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AddAndUpdateFeaturebenifits(FeaturebenifitsDto model, string AddedBy)
        {
            try
            {

                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                string ImagePath = "";

                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .png, .jpg, and .jpeg files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("image", model.ImageFile, allowedExtensions);
                    model.Image = ImagePath;
                }

                if (model.Id == 0)
                {
                    var data = new Featurebenifit()
                    {
                        Title = model.Title,
                        Content = model.Content,
                        Author = AddedBy,
                        Image = model.Image,
                        IsActive = true,
                        PublishedDate = DateTime.Now

                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.Featurebenifits.Find(model.Id);

                    existdata.Title = model.Title;
                    existdata.Content = model.Content;
                    existdata.IsActive = model.IsActive ?? false;
                    existdata.PublishedDate = DateTime.Now;
                    if (model.Image != null)
                    {
                        existdata.Image = model.Image;
                    }

                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddAndUpdateOurTutorial(TutorialDTO model, string AddedBy)
        {
            try
            {
                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".mp4" };
                string ImagePath = "";

                if (model.VideoFile != null)
                {
                    var fileExtension = Path.GetExtension(model.VideoFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .mp4 files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("TutorialVideos", model.VideoFile, allowedExtensions);
                    model.VedioUrl = ImagePath;
                }
                if (model.Id == 0)
                {
                    var data = new OurTutorial()
                    {
                        Title = model.Title,
                        Description = model.Description,
                        VedioUrl = model.VedioUrl,
                        Author = AddedBy


                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {

                    var existdata = _context.OurTutorials.Find(model.Id);

                    existdata.Title = model.Title;
                    existdata.Description = model.Description;
                    existdata.IsActive = model.IsActive;
                    if (model.VedioUrl != null)
                    {
                        existdata.VedioUrl = model.VedioUrl;
                    }

                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddAndUpdateCaseStudies(CaseStudiesDTO model)
        {
            try
            {

                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                string ImagePath = "";

                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .png, .jpg, and .jpeg files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("image", model.ImageFile, allowedExtensions);
                    model.Image = ImagePath;
                }

                if (model.Id == 0)
                {
                    var data = new CaseStudy()
                    {
                        Title = model.Title,
                        Description = model.Description,
                        IsActive = true,
                        Image = model.Image

                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.CaseStudies.Find(model.Id);

                    existdata.Title = model.Title;
                    existdata.Description = model.Description;
                    existdata.IsActive = model.IsActive;
                    if (model.Image != null)
                    {
                        existdata.Image = model.Image;
                    }

                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddAndUpdatePricingPlan(PricingPlanDTO model)
        {
            try
            {

                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                string ImagePath = "";

                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .png, .jpg, and .jpeg files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("image", model.ImageFile, allowedExtensions);
                    model.Image = ImagePath;
                }

                if (model.Id == 0)
                {
                    var data = new PricingPlan()
                    {
                        PlanName = model.PlanName,
                        Price = model.Price,
                        Title = model.Title,
                        Support = model.Support,
                        Image = model.Image,
                        AnnulPrice = model.AnnulPrice,
                        AnnulPriceInPercentage = model.AnnulPriceInPercentage
                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    foreach (var item in model.PlanFeatures)
                    {
                        PricingPlanFeature features = new()
                        {
                            PricingPlanId = data.Id,
                            Feature = item.Feature,
                        };
                        await _context.PricingPlanFeatures.AddAsync(features);
                        await _context.SaveChangesAsync();

                    }
                    return true;
                }
                else
                {
                    var existdata = _context.PricingPlans.Find(model.Id);

                    existdata.PlanName = model.PlanName;
                    existdata.Price = model.Price;
                    existdata.Title = model.Title;
                    existdata.Support = model.Support;
                    existdata.IsActive = model.IsActive;
                    existdata.AnnulPrice = model.AnnulPrice;
                    existdata.AnnulPriceInPercentage = model.AnnulPriceInPercentage;
                    if (model.Image != null)
                    {
                        existdata.Image = model.Image;
                    }
                    // Remove existing feature
                    var existingFeature = await _context.PricingPlanFeatures
                        .Where(s => s.PricingPlanId == existdata.Id)
                        .ToListAsync();
                    _context.PricingPlanFeatures.RemoveRange(existingFeature);

                    // Add new feature
                    foreach (var item in model.PlanFeatures)
                    {
                        PricingPlanFeature feature = new PricingPlanFeature
                        {
                            PricingPlanId = existdata.Id,
                            Feature = item.Feature,
                        };
                        await _context.PricingPlanFeatures.AddAsync(feature);
                    }


                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddAndUpdateProfessionaltax(ProfessionaltaxDto model)
        {
            try
            {

                if (model.Id == 0)
                {
                    var data = new Professionaltax()
                    {
                        Minamount = model.Minamount,
                        Maxamount = model.Maxamount,
                        Amountpercentage = model.Amountpercentage,
                        Iactive = true,
                        Finyear = Convert.ToInt32(model.Finyear),
                        CreateDate = DateTime.Now

                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {



                    var existdata = _context.Professionaltaxes.Find(model.Id);

                    existdata.Minamount = model.Minamount;
                    existdata.Maxamount = model.Maxamount;
                    existdata.Amountpercentage = model.Amountpercentage;
                    existdata.Iactive = model.Iactive;
                    existdata.CreateDate = DateTime.Now;
                    existdata.Finyear = Convert.ToInt32(model.Finyear);

                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddAndUpdateOtherService(OtherService model)
        {
            try
            {
                if (model.Id == 0)
                {
                    var domainmodel = new OtherService()
                    {
                        ServiceName = model.ServiceName,
                        Description = model.Description
                    };
                    _context.Add(domainmodel);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.OtherServices.Find(model.Id);
                    existdata.ServiceName = model.ServiceName;
                    existdata.Description = model.Description;
                    existdata.IsActive = model.IsActive;
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddAndUpdateMissionVisions(MissionVisionDTO model)
        {
            try
            {

                FileOperation fileOperation = new FileOperation(_webHostEnvironment);
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                string ImagePath = "";

                if (model.ImageFile != null)
                {
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException("Only .png, .jpg, and .jpeg files are allowed.");
                    }
                    ImagePath = fileOperation.SaveBase64Image("image", model.ImageFile, allowedExtensions);
                    model.Image = ImagePath;
                }

                if (model.Id == 0)
                {
                    var data = new MissionVision()
                    {
                        MissionVisionName = model.MissionVisionName,
                        Description = model.Description,
                        Image = model.Image

                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.MissionVisions.Find(model.Id);

                    existdata.MissionVisionName = model.MissionVisionName;
                    existdata.Description = model.Description;
                    existdata.IsActive = model.IsActive;
                    if (model.Image != null)
                    {
                        existdata.Image = model.Image;
                    }

                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddAndUpdateLeaveTypemaster(LeaveType model, int VendorId)
        {
            try
            {

                if (model.Id == 0)
                {
                    var data = new LeaveType()
                    {
                        Leavetype1 = model.Leavetype1,
                        Vendorid = VendorId,
                        Isactive = true,
                        Leavevalue = model.Leavevalue,
                        Createddate = DateTime.Now.Date
                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.LeaveTypes.Find(model.Id);

                    existdata.Leavetype1 = model.Leavetype1;
                    existdata.Isactive = model.Isactive;
                    existdata.Leavevalue = model.Leavevalue;
                    existdata.Createddate = DateTime.Now.Date;
                    existdata.Vendorid = VendorId;
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<decimal> CalculateTotalAmountByInvoiceId(string invoiceId)
        {
            try
            {
                var invoiceDetails = await (from ci in _context.CustomerInvoices
                                            where ci.InvoiceNumber == invoiceId
                                            select new
                                            {
                                                ProductPrice = ci.ProductPrice ?? 0,
                                                IGST = ci.Igst ?? 0,
                                                SGST = ci.Sgst ?? 0,
                                                CGST = ci.Cgst ?? 0
                                            }).ToListAsync();

                if (invoiceDetails == null || !invoiceDetails.Any())
                {
                    return 0;
                }

                decimal totalAmount = 0;
                foreach (var item in invoiceDetails)
                {
                    decimal productTotal = item.ProductPrice +
                                           (item.ProductPrice * item.IGST / 100) +
                                           (item.ProductPrice * item.SGST / 100) +
                                           (item.ProductPrice * item.CGST / 100);

                    totalAmount += productTotal;
                }

                totalAmount = decimal.Round(totalAmount, 2, MidpointRounding.AwayFromZero);

                return totalAmount;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<ApprovedwfhApplyList>> GetWfhapplydetailList(int? userId)
        {
            if (!userId.HasValue)
                throw new ArgumentNullException(nameof(userId), "UserId cannot be null");

            try
            {
                var adminLogin = await _context.AdminLogins.FindAsync(userId);

                var empList = await _context.EmployeeRegistrations
                    .Where(x => x.Vendorid == userId && x.IsDeleted == false)
                    .ToListAsync();

                if (empList == null || empList.Count == 0)
                    throw new Exception("No employees found for the specified user.");

                var leaveDetails = new List<ApprovedwfhApplyList>();

                foreach (var emp in empList)
                {
                    var leave = await _context.EmpApplywfhs
                        .Where(p => p.UserId == emp.EmployeeId)
                        .ToListAsync();

                    if (leave == null || leave.Count == 0)
                    {
                        continue;
                    }

                    foreach (var l in leave)
                    {
                        int totalFullday = (l.EndDate.Value.Date - l.Startdate.Value.Date).Days;
                        if (l.Startdate.Value.Date == l.EndDate.Value.Date)
                        {
                            totalFullday = 1;
                        }

                        leaveDetails.Add(new ApprovedwfhApplyList
                        {
                            Id = l.Id,
                            UserId = l.UserId,
                            EmployeeName = $"{emp.FirstName} {(emp.MiddleName != null ? emp.MiddleName + " " : "")}{emp.LastName}",
                            EmpMobileNumber = await _context.EmployeePersonalDetails
                                .Where(e => e.EmpRegId == emp.EmployeeId)
                                .Select(e => e.MobileNumber)
                                .FirstOrDefaultAsync() ?? "Unknown",
                            StartDate = l.Startdate,
                            EndDate = l.EndDate,
                            CreatedDate = (DateTime)l.Currentdate,
                            Reason = l.Reason ?? "No reason provided",
                            Isapprove = l.Iswfh,
                            days = totalFullday
                        });
                    }
                }

                return leaveDetails
                    .OrderByDescending(l => l.StartDate == default(DateTime) ? DateTime.MinValue : l.StartDate)
                    .ThenByDescending(l => l.EndDate == default(DateTime) ? DateTime.MinValue : l.EndDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving leave details.", ex);
            }
        }
        public async Task<bool> AddAndUpdateInvoiceChargesmaster(InvoiceChargesmaster model)
        {
            try
            {
                if (model.Id == 0)
                {
                    var domainmodel = new InvoiceChargesmaster()
                    {
                        Vendorid = model.Vendorid,
                        Isactive = true
                    };
                    _context.Add(domainmodel);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.InvoiceChargesmasters.Find(model.Id);
                    existdata.Vendorid = model.Vendorid;
                    existdata.Isactive = model.Isactive;
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<EmpRelievingletter> GetRelievingletterbyid(int? id)
        {
            try
            {
                var query = await _context.EmpRelievingletters.Where(x => x.Id == id).FirstOrDefaultAsync();
                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> updateRelievingletterdetail(EmpRelievingletter model)
        {
            try
            {
                var existing = await _context.EmpRelievingletters.FindAsync(model.Id);
                if (existing != null)
                {
                    existing.ResignationDate = model.ResignationDate;
                    existing.LastDateofEmployment = model.LastDateofEmployment;
                    existing.EmployeeId = model.EmployeeId;
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddRelievingletterdetail(EmpRelievingletter model, int Userid)
        {
            try
            {
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                EmpRelievingletter of = new EmpRelievingletter()
                {
                    ResignationDate = model.ResignationDate,
                    LastDateofEmployment = model.LastDateofEmployment,
                    Vendorid = adminlogin.Vendorid,
                    EmployeeId = model.EmployeeId,
                };
                _context.EmpRelievingletters.Add(of);
                _context.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> salarydeduction(Salarydeductionmaster model, int VendorId)
        {
            try
            {
                if (model.Id == 0)
                {
                    var data = new Salarydeductionmaster()
                    {
                        Vendorid = VendorId,
                        Deductiontype = model.Deductiontype,
                        Deductionpercentage = model.Deductionpercentage,
                    };
                    _context.Add(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var existdata = _context.Salarydeductionmasters.Find(model.Id);
                    existdata.Deductiontype = model.Deductiontype;
                    existdata.Deductionpercentage = model.Deductionpercentage;
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

}