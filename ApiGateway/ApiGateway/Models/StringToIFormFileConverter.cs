using System.Text;

namespace ApiGateway.Models
{
    public class StringToIFormFileConverter
    {
        public static IFormFile ConvertBase64ToIFormFile(string base64String, string fileName, string contentType)
        {
            // 1. Декодируем строку Base64 в массив байтов.
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // 2. Создаем поток из массива байтов.
            MemoryStream stream = new MemoryStream(imageBytes);

            // 3. Создаем IFormFile на основе потока.
            IFormFile formFile = new FormFile(stream, 0, stream.Length, "name", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType // Пример: "image/jpeg" или "image/png"
            };

            return formFile;
        }

        public static IFormFile ConvertStringToIFormFile(string imageString, string fileName, string contentType, Encoding encoding)
        {
            // 1. Преобразуем строку в массив байтов, используя указанную кодировку.
            byte[] imageBytes = encoding.GetBytes(imageString);

            // 2. Создаем поток из массива байтов.
            MemoryStream stream = new MemoryStream(imageBytes);

            // 3. Создаем IFormFile на основе потока.
            IFormFile formFile = new FormFile(stream, 0, stream.Length, "name", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType // Пример: "image/jpeg" или "image/png"
            };

            return formFile;
        }
    }
}
