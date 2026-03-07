using Agroreuse.Application.Services;
using Agroreuse.Domain.Repositories;
using Agroreuse.Infrastructure.Persistence;
using Agroreuse.Infrastructure.Persistence.Repositories;
using Agroreuse.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Agroreuse.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ArgoreuseContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ArgoreuseContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register specific repositories
        services.AddScoped<ISampleEntityRepository, SampleEntityRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Register file upload service
        services.AddScoped<IFileUploadService, FileUploadService>();

        // Notification service
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();

        // Register email service (mock for now)
        services.AddScoped<IEmailService, MockEmailService>();

        return services;
    }
}
