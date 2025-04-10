using Confluent.Kafka;
using System.Collections.Concurrent;
using ApiGateway.Models.Kafka;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace ApiGateway.Services
{
    public class ProducerService : IProducerService
    {
        private readonly ILogger<ProducerService> _logger;
        private readonly IProducer<Null, ImageMessage> _producer;
        private readonly string _topicKey = "KafkaTopics:SendRequest";
        private readonly string _topic;

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
