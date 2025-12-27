using Common.Contracts.Auth;
using Common.Db.Extensions;
using Common.Db.Factory;
using Common.Policies.Services;
using Core.Application.Services.AuthorizeService;
using Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Service.Apis;

/// <summary>
/// Контроллер авторизации.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
   private readonly IAuthorizeService _authorizeService;
   private readonly IUnitOfWorkFactory<CentralDbContext> _unitOfWorkFactory;
   private readonly IDbResilience _dbResilience;

   /// <summary>
   /// Конструктор.
   /// </summary>
   /// <param name="authorizeService"><see cref="IAuthorizeService"/>.</param>
   /// <param name="unitOfWorkFactory"><see cref="IUnitOfWorkFactory{TDbContext}"/>.</param>
   /// <param name="dbResilience"><see cref="IDbResilience"/>.</param>
   public AuthController(IAuthorizeService authorizeService,
      IUnitOfWorkFactory<CentralDbContext> unitOfWorkFactory, 
      IDbResilience dbResilience)
   {
      _authorizeService = authorizeService;
      _unitOfWorkFactory = unitOfWorkFactory;
      _dbResilience = dbResilience;
   }

   /// <summary>
   /// Обновить пару токенов по refresh токен.
   /// </summary>
   /// <param name="refreshRequestDto"><see cref="RefreshRequestDto"/>.</param>
   /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
   /// <returns><see cref="RefreshResponseDto"/>.</returns>
   [AllowAnonymous]
   [HttpPost("refresh")]
   public async Task<ActionResult<RefreshResponseDto>> Refresh([FromBody] RefreshRequestDto refreshRequestDto,
      CancellationToken cancellationToken = default)
   {
      var result = await _unitOfWorkFactory.CreateWithRetryAsync(async uow =>
         await _authorizeService.RefreshAsync(refreshRequestDto, uow, cancellationToken),
         _dbResilience.Write, cancellationToken);
      return result;
   }
}