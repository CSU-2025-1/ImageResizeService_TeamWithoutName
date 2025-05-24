using Confluent.Kafka;
using MessageRouter.Model;
using MessageRouter.Services.Contracts;

namespace MessageRouter.Services
{
    /// <summary>
    /// Kafka producer service.
    /// </summary>
    public class ProducerService : IProducerService
    {
        private readonly ILogger<ProducerService> _logger;
        private readonly IProducer<Null, ImageMessage> _producer;
        private readonly Dictionary<string, string> _topics = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerService"/> class.
        /// </summary>
        /// <param name="config">The <see cref="IConfiguration"/> interface for accessing the application configuration.</param>
        /// <param name="logger">The interface <see cref="ILogger{ProducerService}"/> for logging.</param>
        /// <param name="producer">Kafka producer.</param>
        public ProducerService(IConfiguration config, ILogger<ProducerService> logger, IProducer<Null, ImageMessage> producer)
        {
            _logger = logger;
            _producer = producer;

            Array topicNames = Enum.GetValues(typeof(TopicName));
            foreach (TopicName topicName in topicNames)
            {
                string topicNameString = topicName.NameToString();
                _topics.Add(topicNameString, GetTopicFromConfig(config, topicNameString));
            }
        }

        /// <summary>
        /// Get topic name from config. 
        /// </summary>
        /// <param name="config">The <see cref="IConfiguration"/> interface for accessing the application configuration.</param>
        /// <param name="key">Key name topic.</param>
        /// <returns>Name topic from config.</returns>
        /// <exception cref="InvalidOperationException">Error getting name topic from config.</exception>
        private string GetTopicFromConfig(IConfiguration config, string key)
        {
            return config[key] ?? throw new InvalidOperationException($"{key} not configured in appsettings.");
        }

        /// <inheritdoc cref="IProducerService.SendImageMessage(TopicName, ImageMessage)"/>
        public async Task<bool> SendImageMessage(TopicName key, ImageMessage imageMessage)
        {
            if (imageMessage == null)
            {
                _logger.LogError("ImageMessage is null. Cannot send to Kafka.");
                return false;
            }

            try
            {
                var deliveryReport = await _producer.ProduceAsync(_topics[key.NameToString()], new Message<Null, ImageMessage> { Value = imageMessage });

                if (deliveryReport.Status == PersistenceStatus.NotPersisted)
                {
                    _logger.LogError($"Message not persisted: {deliveryReport.TopicPartitionOffset}, {deliveryReport.Message}, {deliveryReport.Status}");
                    return false;
                }

                _logger.LogInformation($"Message sent to topic: {deliveryReport.TopicPartitionOffset}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to Kafka");
                return false;
            }
        }
    }
}
