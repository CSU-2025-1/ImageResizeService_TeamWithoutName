using MessageRouter.Model;
using MessageRouter.Services.Contracts;
using MongoDB.Driver;

namespace MessageRouter.Services
{
    /// <summary>
    /// Database service with MondoDB.
    /// </summary>
    public class ImageMongoDatabaseService: IImageDatabaseService
    {
        private readonly ILogger<ImageMongoDatabaseService> _logger;
        private readonly IMongoCollection<ImageDatabase> _images;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageMongoDatabaseService"/> class.
        /// </summary>
        /// <param name="logger">The interface <see cref="ILogger{ImageMongoDatabaseService}"/> for logging.</param>
        /// <param name="imageCollection">Interface <see cref="IMongoCollection{ImageDatabase}"/> for accessing a collection of images in MongoDB.</param>
        public ImageMongoDatabaseService(ILogger<ImageMongoDatabaseService> logger, IMongoCollection<ImageDatabase> imageCollection)
        {
            _logger = logger;
            _images = imageCollection;
        }

        /// <inheritdoc cref="IImageDatabaseService.SaveImage(ImageDatabase)"/>
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
