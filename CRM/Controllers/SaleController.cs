﻿using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using DinkToPdf;
using DinkToPdf.Contracts;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using SelectPdf;
using System.Text;

namespace CRM.Controllers
{
    public class SaleController : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;
        private readonly IEmailService _IEmailService;
        private readonly IConverter _converter;

        public SaleController(admin_NDCrMContext context, ICrmrpo ICrmrpo, IEmailService iEmailService, IConverter _IConverter)
        {
            _context = context;
            _ICrmrpo = ICrmrpo;
            _IEmailService = iEmailService;
            _converter = _IConverter;

        }
        [HttpGet]
        public async Task<IActionResult> Invoice(string InvoiceNumber ,bool clone=false)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {

                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    CustomerInvoiceDTO customerInv = new CustomerInvoiceDTO();
                    var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();

                    ViewBag.checkvendorbillingstateid = _context.VendorRegistrations.Where(v => v.Id == adminlogin.Vendorid).FirstOrDefault().BillingStateId;
                    var Invoicedetail = _context.CustomerInvoicedetails.Where(x => x.InvoiceNumber == InvoiceNumber).FirstOrDefault();
                    DateTime Invoicedate = Invoicedetail?.InvoiceDate ?? DateTime.Now.Date;
                    DateTime InvoiceDuedate = Invoicedetail?.InvoiceDueDate ?? DateTime.Now.Date;
                    var notes = Invoicedetail?.Notes ?? null;
                    var Terms = Invoicedetail?.Terms ?? null;
                   
                    if (InvoiceNumber != null)
                    {
                        if(clone == true)
                        {
                            ViewBag.Heading = "Add Clone Invoice";
                            ViewBag.btnText = "Add Clone";
                        }
                        else
                        {
                            ViewBag.Heading = "Update Invoice";
                            ViewBag.btnText = "Update";
                        }
                        
                        customerInv.customerInvoice = await _context.CustomerInvoices.Where(c => c.InvoiceNumber == InvoiceNumber).ToListAsync();
                        var data = customerInv.customerInvoice.FirstOrDefault();
                        if (customerInv.customerInvoice != null && customerInv.customerInvoice.Count() > 0)
                        {

                            ViewBag.ProductDetails = _context.VendorProductMasters.Where(c => c.IsActive == true)
                                .Select(p => new SelectListItem
                                {
                                    Value = p.Id.ToString(),
                                    Text = p.ProductName,
                                }).ToList();

                            ViewBag.id = data.Id;
                            ViewBag.CustomerId = data.CustomerId;
                            ViewBag.CustomerName = _context.CustomerRegistrations.Where(x => x.Id == data.CustomerId).First().CompanyName;
                            ViewBag.InvoiceNumber = InvoiceNumber;
                            ViewBag.Invoicedate = Invoicedate.ToString("yyyy-MM-dd");
                            ViewBag.InvoiceDuedate = InvoiceDuedate.ToString("yyyy-MM-dd");
                            ViewBag.Notes = notes;
                            ViewBag.Terms = Terms;
                            ViewBag.clone = clone;
                            return View(customerInv);
                        }

                    }

                    ViewBag.Heading = "Add Invoice";
                    ViewBag.btnText = "SAVE";
                    ViewBag.ProductId = null;
                    ViewBag.Price = 0;
                    ViewBag.RenewPrice = null;
                    ViewBag.NoOfRenewMonth = null;
                    ViewBag.HsnSacCode = null;
                    ViewBag.StartDate = null;
                    ViewBag.RenewDate = null;
                    ViewBag.IGST = 0;
                    ViewBag.SGST = 0;
                    ViewBag.CGST = 0;
                    ViewBag.Dueamountdate = null;
                    ViewBag.Invoicedate = Invoicedate.ToString("yyyy-MM-dd"); 
                    ViewBag.InvoiceDuedate = InvoiceDuedate.ToString("yyyy-MM-dd");
                    ViewBag.Notes = null;
                    ViewBag.Terms = null;
                    ViewBag.InvoiceNumber = InvoiceNumber;
                    ViewBag.clone = clone;
                    ViewBag.ProductDetails = _context.VendorProductMasters.Where(c => c.IsActive == true)
                     .Select(p => new SelectListItem
                     {
                         Value = p.Id.ToString(),
                         Text = p.ProductName,
                     })
                     .ToList();
                    return View(customerInv);
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Invoice(List<ProductDetail> model, DateTime? InvoiceDate = null, DateTime? InvoiceDueDate = null, string InvoiceNotes = null, string InvoiceTerms = null, string Invoiceclone = null)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);
                bool isclone = Convert.ToBoolean(Invoiceclone);
                if (adminlogin == null)
                {
                    TempData["Message"] = "Admin not found.";
                    return View(model);
                }

