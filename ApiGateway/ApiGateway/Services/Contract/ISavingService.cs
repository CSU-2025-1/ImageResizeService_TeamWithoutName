using ApiGateway.Models;

namespace ApiGateway.Services.Contract
{
    public interface ISavingService
    {
        Task<string> Check(ImageChecker imageChecker);

    }
}
