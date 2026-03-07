using Agroreuse.Application.Services;
using Agroreuse.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Agroreuse.Server.Services
{
    public class SignalRNotificationPusher : INotificationPusher
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationPusher(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(string userId, object notification)
        {
            await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", notification);
        }
    }
}
