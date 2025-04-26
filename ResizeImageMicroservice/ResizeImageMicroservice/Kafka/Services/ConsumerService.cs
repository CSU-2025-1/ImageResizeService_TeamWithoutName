using Confluent.Kafka;
using ResizeImageMicroservice.Kafka.Models;
using ResizeImageMicroservice.Services;

namespace ResizeImageMicroservice.Kafka.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly ILogger<ConsumerService> _logger;
        private readonly IConsumer<Null, ImageMessage> _consumer;
        private readonly string _topicKey = "KafkaTopics:ResizerImage";
        private readonly string _topic;
        private readonly IProducerService _producerService;

        private readonly IImageResizeService _imageResizeService;

        public ConsumerService(
            IConfiguration config, 
            ILogger<ConsumerService> logger, 
            IConsumer<Null, ImageMessage> consumer, 
            IImageResizeService imageResizeService, 
            IProducerService producerService
            )
        {
            _logger = logger;
            _consumer = consumer;
            _imageResizeService = imageResizeService;
            _producerService = producerService;
            _topic = config[_topicKey] ?? throw new InvalidOperationException($"{_topicKey} not configured in appsettings.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = _consumer.Consume(stoppingToken);
                        _logger.LogInformation($"Consumed message '{cr.Message.Value.Height}' at: '{cr.TopicPartitionOffset}'.");
                        var imageMessage = cr.Message.Value;
                        IFormFile imageFile = ConvertBase64ToIFormFile(imageMessage.Image);

                        byte[] resizedImageBytes = await _imageResizeService.ResizeImageAsync(
                                imageFile,
                                imageMessage.Width,
                                imageMessage.Height,
                                imageMessage.PreserveAspectRatio);

                        _logger.LogInformation($"Image resized successfully.  Resized image byte length: {resizedImageBytes.Length}");

                        bool answerFromProducer = await _producerService.SendToImageResized(
                            new ImageMessage {
                                Id = imageMessage.Id,
                                OriginalImage = imageMessage.OriginalImage,
                                Image = Convert.ToBase64String(resizedImageBytes),
                                Width = imageMessage.Width,
                                Height = imageMessage.Height,
                                PreserveAspectRatio = imageMessage.PreserveAspectRatio,
                                Angle = imageMessage.Angle,
                                Format = imageMessage.Format,
                                IsNeedResize = false,
                                IsNeedRotation = imageMessage.IsNeedRotation
                            }
                        );

                        if (!answerFromProducer)
                        {
                            _logger.LogInformation("Resized image was not sent.");
                        } else
                        {
                            _logger.LogInformation("Resize image was sent.");
                        }
                        
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Consume error: {e.Error.Reason}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during image resizing.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                _consumer.Close();
            }
        }

        private IFormFile ConvertBase64ToIFormFile(string base64String)
        {
            // Remove the header if it exists (e.g., "data:image/png;base64,")
            var base64Data = base64String.Substring(base64String.IndexOf(',') + 1);

            byte[] imageBytes = Convert.FromBase64String(base64Data);

            // Create a MemoryStream from the byte array
            var memoryStream = new MemoryStream(imageBytes);

            // Create a FormFile from the MemoryStream
            IFormFile formFile = new FormFile(memoryStream, 0, memoryStream.Length, "image", "image.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg" // Замените на фактический тип контента, если знаете его.
            };

            return formFile;
        }

        public override void Dispose()
        {
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
