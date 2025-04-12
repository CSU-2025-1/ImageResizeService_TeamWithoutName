using ApiGateway.Models.Kafka;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ApiGateway.Authentication.Service;
using ApiGateway.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Microsoft.OpenApi.Models;

namespace ApiGateway
{
    public class Program
    {
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


            builder.Services.AddScoped<IProducerService, ProducerService>();

            // Регистрация MongoDB
            var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDB"));
            builder.Services.AddSingleton<IMongoClient>(mongoClient);
            builder.Services.AddScoped<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase("Users");
            });

            // Add services to the container.
            builder.Services.AddScoped<IAuthService, AuthService>();

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
            // Настройка Swagger с поддержкой JWT
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
