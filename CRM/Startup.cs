using CRM.Models.CRM;
using CRM.Models.Crm;
using CRM.Repository;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using CRM.Models.DTO;
using BaselineTypeDiscovery;
using Rotativa.AspNetCore;
using System.Security.Policy;
using CRM.Utilities;
using CRM.IUtilities;
using Microsoft.TeamFoundation.TestManagement.WebApi;

namespace CRM
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session timeout
            });

            services.AddDbContext<admin_NDCrMContext>(options => options.UseSqlServer(Configuration.GetConnectionString("db1")));
           

            services.AddControllersWithViews();
            services.AddScoped<ICrmrpo, Crmrpo>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IApiAccount, ApiAccount>();
            services.AddScoped<IEmployee, Employee>();
            services.Configure<URL>(Configuration.GetSection("URL"));

            services.AddCors(options => {
                options.AddPolicy("CorsPolicy", builder => {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });     // Other service configurations


        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Admin}/{action=Login}/{id?}");
            });
        }
    }
}
