namespace ImageRotationMicroservice.Models.ImageMessage
{
    /// <summary>
    /// Presents a message containing information about the image and its processing parameters.
    /// </summary>
    public class ImageMessage
    {
        /// <summary>
        /// The unique identifier of the message.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The original image is in Base64 format.
        /// </summary>
        public string OriginalImage { get; set; }

        /// <summary>
        /// The image is to be processed (can be changed in the process). It is also available in Base64 format.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The desired width of the resulting image.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The desired height of the resulting image.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Specifies whether to keep the proportions of the image when resizing.
        /// </summary>
        public bool PreserveAspectRatio { get; set; }

        /// <summary>
        /// The angle of rotation of the image (in degrees).
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// The desired format of the resulting image (for example, JPEG, PNG, GIF).
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Indicates whether the image size needs to be resized.
        /// </summary>
        public bool IsNeedResize { get; set; }

        /// <summary>
        /// Indicates whether the image needs to be rotated.
        /// </summary>
        public bool IsNeedRotation { get; set; }
    }
}
