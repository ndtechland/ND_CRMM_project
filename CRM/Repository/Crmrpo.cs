using CRM.Models.Crm;
using CRM.Models.CRM;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM.Models.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Primitives;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.DependencyResolver;


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
        public async Task<int> Customer(CustomerRegistration model)
        {
            
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@Company_Name", model.CompanyName));
            parameter.Add(new SqlParameter("@Contact_person_name", model.ContactPersonName));
            parameter.Add(new SqlParameter("@Mobile_number", model.MobileNumber));
            parameter.Add(new SqlParameter("@Alternate_number", model.AlternateNumber));
            parameter.Add(new SqlParameter("@Email", model.Email));
            parameter.Add(new SqlParameter("@GST_Number", model.GstNumber));
            parameter.Add(new SqlParameter("@Billing_Address", model.BillingAddress));
            parameter.Add(new SqlParameter("@Product_Details", model.ProductDetails));
            //parameter.Add(new SqlParameter("@GenrateInvoice", model.GenrateInvoice));
            parameter.Add(new SqlParameter("@Start_date", model.StartDate));
            parameter.Add(new SqlParameter("@Renew_Date", model.RenewDate));

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec CustomerRegistration @Company_Name, @Contact_person_name,@Mobile_number,@Alternate_number,@Email,@GST_Number,@Billing_Address,@Product_Details,@Start_date,@Renew_Date", parameter.ToArray()));

            return result;
        }
        public async Task<List<CustomerRegistration>> CustomerList()
        {
           var result = await _context.CustomerRegistrations.FromSqlRaw<CustomerRegistration>("Customerlist").ToListAsync();
           return result;
        }
        public async Task<int> EmpRegistration(EmployeeRegistration model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@FirstName", model.FirstName));
            parameter.Add(new SqlParameter("@MiddleName", model.MiddleName));
            parameter.Add(new SqlParameter("@LastName", model.LastName));
            parameter.Add(new SqlParameter("@DateOfJoining", DateTime.Now));
            parameter.Add(new SqlParameter("@WorkEmail", model.WorkEmail));
            parameter.Add(new SqlParameter("@GenderID", model.GenderId));
            parameter.Add(new SqlParameter("@WorkLocationID", model.WorkLocationId));
            parameter.Add(new SqlParameter("@DesignationID", model.DesignationId));
            parameter.Add(new SqlParameter("@DepartmentID", model.DepartmentId));
            parameter.Add(new SqlParameter("@IsDeleted", "0"));
            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec EmployeeRegistration @FirstName, @MiddleName,@LastName,@DateOfJoining,@WorkEmail,@GenderID,@WorkLocationID,@DesignationID,@DepartmentID,@IsDeleted", parameter.ToArray()));

            return result;
        }
        public async Task<List<StateMaster>> GetAllState()
        {
            return await _context.StateMasters.ToListAsync();
        }
        public async Task<int> EmployeeBasicinfo(EmployeePersonalDetail model)
        {
            {
                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@Action", 1));
                parameter.Add(new SqlParameter("@ID", model.Id));
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
                parameter.Add(new SqlParameter("@IsDeleted", "0"));
                var result = await Task.Run(() => _context.Database
               .ExecuteSqlRawAsync(@"exec sp_Employee_Personal_Details  @Action,@ID,@Personal_Email_Address,
            @Mobile_Number,@Date_Of_Birth,@Father_Name,@PAN,@Address_Line_1,
              @Address_Line_2,@City,@State_ID,@Pincode,@IsDeleted", parameter.ToArray()));
                return result;
            }
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
        public async Task<List<EmployeeRegistration>> EmployeeList()
        {
            List<EmployeeRegistration> emp = new List<EmployeeRegistration>();
            SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
            SqlCommand cmd = new SqlCommand("EmployeeRegistrationList", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var emps = new EmployeeRegistration()
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    FirstName = Convert.ToString(rdr["FirstName"]),                    
                   MiddleName= Convert.ToString(rdr["MiddleName"]),
                    LastName = Convert.ToString(rdr["LastName"]),
                    EmployeeId = Convert.ToString(rdr["EmployeeId"]),
                    DateOfJoining =((DateTime)rdr["DateOfJoining"]),                   
                    WorkEmail = Convert.ToString(rdr["WorkEmail"]),
                    GenderId = Convert.ToString(rdr["GenderId"]),
                    WorkLocationId = Convert.ToString(rdr["WorkLocationId"]),
                    DesignationId = Convert.ToString(rdr["DesignationId"]),
                    DepartmentId = Convert.ToString(rdr["DepartmentId"]),

                };

                emp.Add(emps);
            }
            return (emp);

        }

        public async Task<List<EmployeePersonalDetail>> EmployeeBasicinfoList()
        {
            List<EmployeePersonalDetail> emp = new List<EmployeePersonalDetail>();
            SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
            SqlCommand cmd = new SqlCommand("EmployeeBasicinfoList", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var emps = new EmployeePersonalDetail()
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    PersonalEmailAddress = Convert.ToString(rdr["PersonalEmailAddress"]),
                    MobileNumber = Convert.ToString(rdr["MobileNumber"]),
                    DateOfBirth = (DateTime)(rdr["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(rdr["DateOfBirth"]) : (DateTime?)null),
                    Age = Convert.ToInt32(rdr["Age"]),
                    FatherName = Convert.ToString(rdr["FatherName"]),
                    Pan = Convert.ToString(rdr["Pan"]),
                    AddressLine1 = Convert.ToString(rdr["AddressLine1"]),
                    AddressLine2 = Convert.ToString(rdr["AddressLine2"]),
                    City = Convert.ToString(rdr["City"]),
                    StateId = Convert.ToString(rdr["StateId"]),
                    Pincode = Convert.ToString(rdr["Pincode"]),

                };

                emp.Add(emps);
            }
            return (emp);
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
        
        public async Task<int> updateEmployee(EmployeeRegistration model)
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

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec sp_updateEmpRegistration @Id,@FirstName, @MiddleName,@LastName,@DateOfJoining,@WorkEmail,@GenderID,@WorkLocationID,@DesignationID,@DepartmentID", parameter.ToArray()));

            return result;
        }


        public async Task<int> Quation(Quation model)
        {

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@Company_Name", model.CompanyName));
            parameter.Add(new SqlParameter("@Customer_Name", model.CustomerName));
            parameter.Add(new SqlParameter("@Email", model.Email));
            parameter.Add(new SqlParameter("@Sales_Person_Name", model.SalesPersonName));
            parameter.Add(new SqlParameter("@Product_ID", model.ProductId));
            parameter.Add(new SqlParameter("@Subject", model.Subject));
            parameter.Add(new SqlParameter("@Amount", model.Amount));
            parameter.Add(new SqlParameter("@Mobile", model.Mobile));
           

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec SP_Quation @Company_Name, @Customer_Name,@Email,@Sales_Person_Name,@Product_ID,@Subject,@Amount,@Mobile", parameter.ToArray()));

            return result;


           
        }


        public async Task<List<Quation>> QuationList()
        {
            var result = await _context.Quations.FromSqlRaw<Quation>("QuationList").ToListAsync();
            return result;
        }

       

    }

}

