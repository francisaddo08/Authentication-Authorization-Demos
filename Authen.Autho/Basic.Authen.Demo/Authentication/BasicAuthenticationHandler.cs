using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;

using Basic.Authen.Demo.Domain;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Basic.Authen.Demo.Authentication
{
    public class BasicAuthenticationHandler :AuthenticationHandler<AuthenticationSchemeOptions>
    {   
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }
        private List<string> _validUsers = new List<string>();
      
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Implement your authentication logic here
            var header = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(header) || !header.StartsWith("Basic "))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            // Decode the base64 encoded credentials
            ReadOnlySpan<char> headerAsSpan = header;
            headerAsSpan = headerAsSpan.Slice("Basic ".Length);
            var base64Credentials = Encoding.UTF8.GetString(Convert.FromBase64String(headerAsSpan.ToString()));
            // Split the credentials into username and password
            if (string.IsNullOrEmpty(base64Credentials))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            var credendentials = base64Credentials.Split(":");
            var username = credendentials[0];
            var password = credendentials.Length > 1 ? credendentials[1] : string.Empty;
            // Check if the user is valid
           
            try
            {
                var registeredUser = System.Text.Json.JsonSerializer.Deserialize<User> (File.ReadAllText("user.json"));
                if (registeredUser != null) 
                {
                  var passwordHasher = new PasswordHasher<User>();
                    var result = passwordHasher.VerifyHashedPassword(registeredUser, registeredUser.PasswordHash, password);
                    if (result == PasswordVerificationResult.Success && registeredUser.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                    {
                        var claims = new List<Claim>();
                        // Add role claims for the authenticated user
                      


                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);
                        return Task.FromResult(AuthenticateResult.Success(ticket));
                    }
                    else
                    {
                        return Task.FromResult(AuthenticateResult.Fail("Invalid username or password."));
                    }

                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail($"Error reading user file: {ex.Message}"));
            }

            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
   
}
