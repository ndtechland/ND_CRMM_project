using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;

namespace CRM.Controllers
{
    public class ScheduledTaskService : IHostedService, IDisposable
    {
        private Timer _doWorkTimer;
        private Timer _updateTasksTimer;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(40);
        private readonly TimeSpan _intervalday = TimeSpan.FromDays(1);
        private readonly ILogger<ScheduledTaskService> _logger;

        public ScheduledTaskService(IServiceProvider serviceProvider, ILogger<ScheduledTaskService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _doWorkTimer = new Timer(DoWork, null, TimeSpan.Zero, _interval);
            _updateTasksTimer = new Timer(UpdateScheduledTasksdata, null, TimeSpan.Zero, _interval);
            Console.WriteLine("Scheduled task started.");
            return Task.CompletedTask;
        }
        private void UpdateScheduledTasksdata(object state)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<admin_NDCrMContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var tasks = context.ScheduledTasks.ToList();
                    DateTime currentTime = DateTime.Now;


                    foreach (var task in tasks)
                    {
                        var time = task.Excutetime.Value.AddHours(2);
                        if (task.Excutetime.HasValue)
                        {
                            if (time < currentTime && task.IsExcute == true)
                            {
                                task.IsExcute = false;
                                Console.WriteLine($"Task {task.Id} is scheduled more than 12 hours from now, triggered at {DateTime.Now}");
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateScheduledTasksdata: {ex.Message}");
            }

            Console.WriteLine($"Scheduled task running at: {DateTime.Now}");
        }
        private void DoWork(object state)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<admin_NDCrMContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var tasks = context.ScheduledTasks.ToList();
                    string currentTime = DateTime.Now.ToString("hh:mm tt");
                    string currentDayOfWeek = DateTime.Now.DayOfWeek.ToString();

                    _logger.LogInformation($"DoWork method called!!!::::::::{DateTime.Now.ToString()}");

                    DateTime today = DateTime.Today;
                    DateTime lastDayOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                  
                    var emplist = context.EmployeeRegistrations.ToList();

                    _logger.LogInformation($"Today's date: {today.ToString("yyyy-MM-dd")}");

                    foreach (var task in tasks)
                    {
                        string scheduleTime = DateTime.Today.Add(task.Scheduletime.Value).ToString("hh:mm tt");
                        _logger.LogInformation($"Task {task.Id}: schedule time: {scheduleTime}, current time: {currentTime}, current day of week: {currentDayOfWeek}");

                        if (scheduleTime == currentTime && task.IsExcute == false && task.Scheduleday.ToString() == currentDayOfWeek)
                        {
                            task.Schedulemethod = CustomerTaxes(context, emailService);
                            task.Excutetime = DateTime.Now;
                            task.IsExcute = true;
                            _logger.LogInformation($"Task {task.Id} triggered at {DateTime.Now}");
                            context.SaveChanges();

                        }
                        if (today == lastDayOfMonth && task.IsExcute == false)
                        {
                            _logger.LogInformation($"Performing end-of-month operations for {today.ToString("yyyy-MM-dd")}.");
                            task.Schedulemethod = EmpLeaveDoWork(context);
                            task.Excutetime = DateTime.Now;
                            task.IsExcute = true;
                            _logger.LogInformation($"End-of-month operations completed successfully.");
                            context.SaveChanges();

                        }
                        //foreach (var employee in emplist)
                        //{
                        //    if (employee.DateOfJoining.HasValue && employee.DateOfJoining.Value.AddMonths(3) == today)
                        //    {
                        //        _logger.LogInformation($"Performing probation leave operations for employee {employee.EmployeeId}.");
                        //        task.Schedulemethod = EmpLeaveProbationperiod(context, employee.EmployeeId);
                        //        task.Excutetime = DateTime.Now;
                        //        task.IsExcute = true;
                        //        _logger.LogInformation($"Probation leave operations completed for employee {employee.EmployeeId}.");
                        //    }
                        //}

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DoWork: {ex.Message}");
            }

            _logger.LogInformation($"Scheduled task running at: {DateTime.Now}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _doWorkTimer?.Change(Timeout.Infinite, 0);
            _updateTasksTimer?.Change(Timeout.Infinite, 0);
            Console.WriteLine("Scheduled task stopped.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _doWorkTimer?.Dispose();
            _updateTasksTimer?.Dispose();
        }
        public string CustomerTaxes(admin_NDCrMContext context, IEmailService emailService)
        {
            var chargeMasters = context.Chargesmasters.ToList();
            var customerInvoices = context.CustomerInvoices.ToList();
            var customerDetails = context.CustomerRegistrations.ToDictionary(c => c.Id);
            var productDetails = context.VendorProductMasters.ToDictionary(p => p.Id, p => p.ProductName);
            var customerdDetails = context.CustomerInvoicedetails.Where(c => customerInvoices.Select(inv => inv.InvoiceNumber).Contains(c.InvoiceNumber)).ToList();

            foreach (var invoice in customerInvoices)
            {
                var invoiceDetails = customerdDetails.FirstOrDefault(c => c.InvoiceNumber == invoice.InvoiceNumber);
                if (invoiceDetails == null)
                {
                    continue; 
                }
                if (invoice.Paymentstatus == 1)
                    continue;

                if (invoiceDetails.InvoiceDueDate.HasValue && invoiceDetails.InvoiceDueDate.Value.Date < DateTime.Now.Date)
                {
                    int overdueDays = (DateTime.Now - invoiceDetails.InvoiceDueDate.Value).Days;
                    if (overdueDays > 0)
                    {
                        var applicableCharge = chargeMasters.FirstOrDefault(charge =>
                        {
                            if (string.IsNullOrEmpty(charge.Chargesname)) return false;
                            var rangeParts = charge.Chargesname.Split('-');
                            return rangeParts.Length == 2 &&
                                   int.TryParse(rangeParts[0], out int startDay) &&
                                   int.TryParse(rangeParts[1], out int endDay) &&
                                   overdueDays >= startDay && overdueDays <= endDay;
                        });

                        if (applicableCharge?.Chargespercentage.HasValue == true)
                        {
                            decimal gstMultiplier = (invoice.ProductPrice ?? 0) *
                                                    (((invoice.Igst ?? 0) + (invoice.Sgst ?? 0) + (invoice.Cgst ?? 0)) / 100);

                            decimal chargeMultiplier = gstMultiplier * applicableCharge.Chargespercentage.Value / 100;
                            invoiceDetails.Taxamount = chargeMultiplier;
                            invoiceDetails.Taxid = applicableCharge.Id;
                        }
                    }
                }

                if (invoice.RenewDate.HasValue)
                {
                    DateTime renewalDate = invoice.RenewDate.Value;
                    DateTime startNotificationDate = renewalDate.AddDays(-7);
                    decimal gstMultiplier = (invoice.ProductPrice ?? 0) *
                                              (((invoice.Igst ?? 0) + (invoice.Sgst ?? 0) + (invoice.Cgst ?? 0)) / 100);
                    if (customerDetails.TryGetValue((int)invoice.CustomerId, out var customerdetail) &&
                        !string.IsNullOrEmpty(customerdetail.Email))
                    {
                        string companyName = customerdetail.CompanyName ?? "Valued Customer";
                        string toEmail = customerdetail.Email;
                        string productName = productDetails.TryGetValue((int)invoice.ProductId, out var name) ? name : "Unknown Product";

                        if (DateTime.Now.Date >= startNotificationDate && DateTime.Now.Date < renewalDate)
                        {
                            emailService.CustomerRenewalEmail(toEmail, companyName, renewalDate.ToString("dd/MM/yyyy"), productName, gstMultiplier);
                            invoiceDetails.IsRenewDate = true;

                        }
                        else if (DateTime.Now.Date >= renewalDate && invoiceDetails.IsRenewDate == false)
                        {
                            emailService.CustomerExpirEmail(toEmail, companyName, renewalDate.ToString("dd/MM/yyyy"), productName);
                            invoiceDetails.IsRenewDate = true;

                        }
                        _logger.LogInformation($"CustomerTaxes !!!::::::::{customerInvoices}");

                    }
                }
            }
            context.SaveChanges();
            return "CustomerTaxes";
        }
        public string EmpLeaveDoWork(admin_NDCrMContext context)
        {
            var leaveTypes = context.LeaveTypes.ToList();
            var leaveMasters = context.Leavemasters.ToList();
            foreach (var leaveMaster in leaveMasters)
            {
                var leaveType = leaveTypes.FirstOrDefault(x => x.Id == leaveMaster.LeavetypeId);

                if (leaveType != null)
                {
                    switch (leaveMaster.LeavetypeId)
                    {
                        case 1:
                            leaveMaster.Value += leaveType.Leavevalue; 
                            leaveMaster.LeaveUpdateDate = DateTime.Now; 
                            break;

                        default:
                            break;
                    }
                }
            }
            context.SaveChanges();
            return "EmpLeaveDoWork";
        }
        public string EmpLeaveProbationperiod(admin_NDCrMContext context, string EmployeeId)
        {
            var emplist = context.EmployeeRegistrations.Where(x => x.EmployeeId == EmployeeId).ToList();
            var vendorId = emplist.FirstOrDefault()?.Vendorid;

            if (vendorId.HasValue)
            {
                var leaveTypes = context.LeaveTypes.Where(x => x.Vendorid == vendorId.Value).ToList();
                foreach (var leaveType in leaveTypes)
                {
                    Leavemaster leavemaster = new Leavemaster
                    {
                        EmpId = EmployeeId, 
                        LeavetypeId = leaveType.Id, 
                        Value = leaveType.Leavevalue, 
                        Vendorid = vendorId.Value, 
                        LeaveUpdateDate = DateTime.Now, 
                        LeaveStartDate = DateTime.Now,
                    };
                    context.Leavemasters.Add(leavemaster);
                }
                context.SaveChanges();
            }
            return "EmpLeaveProbationperiod";
        }

    }

}
