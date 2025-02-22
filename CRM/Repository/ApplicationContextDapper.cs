using System.Data;
using System.Data.SqlClient;

namespace CRM.Repository
{
    public class ApplicationContextDapper
    {
     public IDbConnection CreateConnection() => new SqlConnection("Server=103.154.184.118;Database=Jobforindia_HireJob;Persist Security Info=True;User ID=Jobforindia_HireJob;Password=Job@#123456#@$;MultipleActiveResultSets=True;TrustServerCertificate=true;");
    }
}
