using ApiGateway.Models.Kafka;

namespace ApiGateway.Services
{
    public interface IProducerService
    {
        Task<bool> SendImageAsync(ImageMessage imageMessage);
    }
}
