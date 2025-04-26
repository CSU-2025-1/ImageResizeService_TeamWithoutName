using Confluent.Kafka;
using MessageRouter.Model;
using MessageRouter.Services;
using MessageRouter.Services.Contracts;
using MongoDB.Driver;
using StackExchange.Redis;

namespace MessageRouter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddLogging();

            builder.Services.AddSingleton<IProducer<Null, ImageMessage>>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var logger = sp.GetRequiredService<ILogger<IProducer<Null, ImageMessage>>>();
                var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? throw new InvalidOperationException("Kafka:BootstrapServers not configured in appsettings.");
                var maxRequestSizeString = configuration["Kafka:MaxRequestSize"];
                int maxRequestSize;
                if (string.IsNullOrEmpty(maxRequestSizeString) || !int.TryParse(maxRequestSizeString, out maxRequestSize))
                {
                    maxRequestSize = 10485760;
                    logger.LogWarning("Kafka:MaxRequestSize not configured or invalid. Using default value: {MaxRequestSize}", maxRequestSize);
                }

                var config = new ProducerConfig
                {
                    BootstrapServers = bootstrapServers,
                    MessageMaxBytes = maxRequestSize
                };

                var producerBuilder = new ProducerBuilder<Null, ImageMessage>(config)
                .SetValueSerializer(new ImageMessageSerializer(sp.GetRequiredService<ILogger<ImageMessageSerializer>>()));

                return producerBuilder.Build();
            });

            builder.Services.AddSingleton<IConsumer<Null, ImageMessage>>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var logger = sp.GetRequiredService<ILogger<IConsumer<Null, ImageMessage>>>();
                var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? throw new InvalidOperationException("Kafka:BootstrapServers not configured in appsettings.");
                var groupId = configuration["Kafka:GroupId"] ?? throw new InvalidOperationException("Kafka:GroupId not configured in appsettings.");

                var config = new ConsumerConfig
                {
                    BootstrapServers = bootstrapServers,
                    GroupId = groupId
                };

                var consumerBuilder = new ConsumerBuilder<Null, ImageMessage>(config)
                .SetValueDeserializer(new ImageMessageDeserializer(sp.GetRequiredService<ILogger<ImageMessageDeserializer>>()));

                return consumerBuilder.Build();
            });

            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = builder.Configuration.GetSection("ConnectionStringsMongoDB").Value;
                return new MongoClient(connectionString);
            });

            builder.Services.AddScoped(sp =>
            {
                var client = sp.GetService<IMongoClient>();
                var database = client.GetDatabase("ImageDB");
                return database.GetCollection<ImageDatabase>("Images");
            });

            builder.Services.AddScoped<IMongoCollection<ImageDatabase>>(sp => {
                var client = sp.GetService<IMongoClient>();
                var database = client.GetDatabase("ImageDB");
                return database.GetCollection<ImageDatabase>("Images");
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnectionString = builder.Configuration["RedisConnectionString"];
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });
            builder.Services.AddScoped<IImageDatabaseService, ImageMongoDatabaseService>();
            builder.Services.AddHostedService<ConsumerService>();
            builder.Services.AddSingleton<IImageProcessingService, ImageProcessingService>();
            builder.Services.AddSingleton<IProducerService, ProducerService>();
            builder.Services.AddScoped<ICacheService, RedisCacheService>();


            var host = builder.Build();
            host.Run();
        }
    }
}