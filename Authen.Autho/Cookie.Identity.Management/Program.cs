using Cookie.Identity.Management.Hosting;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container

        builder.Services.AddDotNetServices();

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        app.AddPipelines();
        app.AddEndpoints();

        app.Run();
    }
}