using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddAuthentication("Cookie")
        .AddCookie("MiddlewareCookie", options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/accessdenied";

            });
        var app = builder.Build();
        // Configure the HTTP request pipeline.
        app.UseAuthentication();


        app.MapGet("/", () => "Server is running");
        app.MapGet("/login", (HttpContext ctx) =>
        {
            // Simulate a login
            // In a real application, you would validate user credentials here
            var claims = new List<Claim> {
                 new Claim("User", "Addo"),
                    new Claim(ClaimTypes.Role, "Admin")
                 };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookie");
            ctx.SignInAsync("Cookie", new System.Security.Claims.ClaimsPrincipal(claimsIdentity));
            return "Logged in";

        });

        app.Run();
    }
}