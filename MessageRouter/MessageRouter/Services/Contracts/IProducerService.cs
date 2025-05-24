using MessageRouter.Model;

namespace MessageRouter.Services.Contracts
{
    /// <summary>
    /// Defines the interface for the service for sending image-related messages to Kafka topics.
    /// </summary>
    public interface IProducerService
    {
        /// <summary>
        /// Asynchronously sends a message containing information about the image to the specified Kafka topic.
        /// </summary>
        /// <param name="key">An enumeration <see cref="TopicName"/> that defines the Kafka topic to send the message to.</param>
        /// <param name="imageMessage">The <see cref="ImageMessage"/> object containing information about the image.</param>
        /// <returns>
        /// `true` if the message was successfully sent to the Kafka topic;<br/>
        /// `false' if an error occurred when sending the message.
        /// </returns>
        Task<bool> SendImageMessage(TopicName key, ImageMessage imageMessage);
    }
}
