using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageRotationMicroservice.Services
{
    public class ImageRotationService : IImageRotationService
    {
        private readonly ILogger<ImageRotationService> _logger;

        public ImageRotationService(ILogger<ImageRotationService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> RotateImageAsync(IFormFile imageFile, double angle)
        {
            try
            {
                using var inputStream = imageFile.OpenReadStream();

                inputStream.Position = 0;
                var format = Image.DetectFormat(inputStream);

                inputStream.Position = 0;
                var imageData = await Image.LoadAsync(inputStream);

                imageData.Mutate(x => x.Rotate((float)angle));

                using var outputStream = new MemoryStream();
                await imageData.SaveAsync(outputStream, format);

                return outputStream.ToArray();
            }
            catch (UnknownImageFormatException)
            {
                _logger.LogError("Unsupported image format. Supported formats: JPEG, PNG, BMP.");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error rotation image in ImageService.");
                throw;
            }
        }
    }
}
