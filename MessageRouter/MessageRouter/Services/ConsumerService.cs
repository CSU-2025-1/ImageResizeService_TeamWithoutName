using Confluent.Kafka;
using MessageRouter.Model;
using MessageRouter.Services.Contracts;

namespace MessageRouter.Services
{
    /// <summary>
    /// A consumer service designed for asynchronous receipt and processing of <see cref="ImageMessage"/> messages from a Kafka topic.
    /// </summary>
    public class ConsumerService : BackgroundService
    {
        private readonly ILogger<ConsumerService> _logger;
        private readonly IConsumer<Null, ImageMessage> _consumerRouterImage;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly string _topicKey = "KafkaTopics:RouterImage";
        private readonly string _topic;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerService"/> class.
        /// </summary>
        /// <param name="config">The <see cref="IConfiguration"/> interface for accessing the application configuration.</param>
        /// <param name="logger">The interface <see cref="ILogger{ConsumerService}"/> for logging.</param>
        /// <param name="consumerRouterImage">Interface <see cref="IConsumer{Null, ImageMessage}"/> for receiving messages from a Kafka topic.</param>
        /// <param name="imageProcessingService">The <see cref="IImageProcessingService"/> interface for image processing.</param>
        /// <exception cref="InvalidOperationException">It is discarded if the <c>KafkaTopics key is not configured in the application configuration.:RouterImage</c>.</exception>
        public ConsumerService(
            IConfiguration config,
            ILogger<ConsumerService> logger,
            IConsumer<Null, ImageMessage> consumerRouterImage,
            IImageProcessingService imageProcessingService
            )
        {
            _logger = logger;
            _consumerRouterImage = consumerRouterImage;
            _imageProcessingService = imageProcessingService;
            _topic = config[_topicKey] ?? throw new InvalidOperationException($"{_topicKey} not configured in appsettings.");
        }

        /// <summary>
        /// Asynchronously starts the process of receiving and processing messages from the Kafka topic.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token used to stop the service.</param>
        /// <returns>A task representing an asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumerRouterImage.Subscribe(_topic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = _consumerRouterImage.Consume(stoppingToken);
                        _logger.LogInformation($"Consumed message '{cr.Message.Value.Height}' at: '{cr.TopicPartitionOffset}'.");
                        await _imageProcessingService.ProcessingImageAsync(cr.Message.Value);

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

                _consumerRouterImage.Close();
            }
        }

        /// <summary>
        /// Frees up the resources used by the service.
        /// </summary>
        public override void Dispose()
        {
            _consumerRouterImage.Dispose();
            base.Dispose();
        }
    }
}
