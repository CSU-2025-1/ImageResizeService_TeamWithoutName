
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ResizeImageMicroservice.Services
{
    public class ImageResizeService : IImageResizeService
    {
        private readonly ILogger<ImageResizeService> _logger;

        public ImageResizeService(ILogger<ImageResizeService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> ResizeImageAsync(IFormFile imageFile, int width, int height, bool preserveAspectRatio)
        {
            try {
                using (var image = await Image.LoadAsync(imageFile.OpenReadStream())) {
                    ResizeOptions resizeOptions = new ResizeOptions
                    {
                        Size = new Size(width, height),
                        Mode = preserveAspectRatio ? ResizeMode.Max : ResizeMode.Stretch
                    };
                    image.Mutate(x => x.Resize(resizeOptions));
                    using (var memoryStream = new MemoryStream()) {
                        await image.SaveAsync(memoryStream, new JpegEncoder { Quality = 80 });
                        memoryStream.Position = 0;
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception e) {
                _logger.LogError(e, "Error resizing image in ImageService.");
                throw;
            }
        }
    }
}
