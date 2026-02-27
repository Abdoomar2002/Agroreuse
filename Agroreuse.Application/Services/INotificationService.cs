using Agroreuse.Domain.Entities;

namespace Agroreuse.Application.Services
{
    public interface INotificationService
    {
        Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default);
    }
}
