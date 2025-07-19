using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JWT.Internal.Issuer.Services
{
    public sealed class JsonProviderService
    {
        private readonly IConfiguration _config;

        public JsonProviderService(IConfiguration configuration)
        {
          _config = configuration;

        }
        public string CreateToken()
        {
            string secretKey = _config["Jwt:Secret"]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));  
            return string.Empty;

        }

    }
}
