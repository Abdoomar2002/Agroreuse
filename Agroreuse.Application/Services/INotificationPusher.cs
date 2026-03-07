namespace Agroreuse.Application.Services
{
    public interface INotificationPusher
    {
        Task SendToUserAsync(string userId, object notification);
    }
}
