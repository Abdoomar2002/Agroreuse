using Agroreuse.Application.Services;

namespace Agroreuse.Server.Services
{
    /// <summary>
    /// A composite notification pusher that sends notifications through multiple channels
    /// (SignalR for real-time web and FCM for mobile push notifications)
    /// </summary>
    public class CompositeNotificationPusher : INotificationPusher
    {
        private readonly SignalRNotificationPusher _signalRPusher;
        private readonly FcmNotificationPusher _fcmPusher;
        private readonly ILogger<CompositeNotificationPusher> _logger;

        public CompositeNotificationPusher(
            SignalRNotificationPusher signalRPusher,
            FcmNotificationPusher fcmPusher,
            ILogger<CompositeNotificationPusher> logger)
        {
            _signalRPusher = signalRPusher;
            _fcmPusher = fcmPusher;
            _logger = logger;
        }

        public async Task SendToUserAsync(string userId, object notification)
        {
            var tasks = new List<Task>();

            // Send via SignalR for real-time web clients
            tasks.Add(SafeSendAsync(() => _signalRPusher.SendToUserAsync(userId, notification), "SignalR", userId));

            // Send via FCM for mobile clients
            tasks.Add(SafeSendAsync(() => _fcmPusher.SendToUserAsync(userId, notification), "FCM", userId));

            await Task.WhenAll(tasks);
        }

        private async Task SafeSendAsync(Func<Task> sendAction, string channel, string userId)
        {
            try
            {
                await sendAction();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification via {Channel} to user {UserId}", channel, userId);
            }
        }
    }
}
