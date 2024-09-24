using CRM.Models.Crm;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Utilities
{
    public class Dcrypt
    {
        public string DecryptPassword(string encodedData)
        {
            try
            {
                byte[] todecode_bytes = Convert.FromBase64String(encodedData);
                string decodedString = Encoding.UTF8.GetString(todecode_bytes);
                return decodedString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error decoding password: " + ex.Message);
                return null;
            }
        }
        public string GenerateRandomPassword()
        {
            int length = 12;
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=";
            StringBuilder password = new StringBuilder();
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[sizeof(uint)];
                while (length-- > 0)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    password.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }
            return password.ToString();
        }
    }
}
