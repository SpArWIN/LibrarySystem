namespace Storage.Infrastructure.KeyFolder;

/// <summary>
/// Генератор ключей бакетов.
/// </summary>
public static class KeyGenerator
{
    /// <summary>
    /// Сгенерировать ключ.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="imageName">Имя изоббражения.</param>
    /// <returns>Сгенерированный ключ.</returns>
    public static string GenerateKey(string bucketName, string imageName) => $"{bucketName}:{imageName}";
    
    /// <summary>
    /// Преобразовать ключ.
    /// </summary>
    /// <param name="key">Ключ объекта.</param>
    /// <returns>Преобразованный ключ.</returns>
    public static (string BucketName, string ImageName) ParseKey(string key)
    {
        var parts = key.Split(':');
        return (parts[0], parts[1]);
    }
}