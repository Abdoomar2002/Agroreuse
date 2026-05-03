using System.Reflection;
using Agroreuse.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Agroreuse.Application;

/// <summary>
/// Dependency injection configuration for the Application layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Add Memory Cache for OTP storage
        services.AddMemoryCache();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IOtpService, OtpService>();

        return services;
    }
}
