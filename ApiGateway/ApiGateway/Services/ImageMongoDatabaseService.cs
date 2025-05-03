using ApiGateway.Models;
using ApiGateway.Services.Contract;
using MongoDB.Driver;

namespace ApiGateway.Services
{
    /// <summary>
    /// A class for working with the MongoDB image database.
    /// </summary>
    public class ImageMongoDatabaseService : IImageDatabaseService
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

        /// <inheritdoc cref="IImageDatabaseService.GetImageById(string)"/>
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

        /// <inheritdoc cref="IImageDatabaseService.GetImageByParams(ImageChecker)"/>
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
