using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlanItNoww.Models;
using PlanItNoww.Services;
using PlanItNoww.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PlanItNoww.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        ILogger logger;
        ApplicationEnvironment applicationsettings;
        IHttpContextAccessor httpcontextaccessor;
        
        public JwtMiddleware(
            RequestDelegate next, ILogger<JwtMiddleware> logger,
            IOptions<ApplicationEnvironment> applicationsettings,
            IHttpContextAccessor httpcontextaccessor
            )
        {
            this.next = next;
            this.logger = logger;
            this.applicationsettings = applicationsettings.Value;
            this.httpcontextaccessor = httpcontextaccessor;
            
        }

        public async Task Invoke(HttpContext context, UsersService usersservice)
        {
            try
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token == null)
                {
                    token = httpcontextaccessor.HttpContext.Request.Cookies[AppConstants.AccessTokenKey];
                }
                if (token != null)
                    await attachUserToContext(context, token,usersservice);
                await next(context);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task attachUserToContext(HttpContext context, string token, UsersService usersservice)
        {
            try
            {
                httpcontextaccessor.HttpContext.Items["usercontext"] = usersservice.JwtTokenToUserContext(token);               
            }
            catch (Exception)
            {

            }
        }

    }
}
