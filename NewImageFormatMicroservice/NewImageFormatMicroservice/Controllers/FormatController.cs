using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Bmp;
using NewImageFormatMicroservice.Models;

namespace NewImageFormatMicroservice.Controllers;

[ApiController]
[Route("api/format")]
public class FormatController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ConvertImageFormat([FromForm] FormatRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var inputStream = request.Image.OpenReadStream();

            inputStream.Position = 0;
            var imageData = await Image.LoadAsync(inputStream);

            IImageEncoder encoder = request.Format.ToLower() switch
            {
                "png" => new PngEncoder(),
                "jpeg" => new JpegEncoder(),
                "webp" => new WebpEncoder(),
                "bmp" => new BmpEncoder(),
                _ => throw new ArgumentException("Unsupported format")
            };

            using var outputStream = new MemoryStream();
            await imageData.SaveAsync(outputStream, encoder);

            return File(outputStream.ToArray(), $"image/{request.Format}");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing image: {ex.Message}");
        }
    }
}