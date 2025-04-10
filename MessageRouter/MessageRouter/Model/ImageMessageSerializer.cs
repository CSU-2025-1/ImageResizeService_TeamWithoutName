using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageRouter.Model
{
    public class ImageMessageSerializer : ISerializer<ImageMessage>
    {
        private readonly ILogger<ImageMessageSerializer> _logger;

        public ImageMessageSerializer(ILogger<ImageMessageSerializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public byte[] Serialize(ImageMessage data, SerializationContext context)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(data);
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
