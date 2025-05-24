using ApiGateway.Models.Authentication;

namespace ApiGateway.Services.Contract
{
    /// <summary>
    /// The interface of the authentication service that defines methods for user registration and authentication.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="user">The User object containing information about the user for registration.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A task representing an asynchronous registration operation. Returns a RegistrationResult object containing information about registration success or failure, as well as possible errors.</returns>
        Task<RegistrationResult> Register(User user, string password);

        /// <summary>
        /// Authenticates the user with the provided username and password.
        /// </summary>
        /// <param name="username">User`s name.</param>
        /// <param name="password">User`s password.</param>
        /// <returns>A task representing an asynchronous authentication operation. Returns an AuthenticationResult object containing information about authentication success or failure, as well as a JWT token if successful.</returns>
        Task<AuthenticationResult> Authenticate(string username, string password);
    }
}
