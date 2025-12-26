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
    /// <param name="objectKey">Ключ объекта в бакете.</param>
    /// <returns>Сгенерированный ключ.</returns>
    public static string GenerateKey(string bucketName, string objectKey) => $"{bucketName}:{objectKey}";
    
    /// <summary>
    /// Клююч для глобальных изображений. По типу фона, общих картинок.
    /// </summary>
    /// <param name="folder">Логическая папка (например: "ui", "backgrounds").</param>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Ключ.</returns>
    public static string Global(string folder, string fileName) =>$"global/{folder}/{fileName}";
    
    /// <summary>
    /// Преобразовать ключ.
    /// </summary>
    /// <param name="key">Ключ объекта.</param>
    /// <returns>Преобразованный ключ.</returns>
    public static (string BucketName, string ObjectKey) ParseKey(string key)
    {
       var idx = key.IndexOf(':');
       if (idx < 0 || idx == key.Length - 1)
       {
           throw new FormatException("Invalid key format");
       }
       return (key[..idx], key[(idx + 1)..]);
    }
}