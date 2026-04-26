using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Agroreuse.Infrastructure.Services;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;

namespace Agroreuse.Server.Services
{
    public class FcmNotificationPusher : INotificationPusher
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<FcmNotificationPusher> _logger;
        private readonly FirebaseMessaging _messaging;

        public FcmNotificationPusher(
            UserManager<ApplicationUser> userManager,
            ILogger<FcmNotificationPusher> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _messaging = FirebaseMessaging.DefaultInstance;
        }

        public async Task SendToUserAsync(string userId, object notification)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found for FCM notification", userId);
                    return;
                }

                if (string.IsNullOrEmpty(user.FcmDeviceToken))
                {
                    _logger.LogDebug("User {UserId} has no FCM device token registered", userId);
                    return;
                }

                // Extract notification properties using reflection or dynamic
                var notificationData = notification as NotificationMessage;

                string title = notificationData?.Title?.ToString() ?? "Notification";
                string body = notificationData?.Message?.ToString() ?? "";

                var message = new Message
                {
                    Token = user.FcmDeviceToken,
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Data = ConvertToStringDictionary(notification)
                };

                var response = await _messaging.SendAsync(message);
                _logger.LogInformation("FCM notification sent successfully to user {UserId}. Message ID: {MessageId}", userId, response);
            }
            catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode == MessagingErrorCode.Unregistered)
            {
                _logger.LogWarning("FCM token for user {UserId} is no longer valid. Clearing token.", userId);
                await ClearUserFcmTokenAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send FCM notification to user {UserId}", userId);
            }
        }

        private async Task ClearUserFcmTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.FcmDeviceToken = null;
                await _userManager.UpdateAsync(user);
            }
        }

        private static Dictionary<string, string> ConvertToStringDictionary(object obj)
        {
            var dict = new Dictionary<string, string>();

            if (obj == null) return dict;

            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj);
                if (value != null)
                {
                    dict[prop.Name] = value.ToString() ?? "";
                }
            }

            return dict;
        }
    }
}
