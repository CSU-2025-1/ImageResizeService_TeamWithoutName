using ApiGateway.Models;
using ApiGateway.Services.Contract;

namespace ApiGateway.Services
{
    public class RedisAndMongoSavingService : ISavingService
    {
        private readonly ILogger<RedisAndMongoSavingService> _logger;
        private readonly IImageDatabaseService _imageDatabaseService;
        private readonly ICacheService _cacheService;

        public RedisAndMongoSavingService(ILogger<RedisAndMongoSavingService> logger, IImageDatabaseService imageDatabaseService, ICacheService cacheService)
        {
            _logger = logger;
            _imageDatabaseService = imageDatabaseService;
            _cacheService = cacheService;
        }

        public async Task<string> Check(ImageChecker imageChecker)
        {
            try
            {
                var key = $"{imageChecker.Image}_{imageChecker.Width}_{imageChecker.Height}_{imageChecker.PreserveAspectRatio}_{imageChecker.Angle}_{imageChecker.Format}";
                var imageCache = _cacheService.GetImage(key);
                if (imageCache != null)
                {
                    return imageCache;
                }

                var imageDB = await _imageDatabaseService.GetImageByParams(imageChecker);
                return imageDB.Image;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Mongo or Redis");
                return null;
            }
        }
    }
}
