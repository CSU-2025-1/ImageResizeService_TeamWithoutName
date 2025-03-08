using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Bmp;
using Microsoft.AspNetCore.WebUtilities;
using static System.Net.Mime.MediaTypeNames;
using Image = SixLabors.ImageSharp.Image;

namespace ImageFormatMicroservice.Controllers;

[ApiController]
[Route("api/format")]
public class FormatController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ConvertImageFormat(
        IFormFile image,
        [FromForm] string format)
    {
        try
        {
            using var inputStream = image.OpenReadStream();

            var imageData = await Image.LoadAsync(inputStream);

            IImageEncoder encoder = format.ToLower() switch
            {
                "png" => new PngEncoder(),
                "jpeg" => new JpegEncoder(),
                "webp" => new WebpEncoder(),
                "bmp" => new BmpEncoder(),
                _ => throw new ArgumentException("Unsupported format")
            };

            using var outputStream = new MemoryStream();
            await imageData.SaveAsync(outputStream, encoder);

            return File(outputStream.ToArray(), $"image/{format}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing image: {ex.Message}");
        }
    }
}