using ApiGateway.Models;
using ApiGateway.Services.Contract;
using StackExchange.Redis;

namespace ApiGateway.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;

        }

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
    }
}
