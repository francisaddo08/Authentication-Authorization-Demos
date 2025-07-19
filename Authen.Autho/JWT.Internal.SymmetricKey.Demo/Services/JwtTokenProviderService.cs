using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using System.Security.Claims;
using System.Text;

namespace JWT.Internal.SymmetricKey.Demo.Services
{
    public sealed class JwtTokenProviderService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenProviderService(IConfiguration configuration)
       {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public string CreateToken()
        {
            var secretKey = _configuration["Jwt:Secret:Key"]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub , "Demo"),
                    
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = signingCredentials
            };

            var tokenhandler = new JsonWebTokenHandler();
            var token = tokenhandler.CreateToken(securityTokenDescriptor);
            return token;


        }
    }
}
