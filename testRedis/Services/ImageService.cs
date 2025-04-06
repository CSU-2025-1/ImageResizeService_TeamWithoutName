using System;
using System.IO;

public class ImageService
{
    private readonly RedisService _redisService;
    private readonly MongoDBService _mongoDBService;

    public ImageService(RedisService redisService, MongoDBService mongoDBService)
    {
        _redisService = redisService;
        _mongoDBService = mongoDBService;
    }

    public void SaveImage(string key, string base64Image)
    {
        try
        {
            if (string.IsNullOrEmpty(base64Image))
            {
                throw new ArgumentException("Base64-строка изображения пуста или null.");
            }

            byte[] imageBytes = Convert.FromBase64String(base64Image);

            _redisService.SaveImageToRedis(key, base64Image);
            _mongoDBService.SaveImageToMongoDB(key, base64Image);

            Console.WriteLine($"Изображение успешно сохранено под ключом: {key}");
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка: Переданная строка не является корректной Base64.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении изображения: {ex.Message}");
        }
    }

    public void GetImage(string key)
    {
        string base64Image = _redisService.GetImageFromRedis(key);

        if (!string.IsNullOrEmpty(base64Image))
        {
            OpenImageFromBase64(base64Image, $"Redis ({key})");
        }
        else
        {
            base64Image = _mongoDBService.GetImageFromMongoDB(key);

            if (!string.IsNullOrEmpty(base64Image))
            {
                OpenImageFromBase64(base64Image, $"MongoDB ({key})");
                _redisService.SaveImageToRedis(key, base64Image);
                Console.WriteLine($"Изображение заново закешировано в Redis.");
            }
            else
            {
                Console.WriteLine("Изображение с указанным ключом не найдено ни в Redis, ни в MongoDB.");
            }
        }
    }

    public void DeleteImage(string key)
    {
        _redisService.DeleteKeyFromRedis(key);
        _mongoDBService.DeleteKeyFromMongoDB(key);
    }

    private void OpenImageFromBase64(string base64Image, string source)
    {
        byte[] imageBytes = Convert.FromBase64String(base64Image);
        string tempImagePath = Path.GetTempFileName();
        File.WriteAllBytes(tempImagePath, imageBytes);

        var startInfo = new System.Diagnostics.ProcessStartInfo(tempImagePath)
        {
            UseShellExecute = true
        };
        System.Diagnostics.Process.Start(startInfo);

        Console.WriteLine($"Изображение успешно извлечено и открыто из {source}");
    }
}