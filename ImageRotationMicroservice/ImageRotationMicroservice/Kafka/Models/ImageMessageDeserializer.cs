using Confluent.Kafka;
using System.Text.Json;

namespace ImageRotationMicroservice.Kafka.Models
{
    public class ImageMessageDeserializer : IDeserializer<ImageMessage>
    {
        private readonly ILogger<ImageMessageDeserializer> _logger;

        public ImageMessageDeserializer(ILogger<ImageMessageDeserializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ImageMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return null;
            }

            try
            {
                var jsonString = System.Text.Encoding.UTF8.GetString(data.ToArray());
                return JsonSerializer.Deserialize<ImageMessage>(jsonString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserialization ImageMessage");
                throw;
            }
        }
    }
}
