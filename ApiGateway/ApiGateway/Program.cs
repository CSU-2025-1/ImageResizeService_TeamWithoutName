using ApiGateway.Models;
using ApiGateway.Models.Authentication;
using ApiGateway.Models.ImageMessage;
using ApiGateway.Services;
using ApiGateway.Services.Contract;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;

namespace ApiGateway
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
            var builder = WebApplication.CreateBuilder(args);

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
                    logger.LogWarning("Kafka:MaxRequSestSize not configured or invalid. Using default value: {MaxRequestSize}", maxRequestSize);
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
            builder.Services.AddScoped<ISendingService, KafkaSendingService>();

            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = builder.Configuration.GetSection("ConnectionStringsMongoDB").Value;
                return new MongoClient(connectionString);
            });

            builder.Services.AddScoped(sp =>
            {
                var client = sp.GetService<IMongoClient>();
                var database = client.GetDatabase("UserDB");
                return database.GetCollection<User>("Users");
            });

            builder.Services.AddScoped<IMongoCollection<User>>(sp => {
                var client = sp.GetService<IMongoClient>();
                var database = client.GetDatabase("UserDB");
                return database.GetCollection<User>("Users");
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


            builder.Services.AddScoped<IAuthService, MongoAuthService>();

            builder.Services.AddScoped<IImageDatabaseService, ImageMongoDatabaseService>();

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });

                // Добавление поддержки JWT в Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            /*// Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }*/ //TODO убрать в финале

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization(); 
            app.MapControllers();

            app.Run();
        }
    }
}
