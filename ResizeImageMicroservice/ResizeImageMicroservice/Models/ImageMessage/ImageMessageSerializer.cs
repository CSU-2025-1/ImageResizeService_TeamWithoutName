using Confluent.Kafka;
using System.Text.Json;
using System.Text;

namespace ResizeImageMicroservice.Models.ImageMessage
{
    /// <summary>
    /// Serializes an <see cref="ImageMessage"/> object into a byte array using JSON serialization.
    /// Implements the <see cref="ISerializer{ImageMessage}"/> interface.
    /// </summary>
    public class ImageMessageSerializer : ISerializer<ImageMessage>
    {
        private readonly ILogger<ImageMessageSerializer> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageMessageSerializer"/> class.
        /// </summary>
        /// <param name="logger">A logger for recording information.</param>
        public ImageMessageSerializer(ILogger<ImageMessageSerializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Serializes an <see cref="ImageMessage"/> object into a byte array.
        /// </summary>
        /// <param name="imageMessage">The <see cref="ImageMessage"/> object to serialize.</param>
        /// <param name="context">The serialization context (not used in this implementation, but required by the interface).</param>
        /// <returns>A byte array representing the serialized <see cref="ImageMessage"/> object.</returns>
        /// <exception cref="Exception">Thrown if an error occurs during serialization. The error is also logged.</exception>
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
