using Microsoft.AspNetCore.DataProtection;

namespace Authen.Self.Implementation.Cookie
{
    public class CookieAuthenService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IDataProtectionProvider _provider;

        public CookieAuthenService(IHttpContextAccessor accessor, IDataProtectionProvider provider)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }
        #region
        public void SetCookie(string key, string value)
        {
            var response = _accessor.HttpContext?.Response;
            if (response == null)
            {
                if (_accessor.HttpContext == null)
                {
                    throw new InvalidOperationException("HttpContext is not available.");
                }
            }
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                //Secure = true, // Set to true if using HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(1) // Set expiration as needed
            };
            response?.Cookies.Append(key, value, cookieOptions);
        }
        public string GetCookie()
        {
            var request = _accessor.HttpContext?.Request;
            if (request == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }
            if (request.Cookies.TryGetValue("User=", out var value))
            {
                return value ?? string.Empty;
            }
            else
            {
                return string.Empty;
            }

        }
        #endregion
        #region
        public void SetProtectedCookie(string key, string value)
        {
            var response = _accessor.HttpContext?.Response;
            if (response == null)
            {
                if (_accessor.HttpContext == null)
                {
                    throw new InvalidOperationException("HttpContext is not available.");
                }
            }
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                //Secure = true, // Set to true if using HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(1) // Set expiration as needed
            };
            var protector = _provider.CreateProtector("Cookie-protector");
            var protectedValue = protector.Protect(value);
            response?.Cookies.Append(key, protectedValue, cookieOptions);
        }
        public string GetProtectedCookie()
        {
            var request = _accessor.HttpContext?.Request;
            if (request == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }
            if (request.Cookies.TryGetValue("User=", out var value))
            {
                var protector = _provider.CreateProtector("Cookie-protector");
                return protector.Unprotect( value) ?? string.Empty;
            }
            else
            {
                return string.Empty;
            }

        }

        #endregion

        public bool ValidateCookie(string key)
        {

            return true;
        }
    }
}
