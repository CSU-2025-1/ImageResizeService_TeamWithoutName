using MessageRouter.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (imageMessage.Width != -1 || imageMessage.Height != -1)
                {
                    bool isSent = await _producerService.SendImageMessage(TopicName.ResizerImage, imageMessage);
                    return isSent;
                }

                if (imageMessage.Angle != 361)
                {
                    bool isSent = await _producerService.SendImageMessage(TopicName.RotatorImage, imageMessage);
                    return isSent;
                }

                if (imageMessage.Format != null)
                {
                    bool isSent = await _producerService.SendImageMessage(TopicName.FormatorImage, imageMessage);
                    return isSent;
                }

                //_resultsQueue.Enqueue(imageMessage);
                _logger.LogInformation("Image done");

                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error image in ImageProcessingService.");
                throw;
            }
        }

        /*public ImageMessage GetResult()
        {
            ImageMessage result;
            _resultsQueue.TryDequeue(out result);
            return result;
        }*/
    }
}
