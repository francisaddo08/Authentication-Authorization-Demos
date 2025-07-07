using Cookie.Identity.Management.DataStore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace Cookie.Identity.Management.Hosting
{
    public static class HostingExtentions
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
        {
            // Add authentication services
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", options =>
                {
                    options.RequireAuthenticatedUser().AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                          .RequireClaim("Role", "Admin");



                });

            });

            return services;
        }

        public static WebApplication AddPipelines(this WebApplication app)
        {
            app.UseAuthentication();
            return app;
        }
        public static WebApplication AddEndpoints(this WebApplication app)
        {

            app.MapGet("/", () => "Hello World!");
            app.MapGet("/register", async (string userName, string password, IPasswordHasher<User> hasher) =>
            {
                // Simulate user registration
                var user = new User
                {
                    UserName = userName
                };
                // Hash the password
                user.PasswordHash = hasher.HashPassword(user, password);
                // Save user to file
                await Database.StoreUserInFileAsync(user);
                return user;
            });
            app.MapGet("/login", async (string userName, string password, HttpContext context) =>
            {
                // Simulate a login
                var user = await Database.GetUserFromFile(userName);
                if (user == null)
                {
                    return Results.NotFound("Bad Credentials");
                }
                // Verify the password
                var hasher = app.Services.GetRequiredService<IPasswordHasher<User>>();
                var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (verificationResult != PasswordVerificationResult.Success)
                {
                    return Results.NotFound("Bad Credentials");
                }
                // Create claims and sign in the user
                var claims = user.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();  

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Results.Ok("User logged in");
            });

            app.MapGet("/admin", (HttpContext context) =>
            {
                // Check if the user is authenticated and has the Admin role
                if (context.User.Identity?.IsAuthenticated == true )
                {
                    return Results.Ok("Welcome to the admin area!");
                }
                return Results.Forbid();
            }).RequireAuthorization("Admin");

            return app;
        }

    }
}
