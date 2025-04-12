using StackExchange.Redis;
using System;
using System.IO;

public class RedisService
{
    private readonly IDatabase _db;

    public RedisService(ConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    // Метод для сохранения изображения в Redis
    public void SaveImageToRedis(string key, string base64Image)
    {
        _db.StringSet(key, base64Image);
        Console.WriteLine($"Изображение успешно сохранено в Redis под ключом: {key}");
    }

    // Метод для получения изображения из Redis
    public string GetImageFromRedis(string key)
    {
        return _db.StringGet(key);
    }

    // Метод для удаления ключа из Redis
    public bool DeleteKeyFromRedis(string key)
    {
        if (_db.KeyExists(key))
        {
            _db.KeyDelete(key);
            Console.WriteLine($"Ключ \"{key}\" успешно удален из Redis.");
            return true;
        }
        else
        {
            Console.WriteLine($"Ключ \"{key}\" не найден в Redis.");
            return false;
        }
    }

    // Метод для показа всех ключей в Redis
    public void ShowAllKeysInRedis()
    {
        try
        {
            var server = _db.Multiplexer.GetServer("localhost:6379");
            var keys = server.Keys(pattern: "*");

            if (keys.Any())
            {
                Console.WriteLine("Список ключей в Redis:");
                foreach (var key in keys)
                {
                    Console.WriteLine(key);
                }
            }
            else
            {
                Console.WriteLine("В Redis нет сохраненных ключей.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}