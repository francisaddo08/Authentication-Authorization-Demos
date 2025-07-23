using Basic.Authen.Demo.Authentication;
using Basic.Authen.Demo.Domain;
using Basic.Authen.Demo.Dtos;
using Basic.Authen.Demo.HostingExtention;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ApplicationName = typeof(Program).Assembly.FullName
        });
        
        
        //add swagger
       builder.Services.AddSwaggerWithAuth();

        // Add services to the container.
        builder.Services.AddBasicAuthentication();
        builder.Services.AddBasicAuthorization();
      
        var app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        if(app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basic Auth Demo API V1");
                c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });
        }
        
     

        app.MapGet("/", () => "Hello World!");
        app.MapPost("/admin", async Task<IResult> ([FromBody] RegisterDto register) =>
        {
            if (string.IsNullOrEmpty(register.Username) || string.IsNullOrEmpty(register.Password))
            {
                return Results.BadRequest("Username and password are required.");
            }
            var user = new User(register.Username, register.Password);
            // Hash the password
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, register.Password);

            var userJson = JsonSerializer.Serialize(user);
            try
            {
                await File.WriteAllTextAsync("user.json", userJson);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error saving user: {ex.Message}");
            }

            return Results.Ok("User registered successfully. You can now use Basic Authentication with the provided credentials.");

        }).WithTags("Admin")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);


        app.MapGet("/secure", () => "This is a secure endpoint")
            .RequireAuthorization(); // Require authentication for this endpoint

        app.Run();
    }
}
