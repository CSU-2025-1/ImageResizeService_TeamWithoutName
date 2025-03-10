namespace ResizeImageMicroservice.Services
{
    public interface IImageResizeService
    {
        Task<byte[]> ResizeImageAsync(IFormFile imageFile, int width, int height, bool preserveAspectRatio);
    }
}
