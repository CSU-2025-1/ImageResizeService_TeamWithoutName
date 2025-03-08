using System.ComponentModel.DataAnnotations;

namespace ImageRotationMicroservice.Models
{
    public class RotationRequest
    {
        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Rotation angle is required")]
        [Range(-360, 360, ErrorMessage = "Angle must be between -360 and 360 degrees")]
        public double Angle { get; set; }
    }
}
