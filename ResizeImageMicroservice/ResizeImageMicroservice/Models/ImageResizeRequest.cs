using System.ComponentModel.DataAnnotations;

namespace ResizeImageMicroservice.Models
{
    public class ImageResizeRequest
    {
        [Required]
        public IFormFile Image { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Width must be a positive integer.")]
        public int Width { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Height must be a positive integer.")]
        public int Height { get; set; }

        [Required]
        public bool PreserveAspectRatio { get; set; }
    }
}
