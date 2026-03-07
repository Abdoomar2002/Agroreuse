using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Agroreuse.Application.Services
{
    public interface IOtpService
    {
        string GenerateOtp(string email);
        bool ValidateOtp(string email, string otp);
        void InvalidateOtp(string email);
    }

    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<OtpService> _logger;
        private const int OTP_LENGTH = 6;
        private const int OTP_EXPIRATION_MINUTES = 5;

        public OtpService(IMemoryCache cache, ILogger<OtpService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public string GenerateOtp(string email)
        {
            var otp = GenerateRandomOtp();
            var cacheKey = GetCacheKey(email);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(OTP_EXPIRATION_MINUTES));

            _cache.Set(cacheKey, otp, cacheOptions);

            _logger.LogInformation("OTP generated for {Email}, expires in {Minutes} minutes", email, OTP_EXPIRATION_MINUTES);

            return otp;
        }

        public bool ValidateOtp(string email, string otp)
        {
            var cacheKey = GetCacheKey(email);

            if (_cache.TryGetValue(cacheKey, out string? storedOtp))
            {
                if (storedOtp == otp)
                {
                    _logger.LogInformation("OTP validated successfully for {Email}", email);
                    return true;
                }
            }

            _logger.LogWarning("Invalid OTP attempt for {Email}", email);
            return false;
        }

        public void InvalidateOtp(string email)
        {
            var cacheKey = GetCacheKey(email);
            _cache.Remove(cacheKey);
            _logger.LogInformation("OTP invalidated for {Email}", email);
        }

        private string GenerateRandomOtp()
        {
            var random = new Random();
            var otp = string.Empty;

            for (int i = 0; i < OTP_LENGTH; i++)
            {
                otp += random.Next(0, 10).ToString();
            }

            return otp;
        }

        private string GetCacheKey(string email)
        {
            return $"OTP_{email.ToLowerInvariant()}";
        }
    }
}
