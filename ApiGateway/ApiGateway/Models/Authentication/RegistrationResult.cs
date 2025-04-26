namespace ApiGateway.Models.Authentication
{
    /// <summary>
    /// An enumeration representing the possible results of the user registration operation.
    /// </summary>
    public enum RegistrationResult
    {
        /// <summary>
        /// Registration was successful.
        /// </summary>
        Success,

        /// <summary>
        /// The user with the specified username already exists in the system.
        /// </summary>
        UserAlreadyExists,

        /// <summary>
        /// An error occurred while interacting with the database.
        /// </summary>
        DatabaseError,
    }
}
