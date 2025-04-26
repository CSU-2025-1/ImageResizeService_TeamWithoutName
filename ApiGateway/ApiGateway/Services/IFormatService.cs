namespace ApiGateway.Services
{
    public interface IFormatService
    {
        Task<byte[]> ConvertFormatAsync(IFormFile imageFile, string format);
    }
}
