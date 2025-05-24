using ApiGateway.Models;

namespace ApiGateway.Services.Contract
{
    /// <summary>
    /// Interface for working with image database.
    /// </summary>
    public interface IImageDatabaseService
    {
        /// <summary>
        /// Asynchronously retrieves an image from the database using the specified Id.
        /// </summary>
        /// <param name="id">ID of the image to receive.</param>
        /// <returns>The <see cref="ImageDatabase"/> object representing the image, if it is found in the database.</returns>
        Task<ImageDatabase> GetImageById(string id);

        /// <summary>
        /// Asynchronously retrieves an image from the database based on the parameters contained in the <see cref="ImageChecker"/> object.
        /// </summary>
        /// <param name="imageChecker">The <see cref="ImageChecker"/> object containing the parameters for image search.</param>
        /// <returns>The <see cref="ImageDatabase"/> object representing the image, if it is found in the database.</returns>
        Task<ImageDatabase> GetImageByParams(ImageChecker imageChecker);
    }
}
