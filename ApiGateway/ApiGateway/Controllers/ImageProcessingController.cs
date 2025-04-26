using ApiGateway.Models;
using ApiGateway.Models.ImageMessage;
using ApiGateway.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUlid;

namespace ApiGateway.Controllers
{

    /// <summary>
    /// A controller that contains methods for image processing.
    /// </summary>
    [ApiController]
    [Route("api/images")]
    [Authorize]
    public class ImageProcessingController : ControllerBase
    {
        private readonly ILogger<ImageProcessingController> _logger;
        private readonly ISendingService _producerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProcessingController"/> class.
        /// </summary>
        /// <param name="logger">>A logger for recording information about the controller.</param>
        /// <param name="producerService">A service that transmits a message to kafka.</param>
        public ImageProcessingController(ILogger<ImageProcessingController> logger, ISendingService producerService)
        {
            _logger = logger;
            _producerService = producerService;
        }

        /// <summary>
        /// Accepts the image, the conversion parameters, and sends an image processing request.
        /// </summary>
        /// <param name="request">The <see cref="ImageProcessingRequest"/> object containing the image and image modification parameters.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the registration attempt.<br/>
        /// Returns:<br/>
        ///   - <see cref="StatusCodes.Status200OK"/> (200 OK) Successful sending of an image processing request. Returns the request ID (Ulid).<br/>
        ///   - <see cref="StatusCodes.Status400BadRequest"/> (400 BadRequest) Validation error.<br/>
        ///   - <see cref="StatusCodes.Status500InternalServerError"/> (500 Internal Server Error) The error is on the server side. There may be problems with image conversion, message sending, or other internal errors.<br/>
        /// </returns>
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

                var id = Ulid.NewUlid().ToString();

                var imageMessage = new ImageMessage
                {
                    Id = id,
                    OriginalImage = convertedImage.Result,
                    Image = convertedImage.Result,
                    Height = request.Height ?? -1,
                    Width = request.Width ?? -1,
                    PreserveAspectRatio = request.PreserveAspectRatio,
                    Angle = request.Angle ?? 361,
                    Format = request.Format
                };
                imageMessage.SetStep();

                bool isSend = await _producerService.SendImageAsync(imageMessage);

                if (isSend)
                {
                    return Ok(id);
                } else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error send the image.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error apigateway image in controller.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing the image.");
            }
        }
    }
}
