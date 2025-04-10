using Confluent.Kafka;
using MessageRouter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace MessageRouter.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly ILogger<ConsumerService> _logger;
        private readonly IConsumer<Null, ImageMessage> _consumerRouterImage;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly string _topicKey = "KafkaTopics:RouterImage";
        private readonly string _topic;

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
        
        public override void Dispose()
        {
            _consumerRouterImage.Dispose();
            base.Dispose();
        }
    }
}
