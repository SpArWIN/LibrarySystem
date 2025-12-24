namespace Storage.Domain.Repository;

/// <summary>
/// Репозиторий перезаписи Url.
/// </summary>
public interface IPublicUrlRewriter
{
    /// <summary>
    /// Изменить Url.
    /// </summary>
    /// <param name="url">url.</param>
    /// <returns></returns>
    string Rewrite(string url);
}