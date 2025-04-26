
using MessageRouter.Model;

namespace MessageRouter.Services.Contracts
{
    public interface IImageDatabaseService
    {
        Task<bool> SaveImage(ImageDatabase image);
    }
}
