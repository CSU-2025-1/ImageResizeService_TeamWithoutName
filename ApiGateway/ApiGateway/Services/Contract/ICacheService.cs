using ApiGateway.Models;

namespace ApiGateway.Services.Contract
{
    /// <summary>
    /// Defines the interface for the image caching service.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves the image from the cache using the specified key.
        /// </summary>
        /// <param name="key">The key of the image to be retrieved from the cache.</param>
        /// <returns>An image presented as a string if it is found in the cache.</returns>
        string GetImage(string key);

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
