using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

internal class Program
{
    private static void Main(string[] args)
    {
        var rsaKey = System.Security.Cryptography.RSA.Create();
        rsaKey.ImportRSAPrivateKey(File.ReadAllBytes("privateKey"), out _);

        var builder = WebApplication.CreateBuilder(args);
        // Add Authentication  services to the container.
        builder.Services.AddAuthentication("jwt")
        .AddJwtBearer("jwt", options =>
        {

        });
        var app = builder.Build();

        app.MapGet("/", () =>
        {
            var urls = app.Urls.Select(s => new List<string> { s }).ToList();
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

        app.Run();
    }
}