                var customerId = model.FirstOrDefault()?.CustomerId;

                if (customerId == null)
                {
                    TempData["Message"] = "CustomerId not found.";
                    return View(model);
                }

                var checkInvoice = await _context.CustomerInvoices
                                                  .Where(x => x.CustomerId == customerId)
                                                  .ToListAsync();
                string InvoiceNo = model.FirstOrDefault()?.InvoiceNumber ?? null;

                bool isSuccess = await _ICrmrpo.CustomerInvoice(model, InvoiceNo, (int)adminlogin.Vendorid, InvoiceDate, InvoiceDueDate,InvoiceNotes,InvoiceTerms, Invoiceclone);

                if (isSuccess)
                {

                    foreach (var product in model)
                    {
                        if (product.Id == 0)
                        {
                            TempData["Message"] = "ok";
                        }
                        else
                        {
                            TempData["Message"] = "updok";
                        }
                    }

                    var model1 = new
                    {
                        path = "/Sale/Invoice"
                    };
                    return Ok(model1);
                }
                else
                {
                    TempData["Message"] = "Failed to process invoice.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "An error occurred while processing the invoice.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult GetCustomerNames(string searchTerm)
        {
            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();

            var customers = _context.CustomerRegistrations
                                    .Where(c => c.CompanyName.Contains(searchTerm) && c.Vendorid == adminlogin.Vendorid)
                                    .Select(c => new
                                    {
                                        Id = c.Id,
                                        CompanyName = c.CompanyName
                                    })
                                    .ToList();

            return Json(customers);
        }

        [HttpGet]
        public IActionResult GetCustomerDetailsById(int id, bool clone = false,int InvoiceID = 0)
        {
            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
            var InvoiceDetail = _context.CustomerInvoices.Where(x =>x.Id == InvoiceID).FirstOrDefault();
            var existingInvoice = new CustomerInvoice();
            if (InvoiceDetail != null)
            {
                 existingInvoice = _context.CustomerInvoices.Where(x => x.CustomerId == id && x.InvoiceNumber == InvoiceDetail.InvoiceNumber).FirstOrDefault();

            }
            string InvoiceNo = null;
            if (clone == true)
            {
                InvoiceNo = GenerateInvoiceNumber();
            }
            else if(clone == false)
            {
                InvoiceNo = existingInvoice.InvoiceNumber;
            }
            else
            {
                InvoiceNo = GenerateInvoiceNumber();
            }
            var customer = (from c in _context.CustomerRegistrations
                            join so in _context.States on c.StateId equals so.Id
                            join sb in _context.States on c.BillingStateId equals sb.Id
                            join co in _context.Cities on c.CityId equals co.Id
                            join cb in _context.Cities on c.BillingCityId equals cb.Id
                            where c.Id == id && c.Vendorid == adminlogin.Vendorid
                            select new
                            {
                                BillingAddress = c.BillingAddress,
                                Location = c.Location,
                                OfficeState = so.SName,
                                OfficeCity = co.City1,
                                BillingState = sb.SName,
                                BillingStateId = c.BillingStateId,
                                MobileNumber = c.MobileNumber,
                                Email = c.Email,
                                GstNumber = c.GstNumber,
                                BillingCity = cb.City1,
                                InvoiceNumber = InvoiceNo,
                            }).FirstOrDefault();

            return Json(customer);
        }

        public async Task<IActionResult> CustomerInvoiceList()
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                ViewBag.Paymentmode = _context.Paymentmodes.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PaymentType
                }).ToList();
                List<CustomerInvoiceDTO> data = await _ICrmrpo.GetCustometInvoiceList((int)adminlogin.Vendorid);
                foreach (var item in data)
                {
                    //ViewBag.totalamount = await CalculateTotalAmountByInvoiceId(item.InvoiceNumber);
                    ViewBag.PaymentTypelist = item.Paymentid;
                    //ViewBag.dueamout = item.DueAmount;

                }
                return View(data);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DeleteCustomerInvoice(string InvoiceNumber)
        {
            try
            {
                var dlt = await _context.CustomerInvoices.Where(c => c.InvoiceNumber == InvoiceNumber).ToListAsync();

                if (dlt.Any())
                {
                    _context.CustomerInvoices.RemoveRange(dlt);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Deleted Successfully.";
                }
                else
                {
                    TempData["Message"] = "No invoices found for deletion.";
                }

                return RedirectToAction("CustomerInvoiceList");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                TempData["Error"] = $"Error deleting invoices: {ex.Message}";
                return RedirectToAction("CustomerInvoiceList");
            }
        }
        private string GenerateInvoiceNumber()
        {
            

            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();

            var data = _context.CustomerInvoices.Where(x =>x.VendorId == adminlogin.Vendorid)
                              .OrderByDescending(a => a.InvoiceNumber)
                              .FirstOrDefault();
            // Initialize variables
            string invoiceid = string.Empty;
            int numericValue = 10001; // Default starting value
            var CompanyDetail = _context.VendorRegistrations.Where(x => x.Id == adminlogin.Vendorid).FirstOrDefault();

            // Clean up the company name by removing unwanted substrings
            string companyName = CompanyDetail.CompanyName;
            string[] unwantedWords = new[] { "pvt ltd", "private limited", "ltd", "inc", "corporation", "corp" };

            foreach (var word in unwantedWords)
            {
                companyName = companyName.Replace(word, "", StringComparison.OrdinalIgnoreCase).Trim();
            }

            string result = string.Empty;

            // Split the company name to handle multi-word names
            var words = companyName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 2)
            {
                if (words[0].Equals(words[1], StringComparison.OrdinalIgnoreCase))
                {
                    result = char.ToUpper(words[0][0]).ToString() + char.ToUpper(words[0][1]).ToString();
                }
                else
                {
                    result = char.ToUpper(words[0][0]).ToString() + char.ToUpper(words[0][1]).ToString() + char.ToUpper(words[1][0]);
                }
            }
            else if (words.Length > 2)
            {
                foreach (var word in words)
                {
                    result += char.ToUpper(word[0]);
                }
            }
            else
            {
                result = companyName.Length >= 3 ? companyName.Substring(0, 3).ToUpper() : companyName.ToUpper();
            }

            string firstChars = result;

            // Get the current date
            DateTime now = DateTime.Now;
            int startYear, endYear;

            if (now.Month >= 4)
            {
                startYear = now.Year;
                endYear = now.Year + 1;
            }
            else
            {
                startYear = now.Year - 1;
                endYear = now.Year;
            }

            string financialYear = $"{startYear}-{endYear}";

            // Reset logic based on the financial year and firstChars change
            if (data != null && !string.IsNullOrEmpty(data.InvoiceNumber))
            {
                string[] parts = data.InvoiceNumber.Split('-');
                if (parts.Length > 2 && int.TryParse(parts.Last(), out numericValue))
                {
                    string lastInvoiceFinancialYear = parts[1];
                    string lastInvoiceFirstChars = parts[0].Substring(0, parts[0].Length - 4);


                    // Check if the financial year or firstChars has changed
                    if (Convert.ToInt32(lastInvoiceFinancialYear) != endYear || lastInvoiceFirstChars != firstChars)
                    {
                        numericValue = 10001; // Reset if new financial year or different firstChars
                    }
                    else
                    {
                        numericValue++; // Increment for the same financial year and same firstChars
                    }
                }
            }

            // Format the final invoice number
            invoiceid = $"{firstChars}{financialYear}-{numericValue:D5}";

            return invoiceid;
        }

        [HttpGet]
        public async Task<IActionResult> ProductInvoice(string InvoiceNumber, bool Ismail)
        {
            try
            {
                if (InvoiceNumber != null)
                {
                    int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    CustomerInvoiceDTO invoice = new CustomerInvoiceDTO();
                    ViewBag.Protocol = Request.Scheme;
                    ViewBag.Host = Request.Host.Value;
                    invoice = await _ICrmrpo.CustomerProductInvoice(InvoiceNumber, Ismail);
                    if (invoice != null)
                    {
                        return View(invoice);
                    }
                    else
                    {
                        TempData["msg"] = "No data found";
                        return RedirectToAction("CustomerInvoiceList");
                    }
                }
                else
                {
                    return RedirectToAction("CustomerInvoiceList");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error : " + ex.Message);
            }


        }
        [HttpGet]
        public JsonResult Product(int? id, int? invoiceId)
        {
            var data = (from pm in _context.VendorProductMasters
                        join gm in _context.GstMasters on pm.Gst equals gm.Id
                        where pm.Id == id && pm.IsActive == true
                        select new
                        {
                            pm,
                            gm
                        })
            .AsEnumerable()
            .Select(x => new ProductDetailList
            {
                SGST = Convert.ToDecimal(x.gm.Scgst),
                CGST = Convert.ToDecimal(x.gm.Cgst),
                IGST = Convert.ToDecimal(x.gm.Igst),
                ProductPrice = x.pm.ProductPrice,
                HsnSacCode = x.pm.Hsncode,
            }).FirstOrDefault();

            if (invoiceId != null && invoiceId != 0)
            {
                var invoiceProduct = _context.CustomerInvoices
                    .FirstOrDefault(ip => ip.Id == invoiceId && ip.ProductId == id);

                if (invoiceProduct != null)
                {
                    // Match product data from invoice
                    data = new ProductDetailList
                    {
                        SGST = invoiceProduct.Sgst,
                        CGST = invoiceProduct.Cgst,
                        IGST = invoiceProduct.Igst,
                        ProductPrice = invoiceProduct.ProductPrice,
                        HsnSacCode = invoiceProduct.Hsncode,
                    };
                }
            }

            var result = new
            {
                Data = data,
            };

            return new JsonResult(result);
        }

        public async Task<IActionResult> SendInvoicePDF(string InvoiceNumber, bool Ismail = false)
        {
            try
            {
                string schema = Request.Scheme;
                string host = Request.Host.Value;
                HtmlToPdf converter = new HtmlToPdf();
                string SlipURL = $"{schema}://{host}/Sale/ProductInvoice?InvoiceNumber={InvoiceNumber}&&Ismail={Ismail}";
                PdfDocument doc = converter.ConvertUrl(SlipURL);

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(SlipURL);
                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest("Failed to retrieve content from the URL.");
                    }

                    var htmlContent = await response.Content.ReadAsStringAsync();

                    // Generate PDF from HTML content
                    byte[] pdf = GeneratePdf(htmlContent);
                    //return File(pdf, "application/pdf", "kk.pdf");
                    

                    CustomerInvoiceDTO result = await _ICrmrpo.CustomerProductInvoice(InvoiceNumber, Ismail);

                    if (result == null)
                    {
                        throw new Exception("Invoice not found.");
                    }
                    // Create a unique filename for the PDF
                    var vendor = _context.VendorRegistrations.FirstOrDefault(x => x.Id == result.VendorId);

                    string pdfFileName = $"{"Invoice"}_{InvoiceNumber}.pdf";

                    string filePath = Path.Combine(pdfFileName);

                    // Save the PDF file
                    await System.IO.File.WriteAllBytesAsync(filePath, pdf);
                    string emailBody = $"Dear {result.CompanyName},We hope this email finds you well. Please find your attached invoice for the products/services provided. If you have any questions or need further assistance, feel free to reach out to us.Thank you for your business and continued support.Best regards,{result.VendorCompanyName}";

                    await _IEmailService.SendInvoicePdfEmail(result.Email, emailBody, pdf, pdfFileName, "application/pdf", (int)result.VendorId);
                    return Json(new { success = true, message = "Invoice sent successfully"});
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private byte[] GeneratePdf(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = DinkToPdf.Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 15, Bottom = 15, Left = 15, Right = 15 },
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" },
                FooterSettings = new FooterSettings
                {
                    FontName = "Arial",
                    FontSize = 8,                 
                },
                LoadSettings = { BlockLocalFileAccess = false }
            };

            var htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }
        public async Task<JsonResult> DeleteProdbyUpdate(int id,bool cloneId)
        {
            if (id <= 0)
            {
                return new JsonResult(new { success = false, message = "Invalid ID." });
            }

            try
            {
                var result = await _context.CustomerInvoices.FirstOrDefaultAsync(x => x.Id == id);
                if (result == null)
                {
                    return new JsonResult(new { success = false, message = "Invoice not found." });
                }

                var invoicesToDelete = await _context.CustomerInvoices
                    .Where(c => c.InvoiceNumber == result.InvoiceNumber)
                    .ToListAsync();

                if (!invoicesToDelete.Any())
                {
                    return new JsonResult(new { success = false, message = "No matching invoices found for deletion." });
                }

                decimal totalDueAmount = invoicesToDelete
                    .Select(ci => ci.DueAmount.Value)
                    .FirstOrDefault();

                decimal adjustment = (result.ProductPrice ?? 0) +
                                     (result.ProductPrice ?? 0) * ((result.Igst ?? 0) / 100) +
                                     (result.ProductPrice ?? 0) * ((result.Sgst ?? 0) / 100) +
                                     (result.ProductPrice ?? 0) * ((result.Cgst ?? 0) / 100);

                decimal dueAmount = totalDueAmount - adjustment;

                foreach (var item in invoicesToDelete)
                {
                    item.DueAmount = dueAmount;
                }

                _context.CustomerInvoices.Remove(result);
                await _context.SaveChangesAsync();

                return new JsonResult(new
                {
                    success = true,
                    message = "Deleted Successfully",
                    redirectUrl = $"/Sale/Invoice?InvoiceNumber={result.InvoiceNumber}&&clone={cloneId}"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCustomerInvoiceamount(int InvoiceId, int Paymentid, decimal PaidAmount)
        {
            var invoiceNumber = await _context.CustomerInvoices
                .Where(x => x.Id == InvoiceId)
                .Select(x => x.InvoiceNumber)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(invoiceNumber))
            {
                return Json(new { success = false, message = "Invoice data not found." });
            }

            var invoices = await _context.CustomerInvoices
                .Where(x => x.InvoiceNumber == invoiceNumber)
                .ToListAsync();

            if (!invoices.Any())
            {
                return Json(new { success = false, message = "No invoices found for the given invoice number." });
            }

            decimal previousTotalPaidAmount = invoices.FirstOrDefault(x => x.Id == InvoiceId)?.PaidAmount ?? 0;
            decimal previousDueAmount = invoices.FirstOrDefault(x => x.Id == InvoiceId)?.DueAmount ?? 0;

            decimal totalAmount = await CalculateTotalAmountByInvoiceId(invoiceNumber);

            foreach (var invoice in invoices)
            {
                if (Paymentid == 1 || Paymentid == 3)
                {
                    invoice.PaidAmount = previousTotalPaidAmount + PaidAmount;
                    invoice.DueAmount = totalAmount - invoice.PaidAmount;
                }
                else
                {
                    invoice.PaidAmount = previousTotalPaidAmount;
                    invoice.DueAmount = previousDueAmount;
                }

                if (invoice.DueAmount < 0)
                {
                    invoice.DueAmount = 0;
                }

                invoice.CreatedDate = DateTime.Now;
                invoice.Paymentstatus = Paymentid;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Invoice payment status updated successfully!" });
        }

        public async Task<decimal> CalculateTotalAmountByInvoiceId(string invoiceId)
        {
            try
            {
                var invoiceDetails = await (from ci in _context.CustomerInvoices
                                            where ci.InvoiceNumber == invoiceId
                                            select new
                                            {
                                                ProductPrice = ci.ProductPrice ?? 0,
                                                IGST = ci.Igst ?? 0,
                                                SGST = ci.Sgst ?? 0,
                                                CGST = ci.Cgst ?? 0
                                            }).ToListAsync();

                if (!invoiceDetails.Any())
                {
                    return 0;
                }

                decimal totalAmount = 0;
                foreach (var item in invoiceDetails)
                {
                    decimal productTotal = (item.ProductPrice + (item.ProductPrice * item.IGST / 100) +
                                           (item.ProductPrice * item.SGST / 100) +
                                           (item.ProductPrice * item.CGST / 100));

                    totalAmount += productTotal;
                }

                totalAmount = decimal.Round(totalAmount, 2, MidpointRounding.AwayFromZero);

                return totalAmount;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}
