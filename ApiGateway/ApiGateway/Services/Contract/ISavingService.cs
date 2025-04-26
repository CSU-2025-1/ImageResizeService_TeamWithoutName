using ApiGateway.Models;

namespace ApiGateway.Services.Contract
{
    public interface ISavingService
    {
        Task<string> CheckFull(ImageChecker imageChecker);
        Task<(string, string)> CheckId(string id);
    }
}
