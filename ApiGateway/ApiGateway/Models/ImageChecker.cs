using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    public class ImageChecker
    {
        /// <summary>
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The desired width of the image in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The desired image height in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Determines whether to keep the proportions of the image when resizing.
        /// </summary>
        public bool PreserveAspectRatio { get; set; }

        /// <summary>
        /// The angle of rotation of the image in degrees. Acceptable values are from -360 to 360.
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// The desired output image format. Acceptable values: png, jpeg, webp, bmp.
        /// </summary>
        public string? Format { get; set; }
    }
}
