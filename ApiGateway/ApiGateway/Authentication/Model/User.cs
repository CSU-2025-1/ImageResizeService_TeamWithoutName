using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiGateway.Authentication.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } // Хэш пароля
    }
}
