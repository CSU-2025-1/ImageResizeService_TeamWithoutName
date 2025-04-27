using ImageRotationMicroservice.Models.ImageMessage;

namespace ImageRotationMicroservice.Services.Contract
{
    /// <summary>
    /// Defines the interface for the service for sending messages about rotated images to the Kafka topic.
    /// </summary>
    public interface IProducerService
    {
        /// <summary>
        /// Asynchronously sends a message containing information about the rotated image to the Kafka topic intended for such messages.
        /// </summary>
        /// <param name="imageMessage">The <see cref="ImageMessage"/> object containing information about the rotated image.</param>
        /// <returns>
        /// `true` if the message was successfully sent to the Kafka topic;<br/>
        /// `false' if an error occurred when sending the message.
        /// </returns>
        Task<bool> SendToImageRotated(ImageMessage imageMessage);
    }
}
