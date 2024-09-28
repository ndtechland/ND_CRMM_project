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
        public async Task<int> Customer(Customer model,int vendorid)
        {
            try
            {
                using (var connection = new SqlConnection(Configuration.GetConnectionString("db1"))) // Make sure to define _connectionString
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Company_Name", model.CompanyName);
                    parameters.Add("@Location", model.Location);
                    parameters.Add("@Work_Location", string.Join(",", model.WorkLocation));
                    parameters.Add("@Mobile_number", model.MobileNumber);
                    parameters.Add("@Alternate_number", model.AlternateNumber);
                    parameters.Add("@Email", model.Email);
                    parameters.Add("@GST_Number", model.GstNumber);
                    parameters.Add("@Billing_Address", model.BillingAddress);
                    parameters.Add("@Product_Details", model.ProductDetails);
                    parameters.Add("@Start_date", model.StartDate);
                    parameters.Add("@Renew_Date", model.RenewDate);
                    parameters.Add("@BillingStateId", model.BillingStateId);
                    parameters.Add("@stateId", model.StateId);
                    parameters.Add("@BillingCityId", model.BillingCityId);
                    parameters.Add("@Vendorid", vendorid);
                    parameters.Add("@Renewprice", model.Renewprice);
                    parameters.Add("@NoOfRenewMonth", model.NoOfRenewMonth);
                    parameters.Add("@productprice", model.productprice);
                    parameters.Add("@SCGST", model.Scgst);
                    parameters.Add("@CGST", model.Cgst);
                    parameters.Add("@IGST", model.Igst);
                    parameters.Add("@IsSameAddress", model.IsSameAddress);
                    parameters.Add("@CustomerId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync("CustomerRegistration", parameters, commandType: CommandType.StoredProcedure);

                    int newCustomerId = parameters.Get<int>("@CustomerId");
                    return newCustomerId;


                }
            }
            catch (Exception)
            {
                throw; // Consider logging the exception here
            }
        }

        public async Task<List<CustomerListDto>> CustomerList(string userIdString)
        {
            List<CustomerListDto> cs = new List<CustomerListDto>();
            try
            {
                using (SqlConnection con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("Customerlist", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(userIdString));

                    con.Open();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (await rdr.ReadAsync())
                    {
                        var cse = new CustomerListDto()
                        {
                            Id = Convert.ToInt32(rdr["ID"]),
                            CompanyName = rdr["Company_Name"] == DBNull.Value ? null : Convert.ToString(rdr["Company_Name"]),
                            //WorkLocation = rdr["Work_Location"] == DBNull.Value ? null : rdr["Work_Location"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            WorkLocation = rdr["Work_Location"] == DBNull.Value ? "0" : Convert.ToString(rdr["Work_Location"]),
                            MobileNumber = rdr["Mobile_number"] == DBNull.Value ? "0" : Convert.ToString(rdr["Mobile_number"]),
                            AlternateNumber = rdr["Alternate_number"] == DBNull.Value ? "0" : Convert.ToString(rdr["Alternate_number"]),
                            Email = rdr["Email"] == DBNull.Value ? "0" : Convert.ToString(rdr["Email"]),
                            GstNumber = rdr["GST_Number"] == DBNull.Value ? "0" : Convert.ToString(rdr["GST_Number"]),
                            BillingAddress = rdr["Billing_Address"] == DBNull.Value ? "0" : Convert.ToString(rdr["Billing_Address"]),
                            ProductDetails = rdr["Product_Details"] == DBNull.Value ? "0" : Convert.ToString(rdr["Product_Details"]),
                            StartDate = rdr["Start_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rdr["Start_date"]),
                            RenewDate = rdr["Renew_Date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rdr["Renew_Date"]),
                            StateName = rdr["statename"] == DBNull.Value ? null : Convert.ToString(rdr["statename"]),
                            BillingStateId = rdr["BillingStateId"] == DBNull.Value ? null : Convert.ToString(rdr["BillingStateId"]),
                            BillingCityId = rdr["BillingCityId"] == DBNull.Value ? null : Convert.ToString(rdr["BillingCityId"]),
                            Location = rdr["Location"] == DBNull.Value ? null : Convert.ToString(rdr["Location"]),
                            productprice = rdr["productprice"] == DBNull.Value ? null : Convert.ToString(rdr["productprice"]),
                            Renewprice = rdr["Renewprice"] == DBNull.Value ? null : Convert.ToString(rdr["Renewprice"]),
                            Igst = rdr["Igst"] == DBNull.Value ? null : Convert.ToString(rdr["Igst"]),
                            Scgst = rdr["Scgst"] == DBNull.Value ? null : Convert.ToString(rdr["Scgst"]),
                            Cgst = rdr["Cgst"] == DBNull.Value ? null : Convert.ToString(rdr["Cgst"]),
                        };

                        cs.Add(cse);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching customer list", ex);
            }

            return cs;
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
            List<EmployeeImportExcel> employeeList = _context.EmployeeImportExcels.FromSqlRaw<EmployeeImportExcel>("USP_GetEmployeeDetails").ToListAsync().Result;

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


        public async Task<List<salarydetail>> salarydetail(int Userid)
        {
            List<salarydetail> emp = new List<salarydetail>();
            try
            {
                // Initialize the SQL connection
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("sp_SalaryDetail", con);

                // Add parameters for stored procedure
                //cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = Convert.ToInt32(customerId) });
                //cmd.Parameters.Add(new SqlParameter("@WorkLocation", SqlDbType.Int) { Value = Convert.ToInt32(WorkLocation) });
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Userid });

                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                // Execute the stored procedure and read the result
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                while (await rdr.ReadAsync())
                {
                    var emps = new salarydetail()
                    {
                        Id = Convert.ToInt32(rdr["ID"]),
                        FirstName = rdr["FirstName"] == DBNull.Value ? null : Convert.ToString(rdr["FirstName"]),
                        EmployeeId = rdr["EmployeeId"] == DBNull.Value ? null : Convert.ToString(rdr["EmployeeId"]),
                        MonthlyCtc = rdr["MonthlyCTC"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["MonthlyCTC"]),
                        CustomerID = rdr["Vendorid"] == DBNull.Value ? 0 : Convert.ToInt64(rdr["Vendorid"]),
                        FatherName = rdr["FatherName"] == DBNull.Value ? null : Convert.ToString(rdr["FatherName"]),
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
        }

        public async Task<List<GenerateSalary>> GenerateSalary(int Month, int year, int Userid, string EmployeeId)
        {
            try
            {
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();

                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("GetGenerateSalary", con);
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Convert.ToInt32(adminlogin.Vendorid) });
                cmd.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int) { Value = Convert.ToInt32(Month) });
                cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.Int) { Value = Convert.ToInt32(year) });
                cmd.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.VarChar) { Value = EmployeeId });
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
                        MonthlyCtc = Convert.ToDecimal(rdr["MonthlyCTC"]),
                        SalarySlip = Convert.ToString(rdr["SalarySlip"])
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
        //for excel
        public byte[] EmployeeListForExcel()
        {
            List<EmployeeImportExcel> employeeList = _context.EmployeeImportExcels.FromSqlRaw<EmployeeImportExcel>("USP_GetEmployeeDetails").ToListAsync().Result;

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
                    worksheet.Cell(currentwork, 16).Value = item.Pan;
                    //worksheet.Cell(currentwork, 19).Value = item.AddressLine1;
                    //worksheet.Cell(currentwork, 20).Value = item.AddressLine2;
                    //worksheet.Cell(currentwork, 21).Value = item.City;
                    //worksheet.Cell(currentwork, 22).Value = item.State;
                    //worksheet.Cell(currentwork, 23).Value = item.Pincode;
                    //worksheet.Cell(currentwork, 24).Value = item.AccountHolderName;
                    worksheet.Cell(currentwork, 17).Value = item.BankName;
                    worksheet.Cell(currentwork, 18).Value = item.AccountNumber;
                    //worksheet.Cell(currentwork, 27).Value = item.ReEnterAccountNumber;
                    worksheet.Cell(currentwork, 19).Value = item.Ifsc;
                    worksheet.Cell(currentwork, 20).Value = item.EpfNumber;
                    worksheet.Cell(currentwork, 21).Value = item.EmployeeContributionRate;
                    worksheet.Cell(currentwork, 22).Value = item.DeductionCycle;
                    worksheet.Cell(currentwork, 23).Value = item.AccountType;
                    worksheet.Cell(currentwork, 24).Value = item.AnnualCtc;
                    //worksheet.Cell(currentwork, 34).Value = item.Basic;
                    //worksheet.Cell(currentwork, 35).Value = item.HouseRentAllowance;
                    //worksheet.Cell(currentwork, 36).Value = item.ConveyanceAllowance;
                    //worksheet.Cell(currentwork, 37).Value = item.FixedAllowance;
                    worksheet.Cell(currentwork, 25).Value = item.Epf;
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
                        //WorkLocation = rdr["Work_Location"] == DBNull.Value ? new string[0] : ((string)rdr["Work_Location"]).Split(','),
                        WorkLocation = rdr["Work_Location"] == DBNull.Value ? null : Convert.ToString(rdr["Work_Location"]),
                        MobileNumber = rdr["Mobile_number"] == DBNull.Value ? null : Convert.ToString(rdr["Mobile_number"]),
                        AlternateNumber = (rdr["Alternate_number"] == DBNull.Value ? null : Convert.ToString(rdr["Alternate_number"])),
                        Email = rdr["Email"] == DBNull.Value ? null : Convert.ToString(rdr["Email"]),
                        GstNumber = rdr["GST_Number"] == DBNull.Value ? null : Convert.ToString(rdr["GST_Number"]),
                        BillingAddress = rdr["Billing_Address"] == DBNull.Value ? null : Convert.ToString(rdr["Billing_Address"]),
                        ProductDetails = rdr["ProductDetails"] == DBNull.Value ? null : Convert.ToString(rdr["ProductDetails"]),
                        StartDate = (DateTime)(rdr["Start_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["Start_date"])),
                        RenewDate = (DateTime)(rdr["Renew_Date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["Renew_Date"])),
                        BillingStateId = rdr["BillingStateId"] == DBNull.Value ? null : Convert.ToString(rdr["BillingStateId"]),
                        NoOfRenewMonth = rdr["NoOfRenewMonth"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["NoOfRenewMonth"]),
                        StateId = rdr["stateId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["stateId"]),
                        IsSameAddress = rdr["IsSameAddress"] == DBNull.Value ? null : Convert.ToBoolean(rdr["IsSameAddress"]),
                        //StateId = rdr["stateId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["stateId"]),
                        BillingCityId = rdr["BillingCityId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["BillingCityId"]),
                        Location = rdr["Location"] == DBNull.Value ? null : Convert.ToString(rdr["Location"]),
                        productprice = rdr["productprice"] == DBNull.Value ? null : Convert.ToString(rdr["productprice"]),
                        Renewprice = rdr["Renewprice"] == DBNull.Value ? null : Convert.ToString(rdr["Renewprice"]),
                        Igst = rdr["Igst"] == DBNull.Value ? null : Convert.ToString(rdr["Igst"]),
                        Scgst = rdr["Scgst"] == DBNull.Value ? null : Convert.ToString(rdr["Scgst"]),
                        Cgst = rdr["Cgst"] == DBNull.Value ? null : Convert.ToString(rdr["Cgst"]),
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
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", model.Id, DbType.Int32);
                parameters.Add("@Company_Name", model.CompanyName, DbType.String);
                parameters.Add("@Location", model.Location, DbType.String);
                parameters.Add("@Work_Location", model.WorkLocation, DbType.String);
                parameters.Add("@Mobile_number", model.MobileNumber, DbType.String);
                parameters.Add("@Alternate_number", model.AlternateNumber, DbType.String);
                parameters.Add("@Email", model.Email, DbType.String);
                parameters.Add("@GST_Number", model.GstNumber, DbType.String);
                parameters.Add("@Billing_Address", model.BillingAddress, DbType.String);
                parameters.Add("@Product_Details", model.ProductDetails, DbType.String);
                parameters.Add("@Start_date", model.StartDate, DbType.DateTime);
                parameters.Add("@Renew_Date", model.RenewDate, DbType.DateTime);
                parameters.Add("@BillingStateId", model.BillingStateId, DbType.Int32);
                parameters.Add("@BillingCityId", model.BillingCityId, DbType.Int32);
                parameters.Add("@stateId", model.StateId, DbType.Int32);
                parameters.Add("@Renewprice", model.Renewprice, DbType.Decimal);
                parameters.Add("@NoOfRenewMonth", model.NoOfRenewMonth, DbType.Int32);
                parameters.Add("@productprice", model.productprice, DbType.Decimal);
                parameters.Add("@SCGST", model.Scgst, DbType.Decimal);
                parameters.Add("@CGST", model.Cgst, DbType.Decimal);
                parameters.Add("@IGST", model.Igst, DbType.Decimal);
                parameters.Add("@IsSameAddress", model.IsSameAddress, DbType.Boolean);

                var result = await connection.ExecuteAsync(
                    "sp_updateCustomer_Reg",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
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
        private string GenerateDynamicUsername(string companyName)
        {
            Random rnd = new Random();
            string letters = new string(Enumerable.Range(0, 4)
                .Select(_ => (char)rnd.Next('a', 'z' + 1))
                .ToArray());
            string numbers = rnd.Next(10, 100).ToString() + rnd.Next(1000, 10000).ToString();
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
                                       WorkLocation = customer.WorkLocation,
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
        public async Task<int> UpdateCustomerProfile(CustomerRegistration model, string AddedBy)
        {
            var customer = await _context.CustomerRegistrations.FirstOrDefaultAsync(x => x.Id == model.Id);
            var adminusername = await _context.AdminLogins.FirstOrDefaultAsync(x => x.UserName == AddedBy);
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
            VendorDto cs = new VendorDto();
            try
            {
                DateTime startDate;
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("sp_GetVendorById", con);
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Convert.ToInt32(id) });
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    cs = new VendorDto()
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        CompanyName = rdr["Company_Name"] == DBNull.Value ? null : Convert.ToString(rdr["Company_Name"]),
                        //WorkLocation = rdr["Work_Location"] == DBNull.Value ? new string[0] : ((string)rdr["Work_Location"]).Split(','),
                        CityName = rdr["CityName"] == DBNull.Value ? null : Convert.ToString(rdr["CityName"]),
                        MobileNumber = rdr["Mobile_number"] == DBNull.Value ? null : Convert.ToString(rdr["Mobile_number"]),
                        AlternateNumber = (rdr["Alternate_number"] == DBNull.Value ? null : Convert.ToString(rdr["Alternate_number"])),
                        Email = rdr["Email"] == DBNull.Value ? null : Convert.ToString(rdr["Email"]),
                        GstNumber = rdr["GST_Number"] == DBNull.Value ? null : Convert.ToString(rdr["GST_Number"]),
                        BillingAddress = rdr["Billing_Address"] == DBNull.Value ? null : Convert.ToString(rdr["Billing_Address"]),
                        ProductDetails = rdr["ProductDetails"] == DBNull.Value ? null : Convert.ToString(rdr["ProductDetails"]),
                        StartDate = (DateTime)(rdr["Start_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["Start_date"])),
                        RenewDate = (DateTime)(rdr["Renew_Date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["Renew_Date"])),
                        State = rdr["State"] == DBNull.Value ? null : Convert.ToString(rdr["State"]),
                        BillingStateId = rdr["BillingStateId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["BillingStateId"]),
                        BillingCityId = rdr["BillingCityId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["BillingCityId"]),
                        IsSameAddress = rdr["IsSameAddress"] == DBNull.Value ? null : Convert.ToBoolean(rdr["IsSameAddress"]),
                        StateId = rdr["stateId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["stateId"]),
                        Location = rdr["Billing_Address"] == DBNull.Value ? null : Convert.ToString(rdr["Location"]),
                        Renewprice = rdr["Renewprice"] == DBNull.Value ? null : Convert.ToString(rdr["Renewprice"]),
                        productprice = rdr["productprice"] == DBNull.Value ? null : Convert.ToString(rdr["productprice"]),
                        NoOfRenewMonth = rdr["NoOfRenewMonth"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["NoOfRenewMonth"]),

                    };
                }
                return cs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<VendorRegResultDTO> Vendorreg(VendorDto model)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("db1"))) // Use your actual connection string here
            {
                // Generate dynamic username and password
                string dynamicUserName = GenerateDynamicUsername(model.CompanyName);
                string dynamicPassword = GenerateDynamicPassword();

                // Set up parameters for the stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@Company_Name", model.CompanyName);
                parameters.Add("@Work_Location", string.Join(",", model.CityId));
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
            List<VendorDto> cs = new List<VendorDto>();
            try
            {
                SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
                SqlCommand cmd = new SqlCommand("Vendorlist", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var cse = new VendorDto()
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        CompanyName = rdr["Company_Name"] == DBNull.Value ? null : Convert.ToString(rdr["Company_Name"]),
                        //WorkLocation = rdr["Work_Location"] == DBNull.Value ? null : new string[] { Convert.ToString(rdr["Work_Location"]) },
                        CityName = rdr["CityName"] == DBNull.Value ? "0" : Convert.ToString(rdr["CityName"]),
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
                        Location = rdr["Billing_Address"] == DBNull.Value ? null : Convert.ToString(rdr["Location"]),
                        CompanyImage = rdr["Company_Image"] == DBNull.Value ? null : Convert.ToString(rdr["Company_Image"]),
                        Isactive = rdr["Isactive"] == DBNull.Value ? null : Convert.ToBoolean(rdr["Isactive"]),

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
        }
        public async Task<VendorRegistrationDto> GetVendorProfile(string? id)
        {
            try
            {
                var query = await (from admin in _context.AdminLogins
                                   join customer in _context.VendorRegistrations
                                   on admin.Vendorid equals customer.Id
                                   where admin.Id == Convert.ToInt32(id)
                                   select new VendorRegistrationDto
                                   {
                                       Id = (int)admin.Vendorid,
                                       CompanyName = customer.CompanyName,
                                       CityId = customer.CityId,
                                       MobileNumber = customer.MobileNumber,
                                       Email = customer.Email,
                                       GstNumber = customer.GstNumber,
                                       AlternateNumber = customer.AlternateNumber,
                                       BillingAddress = customer.BillingAddress,
                                       Location = customer.Location,
                                       UserName = admin.UserName,
                                       CompanyImage = customer.CompanyImage,
                                       maplat = customer.Maplat,
                                       maplong = customer.Maplong,
                                       radious = customer.Radious,
                                       BillingCityId = customer.BillingCityId,
                                       BillingStateId = customer.BillingStateId
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
        public async Task<List<LeavemasterDto>> getLeavemaster(int Userid)
        {
            try
            {
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var result = await (from lm in _context.Leavemasters
                                    join lt in _context.LeaveTypes
                                    on lm.LeavetypeId equals lt.Id
                                    join emp in _context.EmployeeRegistrations
                                   on lm.EmpId equals emp.EmployeeId
                                    where emp.Vendorid == adminlogin.Vendorid
                                    select new LeavemasterDto
                                    {
                                        id = lm.Id,
                                        LeavetypeId = lt.Leavetype1,
                                        Value = lm.Value,
                                        EmpId = lm.EmpId,
                                        Createddate = lm.Createddate,
                                        IsActive = lm.IsActive
                                    }).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //Vendor Product
        public async Task<int> AddVendorProduct(VendorProductMaster model,int VendorId)
        {
            if(model.Id==0)
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
                                }).ToListAsync();

            return result;
        }

        public async Task<bool> AddVendorCategory(VendorCategoryMaster model,int VendorId)
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
                    existdata.CategoryName=model.CategoryName;
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
    }

}