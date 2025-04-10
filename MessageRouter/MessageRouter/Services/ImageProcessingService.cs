using MessageRouter.Model;
using System.Collections.Concurrent;

namespace MessageRouter.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly ILogger<ImageProcessingService> _logger;
        private readonly IProducerService _producerService;
        private readonly ConcurrentQueue<ImageMessage> _resultsQueue;

        public ImageProcessingService(ILogger<ImageProcessingService> logger, IProducerService producerService)
        {
            _logger = logger;
            _producerService = producerService;
            _resultsQueue = new ConcurrentQueue<ImageMessage>();
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

                _logger.LogInformation($"Image {imageMessage.Id} done");

                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error image in ImageProcessingService.");
                throw;
            }
        }
    }
}
