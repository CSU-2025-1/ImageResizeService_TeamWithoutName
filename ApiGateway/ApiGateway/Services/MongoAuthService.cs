using ApiGateway.Models.Authentication;
using ApiGateway.Services.Contract;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiGateway.Services
{
    /// <summary>
    /// An implementation of the IAuthService interface that uses MongoDB to store user data.
    /// </summary>
    public class MongoAuthService : IAuthService
    {
        private readonly ILogger<MongoAuthService> _logger;
        private readonly IMongoCollection<User> _users;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpirationInMinutes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoAuthService"/> class.
        /// </summary>
        /// <param name="config">Application configuration containing JWT settings.</param>
        /// <param name="logger">A logger for recording information about the service.</param>
        /// <param name="userCollection">A MongoDB collection that stores user data.</param>
        public MongoAuthService(IConfiguration config, ILogger<MongoAuthService> logger, IMongoCollection<User> userCollection)
        {
            _logger = logger;
            _users = userCollection;
            _jwtSecret = config["Jwt:Secret"];
            _jwtIssuer = config["Jwt:Issuer"];
            _jwtAudience = config["Jwt:Audience"];
            _jwtExpirationInMinutes = int.Parse(config["Jwt:ExpirationInMinutes"]);
        }

        /// <inheritdoc cref="IAuthService.Register(User, string)"/>
        public async Task<RegistrationResult> Register(User user, string password)
        {
            try
            {
                if (await _users.Find(u => u.Username == user.Username).AnyAsync())
                {
                    _logger.LogInformation($"User {user.Username} already exist.");
                    return RegistrationResult.UserAlreadyExists;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                await _users.InsertOneAsync(user);
                _logger.LogInformation($"User {user.Username} was added.");
                return RegistrationResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving user.");
                return RegistrationResult.DatabaseError;
            }
        }

        /// <inheritdoc cref="IAuthService.Authenticate(string, string)"/>
        public async Task<AuthenticationResult> Authenticate(string username, string password)
        {
            try
            {
                var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogInformation($"Invalid username or password.");
                    return new AuthenticationResult
                    {
                        Status = AuthenticationResultStatus.InvalidCredentials
                    };
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation($"For user {user.Username} was created token.");
                return new AuthenticationResult 
                {
                    Status = AuthenticationResultStatus.Success,
                    Token = token,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in authentication user.");
                return new AuthenticationResult
                {
                    Status = AuthenticationResultStatus.DatabaseError,
                };
            }
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The User object for which the token is being generated.</param>
        /// <returns>The string representing the JWT token.</returns>
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