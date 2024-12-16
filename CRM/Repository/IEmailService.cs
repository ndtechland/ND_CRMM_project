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
        Task SendEmailCred(EmpMultiform model,string password,int? UserId);
        Task SendEmailCredentials(string toEmail,string CompanyName, string username, string password);
        Task EmpRandomPasswordSendEmailAsync(ForgotPassword model, string newPassword);
        Task SendInvoicePdfEmail(string toEmail, string body, byte[] filecontent, string filename, string mimetype);
        Task SendEmpLeaveApprovalEmailAsync(string ToEmpEmail, string FirstName, string MiddleName, string LastName, string Subject, string emailBody);
        Task SendMeetEmailAsync(string ToEmpEmail, string FirstName, string MiddleName, string LastName, string emailBody);
        Task CustomerWelcomeEmail(string toEmail, string CompanyName);
        Task CustomerRenewalEmail(string toEmail, string CompanyName, string RenewalDate,string productname, decimal productAmount);
        Task CustomerExpirEmail(string toEmail, string CompanyName, string RenewalDate,string productname);

    }
}
