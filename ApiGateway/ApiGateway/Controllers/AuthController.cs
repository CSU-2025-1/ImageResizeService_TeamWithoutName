using ApiGateway.Models.Authentication;
using ApiGateway.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    /// <summary>
    /// Controller for handling authentication-related operations such as user registration and login.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<ImageProcessingController> _logger;
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service used to perform user registration and authentication.</param>
        /// <param name="logger">A logger for recording information about the controller.</param>
        public AuthController(ILogger<ImageProcessingController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user with the specified username and password.
        /// </summary>
        /// <param name="request">The registration request containing the username and password.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the registration attempt.<br/>
        /// Returns:<br/>
        ///   - <see cref="StatusCodes.Status200OK"/> (200 OK) If the user is successfully registered.<br/>
        ///   - <see cref="StatusCodes.Status400BadRequest"/> (400 BadRequest) Validation error.<br/>
        ///   - <see cref="StatusCodes.Status409Conflict"/> (409 Conflict) If the username already exists.<br/>
        ///   - <see cref="StatusCodes.Status500InternalServerError"/> (500 Internal Server Error) If a database error occurs during registration or an unexpected error is encountered.<br/>
        /// </returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var modelError in error.Value.Errors)
                    {
                        _logger.LogWarning($"Validation error for {error.Key}: {modelError.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Username = request.Username
            };

            var result = await _authService.Register(user, request.Password);
            _logger.LogInformation($"User {user.Username} was register.");

            return result switch
            {
                RegistrationResult.Success => Ok(new { message = "User registered successfully" }),
                RegistrationResult.UserAlreadyExists => Conflict(new { message = "Username already exists" }),
                RegistrationResult.DatabaseError => StatusCode(500, new { message = "An error occurred during registration. Please try again later." }),
                _ => StatusCode(500, new { message = "An unexpected error occurred." }),
            };
        }

        /// <summary>
        /// Authenticates the user and returns the JWT token in case of successful authentication.
        /// </summary>
        /// <param name="request">The request object containing the username and password.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the registration attempt.<br/>
        /// Returns:<br/>
        ///   - <see cref="StatusCodes.Status200OK"/> (200 OK) Successful authentication. Returns the JWT token..<br/>
        ///   - <see cref="StatusCodes.Status401Unauthorized"/> (401 Unauthorized) Invalid credentials.<br/>
        ///   - <see cref="StatusCodes.Status400BadRequest"/> (400 BadRequest) Validation error.<br/>
        ///   - <see cref="StatusCodes.Status500InternalServerError"/> (500 Internal Server Error) if a database error occurs during registration or an unexpected error is encountered.<br/>
        /// </returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var modelError in error.Value.Errors)
                    {
                        _logger.LogWarning($"Validation error for {error.Key}: {modelError.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }

            var result = await _authService.Authenticate(request.Username, request.Password);
            _logger.LogInformation($"User {request.Username} was authenticate.");

            return result.Status switch
            {
                AuthenticationResultStatus.Success => Ok(new { result.Token }),
                AuthenticationResultStatus.InvalidCredentials => Unauthorized(new { message = "Invalid username or password" }),
                AuthenticationResultStatus.DatabaseError => StatusCode(500, new { message = "An error occurred during authentication. Please try again later." }),
                _ => StatusCode(500, new { message = "An unexpected error occurred." }),
            };
        }
    }
}
