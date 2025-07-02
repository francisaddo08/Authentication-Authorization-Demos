using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

namespace Auten.Autho.Cookie.Services
{
    public class AuthService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IHttpContextAccessor ctx, IDataProtectionProvider idp)
        {
            _dataProtectionProvider = idp ?? throw new ArgumentNullException(nameof(idp));
            _httpContextAccessor = ctx ?? throw new ArgumentNullException(nameof(ctx));

        }
        public void Login()
        {
            var cookieProtector = _dataProtectionProvider.CreateProtector("auth-cookie");
            var cookieValue = cookieProtector.Protect("user:addo");
            _httpContextAccessor.HttpContext.Response.Headers["set-cookie"] = $"auth=protected{cookieValue}";

        }
        public string GetCookie()
        {
            var cookie = _httpContextAccessor.HttpContext.Request.Headers.Cookie.FirstOrDefault(h => h.StartsWith("auth=protected"));
            if (cookie == null)
            {
                return "You are not authenticated";
            }
            var cookiePart = cookie.Split("=").Last();
            var protector = _dataProtectionProvider.CreateProtector("auth-cookie");
            var unprotected = protector.Unprotect(cookiePart);
            var cookieArray = unprotected.Split(":");
            var key = cookieArray[0];
            var value = cookieArray[1];
            return value;
        }
      
    }
}
