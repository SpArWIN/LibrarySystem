namespace Core.Infrastructure.Constaints;

/// <summary>
/// Константные ID ролей, чтобы в каждой базе была унификация.
/// </summary>
public static class RoleIds
{
    public static readonly Guid Reader = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Librarian = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid Administrator = Guid.Parse("33333333-3333-3333-3333-333333333333");
}