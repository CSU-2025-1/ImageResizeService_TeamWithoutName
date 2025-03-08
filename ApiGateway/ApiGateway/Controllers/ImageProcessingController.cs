using ApiGateway.Models;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp; 
using SixLabors.ImageSharp.Formats;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageProcessingController : ControllerBase
    {
        private readonly HttpClient _client = new HttpClient();

        private readonly string _resizeUrl;
        private readonly string _rotateUrl;
        private readonly string _formatUrl;

        public ImageProcessingController(IConfiguration config)
        {
            _resizeUrl = config["Microservices:Resize"];
            _rotateUrl = config["Microservices:Rotate"];
            _formatUrl = config["Microservices:Format"];
        }

        [HttpPost]
        public async Task<IActionResult> ProcessImage([FromForm] ImageProcessingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                byte[] imageBytes;
                IImageFormat imageFormat;

                using (var ms = new MemoryStream())
                {
                    await request.Image.CopyToAsync(ms);
                    imageBytes = ms.ToArray();

                    ms.Position = 0;
                    imageFormat = Image.DetectFormat(ms);
                }

                if (request.Width.HasValue || request.Height.HasValue)
                {
                    imageBytes = await ProcessStep(_resizeUrl, imageBytes, new
                    {
                        width = request.Width,
                        height = request.Height
                    });
                }

                if (request.Angle.HasValue)
                {
                    imageBytes = await ProcessStep(_rotateUrl, imageBytes, new
                    {
                        angle = request.Angle.Value
                    });
                }

                if (!string.IsNullOrEmpty(request.Format))
                {
                    imageBytes = await ProcessStep(_formatUrl, imageBytes, new
                    {
                        format = request.Format
                    });
                }

                return File(imageBytes, $"image/{imageFormat.Name.ToLower()}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Processing failed: {ex.Message}");
            }
        }

        private async Task<byte[]> ProcessStep(string url, byte[] image, object parameters)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(image), "image", "image");

            foreach (var prop in parameters.GetType().GetProperties())
            {
                content.Add(new StringContent(prop.GetValue(parameters)?.ToString()), prop.Name);
            }

            var response = await _client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}