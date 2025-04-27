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

        /// <summary>
        /// Asynchronously retrieves an image from the database using the specified Id.
        /// </summary>
        /// <param name="id">ID of the image to receive.</param>
        /// <returns>The <see cref="ImageDatabase"/> object representing the image, if it is found in the database.</returns>
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

        /// <summary>
        /// Asynchronously retrieves an image from the database based on the parameters contained in the <see cref="ImageChecker"/> object.
        /// </summary>
        /// <param name="imageChecker">The <see cref="ImageChecker"/> object containing the parameters for image search.</param>
        /// <returns>The <see cref="ImageDatabase"/> object representing the image, if it is found in the database.</returns>
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
