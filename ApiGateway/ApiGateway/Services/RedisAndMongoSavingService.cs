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

        public async Task<string> CheckFull(ImageChecker imageChecker)
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

        public async Task<(string, string)> CheckId(string id)
        {
            try
            {
                var imageCache = _cacheService.GetImage(id);
                if (imageCache != null)
                {
                    string[] parts = imageCache.Split('_');
                    string image = parts[0]; 
                    string format = parts[1]; 
                    return (image, format);
                }

                var imageDB = await _imageDatabaseService.GetImageById(id);

                _cacheService.SaveImage(new ImageCache {
                    Key = id,
                    Image = imageDB.Image
                });

                return (imageDB.Image, imageDB.Format);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Mongo or Redis");
                return (null, null);
            }
        }
    }
}
