using CRM.Models.Crm;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

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
    }
}
