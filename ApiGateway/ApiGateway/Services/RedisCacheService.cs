using ApiGateway.Models;
using ApiGateway.Services.Contract;
using StackExchange.Redis;

namespace ApiGateway.Services
{
    /// <summary>
    /// Service for working with Redis.
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<RedisCacheService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheService"/> class.
        /// </summary>
        /// <param name="redis">Connection redis.</param>
        /// <param name="logger">A logger for recording information about the service.</param>
        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;

        }

        /// <inheritdoc cref="ICacheService.GetImage(string)"/>
        public string GetImage(string key)
        {
            try
            {
                if (!_db.KeyExists(key))
                {
                    _logger.LogInformation($"Image with key {key} not found in Redis.");
                    return null;
                }

                var image = _db.StringGet(key);
                _logger.LogInformation($"The image was successfully retrieved from Redis with a key: {key}");
                return image.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting image with key {key} from Redis");
                return null;
            }
        }

        /// <inheritdoc cref="ICacheService.SaveImage(ImageCache)"/>
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
