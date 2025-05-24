using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models.Authentication
{
    /// <summary>
    /// Submits a request to register a new user.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// The username for registration.
        /// </summary>
        [Required(ErrorMessage = "The user's name is required.")]
        public string Username { get; set; }

        /// <summary>
        /// The user's password for registration.
        /// </summary>
        [Required(ErrorMessage = "A password is required.")]
        public string Password { get; set; }
    }
}
