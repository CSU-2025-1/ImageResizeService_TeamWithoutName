using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using ImageRotationMicroservice.Models;
using ImageRotationMicroservice.Services;

namespace ImageRotationMicroservice.Controllers
{
    [ApiController]
    [Route("api/rotation")]
    public class RotationController : ControllerBase
    {
        private readonly ILogger<RotationController> _logger;
        private readonly IImageRotationService _imageService;

        public RotationController(ILogger<RotationController> logger, IImageRotationService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RotateImage([FromForm] RotationRequest request)
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
                byte[] rotateImageBytes = await _imageService.RotateImageAsync(
                    request.Image,
                    request.Angle
                    );
                return File(rotateImageBytes, request.Image.ContentType);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error rotation image in controller.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing the image.");
            }
        }
    }
}