namespace ApiGateway.Services.Contract
{
    /// <summary>
    /// Defines the interface for the image format conversion service.
    /// </summary>
    public interface IFormatService
    {
        /// <summary>
        /// Asynchronously converts an image from a string to the specified format.
        /// </summary>
        /// <param name="imageFile">The string representation of the image that needs to be converted. This is usually a Base64 image.</param>
        /// <param name="format">The desired image format, for example, "jpeg", "png".</param>
        /// <returns>An array of bytes representing the converted image.</returns>
        Task<byte[]> ConvertFormatAsync(string imageFile, string format);

        /// <summary>
        /// Defines the image format from the <see cref="IFormFile"/> object.
        /// </summary>
        /// <param name="imageFile">The <see cref="IFormFile"/> object representing the uploaded image.</param>
        /// <returns>A string representing the image format (for example, "jpeg", "png").</returns>
        string GetImageFormat(IFormFile imageFile);
    }
}
