using ApiGateway.Models;

namespace ApiGateway.Services.Contract
{
    /// <summary>
    /// Defines the interface for the saved image verification service.
    /// </summary>
    public interface ISavingService
    {
        /// <summary>
        /// Asynchronously checks for the presence of an image using the specified parameters and, if the image is missing, saves it and returns the identifier of the saved image.
        /// </summary>
        /// <param name="imageChecker">The <see cref="ImageChecker"/> object containing the parameters for checking and saving the image.</param>
        /// <returns>The image is in Base64 format.</returns>
        Task<string> CheckFull(ImageChecker imageChecker);

        /// <summary>
        /// Asynchronously checks for the presence of an image by the specified identifier and returns the image and its format.
        /// </summary>
        /// <param name="id">Image id.</param>
        /// <returns>A tuple containing: <br/>
        /// - A string representation of an image (for example, in Base64 format).<br/>
        /// - A string representing the image format (for example, "jpeg", "png").
        /// </returns>
        Task<(string, string)> CheckId(string id);
    }
}
