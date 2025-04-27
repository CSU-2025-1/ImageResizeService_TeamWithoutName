using ApiGateway.Models;
using ApiGateway.Services.Contract;

namespace ApiGateway.Services
{
    /// <summary>
    /// A class that checks for images in Redis and MongoDB.
    /// </summary>
    public class RedisAndMongoSavingService : ISavingService
    {
        private readonly ILogger<RedisAndMongoSavingService> _logger;
        private readonly IImageDatabaseService _imageDatabaseService;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisAndMongoSavingService"/> class.
        /// </summary>
        /// <param name="logger">A logger for recording information about the service.</param>
        /// <param name="imageDatabaseService">Database service.</param>
        /// <param name="cacheService">Cache service.</param>
        public RedisAndMongoSavingService(ILogger<RedisAndMongoSavingService> logger, IImageDatabaseService imageDatabaseService, ICacheService cacheService)
        {
            _logger = logger;
            _imageDatabaseService = imageDatabaseService;
            _cacheService = cacheService;
        }

        /// <inheritdoc cref="ISavingService.CheckFull(ImageChecker)"/>
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

        /// <inheritdoc cref="ISavingService.CheckId(string)"/>
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
