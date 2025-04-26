using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Bmp;
using ApiGateway.Services.Contract;

namespace ApiGateway.Services
{
    public class FormatService : IFormatService
    {
        private readonly ILogger<FormatService> _logger;

        public FormatService(ILogger<FormatService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> ConvertFormatAsync(string imageFile, string format)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(imageFile);


                using var inputStream = new MemoryStream(imageBytes);
                var image = await Image.LoadAsync(inputStream);

                IImageEncoder encoder = format.ToLower() switch
                {
                    "png" => new PngEncoder(),
                    "jpeg" => new JpegEncoder(),
                    "webp" => new WebpEncoder(),
                    "bmp" => new BmpEncoder(),
                    _ => throw new ArgumentException("Unsupported format")
                };

                using var memoryStream = new MemoryStream();
                await image.SaveAsync(memoryStream, encoder);
                memoryStream.Position = 0;

                return memoryStream.ToArray();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid format requested: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during image format conversion.");
                throw;
            }
        }

        public string GetImageFormat(IFormFile imageFile)
        {
            using var stream = imageFile.OpenReadStream();
            var format = Image.DetectFormat(stream);

            return format?.Name.ToLowerInvariant();
        }
    }
}
