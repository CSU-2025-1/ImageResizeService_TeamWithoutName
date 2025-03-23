using System;
using System.Diagnostics;
using System.IO;
using MongoDB.Driver;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using StackExchange.Redis;

// Класс данных для MongoDB
public class ImageData
{
    public string Id { get; set; }
    public string Base64Content { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        // Инициализация сервисов
        var redis = ConnectionMultiplexer.Connect("redis:6379");
        var redisService = new RedisService(redis);

        // Создание объекта MongoDBService вместо использования MongoDatabase
        var mongoDBService = new MongoDBService("mongodb://mongodb:27017", "ImageDatabase");

        Console.WriteLine("Выберите действие:");
        Console.WriteLine("1. Сохранить изображение в Redis и MongoDB");
        Console.WriteLine("2. Получить изображение из Redis или MongoDB");
        Console.WriteLine("3. Показать все ключи в Redis");
        Console.WriteLine("4. Удалить ключ из Redis и MongoDB");
        Console.WriteLine("5. Выйти");

        while (true)
        {
            Console.Write("Введите номер действия: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SaveImageToServices(redisService, mongoDBService);
                    break;
                case "2":
                    GetImageFromServices(redisService, mongoDBService);
                    break;
                case "3":
                    redisService.ShowAllKeysInRedis();
                    break;
                case "4":
                    DeleteKeyFromServices(redisService, mongoDBService);
                    break;
                case "5":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    // Метод для сохранения изображения в Redis и MongoDB
    static void SaveImageToServices(RedisService redisService, MongoDBService mongoDBService)
    {
        Console.Write("Введите путь к изображению: ");
        string imagePath = Console.ReadLine();

        try
        {
            using (var imageStream = File.OpenRead(imagePath))
            using (var memoryStream = new MemoryStream())
            {
                using var image = Image.Load(imageStream);
                image.Save(memoryStream, new JpegEncoder());
                byte[] imageBytes = memoryStream.ToArray();
                string base64Image = Convert.ToBase64String(imageBytes);

                Console.Write("Введите ключ для изображения: ");
                string key = Console.ReadLine();

                // Сохраняем в Redis
                redisService.SaveImageToRedis(key, base64Image);

                // Сохраняем в MongoDB
                mongoDBService.SaveImageToMongoDB(key, base64Image);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    // Метод для получения изображения из Redis или MongoDB
    static void GetImageFromServices(RedisService redisService, MongoDBService mongoDBService)
    {
        Console.Write("Введите ключ для изображения: ");
        string key = Console.ReadLine();

        try
        {
            // Пытаемся получить изображение из Redis
            string base64Image = redisService.GetImageFromRedis(key);

            if (!string.IsNullOrEmpty(base64Image))
            {
                OpenImageFromBase64(base64Image, $"Redis ({key})");
            }
            else
            {
                // Если изображение не найдено в Redis, ищем в MongoDB
                base64Image = mongoDBService.GetImageFromMongoDB(key);

                if (!string.IsNullOrEmpty(base64Image))
                {
                    OpenImageFromBase64(base64Image, $"MongoDB ({key})");

                    // Заново кэшируем изображение в Redis
                    redisService.SaveImageToRedis(key, base64Image);
                    Console.WriteLine($"Изображение заново закешировано в Redis.");
                }
                else
                {
                    Console.WriteLine("Изображение с указанным ключом не найдено ни в Redis, ни в MongoDB.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    // Открытие изображения из Base64
    static void OpenImageFromBase64(string base64Image, string source)
    {
        byte[] imageBytes = Convert.FromBase64String(base64Image);
        string tempImagePath = Path.GetTempFileName();
        File.WriteAllBytes(tempImagePath, imageBytes);

        ProcessStartInfo startInfo = new ProcessStartInfo(tempImagePath)
        {
            UseShellExecute = true
        };
        System.Diagnostics.Process.Start(startInfo);

        Console.WriteLine($"Изображение успешно извлечено и открыто из {source}");
    }

    // Метод для удаления ключа из Redis и MongoDB
    static void DeleteKeyFromServices(RedisService redisService, MongoDBService mongoDBService)
    {
        Console.Write("Введите ключ для удаления: ");
        string key = Console.ReadLine();

        // Удаляем из Redis
        redisService.DeleteKeyFromRedis(key);

        // Удаляем из MongoDB
        mongoDBService.DeleteKeyFromMongoDB(key);
    }
}