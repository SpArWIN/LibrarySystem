namespace Common.Contracts.Database;

/// <summary>
/// Запрос на создание новой библиотеки( по сути новой базы данных)
/// </summary>
public sealed record CreateLibraryRequestDto
{
    /// <summary> Название новой библиотеки(Новой базы данных.)</summary>
    public required string Name { get; init; }
}