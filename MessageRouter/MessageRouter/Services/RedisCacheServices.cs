using MessageRouter.Model;
using MessageRouter.Services.Contracts;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Services
{
    public class RedisCacheServices : ICacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<RedisCacheServices> _logger;

        public RedisCacheServices(IConnectionMultiplexer redis, ILogger<RedisCacheServices> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;

        }

        public bool SaveImage(ImageCache imageCache)
        {
            try
            {
                var isSave = _db.StringSet(imageCache.Key, imageCache.Image, TimeSpan.FromMinutes(30));
                _logger.LogInformation($"The image was successfully saved in Redis with a key {imageCache.Key}");
                return isSave;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving image with key {imageCache.Key} to Redis");
                return false;
            }
        }
    }
}
