using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace CRM.Utilities
{
    public class Unauthorized
    {
        private readonly RequestDelegate _next;

        public Unauthorized(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            var originalPath = context.Request.Path;


            await _next(context);

            if (context.Response.StatusCode == 401)
            {

                var services = context.RequestServices;
             
                var result = new 
                {
                    Message = "not auth", 
                    StatusCode = "401"
                };

                await context.Response.WriteAsync(result.ToJson());

                    
            }
        }
    }
}
