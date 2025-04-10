namespace MessageRouter.Model
{
    public enum TopicName
    {
        ResizerImage,
        RotatorImage,
        FormatorImage,
        RouterImage
    }

    public static class TopicNameExtensions
    {
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
