namespace Common.Extensions;

/// <summary>
/// Расширение не строки.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Проверяет, не является ли строка Null или пустой.
    /// </summary>
    /// <param name="str">Строка.</param>
    /// <returns>.</returns>
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);
   
    /// <summary>
    /// Проверяет, является ли строка пустой, null или состоящей только из пробелов.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? str) 
        => string.IsNullOrWhiteSpace(str);
    
    /// <summary>
    /// Проверяет, не является ли строка пустой, null или состоящей только из пробелов.
    /// </summary>
    public static bool IsNotNullOrWhiteSpace(this string? str) 
        => !string.IsNullOrWhiteSpace(str);
    
    /// <summary>
    /// Проверяет, не является ли строка пустой или null.
    /// </summary>
    public static bool IsNotNullOrEmpty(this string? str) 
        => !string.IsNullOrEmpty(str);
    
    /// <summary>
    /// Возвращает значение строки или значение по умолчанию, если строка null/пустая.
    /// </summary>
    public static string DefaultIfNullOrEmpty(this string? str, string defaultValue = "") 
        => string.IsNullOrEmpty(str) ? defaultValue : str;
}