using MessageRouter.Model;

namespace MessageRouter.Services.Contracts
{
    /// <summary>
    /// Defines the interface for the image processing service.
    /// </summary>
    public interface IImageProcessingService
    {
        /// <summary>
        /// Asynchronously processes the image contained in the <see cref="ImageMessage"/> object.
        /// </summary>
        /// <param name="imageMessage">The <see cref="ImageMessage"/> object containing information about the image and processing parameters.</param>
        /// <returns>
        /// `true` if the image processing was successful; <br/>
        /// `false` if an error occurred during processing.
        /// </returns>
        Task<bool> ProcessingImageAsync(ImageMessage imageMessage);
    }
}
