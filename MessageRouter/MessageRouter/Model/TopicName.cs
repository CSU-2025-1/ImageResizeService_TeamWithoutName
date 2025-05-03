namespace MessageRouter.Model
{
    /// <summary>
    /// An enumeration defining the names of Kafka topics used for image processing.
    /// </summary>
    public enum TopicName
    {
        /// <summary>
        /// The topic used to send messages to the image resizer service.
        /// </summary>
        ResizerImage,

        /// <summary>
        /// The topic used to send messages to the image rotation service (rotator).
        /// </summary>
        RotatorImage,

        /// <summary>
        /// The topic used to send messages to the image format modification service.
        /// </summary>
        FormatorImage,

        /// <summary>
        /// A topic used for routing messages between different image processing services.
        /// </summary>
        RouterImage
    }

    /// <summary>
    /// Provides extension methods for enumeration <see cref="TopicName"/>.
    /// </summary>
    public static class TopicNameExtensions
    {
        /// <summary>
        /// Converts the value of the enumeration <see cref="TopicName"/> to a string representing the Kafka topic configuration key.
        /// </summary>
        /// <param name="topicName">The value of the enumeration <see cref="TopicName"/> that needs to be converted.</param>
        /// <returns>A string representing the Kafka topic configuration key. String format: "KafkaTopics:{TopicName}".<br/>
        /// If the enumeration value is not defined, it returns "Hello".
        /// </returns>
        public static string NameToString(this TopicName topicName)
        {
            switch (topicName)
            {
                case TopicName.ResizerImage:
                    return "KafkaTopics:ResizerImage";
                case TopicName.RotatorImage:
                    return "KafkaTopics:RotatorImage";
                case TopicName.FormatorImage:
                    return "KafkaTopics:FormatorImage";
                case TopicName.RouterImage:
                    return "KafkaTopics:RouterImage";
                default:
                    return "Hello";
            }
        }
    }
}
