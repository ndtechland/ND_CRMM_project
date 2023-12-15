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
using System.Reflection;
using System;
using System.Reflection.Metadata;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace CRM.Repository
{
    public class EmailService : IEmailService
    {
        private admin_NDCrMContext _context;

        public EmailService(admin_NDCrMContext context)
        {
            _context = context;

        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Anchal Shukla", "anchalshukla7060153412@gmail.com"));
                emailMessage.To.Add(new MailboxAddress("Recipient Name", toEmail));
                emailMessage.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    
                    await client.AuthenticateAsync("anchalshukla7060153412@gmail.com", "A@1234anchal");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            }


    }

}

