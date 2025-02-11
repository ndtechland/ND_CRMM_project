using ClosedXML.Excel;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using DinkToPdf;
using DinkToPdf.Contracts;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using jsreport.AspNetCore;
using jsreport.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using SelectPdf;
using System.Text;
using Umbraco.Core;

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
        public async Task<IActionResult> Invoice(string InvoiceNumber, bool clone = false)
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
                    var ServiceCharge = Invoicedetail?.ServiceCharge ?? null;
                    ViewBag.allowServiceCharge = _context.VendorRegistrations.Where(v => v.Id == adminlogin.Vendorid).FirstOrDefault().SelectCompany == true;

                    if (InvoiceNumber != null)
                    {
                        if (clone == true)
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

                            ViewBag.ProductDetails = _context.VendorProductMasters.Where(c => c.IsActive == true && c.VendorId == adminlogin.Vendorid)
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
                            ViewBag.ServiceCharges = ServiceCharge;
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
                    ViewBag.ServiceCharges = null;
                    ViewBag.InvoiceNumber = InvoiceNumber;
                    ViewBag.clone = clone;
                    ViewBag.Quantity = 1;
                    ViewBag.ProductDetails = _context.VendorProductMasters.Where(c => c.IsActive == true && c.VendorId == adminlogin.Vendorid)
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
        public async Task<IActionResult> Invoice(List<ProductDetail> model, DateTime? InvoiceDate = null, DateTime? InvoiceDueDate = null, string InvoiceNotes = null, string InvoiceTerms = null, string Invoiceclone = null, decimal ServiceCharges = 0)
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

                bool isSuccess = await _ICrmrpo.CustomerInvoice(model, InvoiceNo, (int)adminlogin.Vendorid, InvoiceDate, InvoiceDueDate, InvoiceNotes, InvoiceTerms, Invoiceclone, ServiceCharges);

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
        public IActionResult GetCustomerDetailsById(int id, bool clone = false, int InvoiceID = 0)
        {
            int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
            var InvoiceDetail = _context.CustomerInvoices.Where(x => x.Id == InvoiceID).FirstOrDefault();
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
            else if (clone == false)
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

            var data = _context.CustomerInvoices.Where(x => x.VendorId == adminlogin.Vendorid)
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
        [MiddlewareFilter(typeof(JsReportPipeline))]
        public async Task<IActionResult> SendProductInvoice(string InvoiceNumber, bool Ismail)
        {
            try
            {
                if (string.IsNullOrEmpty(InvoiceNumber))
                {
                    return RedirectToAction("CustomerInvoiceList");
                }

                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                CustomerInvoiceDTO invoice = await _ICrmrpo.CustomerProductInvoice(InvoiceNumber, Ismail);

                if (invoice == null)
                {
                    TempData["msg"] = "No data found for the invoice.";
                    return RedirectToAction("CustomerInvoiceList");
                }

                ViewBag.Protocol = Request.Scheme;
                ViewBag.Host = Request.Host.Value;
                if (Ismail)
                {
                    var jsReportFeature = HttpContext.JsReportFeature();
                    if (jsReportFeature == null)
                    {
                        throw new Exception("JsReport feature is not available in the current context.");
                    }

                    jsReportFeature
                        .Recipe(Recipe.ChromePdf)
                        .OnAfterRender((r) =>
                            HttpContext.Response.Headers["Content-Disposition"] = "attachment; filename=\"Invoice_" + InvoiceNumber + ".pdf\"");

                    return View("ProductInvoice", invoice);
                }
                else
                {
                    return View(invoice);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "An error occurred: " + ex.Message;
                return RedirectToAction("CustomerInvoiceList");
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
                //  HtmlToPdf converter = new HtmlToPdf();
                string SlipURL = $"{schema}://{host}/Sale/SendProductInvoice?InvoiceNumber={InvoiceNumber}&Ismail={Ismail}";
                //  PdfDocument doc = converter.ConvertUrl(SlipURL);

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(SlipURL);
                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest("Failed to retrieve content from the URL.");
                    }


                    byte[] file = await response.Content.ReadAsByteArrayAsync();
                    //var pdfdoc = File(pdfBytes, "application/pdf", "Invoice.pdf");
                    //byte[] file = System.IO.File.ReadAllBytes(pdfdoc);

                    // Set the appropriate content type for PDF and the file name
                    // return File(pdfBytes, "application/pdf", "Invoice.pdf");


                    var htmlContent = await response.Content.ReadAsStringAsync();

                    CustomerInvoiceDTO result = await _ICrmrpo.CustomerProductInvoice(InvoiceNumber, Ismail);
                    if (result == null)
                    {
                        throw new Exception("Invoice not found.");
                    }

                    var vendor = _context.VendorRegistrations.FirstOrDefault(x => x.Id == result.VendorId);
                    string pdfFileName = $"{InvoiceNumber}.pdf";
                    string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CustomerInvoicefile");
                    if (!Directory.Exists(wwwRootPath))
                    {
                        Directory.CreateDirectory(wwwRootPath);
                    }

                    string filePath = Path.Combine(wwwRootPath, pdfFileName);
                    await System.IO.File.WriteAllBytesAsync(filePath, file);

                    string emailBody = $"Dear {result.CompanyName}," +
                                       $"We hope this email finds you well. Please find your attached invoice for the products/services provided." +
                                       $"If you have any questions or need further assistance, feel free to reach out to us." +
                                       $"Thank you for your business and continued support." +
                                       $"Best regards,{result.VendorCompanyName}";

                    await _IEmailService.SendInvoicePdfEmail(result.Email, emailBody, file, pdfFileName, "application/pdf", (int)result.VendorId);

                    return Json(new { success = true, message = "Invoice sent successfully" });
                }



            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<JsonResult> DeleteProdbyUpdate(int id, bool cloneId)
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
                    .Select(ci => ci.DueAmount ?? 0) // Handle potential nulls
                    .FirstOrDefault();

                // Ensure null handling for ProductPrice and ProductQty
                decimal productQty = result.ProductQty ?? 1; // Default to 1 if ProductQty is null
                decimal productPrice = result.ProductPrice ?? 0;

                decimal adjustment = (productPrice * productQty) +
                                     (productPrice * (result.Igst ?? 0) / 100) +
                                     (productPrice * (result.Sgst ?? 0) / 100) +
                                     (productPrice * (result.Cgst ?? 0) / 100);

                decimal dueAmount = totalDueAmount > 0 ? totalDueAmount - adjustment : totalDueAmount;

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
        public async Task<JsonResult> UpdateCustomerInvoiceamount(int InvoiceId, int Paymentid, decimal PaidAmount, DateTime PaymentDate)
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
                    invoice.Paymentdate = PaymentDate;
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
                                            join cid in _context.CustomerInvoicedetails on ci.InvoiceNumber equals cid.InvoiceNumber
                                            where ci.InvoiceNumber == invoiceId
                                            select new
                                            {
                                                ProductPrice = (decimal?)ci.ProductPrice ?? 0,
                                                IGST = (decimal?)ci.Igst ?? 0,
                                                SGST = (decimal?)ci.Sgst ?? 0,
                                                CGST = (decimal?)ci.Cgst ?? 0,
                                                ProductQty = (int?)ci.ProductQty ?? 1,
                                                ServiceCharges = (decimal?)cid.ServiceCharge ?? 0
                                            }).ToListAsync();

                if (invoiceDetails == null || !invoiceDetails.Any())
                {
                    return 0;
                }

                decimal totalAmount = 0;
                decimal serviceCharge = 0;

                foreach (var item in invoiceDetails)
                {
                    decimal productTotal = (item.ProductPrice * item.ProductQty) +
                                           (item.ProductPrice * item.IGST / 100) +
                                           (item.ProductPrice * item.SGST / 100) +
                                           (item.ProductPrice * item.CGST / 100);

                    serviceCharge = ((item.ProductPrice * item.ProductQty) +
                                    (item.ProductPrice * item.IGST / 100) +
                                    (item.ProductPrice * item.SGST / 100) +
                                    (item.ProductPrice * item.CGST / 100))
                                    * (item.ServiceCharges / 100);

                    totalAmount += productTotal + serviceCharge;
                }

                decimal roundedTotal = Math.Round(totalAmount, 0, MidpointRounding.AwayFromZero);
                return roundedTotal;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> CustomerInvoicePaidList()
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
              
                List<CustomerpaidInvoiceDTO> data = await (from ci in _context.CustomerInvoices
                                                           join c in _context.CustomerRegistrations on ci.CustomerId equals c.Id
                                                           join p in _context.VendorProductMasters on ci.ProductId equals p.Id
                                                           join s in _context.States on c.StateId equals s.Id
                                                           join ct in _context.Cities on c.CityId equals ct.Id
                                                           join sb in _context.States on c.BillingStateId equals sb.Id
                                                           join ctb in _context.Cities on c.BillingCityId equals ctb.Id
                                                           join pm in _context.Paymentmodes on ci.Paymentstatus equals pm.Id
                                                           where c.Vendorid == adminlogin.Vendorid && ci.Paymentstatus == 1
                                                           group new { ci, c, sb, ctb }
                                                           by new { c.Id, c.CompanyName, c.MobileNumber, c.Email, sb.SName, ctb.City1 } into grouped
                                                           select new CustomerpaidInvoiceDTO
                                                           {
                                                               id = grouped.Key.Id,
                                                               CompanyName = grouped.Key.CompanyName,
                                                               MobileNumber = grouped.Key.MobileNumber,
                                                               Email = grouped.Key.Email,
                                                               BillingState = grouped.Key.SName,
                                                               BillingCity = grouped.Key.City1,
                                                               NoofInvoice = grouped.Select(g => g.ci.InvoiceNumber).Distinct().Count() 
                                                           }).Distinct().OrderByDescending(x=> x.id).ToListAsync();


                if (data.Count > 0)
                {
                    return View(data);

                }
                else
                {
                    return View(null);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> CustomerPaidInvoiceList(int id)
        {
            try
            {
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefault();
                List<CustomerpaidInvoiceDTO> data = await (from ci in _context.CustomerInvoices
                                                           join c in _context.CustomerRegistrations on ci.CustomerId equals c.Id
                                                           join p in _context.VendorProductMasters on ci.ProductId equals p.Id
                                                           join s in _context.States on c.StateId equals s.Id
                                                           join ct in _context.Cities on c.CityId equals ct.Id
                                                           join sb in _context.States on c.BillingStateId equals sb.Id
                                                           join ctb in _context.Cities on c.BillingCityId equals ctb.Id
                                                           join pm in _context.Paymentmodes on ci.Paymentstatus equals pm.Id
                                                           where ci.CustomerId == id && ci.Paymentstatus == 1
                                                           group new { ci, c, p, s, ct, sb, ctb, pm } by ci.InvoiceNumber into grouped
                                                           select new CustomerpaidInvoiceDTO
                                                           {

                                                               CompanyName = grouped.First().c.CompanyName,
                                                               MobileNumber = grouped.First().c.MobileNumber,
                                                               Email = grouped.First().c.Email,
                                                               BillingState = grouped.First().sb.SName,
                                                               BillingCity = grouped.First().ctb.City1,
                                                               InvoiceNumber = grouped.Key,
                                                               PaymentStatus = grouped.First().pm.PaymentType,
                                                               PaidAmount = grouped.First().ci.PaidAmount ?? 0,
                                                           }).OrderByDescending(ci => ci.InvoiceNumber).ToListAsync();
                foreach (var invoice in data)
                {
                    invoice.TotalAmount = await CalculateTotalAmountByInvoiceId(invoice.InvoiceNumber);
                }
                if (data.Count > 0)
                {
                    return View(data);

                }
                else
                {
                    return View(null);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> ExportCustomerPaidInvoiceReport(string CompanyName)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminLogin = await _context.AdminLogins.FirstOrDefaultAsync(x => x.Id == userId);

                if (adminLogin == null)
                    return NotFound("Admin not found.");

                List<CustomerpaidInvoiceDTO> data = await (from ci in _context.CustomerInvoices
                                                           join c in _context.CustomerRegistrations on ci.CustomerId equals c.Id
                                                           join p in _context.VendorProductMasters on ci.ProductId equals p.Id
                                                           join s in _context.States on c.StateId equals s.Id
                                                           join ct in _context.Cities on c.CityId equals ct.Id
                                                           join sb in _context.States on c.BillingStateId equals sb.Id
                                                           join ctb in _context.Cities on c.BillingCityId equals ctb.Id
                                                           join pm in _context.Paymentmodes on ci.Paymentstatus equals pm.Id
                                                           where c.CompanyName == CompanyName && ci.Paymentstatus == 1
                                                           group new { ci, c, p, s, ct, sb, ctb, pm } by ci.InvoiceNumber into grouped
                                                           select new CustomerpaidInvoiceDTO
                                                           {

                                                               CompanyName = grouped.First().c.CompanyName,
                                                               MobileNumber = grouped.First().c.MobileNumber,
                                                               Email = grouped.First().c.Email,
                                                               BillingState = grouped.First().sb.SName,
                                                               BillingCity = grouped.First().ctb.City1,
                                                               InvoiceNumber = grouped.Key,
                                                               PaymentStatus = grouped.First().pm.PaymentType,
                                                               PaidAmount = grouped.First().ci.PaidAmount ?? 0,
                                                           }).OrderByDescending(ci => ci.InvoiceNumber).ToListAsync();
                foreach (var invoice in data)
                {
                    invoice.TotalAmount = await CalculateTotalAmountByInvoiceId(invoice.InvoiceNumber);
                }
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Invoice Paid Report");

                    int currentwork = 1;
                    worksheet.Cell(currentwork, 1).Value = "Sr.No.";
                    worksheet.Cell(currentwork, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 2).Value = "Invoice Number";
                    worksheet.Cell(currentwork, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 3).Value = "Company Name";
                    worksheet.Cell(currentwork, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 4).Value = "Mobile Number";
                    worksheet.Cell(currentwork, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 5).Value = "Email Id";
                    worksheet.Cell(currentwork, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 6).Value = "Billing State";
                    worksheet.Cell(currentwork, 6).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 7).Value = "Total Payment";
                    worksheet.Cell(currentwork, 7).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 8).Value = "Paid Payment";
                    worksheet.Cell(currentwork, 8).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(currentwork, 9).Value = "Payment Status";
                    worksheet.Cell(currentwork, 9).Style.Fill.BackgroundColor = XLColor.LightGray;

                    currentwork++;

                    var row = 1;
                    foreach (var record in data)
                    {
                        worksheet.Cell(currentwork, 1).Value = row++;
                        worksheet.Cell(currentwork, 2).Value = record.InvoiceNumber;
                        worksheet.Cell(currentwork, 3).Value = record.CompanyName;
                        worksheet.Cell(currentwork, 4).Value = record.MobileNumber;
                        worksheet.Cell(currentwork, 5).Value = record.Email;
                        worksheet.Cell(currentwork, 6).Value = record.BillingState;
                        worksheet.Cell(currentwork, 7).Value = record.TotalAmount;
                        worksheet.Cell(currentwork, 8).Value = record.PaidAmount;
                        worksheet.Cell(currentwork, 9).Value = record.PaymentStatus;

                        currentwork++;
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileContent = stream.ToArray();
                        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Invoice Paid Report.xlsx");
                    }

                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
