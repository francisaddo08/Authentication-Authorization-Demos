using JWT.Internal.SymmetricKey.Demo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var securityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Description = "Enter your JWT Bearer token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",


            };
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
            var securityRequirement = new OpenApiSecurityRequirement();
            var securityRequirementScheme = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            };
            securityRequirement.Add(securityRequirementScheme, new string[] { });

            options.AddSecurityRequirement(securityRequirement);




        });
        builder.Services.AddSingleton<JwtTokenProviderService>();
        //configure authenticatuon with jwt bearer
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
           options =>
           {
               options.RequireHttpsMetadata = false; // Set to true in production
               options.TokenValidationParameters = new TokenValidationParameters()
               {
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret:Key"]!)),
                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                   ValidAudience = builder.Configuration["Jwt:Audience"],
                   ClockSkew = TimeSpan.Zero,

               };


           });
        builder.Services.AddAuthorization();

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Symmetric V1");
                options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });
        }
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/", () => "Hello World!");
        app.MapGet("/login", (JwtTokenProviderService tokenProviderService) =>
        {
            var token = tokenProviderService.CreateToken();
            return token;

        });
        app.MapGet("/Secure", (HttpContext httpContext) =>
        {
            var user = httpContext.User;
            var sub = user.FindFirst("sub")?.Value ?? "Unknown";
            return Results.Ok($"Secured enpoint reached by");

        }).RequireAuthorization();

        app.Run();
    }
}