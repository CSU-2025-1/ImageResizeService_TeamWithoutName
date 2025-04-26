using ApiGateway.Models;

namespace ApiGateway.Services.Contract
{
    public interface IImageDatabaseService
    {
        Task<ImageDatabase> GetImageById(string id);
        Task<ImageDatabase> GetImageByParams(ImageChecker imageChecker);
    }
}
