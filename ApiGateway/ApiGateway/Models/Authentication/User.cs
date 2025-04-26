using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiGateway.Models.Authentication
{
    /// <summary>
    /// Represents the user's identity in the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// A unique MongoDB user ID.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        [BsonElement("username")]
        public string Username { get; set; }

        /// <summary>
        /// The hash of the user's password.
        /// </summary>
        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }
    }
}
