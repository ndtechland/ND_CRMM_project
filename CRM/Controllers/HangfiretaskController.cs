using CRM.Models.Crm;
using DocumentFormat.OpenXml.Spreadsheet;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CRM.Controllers
{
    public class HangfiretaskController : Controller
    {
        private readonly admin_NDCrMContext _context;
        public HangfiretaskController(admin_NDCrMContext _context)
        {
            this._context = _context;
        }
        public void DoWork()
        {
            var lm = _context.Leavemasters.Where(x => x.LeavetypeId == 1).ToList();
            foreach (var leave in lm)
            {
                switch (leave.LeavetypeId)
                {
                    case 1:
                        leave.Value += (decimal)1.50;
                        leave.LeaveUpdateDate = DateTime.Now;
                        break;
                    default:
                        break;
                }
            }
            _context.SaveChanges();
        }

        [HttpGet]
        [Route("DatabaseUpdate")]
        public IActionResult DatabaseUpdate()
        {
            RecurringJob.AddOrUpdate(() => DoWork(), Cron.Monthly);
            return Ok("Database check job initiated!");
        }
        public void DoWorkSixmonth()
        {
            var lm = _context.Leavemasters.Where(x => x.LeavetypeId == 1).ToList();
            foreach (var leave in lm)
            {
                switch (leave.LeavetypeId)
                {
                    case 1:
                        leave.Value += (decimal)1.50;
                        leave.LeaveUpdateDate = DateTime.Now;
                        break;
                    default:
                        break;
                }
            }
            _context.SaveChanges();
        }

        [HttpGet]
        [Route("DatabaseUpdateSixmonth")]
        public IActionResult DatabaseUpdateSixmonth()
        {
            RecurringJob.AddOrUpdate(() => DoWorkSixmonth(), Cron.Monthly(6));
            return Ok("Database check job initiated!");
        }


        //[HttpPost]
        //[Route("Hangfiretask/autocheckoutall")]
        //public IActionResult AutoCheckOutAll()
        //{
        //    var shifts = _context.Officeshifts.ToList();

        //    foreach (var shift in shifts)
        //    {
        //        if (string.IsNullOrEmpty(shift.Endtime)) continue;

        //        TimeSpan endTime;
        //        DateTime parsedEndTime;

        //        try
        //        {
        //            parsedEndTime = DateTime.ParseExact(shift.Endtime, "h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
        //            endTime = parsedEndTime.TimeOfDay;
        //        }
        //        catch (FormatException)
        //        {
        //            Console.WriteLine($"Invalid time format for Endtime: {shift.Endtime}");
        //            continue;
        //        }

        //        if (DateTime.Now.TimeOfDay >= endTime)
        //        {
        //            Console.WriteLine($"Scheduling auto-checkout for shift {shift.Id} at {shift.Endtime}");

        //            RecurringJob.AddOrUpdate<HangfiretaskController>(
        //                $"AutoCheckOutForShift_{shift.Id}",
        //                controller => controller.AutoCheckOutForAllEmployees(shift.Id, endTime),
        //                Cron.Daily(parsedEndTime.Hour, parsedEndTime.Minute)
        //            );
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Current time is before the shift end time {shift.Endtime}. Job not scheduled for shift {shift.Id}.");
        //        }
        //    }

        //    return Ok("Auto-checkout jobs scheduled based on shift end times.");
        //}

        //public async Task AutoCheckOutForAllEmployees(int shiftId, TimeSpan shiftEndTime)
        //{
        //    var currentDate = DateTime.Today;
        //    DateTime now = DateTime.Now;

        //    // Skip if the current time is before the shift end time.
        //    if (now.TimeOfDay < shiftEndTime)
        //    {
        //        Console.WriteLine("It's not yet time for auto-checkout.");
        //        return;
        //    }

        //    var shift = await _context.Officeshifts.FindAsync(shiftId);
        //    if (shift == null)
        //    {
        //        Console.WriteLine($"Shift with ID {shiftId} not found.");
        //        return;
        //    }

        //    DateTime effectiveCheckOutTime = currentDate.Add(shiftEndTime);

        //    // Fetch employees who belong to this shift
        //    var employeesWithCheckIns = await _context.EmployeeRegistrations
        //        .Where(emp => emp.OfficeshiftTypeid == shiftId)
        //        .Select(emp => new
        //        {
        //            OfficeShiftId = emp.OfficeshiftTypeid,
        //            EmployeeId = emp.EmployeeId
        //        })
        //        .ToListAsync();

        //    foreach (var emp in employeesWithCheckIns)
        //    {
        //        // Check latest check-in record for the employee
        //        var empCheckInRecord = await _context.EmpCheckIns
        //            .Where(x => x.EmployeeId == emp.EmployeeId && x.Currentdate.HasValue && x.Currentdate.Value.Date == DateTime.Now.Date)
        //            .OrderByDescending(x => x.Id)
        //            .FirstOrDefaultAsync();

        //        // Skip if already checked out or no check-in was recorded
        //        if (empCheckInRecord == null || empCheckInRecord.CheckIn == false)
        //        {
        //            continue;
        //        }

        //        // Auto-checkout logic
        //        var checkInRecord = await _context.EmployeeCheckInRecords
        //            .Where(g => g.EmpId == emp.EmployeeId &&
        //                        g.CurrentDate.HasValue &&
        //                        g.CurrentDate.Value.Date == currentDate &&
        //                        g.ShiftId == shiftId &&
        //                        g.CheckOuttime == null)
        //            .OrderByDescending(g => g.CheckIntime)
        //            .FirstOrDefaultAsync();

        //        if (checkInRecord == null || checkInRecord.CheckIntime == null)
        //        {
        //            Console.WriteLine($"Skipping employee {emp.EmployeeId}: No valid check-in record found.");
        //            continue;
        //        }

        //        DateTime checkInTime = checkInRecord.CheckIntime.Value;

        //        // Ensure we only calculate working hours if the check-in time is earlier than end time
        //        DateTime checkoutTime = checkInTime < effectiveCheckOutTime ? effectiveCheckOutTime : checkInTime;

        //        TimeSpan workingHours = checkoutTime - checkInTime;
        //        checkInRecord.CheckOuttime = checkoutTime;
        //        checkInRecord.Workinghour = workingHours;
        //        checkInRecord.CurrentDate = DateTime.Now;

        //        Console.WriteLine($"Auto-checkout completed for employee {emp.EmployeeId} at {checkoutTime}. Working hours: {workingHours.TotalHours} hours.");

        //        // Log the checkout into EmployeeCheckIns
        //        var employeeCheckIn = new EmployeeCheckIn
        //        {
        //            EmployeeId = emp.EmployeeId,
        //            CurrentLat = "0.0",
        //            Currentlong = "0.0",
        //            CheckInTime = checkInTime,
        //            CheckOutTime = checkoutTime,
        //            Currentdate = DateTime.Now,
        //            CheckIn = false,
        //            Breakin = false,
        //            Breakout = false
        //        };

        //        await _context.EmployeeCheckIns.AddAsync(employeeCheckIn);
        //    }

        //    await _context.SaveChangesAsync();
        //}

        public void Taxes()
        {
            var chargeMasters = _context.Chargesmasters.ToList();
            var customerInvoices = _context.CustomerInvoices.ToList();

            foreach (var invoice in customerInvoices)
            {
                if (invoice.Dueamountdate.HasValue)
                {
                    int overdueDays = (DateTime.Now - invoice.Dueamountdate.Value).Days;

                    if (overdueDays > 0)
                    {
                        var charge = chargeMasters
                            .FirstOrDefault(x =>
                            {
                                var rangeParts = x.Chargesname?.Split('-');
                                if (rangeParts != null && rangeParts.Length == 2)
                                {
                                    int startDay = int.Parse(rangeParts[0]);
                                    int endDay = int.Parse(rangeParts[1]);

                                    return overdueDays >= startDay && overdueDays <= endDay;
                                }

                                return false;
                            });

                        if (charge != null && charge.Chargespercentage.HasValue)
                        {
                            decimal gstMultiplier = (decimal)((invoice.ProductPrice * invoice.Igst / 100 ?? 0) + (invoice.ProductPrice * invoice.Sgst / 100 ?? 0) + (invoice.ProductPrice * invoice.Cgst / 100 ?? 0));

                            decimal chargeMultiplier = (decimal)(gstMultiplier * charge.Chargespercentage.Value / 100);

                            invoice.Taxamount = chargeMultiplier;
                            invoice.Taxid = charge.Id;
                        }
                    }
                }
            }

            _context.SaveChanges();
        }


        [HttpPost]
        [Route("Hangfiretask/UpdateTaxes")]
        public IActionResult UpdateTaxes()
        {
            RecurringJob.AddOrUpdate(() => Taxes(), Cron.Daily);
            return Ok("Database check job initiated!");
        }

    }
}
