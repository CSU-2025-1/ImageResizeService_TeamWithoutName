namespace ApiGateway.Models.Authentication
{
    /// <summary>
    /// Represents the result of an authentication operation.
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// Authentication status.
        /// </summary>
        public AuthenticationResultStatus Status { get; set; }

        /// <summary>
        /// A JWT (JSON Web Token) generated for an authenticated user.<br/>
        /// The value will be filled in only if authentication is successful (Status = AuthenticationResultStatus.Success).
        /// </summary>
        public string Token { get; set; } 
    }
}
