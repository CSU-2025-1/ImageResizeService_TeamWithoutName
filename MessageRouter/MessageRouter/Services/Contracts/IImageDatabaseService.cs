using MessageRouter.Model;

namespace MessageRouter.Services.Contracts
{
    /// <summary>
    /// Interface for working with image database.
    /// </summary>
    public interface IImageDatabaseService
    {
        /// <summary>
        /// Asynchronously saves information about the image to the database.
        /// </summary>
        /// <param name="image">The <see cref="ImageDatabase"/> object containing information about the image to be saved.</param>
        /// <returns>
        /// `true` if the image has been successfully saved in the database; <br/>
        /// `false' if an error occurred while saving the image.</returns>
        Task<bool> SaveImage(ImageDatabase image);
    }
}
