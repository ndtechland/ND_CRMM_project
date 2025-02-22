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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using CRM.Controllers;
using Employee = CRM.Repository.Employee;
using Hangfire;
using DinkToPdf.Contracts;
using DinkToPdf;
using jsreport.AspNetCore;
using jsreport.Local;
using jsreport.Binary;
using CRM.Data;

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
            // Session configuration
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1); // Set session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Database context
            services.AddDbContext<admin_NDCrMContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("db1")));
            services.AddDbContext<Jobforindia_HireJobContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("db2")));
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddControllersWithViews();

            // JWT Token configuration
            var Key = "8Zz5tw0Ionm3XPZZfN0NOml3z9FMfmpgXwovR9fp6ryDIoGRM8EPHAB6iHsc0fb";
            services.AddSingleton<IJwtToken>(new JwtToken(Key));

            // Authentication (JWT + Cookie)
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Admin/Login";
                options.LogoutPath = "/Admin/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            // Add services
            services.AddScoped<ICrmrpo, Crmrpo>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IApiAccount, ApiAccount>();
            services.AddScoped<IEmployee, Employee>();
            services.AddScoped<IHome, Home>();
            services.AddScoped<Dcrypt>();
            services.AddScoped<Encrypt>();
           services.AddScoped<ApplicationContextDapper>();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            ///added new 
            services.AddHostedService<ScheduledTaskService>();
            services.Configure<URL>(Configuration.GetSection("URL"));

            // CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            services.AddJsReport(new LocalReporting()
      .UseBinary(JsReportBinary.GetBinary())
      .AsUtility()
      .Create());

            //services.AddHangfire(config =>
            //{
            //    config.UseSqlServerStorage(Configuration.GetConnectionString("db1"));
            //});
            // Controllers with views
            services.AddControllersWithViews();
            //services.AddHangfireServer();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
         
            // Use custom unauthorized middleware
            app.UseMiddleware<Unauthorized>();
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            //app.UseHangfireServer();
            //app.UseHangfireDashboard("/hangfire");
            // Routing and CORS
            app.UseRouting();
            app.UseCors("CorsPolicy");

            // Authentication and Authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Endpoint routing
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                pattern: "{controller=Admin}/{action=Login}/{id?}");
            });
        }
    }
}
