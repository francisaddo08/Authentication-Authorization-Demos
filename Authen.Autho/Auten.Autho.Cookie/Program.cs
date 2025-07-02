using Auten.Autho.Cookie.Services;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

namespace Auten.Autho.Cookie
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddDataProtection();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<AuthService>();

            var app = builder.Build();
            #region
            app.Use((ctx, next) => {

                var cookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("ProtectedAuth="));
                if(cookie != null)
                {
                    var protector = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
                    var cookieParts = cookie.Split("=").Last();
                    var cookieArray = cookieParts.Split(":");
                    var key = cookieArray[0];
                    var value = cookieArray[1];
                    ctx.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(key, value) }));
                }
                else
                {
                    ctx.User = new ClaimsPrincipal(new ClaimsIdentity());
                }
                return next();
            });
            #endregion


            app.MapGet("/", () => "Hello World!");

            #region 
            app.MapGet("/login", (HttpContext ctx) =>
            {
                ctx.Response.Headers["set-cookie"] = "auth=usr:addo";
                return Results.Ok("Login endpoint hit. You can implement your login logic here.");
            });
            app.MapGet("/user", (HttpContext ctx) => 
            { 
              var cookie = ctx.Request.Headers.Cookie.FirstOrDefault(h => h.StartsWith("auth="));
                var cookieParts = cookie?.Split("=").Last();
                var cookieArray = cookieParts?.Split(":");
                var key = cookieArray?[0];
                var value = cookieArray?[1];
                return $"{value}";
            });
            #endregion

            #region
            app.MapGet("/loginProtected", (HttpContext ctx, IDataProtectionProvider idp) =>
            {
                var cookieProtector = idp.CreateProtector("auth-cookie");
                ctx.Response.Headers["set-cookie"] = $"ProtectedAuth={cookieProtector.Protect("user:addo")}";
                return Results.Ok("Cookie data protected");
            });

            app.MapGet("/protected-user", (HttpContext ctx, IDataProtectionProvider idp) =>
            {
                var cookieProtector = idp.CreateProtector("auth-cookie");
                var cookie = ctx.Request?.Headers?.Cookie.FirstOrDefault(h => h.StartsWith("ProtectedAuth="));
                var cookiePart = cookie.Split("=").Last();
                var unprotectCookieArray = cookieProtector.Unprotect(cookiePart);
                var parts = unprotectCookieArray.Split(":");
                var key = parts[0];
                var value = parts[1];
                return $"{value}";
            });

            #endregion

            #region

            app.MapGet("/login-AuthService", (AuthService authservice) =>
            {
                authservice.Login();
                return Results.Ok("Cookie data protected using user service");
            });
            app.MapGet("/User-AuthService", (AuthService auth) =>
            {
                var user = auth.GetCookie();
                return $"{user}";
            });

            #endregion
            #region
            app.MapGet("/middleware-user", (HttpContext ctx) =>
            {
                return ctx.User;
               
            });
            #endregion
            app.Run();
        }
    }
}
