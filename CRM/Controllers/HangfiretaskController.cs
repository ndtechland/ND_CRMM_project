using CRM.Models.Crm;
using CRM.Repository;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Multiplier;
using System.Globalization;

namespace CRM.Controllers
{
    public class HangfiretaskController : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly IEmailService _emailService;

        public HangfiretaskController(admin_NDCrMContext _context, IEmailService emailService)
        {
            this._context = _context;
            _emailService = emailService;

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


      

      




    }
}
