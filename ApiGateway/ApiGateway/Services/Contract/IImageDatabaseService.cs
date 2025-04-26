using ApiGateway.Models;

namespace ApiGateway.Services.Contract
{
    public interface IImageDatabaseService
    {
        Task<ImageDatabase> GetImage(string id);
    }
}
