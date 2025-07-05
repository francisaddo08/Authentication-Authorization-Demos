using Microsoft.AspNetCore.Authentication.Cookies;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container
        builder.Services.AddAuthentication()
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
          var app = builder.Build(CookieAuthenticationDefaults.AuthenticationScheme);

        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}