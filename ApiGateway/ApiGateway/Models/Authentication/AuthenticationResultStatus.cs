namespace ApiGateway.Models.Authentication
{
    /// <summary>
    /// An enumeration representing the possible statuses of the authentication result.
    /// </summary>
    public enum AuthenticationResultStatus
    {
        /// <summary>
        /// Authentication was successful.
        /// </summary>
        Success,

        /// <summary>
        /// Incorrect credentials were provided (incorrect username or password).
        /// </summary>
        InvalidCredentials,

        /// <summary>
        /// An error occurred while interacting with the database.
        /// </summary>
        DatabaseError,
    }
}
