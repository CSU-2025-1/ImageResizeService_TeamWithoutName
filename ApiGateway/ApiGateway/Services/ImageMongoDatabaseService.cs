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

        public async Task<ImageDatabase> GetImage(string id)
        {
            try
            {
                var imageDatabase = await _images.Find(i => i.Id.Equals(id)).FirstOrDefaultAsync();
                _logger.LogInformation($"Image {id} get in database.");
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
