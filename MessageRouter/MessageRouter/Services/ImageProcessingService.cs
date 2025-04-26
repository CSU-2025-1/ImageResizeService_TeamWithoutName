using MessageRouter.Model;
using MessageRouter.Services.Contracts;

namespace MessageRouter.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly ILogger<ImageProcessingService> _logger;
        private readonly IProducerService _producerService;
        private readonly IImageDatabaseService _imageDatabaseService;
        private readonly ICacheService _cacheService;

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

                bool isSavedCache = _cacheService.SaveImage(new ImageCache
                {
                    Key = $"{imageMessage.OriginalImage}_{imageMessage.Width}_{imageMessage.Height}_{imageMessage.PreserveAspectRatio}_{imageMessage.Angle}_{imageMessage.Format}",

                    Image = imageMessage.Image
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
