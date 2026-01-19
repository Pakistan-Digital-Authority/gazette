using gop.Enums;

namespace gop.Interfaces;

/// <summary>
/// To check user role
/// </summary>
public interface IAuthAccessService
{
    /// <summary>
    /// Checking user role
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    Task<bool> UserHasRoleAsync(Guid userId, UserRoleEnum roleName);
}