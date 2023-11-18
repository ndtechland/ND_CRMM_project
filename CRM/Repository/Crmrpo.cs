using CRM.Models.Crm;
using CRM.Models.CRM;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CRM.Repository
{
    public class Crmrpo : ICrmrpo
    {
        private admin_NDCrMContext _context;
        public Crmrpo(admin_NDCrMContext context)
        {
            _context = context;
        }

        //public async Task<int> Login(AdminLogin model)
        //{
        //    var parameter = new List<SqlParameter>();
        //    parameter.Add(new SqlParameter("@UserName", model.UserName));
        //    parameter.Add(new SqlParameter("@password", model.Password));


        //    var result = await Task.Run(() => _context.Database
        //   .ExecuteSqlRawAsync(@"exec usp_adminlogin @UserName, @password", parameter.ToArray()));

        //    return result;
        //}
        public DataTable Login(AdminLogin model)
        {
            SqlConnection con = new SqlConnection(_context.Database.GetConnectionString());
            SqlCommand cmd = new SqlCommand("usp_adminlogin",con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@UserName", model.UserName));
            cmd.Parameters.Add(new SqlParameter("@password", model.Password));
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt= new DataTable();
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


            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec sp_Addproduct @ProductName, @Category,@HSN_SAC_Code,@GST,@Price", parameter.ToArray()));

            return result;
        }

        public async Task<List<ProductMaster>> ProductList()
        {
            return await _context.ProductMasters
                .FromSqlRaw<ProductMaster>("sp_Productlist")
                .ToListAsync();
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
            parameter.Add(new SqlParameter("@Start_date", model.StartDate));
            parameter.Add(new SqlParameter("@Renew_Date", model.RenewDate));

            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec CustomerRegistration @Company_Name, @Contact_person_name,@Mobile_number,@Alternate_number,@Email,@GST_Number,@Billing_Address,@Product_Details,@Start_date,@Renew_Date", parameter.ToArray()));

            return result;
        }
        public async Task<List<CustomerRegistration>> CustomerList()
        {
            return await _context.CustomerRegistrations
                .FromSqlRaw<CustomerRegistration>("Customerlist")
                .ToListAsync();
        }



        public async Task<int> Banner(BannerMaster model)
        {
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@BannerImage", model.BannerImage));
            parameter.Add(new SqlParameter("@Bannerdescription", model.Bannerdescription));
            parameter.Add(new SqlParameter("@BannerPath", model.BannerPath));
            parameter.Add(new SqlParameter("@AddedBy", model.AddedBy));
            var result = await Task.Run(() => _context.Database
           .ExecuteSqlRawAsync(@"exec Sp_Banner @BannerImage,@Bannerdescription,@BannerPath,@AddedBy", parameter.ToArray()));
            return result;
        }
       
    }
}
