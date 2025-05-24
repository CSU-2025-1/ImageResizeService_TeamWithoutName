using ApiGateway.Models.ImageMessage;

namespace ApiGateway.Services.Contract
{
    /// <summary>
    /// An interface for image sending services.
    /// </summary>
    public interface ISendingService
    {
        /// <summary>
        /// It sends the image asynchronously.
        /// </summary>
        /// <param name="imageMessage">An <see cref="ImageMessage"/> object containing information about the image.</param>
        /// <returns>A task representing an asynchronous sending operation. Returns `true` if the sending was successful, otherwise `false'.</returns>
        Task<bool> SendImageAsync(ImageMessage imageMessage);
    }
}
