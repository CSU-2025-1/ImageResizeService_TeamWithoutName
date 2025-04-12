
using Confluent.Kafka;
using ImageRotationMicroservice.Kafka.Models;
using ImageRotationMicroservice.Kafka.Services;
using ImageRotationMicroservice.Services;

namespace ImageRotationMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
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



            /*builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();*/

            var host = builder.Build();
            host.Run();
        }
    }
}
