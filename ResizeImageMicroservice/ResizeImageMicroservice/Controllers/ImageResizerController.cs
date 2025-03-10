using Microsoft.AspNetCore.Mvc;
using ResizeImageMicroservice.Models;
using ResizeImageMicroservice.Services;

namespace ResizeImageMicroservice.Controllers
{
    [ApiController]
    [Route("image-resizer")]
    public class ImageResizerController: ControllerBase
    {
        private readonly ILogger<ImageResizerController> _logger;
        private readonly IImageResizeService _imageService;

        public ImageResizerController(ILogger<ImageResizerController> logger, IImageResizeService imageService) {
            _logger = logger;
            _imageService = imageService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResizeImage([FromForm] ImageResizeRequest request){
            if (!ModelState.IsValid){
                foreach (var error in ModelState)
                {
                    foreach (var modelError in error.Value.Errors)
                    {
                        _logger.LogWarning($"Validation error for {error.Key}: {modelError.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }
            try {
                byte[] resizedImageBytes = await _imageService.ResizeImageAsync(
                    request.Image, 
                    request.Width, 
                    request.Height, 
                    request.PreserveAspectRatio
                    );
                return File(resizedImageBytes, "image/jpeg");
            }
            catch (Exception e){
                _logger.LogError(e, "Error resizing image in controller.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing the image.");
            }
        }

    }
}
