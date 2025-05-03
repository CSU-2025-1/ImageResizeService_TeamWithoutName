namespace ImageRotationMicroservice.Services.Contract
{
    /// <summary>
    /// An interface that provides functionality for rotating images.
    /// </summary>
    public interface IImageRotationService
    {
        /// <summary>
        /// Asynchronously rotates the image transmitted as an <see cref="IFormFile"/> by a specified angle.
        /// </summary>
        /// <param name="imageFile">An <see cref="IFormFile"/> representing the image for rotation.</param>
        /// <param name="angle">The angle of rotation in degrees.</param>
        /// <returns>A task containing an array of bytes representing the rotated image.</returns>
        Task<byte[]> RotateImageAsync(IFormFile imageFile, double angle);
    }
}
