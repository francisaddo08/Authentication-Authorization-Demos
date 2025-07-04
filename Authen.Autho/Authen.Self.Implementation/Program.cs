using Authen.Self.Implementation.HostingExtensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var app = builder.buildServices().ConfigurePipeline();


    

        app.Run();
    }
}

