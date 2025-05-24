using Confluent.Kafka;
using ImageRotationMicroservice.Models.ImageMessage;
using ImageRotationMicroservice.Services;
using ImageRotationMicroservice.Services.Contract;

namespace ImageRotationMicroservice
{
    /// <summary>
    /// The main class that contains the entry point and the application configuration.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main application method that performs Web API configuration, service registration, middleware configuration, and application launch.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        /// <exception cref="InvalidOperationException">Thrown when required configuration settings, such as Kafka BootstrapServers, are missing from the application's configuration.</exception>
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddLogging();

            builder.Services.AddSingleton<IProducer<Null, ImageMessage>>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var logger = sp.GetRequiredService<ILogger<ProducerService>>();
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
                var logger = sp.GetRequiredService<ILogger<ConsumerService>>(); // Or just ILogger
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

            builder.Services.AddHostedService<ConsumerService>();
            builder.Services.AddScoped<IImageRotationService, ImageRotationService>();
            builder.Services.AddSingleton<IProducerService, ProducerService>();

            var host = builder.Build();
            host.Run();
        }
    }
}
