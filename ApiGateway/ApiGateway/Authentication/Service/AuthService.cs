using ApiGateway.Authentication.Model;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;

namespace ApiGateway.Authentication.Service
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<User> _users;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpirationInMinutes;

        public AuthService(IConfiguration config, IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
            _jwtSecret = config["Jwt:Secret"];
            _jwtIssuer = config["Jwt:Issuer"];
            _jwtAudience = config["Jwt:Audience"];
            _jwtExpirationInMinutes = int.Parse(config["Jwt:ExpirationInMinutes"]);
        }

        public async Task<User> Register(User user, string password)
        {
            if (await _users.Find(u => u.Username == user.Username).AnyAsync())
            {
                throw new Exception("Username already exists");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<string> Authenticate(string username, string password)
        {
            var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new Exception("Invalid username or password");
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationInMinutes),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}