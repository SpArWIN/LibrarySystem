namespace Common.Contracts.Database;

/// <summary>
/// Ответ на создание библиотеки.
/// </summary>
public sealed class CreateLibraryResponseDto
{
    /// <summary>ID библиотеки.</summary>
    public required Guid LibraryId { get; init; }
}