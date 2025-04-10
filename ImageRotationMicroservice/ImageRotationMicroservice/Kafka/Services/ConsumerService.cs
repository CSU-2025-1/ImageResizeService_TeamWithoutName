using Confluent.Kafka;
using ImageRotationMicroservice.Kafka.Models;
using ImageRotationMicroservice.Services;

namespace ImageRotationMicroservice.Kafka.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly ILogger<ConsumerService> _logger;
        private readonly IConsumer<Null, ImageMessage> _consumer;
        private readonly string _topicKey = "KafkaTopics:RotatorImage";
        private readonly string _topic;
        private readonly IProducerService _producerService;

        private readonly IImageRotationService _imageRotationService;

        public ConsumerService(
            IConfiguration config, 
            ILogger<ConsumerService> logger, 
            IConsumer<Null, ImageMessage> consumer,
            IImageRotationService imageResizeService, 
            IProducerService producerService
            )
        {
            _logger = logger;
            _consumer = consumer;
            _imageRotationService = imageResizeService;
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

                        byte[] resizedImageBytes = await _imageRotationService.RotateImageAsync(
                                imageFile,
                                imageMessage.Angle);

                        _logger.LogInformation($"Image rotated successfully.");

                        bool answerFromProducer = await _producerService.SendToImageResized(
                            new ImageMessage
                            {
                                Id = imageMessage.Id,
                                Image = Convert.ToBase64String(resizedImageBytes),
                                Width = imageMessage.Width,
                                Height = imageMessage.Height,
                                PreserveAspectRatio = imageMessage.PreserveAspectRatio,
                                Angle = 361,
                                Format = imageMessage.Format,
                                IsNeedResize = imageMessage.IsNeedResize,
                                IsNeedRotation = false
                            }
                        );

                        if (!answerFromProducer)
                        {
                            _logger.LogInformation("Rotated image was not sent.");
                        }
                        else
                        {
                            _logger.LogInformation("Rotated image was sent.");
                        }

                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Consume error: {e.Error.Reason}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during image rotating.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _consumer.Close();
            }
        }

        private IFormFile ConvertBase64ToIFormFile(string base64String)
        {
            var base64Data = base64String.Substring(base64String.IndexOf(',') + 1);

            byte[] imageBytes = Convert.FromBase64String(base64Data);

            var memoryStream = new MemoryStream(imageBytes);

            IFormFile formFile = new FormFile(memoryStream, 0, memoryStream.Length, "image", "image.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg" 
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
