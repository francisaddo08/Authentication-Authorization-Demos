using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

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
                Expires = DateTimeOffset.UtcNow.AddMinutes(5) // Set expiration as needed
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
            if (request.Cookies.TryGetValue("User", out var value))
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
                Expires = DateTimeOffset.UtcNow.AddMinutes(5) // Set expiration as needed
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
            if (request.Cookies.TryGetValue("User", out var value))
            {
                var protector = _provider.CreateProtector("Cookie-protector");
                return protector.Unprotect(value) ?? string.Empty;
            }
            else
            {
                return string.Empty;
            }

        }

        #endregion

        public bool SetClaimsIsSuccess()
        {
            var user = GetCookie() ?? GetProtectedCookie() ?? string.Empty;
            if (string.IsNullOrEmpty(user))
            {
                return false; // No user found in cookies
            }
            //list of claims
            var claims = new List<Claim>
            {
                new Claim("User", user),
                new Claim(ClaimTypes.Role, "Admin"),
                // Add more claims as needed
            };
            // Create a ClaimsIdentity
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            // Create a ClaimsPrincipal
            var principal = new ClaimsPrincipal(identity);
            // Set the user in the current HttpContext
            if (_accessor.HttpContext != null)
            {
                _accessor.HttpContext.User = principal;
            }
            else
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }
            return true; // Claims set successfully
        }
        public void ClearCookies()
        {
            var response = _accessor.HttpContext?.Response;
            if (response == null)
            {
                if (_accessor.HttpContext == null)
                {
                    throw new InvalidOperationException("HttpContext is not available.");
                }
            }
            response?.Cookies.Delete("User");
            response?.Cookies.Delete("User=");
        }
        public void ExpireCookies()
        {
            var response = _accessor.HttpContext?.Response;
            var request = _accessor.HttpContext?.Request;   
            if (response == null || request == null)
            {
               
                    throw new InvalidOperationException("HttpContext is not available.");
                
            }
            response?.Cookies.Append("User", "", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1) // Set expiration to the past to expire the cookie
            });

            // alternative
            var httpCookieUser = request.Cookies["User"];
            if (httpCookieUser != null)
            {
                response?.Cookies.Append("User", "", new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(-1) // Set expiration to the past to expire the cookie
                });
            }
            var httpCookieUserProtected = request.Cookies["User="];
            if (httpCookieUserProtected != null)
            {
                response?.Cookies.Append("User=", "", new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(-1) // Set expiration to the past to expire the cookie
                });
            }
        }
    }
}
