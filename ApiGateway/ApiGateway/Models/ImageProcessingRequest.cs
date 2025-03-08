using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    public class ImageProcessingRequest
    {
        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Width must be at least 1 pixel")]
        public int? Width { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Height must be at least 1 pixel")]
        public int? Height { get; set; }

        [Range(-360, 360, ErrorMessage = "Rotation angle must be between -360 and 360 degrees")]
        public double? Angle { get; set; }

        [RegularExpression("^(png|jpeg|webp|bmp)$",
            ErrorMessage = "Supported formats: png, jpeg, webp, bmp")]
        public string? Format { get; set; }
    }
}
