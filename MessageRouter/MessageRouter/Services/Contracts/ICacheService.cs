using MessageRouter.Model;

namespace MessageRouter.Services.Contracts
{
    /// <summary>
    /// Defines the interface for the image caching service.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Saves the image to the cache. 
        /// </summary>
        /// <param name="imageCache">The <see cref="ImageCache"/> object containing the key and the image data to be cached.</param>
        /// <returns>`true` if the image has been successfully cached;
        /// `false` if saving failed. For example, if the cache is full or another error has occurred.
        /// </returns>
        bool SaveImage(ImageCache imageCache);
    }
}
