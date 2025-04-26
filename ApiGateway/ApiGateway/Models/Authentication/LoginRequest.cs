using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models.Authentication
{
    /// <summary>
    /// Submits a request for user authentication.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// The username for authentication.
        /// </summary>
        [Required(ErrorMessage = "The user's name is required.")]
        public string Username { get; set; }

        /// <summary>
        /// The user's password for authentication.
        /// </summary>
        [Required(ErrorMessage = "A password is required.")]
        public string Password { get; set; }
    }
}
