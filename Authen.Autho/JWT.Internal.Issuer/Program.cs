using JWT.Internal.Issuer.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddSingleton<JsonProviderService>();
        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");
        app.MapGet("/login", (JsonProviderService jsonProviderService) =>
        {
            
           
            return jsonProviderService.CreateToken();

        });

        app.Run();
    }
}