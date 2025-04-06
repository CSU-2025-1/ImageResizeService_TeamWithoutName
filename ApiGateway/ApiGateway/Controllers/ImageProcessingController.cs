using ApiGateway.Models.Kafka;
using ApiGateway.Models;
using ApiGateway.Services;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp; 

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageProcessingController : ControllerBase
    {
        private readonly ILogger<ImageProcessingController> _logger;
        private readonly IProducerService _imageService;

        public ImageProcessingController(ILogger<ImageProcessingController> logger, IProducerService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
                var convertedImage = request.Image.ConvertToBase64(_logger);
                if(convertedImage.Result == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error with image.");
                }

                bool processingImageBytes = await _imageService.SendImageAsync(
                    new ImageMessage
                    {
                        Image = convertedImage.Result,
                        Height = request.Height ?? -1,
                        Width = request.Width ?? -1,
                        PreserveAspectRatio = request.PreserveAspectRatio,
                        Angle = request.Angle ?? 361,
                        Format = request.Format
                    });

                if (processingImageBytes)
                {
                    return Ok();
                } else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error send the image.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error apigateway image in controller.");
                throw e;
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing the image.");
            }
        }

        /*[HttpGet("GetImage")] // Добавлен атрибут маршрута
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        public IActionResult GetImage()
        {
            //TODO вытягивание из бд
        }*/
    }
}
