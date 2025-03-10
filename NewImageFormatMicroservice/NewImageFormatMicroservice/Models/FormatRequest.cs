using System.ComponentModel.DataAnnotations;

namespace NewImageFormatMicroservice.Models
{
    public class FormatRequest
    {
        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Format is required")]
        public string Format { get; set; }
    }
}
