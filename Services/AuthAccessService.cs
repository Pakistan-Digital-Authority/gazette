using gop.Data;
using gop.Enums;
using gop.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gop.Services;

/// <summary>
/// To check user role
/// </summary>
public class AuthAccessService : IAuthAccessService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="context"></param>
    public AuthAccessService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Checking user role
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<bool> UserHasRoleAsync(Guid userId, UserRoleEnum roleName)
    {
        return await _context.Users.AnyAsync(ur => ur.Id == userId && ur.Role == roleName);
    }
}