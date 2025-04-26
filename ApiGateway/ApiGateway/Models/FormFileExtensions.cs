namespace ApiGateway.Models
{
    /// <summary>
    /// Provides extension methods for the <see cref="IFormFile"/> interface for working with files uploaded via forms.
    /// </summary>
    public static class FormFileExtensions
    {
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
