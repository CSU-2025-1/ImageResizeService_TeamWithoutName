using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using MongoDB.Driver;
using testRedisApi.MongoDBService;
using testRedisApi.Models;
using testRedisApi.RedisService;
using testRedisApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Регистрация RedisService
builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("redis"));
builder.Services.AddSingleton<RedisService>();

// Регистрация MongoDBService
builder.Services.AddSingleton(sp => new MongoDBService("mongodb://mongodb", "ImageDatabase"));

// Регистрация ImageService
builder.Services.AddSingleton<ImageService>();

// Добавление Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Включение Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Маршруты API
app.MapPost("/images", async (ImageService imageService, ImageRequest request) =>
{
    try
    {
        imageService.SaveImage(request.Key, request.Base64Image);
        return Results.Ok(new { message = "Изображение успешно сохранено." });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/images/{key}", async (ImageService imageService, string key) =>
{
    var base64Image = imageService.GetImage(key);
    if (base64Image == null)
        return Results.NotFound(new { message = "Изображение не найдено." });

    return Results.Ok(new { image = base64Image });
});

app.MapDelete("/images/{key}", async (ImageService imageService, string key) =>
{
    imageService.DeleteImage(key);
    return Results.Ok(new { message = "Изображение успешно удалено." });
});

app.Run();

public record ImageRequest(string Key, string Base64Image);