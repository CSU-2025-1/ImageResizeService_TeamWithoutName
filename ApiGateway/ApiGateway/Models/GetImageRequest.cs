using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    public class GetImageRequest
    {
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; }
    }
}
