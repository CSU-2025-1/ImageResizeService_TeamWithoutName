using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ApiGateway.Models
{
    /// <summary>
    /// Provides extension methods for the <see cref="IFormFile"/> interface for working with files uploaded via forms.
    /// </summary>
    public static class FormFileExtensions
    {

        public static async Task<byte[]> RemoveMetadataAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using (Image image = await Image.LoadAsync(memoryStream))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        await image.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = 95 });
                        return outputStream.ToArray();
                    }
                }
            }
        }

        public static async Task<string?> ConvertToBase64WithoutMetadataAsync(this IFormFile? file, ILogger? logger = null)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            try
            {
                byte[] imageDataWithoutMetadata = await RemoveMetadataAsync(file);
                return Convert.ToBase64String(imageDataWithoutMetadata);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error converting file to Base64 after removing metadata");
                return null;
            }
        }

        /// <summary>
        /// Converts the contents of the uploaded file (<see cref="IFormFile"/>) to a Base64 encoded string.
        /// </summary>
        /// <param name="file">The uploaded file that needs to be converted. Maybe <see langword="null"/>.</param>
        /// <param name="logger">An optional logger instance for recording error messages. If <see langword="null"/> is passed, logging is not performed.</param>
        /// <returns>A string containing the contents of a Base64-encoded file. Returns <see langword="null"/> if the file is <see langword="null"/>, the file length is 0 or an error occurred when converting the file.</returns>
        public static async Task<string?> ConvertToBase64(this IFormFile? file, ILogger? logger = null)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error converting file to Base64");
                return null;
            }
        }
    }
}
