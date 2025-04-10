using Confluent.Kafka;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.SignalR;

namespace ApiGateway.Models.Kafka
{
    public class ImageMessageSerializer : ISerializer<ImageMessage>
    {
        private readonly ILogger<ImageMessageSerializer> _logger;

        public ImageMessageSerializer(ILogger<ImageMessageSerializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public byte[] Serialize(ImageMessage imageMessage, SerializationContext context)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(imageMessage);
                var result = Encoding.UTF8.GetBytes(jsonString);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serialization ImageMessage");
                throw;
            }
        }
    }

}
