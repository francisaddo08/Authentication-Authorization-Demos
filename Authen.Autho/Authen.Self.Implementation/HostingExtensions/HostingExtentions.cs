using Authen.Self.Implementation.Cookie;
using Newtonsoft.Json;
namespace Authen.Self.Implementation.HostingExtensions
{
    public static class HostingExtentions
    {
        public static IServiceCollection AddFrameworkServices(this IServiceCollection services)
        {
            // Register the HttpContextAccessor
            services.AddHttpContextAccessor();
            // Register the IDataProtectionProvider
            services.AddDataProtection();
            
            return services;
        }
        public static IServiceCollection AddAppilicationServices(this IServiceCollection services)
        {
            // Register the CookieAuthenService
            services.AddScoped<CookieAuthenService>();
            return services;
        }
        public static WebApplication buildServices(this WebApplicationBuilder builder)
        {
            // Add framework services
            builder.Services.AddFrameworkServices();
            // Add application services
            builder.Services.AddAppilicationServices();

            return builder.Build();
        }
        public static WebApplication AddCookieEndpoints(this WebApplication app)
        {
            app.Map("/", () => "Authen.Self.Implementation is running!");
            #region
            app.MapGet("cookie/login",  (CookieAuthenService cookieService, HttpContext ctx) =>
            {
                cookieService.SetCookie("User", "Addo");
                return Results.Ok( new {Key = "User", Value = "Addo"});
            });
            app.MapGet("cookie/getuser", (CookieAuthenService cookieService, HttpContext ctx) =>
            {
                var user = cookieService.GetCookie();
                if (string.IsNullOrEmpty(user))
                {
                    return Results.NotFound("User not found in cookies.");
                }
                return Results.Ok(user);
            });
            #endregion
            #region
            app.MapGet("cookie/protectedlogin", (CookieAuthenService cookieService, HttpContext ctx) =>
            {
                cookieService.SetProtectedCookie("User", "Addo");
                return Results.Ok(new { Key = "User=", Value = "Addo" });
            });
            app.MapGet("cookie/getprotecteduser", (CookieAuthenService cookieService, HttpContext ctx) =>
            {
                var user = cookieService.GetProtectedCookie();
                if (string.IsNullOrEmpty(user))
                {
                    return Results.NotFound("User not found in cookies.");
                }
                return Results.Ok(user);
            });
            #endregion
            return app; 
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
           app.AddCookieEndpoints();

            return app;
        }
    }
}
