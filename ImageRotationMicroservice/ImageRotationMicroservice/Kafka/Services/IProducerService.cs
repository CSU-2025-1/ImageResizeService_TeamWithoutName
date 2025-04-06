using ImageRotationMicroservice.Kafka.Models;

namespace ImageRotationMicroservice.Kafka.Services
{
    public interface IProducerService
    {
        Task<bool> SendToImageResized(ImageMessage imageMessage);
    }
}
