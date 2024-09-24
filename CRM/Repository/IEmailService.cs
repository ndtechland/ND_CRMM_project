using CRM.Models.Crm;
using CRM.Models.CRM;
using CRM.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CRM.Repository
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, byte[] filecontent, string filename, string mimetype);
        Task SendEmailCred(EmpMultiform model,string password);
        Task SendEmailCredentials(string toEmail,string CompanyName, string username, string password);
        Task EmpRandomPasswordSendEmailAsync(ForgotPassword model, string newPassword, string userId);


    }
}
