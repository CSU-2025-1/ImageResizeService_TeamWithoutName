using ApiGateway.Models;

namespace ApiGateway.Services.Contract
{
    public interface ICacheService
    {
        string GetImage(string key);
        bool SaveImage(ImageCache imageCache);

    }
}
