namespace ApiGateway.Services.Contract
{
    public interface IFormatService
    {
        Task<byte[]> ConvertFormatAsync(string imageFile, string format);

        string GetImageFormat(IFormFile imageFile);
    }
}
