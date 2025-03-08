using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using ImageRotationMicroservice.Models;

namespace ImageRotationMicroservice.Controllers
{
    [ApiController]
    [Route("api/rotation")]
    public class RotationController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> RotateImage([FromForm] RotationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using var inputStream = request.Image.OpenReadStream();

                inputStream.Position = 0;
                var format = Image.DetectFormat(inputStream);

                inputStream.Position = 0;
                var imageData = await Image.LoadAsync(inputStream);

                imageData.Mutate(x => x.Rotate((float)request.Angle));

                using var outputStream = new MemoryStream();
                await imageData.SaveAsync(outputStream, format);

                return File(outputStream.ToArray(), request.Image.ContentType);
            }
            catch (UnknownImageFormatException)
            {
                return BadRequest("Unsupported image format. Supported formats: JPEG, PNG, BMP, GIF, WebP, TGA, TIFF");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing image: {ex.Message}");
            }
        }
    }
}