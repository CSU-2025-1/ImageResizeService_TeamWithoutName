using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

namespace ImageRotationMicroservice.Controllers;

[ApiController]
[Route("api/rotation")]
public class RotationController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RotateImage(
        IFormFile image,
        [FromForm] double angle)
    {
        try
        {
            using var inputStream = image.OpenReadStream();

            var imageData = await Image.LoadAsync(inputStream);

            var format = Image.DetectFormat(inputStream);

            imageData.Mutate(x => x.Rotate((float)angle));

            using var outputStream = new MemoryStream();
            await imageData.SaveAsync(outputStream, format);

            return File(outputStream.ToArray(), image.ContentType);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing image: {ex.Message}");
        }
    }
}