
using CRM.Controllers;
using Microsoft.AspNetCore;
using NLog.Web;
using Syncfusion.Pdf.Graphics;

namespace CRM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                //logger.Debug("init main function");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Information);
                    }).UseNLog();
                })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<ScheduledTaskService>();
            });

    
}
}
