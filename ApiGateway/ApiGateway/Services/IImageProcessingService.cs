namespace ApiGateway.Services
{
    public interface IImageProcessingService
    {
        Task<byte[]> ProcessingImageAsync(IFormFile imageFile, int? Width, int? Height, bool? PreserveAspectRatio, double? Angle, string? Format);
    }
}
