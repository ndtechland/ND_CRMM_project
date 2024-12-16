using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
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
        // private readonly TimeSpan _intervalday = TimeSpan.FromMinutes(1); 
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
                    Console.WriteLine(tasks[0].Scheduletime.ToString());

                    string currentTime = DateTime.Now.ToString("hh:mm tt");
                    string currentDayOfWeek = DateTime.Now.DayOfWeek.ToString();

                    _logger.LogInformation($"DoWork method called!!!::::::::{DateTime.Now.ToString()}");

                    foreach (var task in tasks)
                    {
                       
                        string scheduleTime = DateTime.Today.Add(task.Scheduletime.Value).ToString("hh:mm tt");
                        _logger.LogInformation($"DoWork method value !!!::::::::{task.Scheduletime.HasValue},{scheduleTime},{task.Scheduleday.ToString()},{currentTime},{currentDayOfWeek}");

                        if (scheduleTime == currentTime && task.IsExcute == false && task.Scheduleday.ToString() == currentDayOfWeek)
                        {
                            task.Schedulemethod = CustomerTaxes(context, emailService);
                            task.Excutetime = DateTime.Now;
                            task.IsExcute = true;
                            Console.WriteLine($"Task {task.Id} triggered at {DateTime.Now}");
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in StartAsync: {ex.Message}");
            }
            Console.WriteLine($"Scheduled task running at: {DateTime.Now}");
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

            foreach (var invoice in customerInvoices)
            {
                if (invoice.Paymentstatus == 1)
                    continue;

                if (invoice.Dueamountdate.HasValue)
                {
                    int overdueDays = (DateTime.Now - invoice.Dueamountdate.Value).Days;
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
                            invoice.Taxamount = chargeMultiplier;
                            invoice.Taxid = applicableCharge.Id;
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
                            invoice.IsRenewDate = true;

                        }
                        else if (DateTime.Now.Date >= renewalDate && invoice.IsRenewDate == false)
                        {
                            emailService.CustomerExpirEmail(toEmail, companyName, renewalDate.ToString("dd/MM/yyyy"), productName);
                            invoice.IsRenewDate = true;

                        }
                        _logger.LogInformation($"CustomerTaxes !!!::::::::{customerInvoices}");

                    }
                }
            }
            context.SaveChanges();
            return "CustomerTaxes";
        }
    }
   
}
