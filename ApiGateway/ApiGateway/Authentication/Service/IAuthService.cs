using ApiGateway.Authentication.Model;

namespace ApiGateway.Authentication.Service
{
    public interface IAuthService
    {
        Task<User> Register(User user, string password);

        Task<string> Authenticate(string username, string password);
    }
}
