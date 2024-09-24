using CRM.Models.Crm;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Utilities
{
    public class Encrypt
    {
        public string EncryptPassword(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
    }
}
