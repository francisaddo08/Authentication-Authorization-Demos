using Microsoft.AspNetCore.Http.HttpResults;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddScoped<IMyService, MyService>();
        var app = builder.Build();

        //inline middleware
        app.Use((context, next) =>
        {
            //add custom header to every response
            context.Response.Headers.Append("X-Custom-Header", "Customer value");
            return next();
        });

        app.MapGet("/", (HttpContext context, IMyService myService) =>
        {
            return Results.Ok(myService.GetHello());
        });

        app.Run();
    }
}

public class MyMiddleware
{
    private readonly RequestDelegate _next;
    public MyMiddleware(RequestDelegate requestDelegate)
    {
        _next = requestDelegate;

    }
    public async Task Invoke(HttpContext context)
    {
        //add custom header to every response
        context.Response.Headers.Append("X-Custom-Header", "Customer value");
        await _next.Invoke(context);
    }
}
public static class MyMiddlewareExtensions
{
    public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MyMiddleware>();
    }
}

    public class MyService : IMyService
    {
        public string GetHello() => """Hello World """;

    }

    public interface IMyService
    {
        public string GetHello();
    }