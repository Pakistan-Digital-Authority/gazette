using gop.Enums;
using gop.Interfaces;
using gop.Security.CurrentUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace gop.Validators;

/// <summary>
/// User permission to access
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly UserRoleEnum _role;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="role"></param>
    public HasPermissionAttribute(UserRoleEnum role)
    {
        _role = role;
    }

    /// <summary>
    /// Checking authorization
    /// </summary>
    /// <param name="context"></param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var currentUserProvider = context.HttpContext.RequestServices.GetService<ICurrentUserProvider>();
        var authService = context.HttpContext.RequestServices.GetService<IAuthAccessService>();
        var userInfo = currentUserProvider.GetCurrentUser();
        var hasPermission = await authService.UserHasRoleAsync(userInfo.Id, _role);
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}