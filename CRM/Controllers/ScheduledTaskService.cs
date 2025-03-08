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

			_doWorkTimer = new Timer(DoWork, null, TimeSpan.Zero, _intervalday);
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
                   // DateTime lastDayOfMonth = DateTime.Today;

                   
                    var emplist = context.EmployeeRegistrations.Where(x=>x.Isactive == true && x.IsDeleted ==false).ToList();

					_logger.LogInformation($"Today's date: {today.ToString("yyyy-MM-dd")}");

					foreach (var task in tasks)
					{
						string scheduleTime = DateTime.Today.Add(task.Scheduletime.Value).ToString("hh:mm tt");
						_logger.LogInformation($"Task {task.Id}: schedule time: {scheduleTime}, current time: {currentTime}, current day of week: {currentDayOfWeek}");

						if (scheduleTime == currentTime && task.IsExcute == false && task.Scheduleday.ToString() == currentDayOfWeek)
						{
							if (task.Id != null)
							{
								CustomerTaxes(context, emailService);
							}
							var currentTask = context.ScheduledTasks.FirstOrDefault(t => t.Id == task.Id);
							if (currentTask != null)
							{
								currentTask.Schedulemethod = "CustomerTaxes";
								currentTask.Excutetime = DateTime.Now;
								currentTask.IsExcute = true;

								_logger.LogInformation($"Task {task.Id} updated successfully.");

								context.SaveChanges();
							}
						}
						if (today == lastDayOfMonth && task.IsExcute == false)
						{
							_logger.LogInformation($"Performing end-of-month operations for {today:yyyy-MM-dd}.");

							foreach (var employee in emplist)
							{
								if (scheduleTime == currentTime && task.Id != null)
								{
									EmpLeaveDoWork(context, employee.EmployeeId, (int)employee.Vendorid);
								}
							}
							var currentTask = context.ScheduledTasks.FirstOrDefault(t => t.Id == task.Id);
							if (currentTask != null)
							{
								currentTask.Schedulemethod = "EmpLeaveDoWork";
								currentTask.Excutetime = DateTime.Now;
								currentTask.IsExcute = true;

								_logger.LogInformation($"Task {task.Id} updated successfully.");

								context.SaveChanges();  
							}
						}

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
							decimal Productprice = gstMultiplier + (decimal)invoice.ProductPrice;
							decimal chargeMultiplier = Productprice * applicableCharge.Chargespercentage.Value / 100;
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
					decimal Productprice = gstMultiplier + (decimal)invoice.ProductPrice;
					if (customerDetails.TryGetValue((int)invoice.CustomerId, out var customerdetail) &&
						!string.IsNullOrEmpty(customerdetail.Email))
					{
						string companyName = customerdetail.CompanyName ?? "Valued Customer";
						string toEmail = customerdetail.Email;
						string productName = productDetails.TryGetValue((int)invoice.ProductId, out var name) ? name : "Unknown Product";

						if (DateTime.Now.Date >= startNotificationDate && DateTime.Now.Date < renewalDate)
						{
							emailService.CustomerRenewalEmail(toEmail, companyName, renewalDate.ToString("dd/MM/yyyy"), productName, Productprice, (int)invoice.VendorId);
							invoiceDetails.IsRenewDate = true;

						}
						else if (DateTime.Now.Date >= renewalDate && invoiceDetails.IsRenewDate == false)
						{
							emailService.CustomerExpirEmail(toEmail, companyName, renewalDate.ToString("dd/MM/yyyy"), productName, (int)invoice.VendorId);
							invoiceDetails.IsRenewDate = true;

						}
						_logger.LogInformation($"CustomerTaxes !!!::::::::{customerInvoices}");

					}
				}
			}
			context.SaveChanges();
			return "CustomerTaxes";
		}
		public string EmpLeaveDoWork(admin_NDCrMContext context, string EmployeeId, int Vendorid)
		{
			var leaveTypes = context.LeaveTypes.Where(x => x.Vendorid == Vendorid).ToList();
			var leaveMasters = context.Leavemasters.Where(x => x.EmpId == EmployeeId && x.Vendorid == Vendorid).ToList();
			var emplist = context.EmployeeRegistrations.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();

			foreach (var leaveMaster in leaveMasters)
			{
				var leaveType = leaveTypes.FirstOrDefault(x => x.Id == leaveMaster.LeavetypeId);

				if (leaveType != null)
				{
					if (leaveMaster.LeavetypeId != null)
					{
						leaveMaster.Value += leaveType.Leavevalue;
						leaveMaster.LeaveUpdateDate = DateTime.Now;
					}
				}
			}

			context.SaveChanges();
			return "EmpLeaveDoWork";
		}
		//     public string EmpLeaveDoWork(admin_NDCrMContext context, string EmployeeId, int Vendorid)
		//     {
		//         var leaveTypes = context.LeaveTypes.Where(x => x.Vendorid == Vendorid).ToList();
		//         var leaveMasters = context.Leavemasters.Where(x => x.EmpId == EmployeeId && x.Vendorid == Vendorid).ToList();
		//         var emplist = context.EmployeeRegistrations.FirstOrDefault(x => x.EmployeeId == EmployeeId);

		//DateTime joiningDate = emplist.DateOfJoining.Value;

		//         bool isEligibleForLeave = DateTime.Now >= joiningDate.AddMonths(3);

		//         foreach (var leaveMaster in leaveMasters)
		//         {
		//             var leaveType = leaveTypes.FirstOrDefault(x => x.Id == leaveMaster.LeavetypeId);

		//             if (leaveType != null && leaveMaster.LeavetypeId != null)
		//             {
		//                 leaveMaster.Value += isEligibleForLeave ? leaveType.Leavevalue : 0;
		//                 leaveMaster.LeaveUpdateDate = DateTime.Now;
		//             }
		//         }

		//         context.SaveChanges();
		//         return "EmpLeaveDoWork";
		//     }

	}

}
