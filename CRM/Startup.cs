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
                options.IdleTimeout = TimeSpan.FromHours(1); // Set session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddDbContext<admin_NDCrMContext>(options => options.UseSqlServer(Configuration.GetConnectionString("db1")));
            services.AddControllersWithViews();

            var Key = "this is my test key";
            services.AddSingleton<IJwtToken>(new JwtToken(Key));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Configure authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Identity.Application";
                options.DefaultChallengeScheme = "Identity.Application";
            })
            .AddCookie("Identity.Application", options =>
            {
                options.LoginPath = "/Admin/Login";
                options.LogoutPath = "/Admin/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

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
            app.UseMiddleware<Unauthorized>();

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
            app.UseAuthentication(); // Ensure this is added
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
