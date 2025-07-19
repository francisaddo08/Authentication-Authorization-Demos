using Microsoft.AspNetCore.Authentication.JwtBearer;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // add services , this is where we configure the JWT authentication and short-circuiting behavior
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => 
        {
            options.Events = new JwtBearerEvents()
            {
                OnMessageReceived = (context) =>
                {
                    return Task.CompletedTask;

                }

            };

        });
        builder.Services.AddAuthorization();
        var app = builder.Build();
        // add middleware
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/", () =>
        {

            return Results.Ok( new { Message = " This is a short circuit example for JWT authentication." });
        });
        app.MapGet("/token", () => "Hello World!");
        app.MapGet("/secure", (HttpContext ctx) =>
        {
            return Results.Ok($"Secure Endpoint Accessed by {ctx.User.Identity?.Name}");
        }).RequireAuthorization();


        app.Run();
    }
}