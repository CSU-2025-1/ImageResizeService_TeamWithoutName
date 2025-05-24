using Confluent.Kafka;
using ResizeImageMicroservice.Models.ImageMessage;
using ResizeImageMicroservice.Services.Contract;

namespace ResizeImageMicroservice.Services
{
    /// <summary>
    /// The `ConsumerService' is a background service that consumes messages from a Kafka topic,
    /// performs image resizing, and sends the resized image to another Kafka topic.
    /// </summary>
    public class ConsumerService : BackgroundService
    {
        private readonly ILogger<ConsumerService> _logger;
        private readonly IConsumer<Null, ImageMessage> _consumer;
        private readonly string _topicKey = "KafkaTopics:ResizerImage";
        private readonly string _topic;
        private readonly IProducerService _producerService;
        private readonly IImageResizeService _imageResizeService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerService"/> class.
        /// </summary>
        /// <param name="config">The <see cref="IConfiguration"/> interface for accessing the application configuration.</param>
        /// <param name="logger">The interface <see cref="ILogger{ConsumerService}"/> for logging.</param>
        /// <param name="consumerRouterImage">Interface <see cref="IConsumer{Null, ImageMessage}"/> for receiving messages from a Kafka topic.</param>
        /// <param name="imageResizeService">The <see cref="IImageResizeService"/> interface for image resizing.</param>
        /// <param name="producerService">The <see cref="IProducerService"/> interface for producer Kafka.</param>
        /// <exception cref="InvalidOperationException">It is discarded if the <c>KafkaTopics key is not configured in the application configuration.:RouterImage</c>.</exception>
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

        /// <summary>
        /// The main method of running a background service. This method subscribes to a Kafka topic,
        /// consumes messages, resizes the image, and sends the modified image to another topic.
        /// </summary>
        /// <param name="stoppingToken">The `CancellationToken' that signals the need to stop the service.</param>
        /// <returns>A task representing an asynchronous operation.</returns>
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
                _consumer.Close();
            }
        }

        /// <summary>
        /// Converts a Base64 string to an 'IFormFile` object.
        /// </summary>
        /// <param name="base64String">The Base64 string representing the image.</param>
        /// <returns>The <see cref="IFormFile"/> object representing the image.</returns>
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

        /// <inheritdoc cref="BackgroundService.Dispose"/>
        public override void Dispose()
        {
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
