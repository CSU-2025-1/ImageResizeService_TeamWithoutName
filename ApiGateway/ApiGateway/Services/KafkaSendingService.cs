using ApiGateway.Models.ImageMessage;
using ApiGateway.Services.Contract;
using Confluent.Kafka;

namespace ApiGateway.Services
{
    /// <summary>
    /// A service for sending messages to Kafka.
    /// </summary>
    public class KafkaSendingService : ISendingService
    {
        private readonly ILogger<KafkaSendingService> _logger;
        private readonly IProducer<Null, ImageMessage> _producer;
        private readonly string _topicKey = "KafkaTopics:SendRequest";
        private readonly string _topic;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaSendingService"/> class.
        /// </summary>
        /// <param name="config">Application configuration containing Topic settings.</param>
        /// <param name="logger">A logger for recording information about the service.</param>
        /// <param name="producer">Kafka producer.</param>
        /// <exception cref="InvalidOperationException">Thrown when required configuration settings, such as Kafka Topic, are missing from the application's configuration.</exception>
        public KafkaSendingService(
            IConfiguration config,
            ILogger<KafkaSendingService> logger, 
            IProducer<Null, ImageMessage> producer
            )
        {
            _logger = logger;
            _producer = producer;
            _topic = config[_topicKey] ?? throw new InvalidOperationException($"{_topicKey} not configured in appsettings.");
        }

        /// <inheritdoc cref="ISendingService.SendImageAsync(ImageMessage)"/>
        public async Task<bool> SendImageAsync(ImageMessage imageMessage)
        {
            try
            {
                var deliveryReport = await _producer.ProduceAsync(_topic, new Message<Null, ImageMessage> { Value = imageMessage });

                if (deliveryReport.Status == PersistenceStatus.NotPersisted)
                {
                    _logger.LogError($"Message not persisted: {deliveryReport.TopicPartitionOffset}, {deliveryReport.Message}, {deliveryReport.Status}");
                    return false;
                }

                _logger.LogInformation($"Message sent to topic: {deliveryReport.TopicPartitionOffset}");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error image in ImageProcessingService.");
                return false;
            }
        }
    }
}
