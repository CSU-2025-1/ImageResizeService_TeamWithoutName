using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Bmp;
using ApiGateway.Services.Contract;

namespace ApiGateway.Services
{
    /// <summary>
    /// An implementation for converting image formats.
    /// </summary>
    public class FormatService : IFormatService
    {
        private readonly ILogger<FormatService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatService"/> class.
        /// </summary>
        /// <param name="logger">The interface <see cref="ILogger{FormatService}"/> for logging.</param>
        public FormatService(ILogger<FormatService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously converts an image from a string in Base64 format to the specified format.
        /// </summary>
        /// <param name="imageFile">The string representation of the image in Base64 format that needs to be converted.</param>
        /// <param name="format">The desired image format, for example, "jpeg", "png", "webp", "bmp".</param>
        /// <returns>An array of bytes representing the converted image in the specified format.</returns>
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
        
        /// <inheritdoc cref="IFormatService.GetImageFormat(IFormFile)"/>
        public string GetImageFormat(IFormFile imageFile)
        {
            using var stream = imageFile.OpenReadStream();
            var format = Image.DetectFormat(stream);

            return format?.Name.ToLowerInvariant();
        }
    }
}
