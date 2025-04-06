using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp; 

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/images")]
    [Authorize]
    public class ImageProcessingController : ControllerBase
    {
        private readonly ILogger<ImageProcessingController> _logger;
        private readonly IImageProcessingService _imageService;

        public ImageProcessingController(ILogger<ImageProcessingController> logger, IImageProcessingService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResizeImage([FromForm] ImageProcessingRequest request)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var modelError in error.Value.Errors)
                    {
                        _logger.LogWarning($"Validation error for {error.Key}: {modelError.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }
            try
            {
                using var imageStream = request.Image.OpenReadStream();
                var imageFormat = Image.DetectFormat(imageStream);

                byte[] processingImageBytes = await _imageService.ProcessingImageAsync(
                    request.Image,
                    request.Width,
                    request.Height,
                    request.PreserveAspectRatio,
                    request.Angle,
                    request.Format
                    );

                if (!string.IsNullOrEmpty(request.Format))
                {
                    return File(processingImageBytes, $"image/{request.Format.ToLower()}");
                }
                return File(processingImageBytes, $"image/{imageFormat.Name.ToLower()}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error apigateway image in controller.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing the image.");
            }
        }
    }
}