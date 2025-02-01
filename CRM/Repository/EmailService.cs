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
using DocumentFormat.OpenXml.Spreadsheet;
using CRM.Controllers;


namespace CRM.Repository
{
    public class EmailService : IEmailService
    {
        private admin_NDCrMContext _context;

        public EmailService(admin_NDCrMContext context)
        {
            _context = context;

        }
        //Send all Employeeletters
        public async Task SendEmailAsync(string toEmail, string subject, string body, byte[] filecontent, string filename, string mimetype, int vendorid)
        {
            try
            {
                var vendordetails = _context.VendorRegistrations.Where(x => x.Id == vendorid).FirstOrDefault();
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(vendordetails.CompanyName, "care@ndtechland.com"));
                emailMessage.To.Add(new MailboxAddress("Recipient Name", toEmail));
                emailMessage.ReplyTo.Clear();
                emailMessage.ReplyTo.Add(new MailboxAddress("", vendordetails.Email));
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

                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //EmployeeLoginCredentials
        public async Task SendEmailCred(EmpMultiform model, string password, int? UserId)
        {
            try
            {
                var vendordetails = _context.VendorRegistrations.Where(x => x.Id == UserId).FirstOrDefault();
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(vendordetails.CompanyName, "care@ndtechland.com"));
                emailMessage.To.Add(new MailboxAddress("Recipient Name", model.PersonalEmailAddress));
                emailMessage.ReplyTo.Clear();
                emailMessage.ReplyTo.Add(new MailboxAddress("", vendordetails.Email));
                emailMessage.Subject = "Your Employee Login Credentials";
                var body = $@"
        Dear {model.FirstName} {model.LastName},
        Welcome to {vendordetails.CompanyName}! Below are your login credentials to access your employee account:
        Username: {model.EmployeeId}
        Password: {password}
        You can log in via the web or use our mobile app for easy access. Download the N D Connect app from the links below:
        Google Play Store: https://play.google.com/store/apps/details?id=com.ndconnect.nd_connect_techland&pcampaignid=web_share
        Apple App Store: https://play.google.com/store/apps/details?id=com.ndconnect.nd_connect_techland&pcampaignid=web_share
        Please use these credentials to log in to your employee portal and manage your profile, attendance, and other related features.
        Best regards,
        {vendordetails.CompanyName}
        Phone: {vendordetails.MobileNumber}
        Email: {vendordetails.Email}";

                var textPart = new TextPart("plain")
                {
                    Text = body
                };

                var multipart = new Multipart("mixed");
                multipart.Add(textPart);

                emailMessage.Body = multipart;

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //VendorRegistration
        public async Task SendEmailCredentials(string toEmail, string CompanyName, string username, string password)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("N D Techland Private Limited", "care@ndtechland.com"));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = "Welcome to N D Connect!";

            emailMessage.Body = new TextPart("html")
            {
                Text = $@"
        <p>Dear {CompanyName},</p>
        <p>Welcome to N D Connect!</p>
        <p>We’re thrilled to have you on board. At N D Connect, we’re committed to helping you streamline your operations and achieve your business goals. Our platform provides comprehensive solutions, from customer management and invoicing to employee tracking and more.</p>
        
        <h3>Your Account Details:</h3>
        <ul>
            <li>Username: {username}</li>
            <li>Password: {password}</li>
        </ul>
        
        <h3>Getting Started:</h3>
        <ul>
            <li><strong>Log In:</strong> Access your account and explore our features using your credentials.</li>
            <li><strong>Support:</strong> If you have any questions or need assistance, don’t hesitate to contact us at 0120-4354103 or email us at <a href='mailto:customer@ndtechland.com'>customer@ndtechland.com</a>.</li>
        </ul>
        
        <p>Thank you for choosing N D Connect. We look forward to working with you and supporting your success.</p>
        <p>Best regards,</p>
        <p>N D Techland Private Limited<br />
        Phone: 0120-4354103<br />"
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    // Connect to your SMTP server
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");
                    await client.SendAsync(emailMessage);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        public async Task EmpRandomPasswordSendEmailAsync(ForgotPassword model, string newPassword)
        {
            try
            {
                var employee = await _context.EmployeeRegistrations.FirstOrDefaultAsync(x => x.WorkEmail == model.Email);
                var companyname = await _context.VendorRegistrations.Where(x => x.Id == employee.Vendorid).FirstOrDefaultAsync();
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(companyname.CompanyName, "care@ndtechland.com"));
                emailMessage.To.Add(new MailboxAddress("Recipient Name", model.Email));
                emailMessage.ReplyTo.Clear();
                emailMessage.ReplyTo.Add(new MailboxAddress("", companyname.Email));
                emailMessage.Subject = ""+ companyname + " - Password Reset";
                var message = "<p><strong>Hi :</strong> " + employee.EmployeeId + "</p>" +
                              "<p>You have successfully reset your password. Your temporary password is: " +
                              "<strong style='color: black ;'>" + newPassword + "</strong></p>" +
                              "<p><strong>Please log in and change it as soon as possible for security reasons.</strong></p>";
                var textPart = new TextPart("html")
                {
                    Text = message
                };

                var multipart = new Multipart("mixed");
                multipart.Add(textPart);

                emailMessage.Body = multipart;

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //CustomerInvoice
        public async Task SendInvoicePdfEmail(string toEmail, string body, byte[] filecontent, string filename, string mimetype, int vendorid)
        {
            try
            {
                var companyname = await _context.VendorRegistrations.Where(x => x.Id == vendorid).FirstOrDefaultAsync();

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(companyname.CompanyName, "care@ndtechland.com"));
                emailMessage.To.Add(new MailboxAddress("Recipient Name", toEmail));
                emailMessage.ReplyTo.Clear();
                emailMessage.ReplyTo.Add(new MailboxAddress("", companyname.Email));
                emailMessage.Subject = "Invoice Pdf";

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
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task SendEmpLeaveApprovalEmailAsync(string ToEmpEmail, string FirstName, string MiddleName, string LastName, string Subject, string emailBody, int vendorid)
        {
            var companyname = await _context.VendorRegistrations.Where(x => x.Id == vendorid).FirstOrDefaultAsync();

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(companyname.CompanyName, "care@ndtechland.com"));
            emailMessage.To.Add(new MailboxAddress("Recipient Name", ToEmpEmail));
            emailMessage.ReplyTo.Clear();
            emailMessage.ReplyTo.Add(new MailboxAddress("", companyname.Email));
            emailMessage.Subject = Subject;

            emailMessage.Body = new TextPart("html")
            {
                Text = emailBody
            };

            using (var client = new SmtpClient())
            {
                try
                {

                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");

                    await client.SendAsync(emailMessage);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
        public async Task SendMeetEmailAsync(string ToEmpEmail, string FirstName, string MiddleName, string LastName, string emailBody, int vendorid)
        {
            var companyname = await _context.VendorRegistrations.Where(x => x.Id == vendorid).FirstOrDefaultAsync();

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(companyname.CompanyName, "care@ndtechland.com"));
            emailMessage.To.Add(new MailboxAddress("Recipient Name", ToEmpEmail));
            emailMessage.ReplyTo.Clear();
            emailMessage.ReplyTo.Add(new MailboxAddress("", companyname.Email));
            emailMessage.Subject = "Join Our Meeting";

            emailMessage.Body = new TextPart("html")
            {
                Text = emailBody
            };

            using (var client = new SmtpClient())
            {
                try
                {

                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");

                    await client.SendAsync(emailMessage);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
        public async Task CustomerWelcomeEmail(string toEmail, string CompanyName, int VendorId)
        {
            var companyname = await _context.VendorRegistrations.Where(x => x.Id == VendorId).FirstOrDefaultAsync();

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(companyname.CompanyName, "care@ndtechland.com"));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.ReplyTo.Clear();
            emailMessage.ReplyTo.Add(new MailboxAddress("", companyname.Email));
            emailMessage.Subject = $"Welcome to {companyname.CompanyName}!";

            emailMessage.Body = new TextPart("html")
            {
                Text = $@"
<p>Dear {CompanyName},</p>
<p>Welcome to {companyname.CompanyName}!</p>
<p>We’re thrilled to have you on board. At {companyname.CompanyName}, we’re committed to helping you streamline your operations and achieve your business goals. Our platform provides comprehensive solutions, from customer management and invoicing to employee tracking and more.</p>

<p>Thank you for choosing {companyname.CompanyName}. We look forward to working with you and supporting your success.</p>
<p>Best regards,</p>
<p>{companyname.CompanyName}<br />
Phone: {companyname.MobileNumber}<br />"
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    // Connect to your SMTP server
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");

                    await client.SendAsync(emailMessage);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
        public async Task CustomerRenewalEmail(string toEmail, string CompanyName, string RenewalDate, string productname, decimal productAmount, int VendorId)
        {
            var vendorrecods = _context.VendorRegistrations.Where(x => x.Id == VendorId).FirstOrDefault();
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(vendorrecods.CompanyName, "care@ndtechland.com"));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.ReplyTo.Clear();
            emailMessage.ReplyTo.Add(new MailboxAddress("", vendorrecods.Email));
            emailMessage.Cc.Add(new MailboxAddress("", vendorrecods.Email));

            emailMessage.Subject = $"Reminder: Upcoming Renewal for Your {vendorrecods.CompanyName} Product";

            emailMessage.Body = new TextPart("html")
            {
                Text = $@"
        <p>Dear {CompanyName},</p>
        <p>This is a friendly reminder that your product subscription with {vendorrecods.CompanyName} is approaching its renewal date on <strong>{RenewalDate}</strong>.
        <br/>To ensure uninterrupted service and continue enjoying our comprehensive features, please complete your renewal before the due date.</p>

        <p><strong>Subscription Details:</strong></p>
        <ul>
            <li><strong>Product Name:</strong> {productname}</li>
            <li><strong>Amount:</strong> {productAmount}</li>
            <li><strong>Renewal Date:</strong> {RenewalDate}</li>
        </ul>

        <p>If you need any assistance with the renewal process or have any questions, please feel free to contact us at <strong>{vendorrecods.MobileNumber}</strong>. We’re here to help!</p>

        <p>Thank you for your continued support and for choosing {vendorrecods.CompanyName}.</p>

        <p>Best regards,</p>
        <p><strong>{vendorrecods.CompanyName}</strong><br />
        Phone: <strong>{vendorrecods.MobileNumber} </strong>"
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    // Connect to your SMTP server
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");

                    await client.SendAsync(emailMessage);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        public async Task CustomerExpirEmail(string toEmail, string CompanyName, string ExpirationDate, string productName, int VendorId)
        {
            var vendorrecods = _context.VendorRegistrations.Where(x => x.Id == VendorId).FirstOrDefault();

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(vendorrecods.CompanyName, "care@ndtechland.com"));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.ReplyTo.Clear();
            emailMessage.ReplyTo.Add(new MailboxAddress("", vendorrecods.Email));
            emailMessage.Cc.Add(new MailboxAddress("", vendorrecods.Email));
            emailMessage.Subject = $"Action Required: Your Product {productName} Has Expired";

            emailMessage.Body = new TextPart("html")
            {
                Text = $@"
<p>Dear {CompanyName},</p>
<p>We hope this message finds you well.</p>
<p>This is a reminder that your product, <strong>{productName}</strong>, expired on <strong>{ExpirationDate}</strong>. 
<br/>To ensure you continue receiving uninterrupted service, please take the necessary action to renew or replace the product.</p>

<p><strong>Product Details:</strong></p>
<ul>
    <li><strong>Product Name:</strong> {productName}</li>
    <li><strong>Expiration Date:</strong> {ExpirationDate}</li>
</ul>

<p>If you need assistance with renewal or have any questions, please feel free to contact us at <strong>{vendorrecods.MobileNumber}</strong>. We’re here to help!</p>

<p>Thank you for your prompt attention to this matter.</p>

<p>Best regards,</p>
<p><strong>{vendorrecods.CompanyName}</strong><br />
        Phone: <strong>{vendorrecods.MobileNumber} </strong>"
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    // Connect to your SMTP server
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");

                    await client.SendAsync(emailMessage);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        public async Task SendVendorInvoiceEmailAsync(string toEmail, string subject, string body, byte[] filecontent, string filename, string mimetype)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("N D Techland Private Limited", "care@ndtechland.com"));
                emailMessage.To.Add(new MailboxAddress("Recipient Name", toEmail));

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

                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("care@ndtechland.com", "yetztyfbqlfvknyg");
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

