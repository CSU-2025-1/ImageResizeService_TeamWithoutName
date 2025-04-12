using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp; 

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageProcessingController : ControllerBase
    {
        private readonly ILogger<ImageProcessingController> _logger;
        private readonly IImageProcessingService _imageService;
        private readonly IFormatService _formatService;

        public ImageProcessingController(ILogger<ImageProcessingController> logger, IImageProcessingService imageService, IFormatService formatService)
        {
            _logger = logger;
            _imageService = imageService;
            _formatService = formatService;
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

        [HttpGet("random-image")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRandomImage()
        {
            try
            {
                string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");

                var imageFiles = Directory.GetFiles(imagesDirectory);

                if (!imageFiles.Any())
                {
                    return NotFound("No images found in the directory.");
                }

                Random random = new Random();
                string randomImagePath = imageFiles[random.Next(imageFiles.Length)];

                string[] supportedFormats = { "png", "jpeg", "webp", "bmp" };
                string randomFormat = supportedFormats[random.Next(supportedFormats.Length)];

                using (var fileStream = new FileStream(randomImagePath, FileMode.Open))
                {
                    IFormFile formFile = new FormFile(fileStream, 0, fileStream.Length, "image", Path.GetFileName(randomImagePath));

                    byte[] formattedImageBytes = await _formatService.ConvertFormatAsync(formFile, randomFormat);

                    return File(formattedImageBytes, $"image/{randomFormat}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing random image request.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing the image.");
            }
        }
    }
}