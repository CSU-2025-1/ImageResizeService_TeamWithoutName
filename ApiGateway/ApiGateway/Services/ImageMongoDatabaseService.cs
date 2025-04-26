using ApiGateway.Models;
using ApiGateway.Services.Contract;
using MongoDB.Driver;

namespace ApiGateway.Services
{
    public class ImageMongoDatabaseService : IImageDatabaseService
    {
        private readonly ILogger<ImageMongoDatabaseService> _logger;
        private readonly IMongoCollection<ImageDatabase> _images;

        public ImageMongoDatabaseService(ILogger<ImageMongoDatabaseService> logger, IMongoCollection<ImageDatabase> imageCollection)
        {
            _logger = logger;
            _images = imageCollection;
        }

        public async Task<ImageDatabase> GetImageById(string id)
        {
            try
            {
                var imageDatabase = await _images.Find(i => i.Id.Equals(id)).FirstOrDefaultAsync();
                _logger.LogInformation($"Image {id} get from database.");
                return imageDatabase;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get from database in ImageDatabaseService.");
                return new ImageDatabase();
            }
        }

        public async Task<ImageDatabase> GetImageByParams(ImageChecker imageChecker)
        {
            try
            {
                var imageDatabase = await _images.Find(i => 
                    i.OriginalImage.Equals(imageChecker.Image) && 
                    i.Width == imageChecker.Width &&
                    i.Height == imageChecker.Height &&
                    i.PreserveAspectRatio == imageChecker.PreserveAspectRatio &&
                    i.Angle == imageChecker.Angle &&
                    i.Format.Equals(imageChecker.Format)
                ).FirstOrDefaultAsync();
                _logger.LogInformation($"Image by params get from database.");
                return imageDatabase;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get from database in ImageDatabaseService.");
                return new ImageDatabase();
            }
        }
    }
}
