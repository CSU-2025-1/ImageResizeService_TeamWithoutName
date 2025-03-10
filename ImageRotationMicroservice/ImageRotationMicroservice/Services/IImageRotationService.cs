namespace ImageRotationMicroservice.Services
{
    public interface IImageRotationService
    {
        Task<byte[]> RotateImageAsync(IFormFile imageFile, double Angle);
    }
}
