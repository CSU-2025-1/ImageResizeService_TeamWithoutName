using MessageRouter.Model;
using MessageRouter.Services.Contracts;

namespace MessageRouter.Services
{
    /// <summary>
    /// Service for processing image.
    /// </summary>
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly ILogger<ImageProcessingService> _logger;
        private readonly IProducerService _producerService;
        private readonly IImageDatabaseService _imageDatabaseService;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProcessingService"/> class.
        /// </summary>
        /// <param name="logger">The interface <see cref="ILogger{ImageProcessingService}"/> for logging.</param>
        /// <param name="producerService">Kafka producer service.</param>
        /// <param name="imageDatabaseService">Image database sevice.</param>
        /// <param name="cacheService">Cache service.</param>
        public ImageProcessingService(
            ILogger<ImageProcessingService> logger, 
            IProducerService producerService, 
            IImageDatabaseService imageDatabaseService,
            ICacheService cacheService
            )
        {
            _logger = logger;
            _producerService = producerService;
            _imageDatabaseService = imageDatabaseService;
            _cacheService = cacheService;
        }

        /// <inheritdoc cref="IImageProcessingService.ProcessingImageAsync(ImageMessage)"/>
        public async Task<bool> ProcessingImageAsync(ImageMessage imageMessage)
        {
            try
            {
                if (imageMessage.IsNeedResize)
                {
                    bool isSent = await _producerService.SendImageMessage(TopicName.ResizerImage, imageMessage);
                    return isSent;
                }

                if (imageMessage.IsNeedRotation)
                {
                    bool isSent = await _producerService.SendImageMessage(TopicName.RotatorImage, imageMessage);
                    return isSent;
                }

                bool isSavedCacheFull = _cacheService.SaveImage(new ImageCache
                {
                    Key = $"{imageMessage.OriginalImage}_{imageMessage.Width}_{imageMessage.Height}_{imageMessage.PreserveAspectRatio}_{imageMessage.Angle}_{imageMessage.Format}",

                    Image = imageMessage.Image
                });

                bool isSavedCacheId = _cacheService.SaveImage(new ImageCache
                {
                    Key = $"{imageMessage.Id}",

                    Image = $"{imageMessage.Image}_{imageMessage.Format}"
                });

                bool isSaved = await _imageDatabaseService.SaveImage(new ImageDatabase
                {
                    Id = imageMessage.Id,
                    OriginalImage = imageMessage.OriginalImage,
                    Image = imageMessage.Image,
                    Width = imageMessage.Width,
                    Height = imageMessage.Height,
                    PreserveAspectRatio = imageMessage.PreserveAspectRatio,
                    Angle = imageMessage.Angle,
                    Format = imageMessage.Format,
                });

                return isSaved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error image in ImageProcessingService.");
                return false;
            }
        }
    }
}
