using Core.Application.Caching;
using Core.Application.Services.PreloadedImages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Service.Apis;

/// <summary>
/// Публичный контроллер для доступа к предзагруженным изображениям.
/// </summary>
[ApiController]
[Route("api/preloaded-images/{bucket}/{objectKey}")]
public sealed class PreloadedImagesController : ControllerBase
{
    private readonly IPreloadedImageUrlService _service;

    public PreloadedImagesController(IPreloadedImageUrlService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить информацию о предзагруженном изображении по бакету и ключу объекта.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("")]
    public async Task<ActionResult<PreloadedImageCacheEntry>> Get(
        string bucket,
        string objectKey,
        CancellationToken ct = default)
    {
        string decodedObjectKey = System.Net.WebUtility.UrlDecode(objectKey);
        var entry = await _service.GetAsync(bucket, decodedObjectKey, ct);
        if (entry is null)
        {
            return NotFound();
        }

        return Ok(entry);
    }

    /// <summary>
    /// Получить только публичную ссылку на предзагруженное изображение.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("url")]
    public async Task<ActionResult<string>> GetUrl(
        string bucket,
        string objectKey,
        CancellationToken ct = default)
    {
        string decodedObjectKey = System.Net.WebUtility.UrlDecode(objectKey);
        var entry = await _service.GetAsync(bucket, decodedObjectKey, ct);
        if (entry is null || string.IsNullOrWhiteSpace(entry.PublicUrl))
        {
            return NotFound();
        }

        return Ok(entry.PublicUrl);
    }
}

