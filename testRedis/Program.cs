using System;
using StackExchange.Redis;
using MongoDB.Driver;

class Program
{
    static void Main(string[] args)
    {
        // Инициализация сервисов
        var redis = ConnectionMultiplexer.Connect("redis:6379");
        var redisService = new RedisService(redis);

        var mongoDBService = new MongoDBService("mongodb://mongodb:27017", "ImageDatabase");

        var imageService = new ImageService(redisService, mongoDBService);

        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Сохранить изображение");
            Console.WriteLine("2. Получить изображение");
            Console.WriteLine("3. Показать все ключи в Redis");
            Console.WriteLine("4. Удалить ключ");
            Console.WriteLine("5. Выйти");

            Console.Write("Введите номер действия: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введите путь к изображению: ");
                    string imagePath = Console.ReadLine();
                    Console.Write("Введите ключ для изображения: ");
                    string key = Console.ReadLine();
                    imageService.SaveImage(key, imagePath);
                    break;

                case "2":
                    Console.Write("Введите ключ для изображения: ");
                    key = Console.ReadLine();
                    imageService.GetImage(key);
                    break;

                case "3":
                    redisService.ShowAllKeysInRedis();
                    break;

                case "4":
                    Console.Write("Введите ключ для удаления: ");
                    key = Console.ReadLine();
                    imageService.DeleteImage(key);
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
}