namespace Storage.Application.Extensions;

/// <summary>
/// Расширене на бакеты.
/// </summary>
public static class BucketTypeExtensions
{
    /// <summary>
    /// Получить Content-Type по расширению файла.
    /// </summary>
    public static string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}