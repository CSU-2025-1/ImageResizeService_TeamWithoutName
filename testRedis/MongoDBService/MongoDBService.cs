using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Linq;

public class MongoDBService
{
    private readonly IMongoCollection<ImageData> _imagesCollection;

    public MongoDBService(string connectionString, string dbName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(dbName);
        _imagesCollection = database.GetCollection<ImageData>("Images");
    }

    // Метод для сохранения изображения в MongoDB
    public void SaveImageToMongoDB(string key, string base64Image)
    {
        var imageData = new ImageData
        {
            Id = key,
            Base64Content = base64Image
        };

        _imagesCollection.ReplaceOne(
            filter: Builders<ImageData>.Filter.Eq(x => x.Id, key),
            replacement: imageData,
            options: new ReplaceOptions { IsUpsert = true }
        );

        Console.WriteLine($"Изображение успешно сохранено в MongoDB под ключом: {key}");
    }

    // Метод для получения изображения из MongoDB
    public string GetImageFromMongoDB(string key)
    {
        var filter = Builders<ImageData>.Filter.Eq(x => x.Id, key);
        var imageData = _imagesCollection.Find(filter).FirstOrDefault();

        if (imageData != null && !string.IsNullOrEmpty(imageData.Base64Content))
        {
            return imageData.Base64Content;
        }

        Console.WriteLine($"Изображение с ключом \"{key}\" не найдено в MongoDB.");
        return null;
    }

    // Метод для удаления ключа из MongoDB
    public bool DeleteKeyFromMongoDB(string key)
    {
        var filter = Builders<ImageData>.Filter.Eq(x => x.Id, key);
        var deleteResult = _imagesCollection.DeleteOne(filter);

        if (deleteResult.DeletedCount > 0)
        {
            Console.WriteLine($"Ключ \"{key}\" успешно удален из MongoDB.");
            return true;
        }
        else
        {
            Console.WriteLine($"Ключ \"{key}\" не найден в MongoDB.");
            return false;
        }
    }
}