namespace ApiGateway.Models
{
    public static class FormFileExtensions
    {
        public static async Task<string?> ConvertToBase64(this IFormFile? file, ILogger? logger = null)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error converting file to Base64");
                return null;
            }
        }
    }
}
