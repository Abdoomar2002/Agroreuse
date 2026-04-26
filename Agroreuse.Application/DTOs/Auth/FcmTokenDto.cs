namespace Agroreuse.Application.DTOs.Auth
{
    /// <summary>
    /// DTO for registering or updating an FCM device token
    /// </summary>
    public class FcmTokenDto
    {
        /// <summary>
        /// The FCM device token received from Firebase on the mobile device
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}
