using Confluent.Kafka;
using System.Text.Json;

namespace MessageRouter.Model
{
    /// <summary>
    /// Implementation of the <see cref="IDeserializer{ImageMessage}"/> interface for deserializing messages like <see cref="ImageMessage"/> from an array of bytes to an object.
    /// </summary>
    public class ImageMessageDeserializer : IDeserializer<ImageMessage>
    {
        private readonly ILogger<ImageMessageDeserializer> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageMessageDeserializer"/> class.
        /// </summary>
        /// <param name="logger">The interface <see cref="ILogger{ImageMessageDeserializer}"/> for logging.</param>
        /// <exception cref="ArgumentNullException">It is thrown if the parameter <paramref name="logger"/> has the value `null'.</exception>
        public ImageMessageDeserializer(ILogger<ImageMessageDeserializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Deserializes a <see cref="ImageMessage"/> message from an array of bytes.
        /// </summary>
        /// <param name="data">An array of bytes containing a serialized message in JSON format.</param>
        /// <param name="isNull">Specifies whether the message is `null`.</param>
        /// <param name="context">The context of serialization. It is used to provide additional information during deserialization.</param>
        /// <returns>The <see cref="ImageMessage"/> object representing the deserialized message.
        /// Returns `null` if the parameter <paramref name="isNull"/> has the value `true'.
        /// </returns>
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
