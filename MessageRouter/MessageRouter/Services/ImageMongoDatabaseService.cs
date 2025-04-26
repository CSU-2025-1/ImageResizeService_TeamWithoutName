using MessageRouter.Model;
using MessageRouter.Services.Contracts;
using MongoDB.Driver;

namespace MessageRouter.Services
{
    public class ImageMongoDatabaseService: IImageDatabaseService
    {
        private readonly ILogger<ImageMongoDatabaseService> _logger;
        private readonly IMongoCollection<ImageDatabase> _images;

        public ImageMongoDatabaseService(ILogger<ImageMongoDatabaseService> logger, IMongoCollection<ImageDatabase> imageCollection)
        {
            _logger = logger;
            _images = imageCollection;
        }
        public async Task<bool> SaveImage(ImageDatabase image)
        {
            try
            {
                await _images.InsertOneAsync(image);
                _logger.LogInformation($"Image {image.Id} was save in database.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error save into database in ImageDatabaseService.");
                return false;
            }
        }
    }
}
