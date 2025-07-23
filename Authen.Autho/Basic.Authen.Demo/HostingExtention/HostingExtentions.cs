using Basic.Authen.Demo.Authentication;
using Basic.Authen.Demo.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

namespace Basic.Authen.Demo.HostingExtention
{
    public static class HostingExtentions
    {
        public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Basic Auth Demo API", Version = "v1" });
                var securityScheme = new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme",


                };
                options.AddSecurityDefinition("Basic", securityScheme);
                // Define the security requirement
                var securityRequirementScheme = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Basic"
                    }
                };
                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(securityRequirementScheme, new string[] { });

                options.AddSecurityRequirement(securityRequirement);

            });


            return services;
        }
        public static IServiceCollection AddBasicAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication("BasicAuthentication")
           .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            return services;
        }
        public static IServiceCollection AddBasicAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                var authorizationRequirement = new IAuthorizationRequirement[] { new RolesAuthorizationRequirement(new[]{"Admin"}) };
                options.AddPolicy("", new AuthorizationPolicy(authorizationRequirement, []) { });
            });

            return services;
        }
        public static IServiceCollection AddApplicationServices(this WebApplicationBuilder builder)
        {
            
            return builder.Services;
        }
    }
}
