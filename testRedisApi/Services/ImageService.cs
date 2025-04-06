namespace testRedisApi.Services;
using System;
using System.IO;
using SixLabors.ImageSharp;
using testRedisApi.MongoDBService; 
using testRedisApi.RedisService;   
using testRedisApi.Models;

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
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            using var memoryStream = new MemoryStream(imageBytes);
            using var image = Image.Load(memoryStream);

            // Сохраняем изображение обратно в формате JPEG
            using var outputMemoryStream = new MemoryStream();
            image.Save(outputMemoryStream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
            string updatedBase64Image = Convert.ToBase64String(outputMemoryStream.ToArray());

            _redisService.SaveImageToRedis(key, updatedBase64Image);
            _mongoDBService.SaveImageToMongoDB(key, updatedBase64Image);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении изображения: {ex.Message}");
        }
    }

    public string GetImage(string key)
    {
        string base64Image = _redisService.GetImageFromRedis(key);

        if (!string.IsNullOrEmpty(base64Image))
        {
            return base64Image; // Возвращаем изображение из Redis
        }
        else
        {
            base64Image = _mongoDBService.GetImageFromMongoDB(key);

            if (!string.IsNullOrEmpty(base64Image))
            {
                _redisService.SaveImageToRedis(key, base64Image); // Кэшируем в Redis
                return base64Image; // Возвращаем изображение из MongoDB
            }
            else
            {
                Console.WriteLine("Изображение с указанным ключом не найдено ни в Redis, ни в MongoDB.");
                return null; // Возвращаем null, если изображение не найдено
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