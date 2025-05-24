namespace ApiGateway.Models.ImageMessage
{
    /// <summary>
    /// Provides extension methods for the ImageMessage class.
    /// </summary>
    public static class ImageMessageExstension
    {
        /// <summary>
        /// Determines whether to resize and rotate the image based on the values of the Width, Height, and Angle properties.<br/>
        /// Sets the values of the IsNeedResize and IsNeedRotation properties in the ImageMessage object.
        /// </summary>
        /// <param name="imageMessage">An ImageMessage object for which it is necessary to determine whether resizing and rotation are required.</param>
        public static void SetStep(this ImageMessage imageMessage)
        {
            imageMessage.IsNeedResize = imageMessage.Height > 0 || imageMessage.Width > 0;
            imageMessage.IsNeedRotation = imageMessage.Angle < 360 || imageMessage.Angle > -360;
        }
    }
}
