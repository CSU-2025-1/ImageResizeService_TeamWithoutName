using SixLabors.ImageSharp.Formats;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace ApiGateway.Services
{
    public class ImageProcessingService : IImageProcessingService
    {

        private readonly HttpClient _client = new HttpClient();

        private readonly string _resizeUrl;
        private readonly string _rotateUrl;
        private readonly string _formatUrl;

        private readonly ILogger<ImageProcessingService> _logger;

        public ImageProcessingService(ILogger<ImageProcessingService> logger, IConfiguration config)
        {
            _logger = logger;
            _resizeUrl = config["Microservices:Resize"];
            _rotateUrl = config["Microservices:Rotate"];
            _formatUrl = config["Microservices:Format"];
        }

        public async Task<byte[]> ProcessingImageAsync(IFormFile imageFile, int? width, int? height, bool? preserveAspectRatio, double? angle, string? format)
        {
            try
            {
                byte[] imageBytes;
                IImageFormat imageFormat;

                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    imageBytes = ms.ToArray();

                    ms.Position = 0;
                    imageFormat = Image.DetectFormat(ms);
                }

                if (width.HasValue || height.HasValue || preserveAspectRatio.HasValue)
                {
                    // Отправить в Kafka: событие "Изменение размера"
                    // await _kafkaProducer.ProduceAsync("image-processing", new {
                    //     Event = "ResizeCompleted",
                    //     Width = width,
                    //     Height = height,
                    //     PreserveAspectRatio = preserveAspectRatio
                    // });

                    imageBytes = await ProcessStep(_resizeUrl, imageBytes, new
                    {
                        width = width,
                        height = height,
                        preserveAspectRatio = preserveAspectRatio
                    });
                }

                if (angle.HasValue)
                {
                    // Отправить в Kafka: событие "Поворот изображения"
                    // await _kafkaProducer.ProduceAsync("image-processing", new {
                    //     Event = "RotationCompleted",
                    //     Angle = angle.Value
                    // });

                    imageBytes = await ProcessStep(_rotateUrl, imageBytes, new
                    {
                        angle = angle.Value
                    });
                }

                if (!string.IsNullOrEmpty(format))
                {
                    // Отправить в Kafka: событие "Конвертация формата"
                    // await _kafkaProducer.ProduceAsync("image-processing", new {
                    //     Event = "FormatConversionCompleted",
                    //     Format = format
                    // });

                    imageBytes = await ProcessStep(_formatUrl, imageBytes, new
                    {
                        format = format
                    });
                }

                // Отправить в Kafka: событие "Обработка завершена успешно"
                // await _kafkaProducer.ProduceAsync("image-processing", new {
                //     Event = "ProcessingSucceeded",
                //     FinalSize = imageBytes.Length
                // });

                return imageBytes.ToArray();
            }
            catch (Exception e)
            {
                // Отправить в Kafka: событие "Ошибка обработки"
                // await _kafkaProducer.ProduceAsync("image-processing", new {
                //     Event = "ProcessingFailed",
                //     Error = e.Message
                // });

                _logger.LogError(e, "Error image in ImageProcessingService.");
                throw;
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
