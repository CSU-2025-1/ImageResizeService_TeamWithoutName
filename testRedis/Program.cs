using System;
using System.Diagnostics;
using System.IO;
using StackExchange.Redis;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using MongoDB.Driver;

class Program
{
    // Подключение к Redis
    private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379"); //6379 - стандартный хост редиса
    private static readonly IDatabase db = redis.GetDatabase(); //подключаемся к редиске чтоб работать с ней

    // Подключение к MongoDB
    class MongoDatabase
    {
        private static MongoClient mongoClient;
        private static IMongoDatabase database;

        // Метод для инициализации подключения

        //ImageDatabase - создаст базу если ее нету
        public static void Connect(string connectionString = "mongodb://localhost:27017", string dbName = "ImageDatabase") //27017 - стандартный порт для монго
        {
            mongoClient = new MongoClient(connectionString);
            database = mongoClient.GetDatabase(dbName);
        }

        // Получение коллекции для изображений
        public static IMongoCollection<ImageData> GetImagesCollection()
        {
            if (database == null) //проверяем что метод connect вызвался
            {
                throw new InvalidOperationException("MongoDB не подключен. Вызовите метод Connect() перед использованием.");
            }
            return database.GetCollection<ImageData>("Images"); //получаем коллекцию с картинками
        }
    }

    static void Main(string[] args)
    {
        MongoDatabase.Connect("mongodb://localhost:27017", "ImageDatabase");

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
                    SaveImageToRedisAndMongoDB();
                    break;
                case "2":
                    GetImageFromRedisOrMongoDB();
                    break;
                case "3":
                    ShowAllKeysInRedis();
                    break;
                case "4":
                    DeleteKeyFromRedisAndMongoDB();
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
    static void SaveImageToRedisAndMongoDB()
    {
        Console.Write("Введите путь к изображению: ");
        string imagePath = Console.ReadLine();

        try
        {
            using (var imageStream = File.OpenRead(imagePath))
            using (var memoryStream = new MemoryStream()) //создает поток для временного храрнения данных изображения
            {
                using var image = Image.Load(imageStream); //Загружает изображение 
                image.Save(memoryStream, new JpegEncoder()); //Сохраняет изображение в формате JPEG в поток memoryStream
                byte[] imageBytes = memoryStream.ToArray(); //поток в байты
                string base64Image = Convert.ToBase64String(imageBytes); //Преобразует массив байтов в строку в формате Base64 (

                Console.Write("Введите ключ для изображения: ");
                string key = Console.ReadLine();

                // Сохраняем в Redis
                db.StringSet(key, base64Image); //сохраняем ключ / значение
                Console.WriteLine($"Изображение успешно сохранено в Redis под ключом: {key}");

                // Сохраняем в MongoDB
                var imagesCollection = MongoDatabase.GetImagesCollection(); //получаем коллекцию
                var imageData = new ImageData //Создаёт объект ImageData с ключем и байтиами изображения
                {
                    Id = key,
                    Base64Content = base64Image
                };
                imagesCollection.ReplaceOne( //ищет такой же ключ, если есть то заменяет, если нет то создает новый
                    filter: Builders<ImageData>.Filter.Eq(x => x.Id, key),
                    replacement: imageData,
                    options: new ReplaceOptions { IsUpsert = true }
                );
                Console.WriteLine($"Изображение успешно сохранено в MongoDB под ключом: {key}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    // Метод для получения изображения из Redis или MongoDB
    static void GetImageFromRedisOrMongoDB()
    {
        Console.Write("Введите ключ для изображения: ");
        string key = Console.ReadLine();

        try
        {
            // Пытаемся получить изображение из Redis
            string base64Image = db.StringGet(key); //получаем значение по ключу из редиски

            if (!string.IsNullOrEmpty(base64Image))
            {
                byte[] imageBytes = Convert.FromBase64String(base64Image);//Преобразует строку Base64 обратно в массив байтов
                string tempImagePath = Path.GetTempFileName();//Создаёт временный файл на компьютере
                File.WriteAllBytes(tempImagePath, imageBytes);//Записывает массив байтов (изображение) во временный файл

                ProcessStartInfo startInfo = new ProcessStartInfo(tempImagePath) //Открывает временный файл с изображением с помощью программы какой то 
                {
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(startInfo);

                Console.WriteLine($"Изображение успешно извлечено и открыто из Redis ({key})");
            }
            else
            {
                // Если изображение не найдено в Redis, ищем в MongoDB
                var imagesCollection = MongoDatabase.GetImagesCollection();
                var filter = Builders<ImageData>.Filter.Eq(x => x.Id, key);
                var imageData = imagesCollection.Find(filter).FirstOrDefault();

                if (imageData != null && !string.IsNullOrEmpty(imageData.Base64Content))
                {
                    byte[] imageBytes = Convert.FromBase64String(imageData.Base64Content);
                    string tempImagePath = Path.GetTempFileName();
                    File.WriteAllBytes(tempImagePath, imageBytes);

                    ProcessStartInfo startInfo = new ProcessStartInfo(tempImagePath)
                    {
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(startInfo);

                    Console.WriteLine($"Изображение успешно извлечено и открыто из MongoDB ({key})");

                    // Заново кэшируем изображение в Redis
                    db.StringSet(key, imageData.Base64Content);
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

    // Метод для показа всех ключей в Redis
    static void ShowAllKeysInRedis()
    {
        try
        {
            var server = redis.GetServer("localhost:6379");
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

    // Метод для удаления ключа из Redis и MongoDB
    static void DeleteKeyFromRedisAndMongoDB()
    {
        Console.Write("Введите ключ для удаления: ");
        string key = Console.ReadLine();

        try
        {
            // Удаляем из Redis
            if (db.KeyExists(key))
            {
                db.KeyDelete(key);
                Console.WriteLine($"Ключ \"{key}\" успешно удален из Redis.");
            }
            else
            {
                Console.WriteLine($"Ключ \"{key}\" не найден в Redis.");
            }

            // Удаляем из MongoDB
            var imagesCollection = MongoDatabase.GetImagesCollection();
            var filter = Builders<ImageData>.Filter.Eq(x => x.Id, key);
            var deleteResult = imagesCollection.DeleteOne(filter);

            if (deleteResult.DeletedCount > 0)
            {
                Console.WriteLine($"Ключ \"{key}\" успешно удален из MongoDB.");
            }
            else
            {
                Console.WriteLine($"Ключ \"{key}\" не найден в MongoDB.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении ключа: {ex.Message}");
        }
    }
}

// Класс для работы с MongoDB
class MongoDatabase
{
    private static MongoClient mongoClient;
    private static IMongoDatabase database;

    public static void Connect(string connectionString, string dbName)
    {
        mongoClient = new MongoClient(connectionString);
        database = mongoClient.GetDatabase(dbName);
    }

    public static IMongoCollection<ImageData> GetImagesCollection()
    {
        return database.GetCollection<ImageData>("Images");
    }
}

// Класс для хранения данных об изображениях
public class ImageData
{
    public string Id { get; set; } 
    public string Base64Content { get; set; } 
}