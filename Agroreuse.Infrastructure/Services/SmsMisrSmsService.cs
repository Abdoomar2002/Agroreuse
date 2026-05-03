using Agroreuse.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Agroreuse.Infrastructure.Services
{
    /// <summary>
    /// SMS service implementation using the SMS MISR OTP API.
    /// Configuration values are read from the "SmsMisr" section in appsettings.json.
    /// </summary>
    public class SmsMisrSmsService : ISmsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SmsMisrSmsService> _logger;
        private readonly string _environment;
        private readonly string _username;
        private readonly string _password;
        private readonly string _sender;
        private readonly string _template;
        private readonly string _baseUrl;

        public SmsMisrSmsService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<SmsMisrSmsService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            var section = configuration.GetSection("SmsMisr");
            _environment = section["Environment"] ?? "2";
            _username    = section["Username"]    ?? throw new InvalidOperationException("SmsMisr:Username is not configured.");
            _password    = section["Password"]    ?? throw new InvalidOperationException("SmsMisr:Password is not configured.");
            _sender      = section["Sender"]      ?? throw new InvalidOperationException("SmsMisr:Sender is not configured.");
            _template    = section["Template"]    ?? throw new InvalidOperationException("SmsMisr:Template is not configured.");
            _baseUrl     = section["BaseUrl"]     ?? "https://smsmisr.com/api/OTP/";
        }

        /// <summary>
        /// Sends an OTP via the SMS MISR dedicated OTP endpoint.
        /// </summary>
        public async Task<bool> SendOtpAsync(string phoneNumber, string otp)
        {
            try
            {
                var url = $"{_baseUrl}?environment={_environment}" +
                          $"&username={Uri.EscapeDataString(_username)}" +
                          $"&password={Uri.EscapeDataString(_password)}" +
                          $"&sender={Uri.EscapeDataString(_sender)}" +
                          $"&mobile={Uri.EscapeDataString(NormalizePhone(phoneNumber))}" +
                          $"&template={Uri.EscapeDataString(_template)}" +
                          $"&otp={Uri.EscapeDataString(otp)}";

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);

                var body = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("SMS MISR OTP response for {Phone}: {Status} {Body}",
                    phoneNumber, (int)response.StatusCode, body);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP via SMS MISR to {Phone}", phoneNumber);
                return false;
            }
        }

        /// <summary>
        /// Sends a free-text SMS message. Note: SMS MISR OTP API only supports OTP messages;
        /// for general messages a different endpoint would be needed. Falls back to logging.
        /// </summary>
        public Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            _logger.LogInformation("SendSmsAsync (general) called for {Phone}: {Message}", phoneNumber, message);
            // Free-text sending is not supported by the OTP endpoint.
            // Wire a transactional SMS endpoint here if needed in the future.
            return Task.FromResult(true);
        }

        private static string NormalizePhone(string phone)
        {
            // Strip leading + or spaces; SMS MISR expects digits only, e.g. 201025784881
            return phone.TrimStart('+').Replace(" ", "").Replace("-", "");
        }
    }
}
