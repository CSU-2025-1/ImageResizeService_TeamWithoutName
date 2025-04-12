using MongoDB.Bson.Serialization.Attributes;
namespace testRedisApi.Models;

public class ImageData
{
    [BsonId] 
    public string Id { get; set; }

    public string Base64Content { get; set; }
}