using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    /// <summary>
    /// The request model for image processing. It contains information about the image, size, rotation, and output format.
    /// </summary>
    public class ImageProcessingRequest
    {
        /// <summary>
        /// The image to process. Required field.
        /// </summary>
        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; }

        /// <summary>
        /// The desired width of the image in pixels.
        /// </summary>
        [DefaultValue(100)]
        [Range(1, int.MaxValue, ErrorMessage = "Width must be at least 1 pixel")]
        public int? Width { get; set; } = 100;

        /// <summary>
        /// The desired image height in pixels.
        /// </summary>
        [DefaultValue(100)]
        [Range(1, int.MaxValue, ErrorMessage = "Height must be at least 1 pixel")]
        public int? Height { get; set; } = 100;

        /// <summary>
        /// Determines whether to keep the proportions of the image when resizing.
        /// </summary>
        public bool? PreserveAspectRatio { get; set; }

        /// <summary>
        /// The angle of rotation of the image in degrees. Acceptable values are from -360 to 360.
        /// </summary>
        [Range(-360, 360, ErrorMessage = "Rotation angle must be between -360 and 360 degrees")]
        public double? Angle { get; set; }

        /// <summary>
        /// The desired output image format. Acceptable values: png, jpeg, webp, bmp.
        /// </summary>
        [RegularExpression("^(png|jpeg|webp|bmp)$",
            ErrorMessage = "Supported formats: png, jpeg, webp, bmp")]
        public string? Format { get; set; }
    }
}
