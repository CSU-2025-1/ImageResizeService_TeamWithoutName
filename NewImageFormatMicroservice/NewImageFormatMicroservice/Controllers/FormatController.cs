using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Bmp;
using NewImageFormatMicroservice.Models;
using NewImageFormatMicroservice.Services;

namespace NewImageFormatMicroservice.Controllers;

[ApiController]
[Route("api/format")]
public class FormatController : ControllerBase
{
    private readonly IFormatService _FormatService;

    public FormatController(IFormatService FormatService)
    {
        _FormatService = FormatService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConvertImageFormat([FromForm] FormatRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            byte[] convertedImageBytes = await _FormatService.ConvertFormatAsync(request.Image, request.Format);
            return File(convertedImageBytes, $"image/{request.Format}");
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