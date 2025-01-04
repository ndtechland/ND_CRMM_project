using CRM.Controllers;
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
        Task SendEmailAsync(string toEmail, string subject, string body, byte[] filecontent, string filename, string mimetype, int vendorid);
        Task SendEmailCred(EmpMultiform model,string password,int? UserId);
        Task SendEmailCredentials(string toEmail,string CompanyName, string username, string password);
        Task EmpRandomPasswordSendEmailAsync(ForgotPassword model, string newPassword);
        Task SendInvoicePdfEmail(string toEmail, string body, byte[] filecontent, string filename, string mimetype, int vendorid);
        Task SendEmpLeaveApprovalEmailAsync(string ToEmpEmail, string FirstName, string MiddleName, string LastName, string Subject, string emailBody, int vendorid);
        Task SendMeetEmailAsync(string ToEmpEmail, string FirstName, string MiddleName, string LastName, string emailBody, int vendorid);
        Task CustomerWelcomeEmail(string toEmail, string CompanyName,int VendorId);
        Task CustomerRenewalEmail(string toEmail, string CompanyName, string RenewalDate,string productname, decimal productAmount,int VendorId);
        Task CustomerExpirEmail(string toEmail, string CompanyName, string RenewalDate,string productname, int VendorId);
        Task SendVendorInvoiceEmailAsync(string toEmail, string subject, string body, byte[] filecontent, string filename, string mimetype);

    }
}
