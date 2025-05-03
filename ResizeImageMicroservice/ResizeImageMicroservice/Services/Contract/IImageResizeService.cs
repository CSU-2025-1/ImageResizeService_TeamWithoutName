namespace ResizeImageMicroservice.Services.Contract
{
    /// <summary>
    /// Defines the interface for the image resizing service.
    /// </summary>
    public interface IImageResizeService
    {
        /// <summary>
        /// Asynchronously resizes the image represented as an object <see cref="IFormFile"/>.
        /// </summary>
        /// <param name="imageFile">The <see cref="IFormFile"/> object representing the image that needs to be resized.</param>
        /// <param name="width">The desired width of the image in pixels.</param>
        /// <param name="height">The desired image height in pixels.</param>
        /// <param name="preserveAspectRatio">Specifies whether to keep the proportions of the image when resizing. If `true`, the proportions are preserved, if `false`, the image may be distorted.</param>
        /// <returns>An array of bytes representing a resized image.</returns>
        Task<byte[]> ResizeImageAsync(IFormFile imageFile, int width, int height, bool preserveAspectRatio);
    }
}
