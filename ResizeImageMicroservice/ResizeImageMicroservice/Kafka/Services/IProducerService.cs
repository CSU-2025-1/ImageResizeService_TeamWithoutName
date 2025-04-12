using ResizeImageMicroservice.Kafka.Models;

namespace ResizeImageMicroservice.Kafka.Services
{
    public interface IProducerService
    {
        Task<bool> SendToImageResized(ImageMessage imageMessage);
    }
}
