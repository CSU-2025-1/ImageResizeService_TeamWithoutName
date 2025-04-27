using Confluent.Kafka;
using ResizeImageMicroservice.Models.ImageMessage;
using ResizeImageMicroservice.Services.Contract;

namespace ResizeImageMicroservice.Services
{
    /// <summary>
    /// Kafka producerService
    /// </summary>
    public class ProducerService : IProducerService
    {
        private readonly ILogger<ProducerService> _logger;
        private readonly IProducer<Null, ImageMessage> _producer;
        private readonly string _topicKey = "KafkaTopics:RouterImage";
        private readonly string _topic;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerService"/> class.
        /// </summary>
        /// <param name="config">The <see cref="IConfiguration"/> interface for accessing the application configuration.</param>
        /// <param name="logger">The interface <see cref="ILogger{ProducerService}"/> for logging.</param>
        /// <param name="producer">Kafka producer.</param>
        public ProducerService(
            IConfiguration config, 
            ILogger<ProducerService> logger, 
            IProducer<Null, ImageMessage> producer
            )
        {
            _logger = logger;
            _producer = producer;
            _topic = config[_topicKey] ?? throw new InvalidOperationException($"{_topicKey} not configured in appsettings.");
        }

        /// <inheritdoc cref="IProducerService.SendToImageResized(ImageMessage)"/>
        public async Task<bool> SendToImageResized(ImageMessage imageMessage)
        {
            if (imageMessage == null)
            {
                _logger.LogError("ImageMessage is null. Cannot send to Kafka.");
                return false;
            }

            var deliveryReport = await _producer.ProduceAsync(_topic,
                new Message<Null, ImageMessage> { Value = imageMessage });

            if (deliveryReport.Status == PersistenceStatus.NotPersisted)
            {
                _logger.LogError($"Message not persisted: {deliveryReport.TopicPartitionOffset}, {deliveryReport.Message}, {deliveryReport.Status}");
                return false;
            }
            _logger.LogInformation($"Message sent to topic: {deliveryReport.TopicPartitionOffset}");
            return true;
        }
    }
}

