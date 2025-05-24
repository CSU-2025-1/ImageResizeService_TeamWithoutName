namespace MessageRouter.Model
{
    /// <summary>
    /// Represents a model for storing images in the cache.
    /// </summary>
    public class ImageCache
    {
        /// <summary>
        /// The key used to identify the image in the cache.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// An image presented as a string.
        /// </summary>
        public string Image { get; set; }
    }
}
