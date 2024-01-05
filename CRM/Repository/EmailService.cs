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
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using DocumentFormat.OpenXml.Vml;


namespace CRM.Repository
{
    public class EmailService : IEmailService
    {
        private admin_NDCrMContext _context;

        public EmailService(admin_NDCrMContext context)
        {
            _context = context;

        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, byte[] filecontent, string filename, string mimetype)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Anchal Shukla", "sanchalanchal69@gmail.com"));
                emailMessage.To.Add(new MailboxAddress("Recipient Name", toEmail));
                emailMessage.Cc.Add(new MailboxAddress("Recipient Name", "ndcaretrust@gmail.com"));
                emailMessage.Bcc.Add(new MailboxAddress("Recipient Name", "ndinfotechnoida@gmail.com"));
                emailMessage.Subject = subject;

                var textPart = new TextPart("plain")
                {
                    Text = body
                };

                var attachment = new MimePart(mimetype)
                {
                    Content = new MimeContent(new MemoryStream(filecontent), ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = filename
                };

                var multipart = new Multipart("mixed");
                multipart.Add(textPart);
                multipart.Add(attachment);

                emailMessage.Body = multipart;

                using (var client = new SmtpClient())
                {

                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("sanchalanchal69@gmail.com", "zommitmpkowdhkei");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }

}

