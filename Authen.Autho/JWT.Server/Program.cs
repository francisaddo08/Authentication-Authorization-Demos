using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;


internal class Program
{
    private static void Main(string[] args)
    {
        var rsaKey = RSA.Create();
        rsaKey.ImportRSAPrivateKey(File.ReadAllBytes("privateKey"), out _);

        var builder = WebApplication.CreateBuilder(args);
        // Add Authentication  services to the container.
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
           // options.MapInboundClaims = false;

        });
        builder.Services.AddAuthorization(options =>
        {
          
        });

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapGet("/", () =>
        {
            var urls = app.Urls.Select(s => new List<string> { s });
            return Results.Ok(new { Name = app.Environment.ApplicationName, urls });

        });
        app.MapGet("/login", () =>
        {
            //create a security key
            var securityKey = new RsaSecurityKey(rsaKey);
            //generate a token
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "https://localhost:7111",
                Subject = new ClaimsIdentity(new[]
                {
                  new Claim("sub", Guid.NewGuid().ToString()),
                    new Claim("name", "Addo"),
                }),
                
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256)


            };
            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            
            
            return token;
        });
        app.MapGet("/secure", () =>
        {

            return Results.Ok(new { Message = $" Secure resource!" });
        }).RequireAuthorization();


        app.Run();
    }
}